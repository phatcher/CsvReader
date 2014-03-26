namespace LumenWorks.Framework.IO.Csv
{
    using System;

    /// <summary>
    /// Metadata about a CSV column.
    /// </summary>
    public class Column
    {
        private Type type;
        private string typeName;

        /// <summary>
        /// Creates a new instance of the <see cref="Column" /> class.
        /// </summary>
        public Column()
        {
            Type = typeof(string);
        }

        /// <summary>
        /// Get or set the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set the type.
        /// </summary>
        public Type Type
        {
            get { return type; }
            set
            {
                this.type = value;
                typeName = value.Name;
            }
        }

        /// <summary>
        /// Converts the value into the column type.
        /// </summary>
        /// <param name="value">Value to use</param>
        /// <returns>Converted value.</returns>
        public object Convert(string value)
        {
            object x;
            TryConvert(value, out x);

            return x;
        }

        /// <summary>
        /// Converts the value into the column type.
        /// </summary>
        /// <param name="value">Value to use</param>
        /// <param name="result">Object to hold the converted value.</param>
        /// <returns>true if the conversion was successful, otherwise false.</returns>
        public bool TryConvert(string value, out object result)
        {
            bool converted;

            switch (typeName)
            {
                case "Guid":
                    try
                    {
                        result = new Guid(value);
                        converted = true;
                    }
                    catch
                    {
                        result = Guid.Empty;
                        converted = false;
                    }
                    break;

                case "Int32":
                    {
                        Int32 x;
                        converted = int.TryParse(value, out x);
                        result = x;
                    }
                    break;

                case "Int64":
                    {
                        Int64 x;
                        converted = long.TryParse(value, out x);
                        result = x;
                    }
                    break;

                case "Single":
                    {
                        Single x;
                        converted = float.TryParse(value, out x);
                        result = x;
                    }
                    break;

                case "Double":
                    {
                        Double x;
                        converted = double.TryParse(value, out x);
                        result = x;
                    }
                    break;

                case "Decimal":
                    {
                        Decimal x;
                        converted = decimal.TryParse(value, out x);
                        result = x;
                    }
                    break;

                case "DateTime":
                    {
                        DateTime x;
                        converted = DateTime.TryParse(value, out x);
                        result = x;
                    }
                    break;

                default:
                    converted = false;
                    result = value;
                    break;
            }

            return converted;
        }
    }
}
