using System.IO;
using System.Text;

namespace LumenWorks.Framework.IO
{
    public class NullRemovalStreamReader : StreamReader
    {
        public NullRemovalStreamReader(Stream stream, bool addMark, int threshold, Encoding encoding, int bufferSize = 4096, bool detectEncoding = false)
            : base(new NullRemovalStream(stream, addMark, threshold, bufferSize), encoding, detectEncoding, bufferSize)
        {
        }
    }
}
