using System;
using System.IO;

using LumenWorks.Framework.IO.Csv.Resources;

namespace LumenWorks.Framework.IO
{
    public class NullRemovalStream : Stream
    {
        private bool _addMark;
        private byte[] _buffer;
        private int _bufferIndex;
        private int _bufferSize;
        private byte[] _storage;
        private int _storageIndex;
        private int _storageSize;
        private Stream _source;
        private int _threshold;

        /// <summary>
        ///     A stream implmentation that removes consecutive null bytes above a threshold from source
        /// </summary>
        /// <param name="source"> A <see cref="T:Stream" /> pointing to the source data</param>
        /// <param name="addMark">
        ///     add a mark ([removed x null bytes]) to indicate removal if set to <see langword="true" />, remove silently if
        ///     <see langword="false" />
        /// </param>
        /// <param name="threshold"> only consecutive null bytes above this threshold will be removed or replaced by a mark</param>
        /// <param name="bufferSize">Size of buffer</param>
        public NullRemovalStream(Stream source, bool addMark = true, int threshold = 60, int bufferSize = 4096)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, ExceptionMessage.BufferSizeTooSmall);
            }

            _source = source ?? throw new ArgumentNullException(nameof(source));
            _addMark = addMark;
            _buffer = new byte [bufferSize];
            _threshold = threshold < 60 ? 60 : threshold;
            PopulateBuffer();

            _storage = new byte[4096];
            _storageIndex = 0;
            _storageSize = 0;
        }

        public override bool CanRead => _source.CanRead;

        public override bool CanSeek => _source.CanSeek;

        public override bool CanWrite => _source.CanWrite;

        public override long Length => _source.Length;

        public override long Position
        {
            get => _source.Position;
            set => _source.Position = value;
        }

#if !NETSTANDARD1_3
        public override void Close() => _source.Close();
#endif

        public override void Flush() => _source.Flush();

        public override long Seek(long offset, SeekOrigin origin) => _source.Seek(offset, origin);

        public override void SetLength(long value) => _source.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => _source.Write(buffer, offset, count);

        public override int Read(byte[] target, int offset, int count)
        {
            if (count > target.Length - offset)
            {
                return 0;
            }

            var targetIndex = offset;
            var dataRead = 0;
            var nullCount = 0;

            if (_bufferSize == 0)
            {
                return 0;
            }

            var lastByteInBuffer = 1;
            while (dataRead < count)
            {
                lastByteInBuffer = 1;
                byte current = 0;
                var readFromStorage = false;
                if (_storageSize > 0)
                {
                    // read from temporary storage first
                    current = _storage[_storageIndex++];
                    _storageSize--;
                    readFromStorage = true;
                }
                else
                {
                    // if last PopulateBuffer() exhausted the source stream, _bufferSize will be less than 4096
                    if (_bufferIndex == _bufferSize)
                    {
                        // save the current last byte in _buffer before populating _buffer again
                        lastByteInBuffer = _bufferIndex > 0 ? _buffer[_bufferIndex - 1] : lastByteInBuffer;
                        PopulateBuffer();
                        if (_bufferSize == 0)
                        {
                            break;
                        }
                    }
                    current = _buffer[_bufferIndex];
                }

                if (current == 0 && !readFromStorage)
                {
                    nullCount++;
                }
                else
                {
                    var processed = false;
                    if (_storageSize == 0 && _storageIndex > 0 && !readFromStorage)
                    {
                        // last iteration was reading from storage so no need to process again even the last byte was null; best place to reset storage index
                        _storageIndex = 0;
                        processed = true;
                    }
                    var lastIsNull = !readFromStorage && !processed && (_bufferIndex > 0 ? _buffer[_bufferIndex - 1] == 0 : lastByteInBuffer == 0);
                    var newTargetIndex = targetIndex;
                    if (lastIsNull)
                    {
                        // first non null byte
                        newTargetIndex = Process(target, targetIndex, nullCount);
                        if (newTargetIndex == target.Length)
                        {
                            return dataRead + newTargetIndex - targetIndex;
                        }
                        nullCount = 0;
                    }
                    target[newTargetIndex] = current;
                    dataRead += newTargetIndex - targetIndex + 1;
                    targetIndex = newTargetIndex + 1;
                }
                if (!readFromStorage)
                {
                    _bufferIndex++;
                }
            }
            if (nullCount > 0 && dataRead == 0 || _bufferSize == 0 && lastByteInBuffer == 0)
            {
                // the end of the source stream is a null byte so couldn't enter the else block in the while loop above, do the needful
                return Process(target, targetIndex, nullCount);
            }
            return dataRead;
        }

        private int Process(byte[] target, int targetIndex, int nullCount)
        {
            if (nullCount < _threshold)
            {
                while (nullCount > 0 && targetIndex < target.Length)
                {
                    target[targetIndex] = 0;
                    targetIndex++;
                    nullCount--;
                }
                while (nullCount > 0)
                {
                    _storage[_storageSize++] = 0;
                    nullCount--;
                }
                return targetIndex;
            }

            if (!_addMark)
            {
                return targetIndex;
            }

            var template = "[removed {0} null bytes]";
            var mark = string.Format(template, nullCount);
            foreach (var c in mark)
            {
                if (targetIndex < target.Length)
                {
                    target[targetIndex] = Convert.ToByte(c);
                    targetIndex++;
                }
                else
                {
                    _storage[_storageSize++] = Convert.ToByte(c);
                }
            }
            return targetIndex;
        }

        private void PopulateBuffer()
        {
            _bufferSize = _source.Read(_buffer, 0, _buffer.Length);
            _bufferIndex = 0;
        }
    }
}
