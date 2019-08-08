namespace Microsoft.Graph.Search
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// List of formatters.
    /// </summary>
    public class SearchFormatProvider
    {
        /// <summary>
        /// String formatter name.
        /// </summary>
        private const string StringFormatterName = "System.String";

        /// <summary>
        /// Formatters supported.
        /// </summary>
        private Dictionary<string, ISearchFilterFormatter> formatters;

        /// <summary>
        /// Create new instance of <see cref="FormatProvider"/>
        /// </summary>
        internal SearchFormatProvider()
        {
            this.formatters = new Dictionary<string, ISearchFilterFormatter>();
            Type baseFilterFormatterType = typeof(SearchFilterFormatterBase);
            foreach (Type type in Assembly.GetAssembly(baseFilterFormatterType).GetTypes())
            {
                if (type.IsClass &&
                    !type.IsAbstract &&
                    type.IsSubclassOf(baseFilterFormatterType))
                {
                    ISearchFilterFormatter formatter = (SearchFilterFormatterBase)Activator.CreateInstance(type);
                    this.formatters.Add(formatter.Type, formatter);
                }
            }
        }

        /// <summary>
        /// Returns correct formatter for a type. Defaults to string.
        /// </summary>
        /// <param name="type">Type full name.</param>
        /// <returns></returns>
        internal ISearchFilterFormatter this[string type]
        {
            get
            {
                if (this.formatters.ContainsKey(type))
                {
                    return this.formatters[type];
                }

                return this.formatters[SearchFormatProvider.StringFormatterName];
            }
        }
    }
}
