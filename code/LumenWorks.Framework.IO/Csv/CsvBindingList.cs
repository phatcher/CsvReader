using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace LumenWorks.Framework.IO.Csv
{
#if !NETSTANDARD1_3
    /// <summary>
    /// Represents a binding list wrapper for a CSV reader.
    /// </summary>
    public class CsvBindingList : IBindingList, ITypedList, IList<string[]>
    {
        /// <summary>
        /// Contains the linked CSV reader.
        /// </summary>
        private CachedCsvReader _csv;

        /// <summary>
        /// Contains the cached record count.
        /// </summary>
        private int _count;

        /// <summary>
        /// Contains the cached property descriptors.
        /// </summary>
        private PropertyDescriptorCollection _properties;

        /// <summary>
        /// Contains the current sort property.
        /// </summary>
        private CsvPropertyDescriptor _sort;

        /// <summary>
        /// Contains the current sort direction.
        /// </summary>
        private ListSortDirection _direction;

        /// <summary>
        /// Initializes a new instance of the CsvBindingList class.
        /// </summary>
        /// <param name="csv"></param>
        public CsvBindingList(CachedCsvReader csv)
        {
            _csv = csv;
            _count = -1;
            _direction = ListSortDirection.Ascending;
        }

        public void AddIndex(PropertyDescriptor property)
        {
        }

        public bool AllowNew
        {
            get
            {
                return false;
            }
        }

        public void ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {
            _sort = (CsvPropertyDescriptor)property;
            _direction = direction;

            _csv.ReadToEnd();

            _csv.Records.Sort(new CsvRecordComparer(_sort.Index, _direction));
        }

        public PropertyDescriptor SortProperty
        {
            get
            {
                return _sort;
            }
        }

        public int Find(PropertyDescriptor property, object key)
        {
            int fieldIndex = ((CsvPropertyDescriptor)property).Index;
            string value = (string)key;

            int recordIndex = 0;
            int count = this.Count;

            while (recordIndex < count && _csv[recordIndex, fieldIndex] != value)
                recordIndex++;

            return recordIndex == count ? -1 : recordIndex;
        }

        public bool SupportsSorting
        {
            get { return true; }
        }

        public bool IsSorted
        {
            get { return _sort != null; }
        }

        public bool AllowRemove
        {
            get { return false; }
        }

        public bool SupportsSearching
        {
            get { return true; }
        }

        public System.ComponentModel.ListSortDirection SortDirection
        {
            get { return _direction; }
        }

        public event System.ComponentModel.ListChangedEventHandler ListChanged
        {
            add { }
            remove { }
        }

        public bool SupportsChangeNotification
        {
            get { return false; }
        }

        public void RemoveSort()
        {
            _sort = null;
            _direction = ListSortDirection.Ascending;
        }

        public object AddNew()
        {
            throw new NotSupportedException();
        }

        public bool AllowEdit
        {
            get { return false; }
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (_properties == null)
            {
                var properties = new PropertyDescriptor[_csv.FieldCount];

                for (var i = 0; i < properties.Length; i++)
                {
                    properties[i] = new CsvPropertyDescriptor(((System.Data.IDataReader) _csv).GetName(i), i);
                }

                _properties = new PropertyDescriptorCollection(properties);
            }

            return _properties;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return string.Empty;
        }

        public int IndexOf(string[] item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, string[] item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public string[] this[int index]
        {
            get
            {
                _csv.MoveTo(index);
                return _csv.Records[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void Add(string[] item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(string[] item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(string[][] array, int arrayIndex)
        {
            _csv.MoveToStart();

            while (_csv.ReadNextRecord())
            {
                _csv.CopyCurrentRecordTo(array[arrayIndex++]);
            }
        }

        public int Count
        {
            get
            {
                if (_count < 0)
                {
                    _csv.ReadToEnd();
                    _count = (int)_csv.CurrentRecordIndex + 1;
                }

                return _count;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(string[] item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<string[]> GetEnumerator()
        {
            return _csv.GetEnumerator();
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            throw new NotSupportedException();
        }

        public int IndexOf(object value)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            _csv.MoveToStart();

            while (_csv.ReadNextRecord())
            {
                _csv.CopyCurrentRecordTo((string[]) array.GetValue(index++));
            }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
#endif
}