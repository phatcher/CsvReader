using System;
using System.ComponentModel;

namespace LumenWorks.Framework.IO.Csv
{
#if !NETSTANDARD1_3
    /// <summary>
    /// Represents a CSV field property descriptor.
    /// </summary>
    public class CsvPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the CsvPropertyDescriptor class.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <param name="index">The field index.</param>
        public CsvPropertyDescriptor(string fieldName, int index) : base(fieldName, null)
        {
            Index = index;
        }

        /// <summary>
        /// Gets the field index.
        /// </summary>
        /// <value>The field index.</value>
        public int Index { get; private set; }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return ((string[]) component)[Index];
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(CachedCsvReader); }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
#endif
}