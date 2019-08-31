namespace Microsoft.Graph.Search
{
    using System;
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.Graph.Utilities;
    
    /// <summary>
    /// Base filter formatter.
    /// </summary>
    internal abstract class SearchFilterFormatterBase : ISearchFilterFormatter
    {
        /// <inheritdoc cref="System.Type"/>
        public abstract string Type { get; }

        /// <summary>
        /// Indicate if quotes are required around value.
        /// </summary>
        protected virtual bool QuoteRequired
        {
            get { return false; }
        }

        /// <inheritdoc cref="IFilterFormatter.Format"/>
        public string Format(object obj, FilterOperator filterOperator, PropertyDefinition propertyDefinition)
        {
            this.ThrowIfNull(obj);
            if (obj is string)
            {
                return this.FormatString(
                    obj.ToString(),
                    filterOperator,
                    this.FormatPropertyName(
                        obj.ToString(),
                        propertyDefinition));
            }

            this.ValidateIfObjectInstanceOfType(
                obj,
                propertyDefinition);

            return this.FormatInternal(
                obj,
                filterOperator,
                propertyDefinition);
        }

        /// <summary>
        /// Format internal.
        /// </summary>
        /// <param name="obj">Object to format.</param>
        /// <param name="filterOperator">Filter operator.</param>
        /// <param name="propertyDefinition">Property definition.</param>
        /// <returns></returns>
        protected abstract string FormatInternal(object obj, FilterOperator filterOperator, PropertyDefinition propertyDefinition);

        /// <summary>
        /// Format string.
        /// </summary>
        /// <param name="obj">Object value.</param>
        /// <param name="filterOperator">Filter operator.</param>
        /// <param name="propertyPath">Property path.</param>
        /// <returns></returns>
        protected string FormatString(string obj, FilterOperator filterOperator, string propertyPath)
        {
            // special case for startswith - it must be in format startswith(property, 'value')
            if (filterOperator == FilterOperator.startswith)
            {
                return $"startswith({propertyPath},'{obj}')";
            }

            return this.QuoteRequired
                ? $"{propertyPath} {filterOperator} '{obj}'"
                : $"{propertyPath} {filterOperator} {obj}";
        }

        /// <summary>
        /// Format property name if required.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="propertyDefinition">Property definition.</param>
        /// <returns></returns>
        protected virtual string FormatPropertyName(string obj, PropertyDefinition propertyDefinition)
        {
            return propertyDefinition.Name;
        }

        /// <summary>
        /// Validate object instance of type.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="propertyDefinition">Property definition.</param>
        private void ValidateIfObjectInstanceOfType(object obj, PropertyDefinition propertyDefinition)
        {
            if (!propertyDefinition.Type.IsInstanceOfType(obj))
            {
                throw new ArgumentException(
                    $"'{obj.GetType().FullName}' cannot be formatted with '{this.GetType().FullName}'.");
            }
        }

        /// <summary>
        /// Throw if object null.
        /// </summary>
        /// <param name="obj"></param>
        private void ThrowIfNull(object obj)
        {
            ArgumentValidator.ThrowIfNull(
                obj,
                nameof(obj));
        }
    }

    /// <summary>
    /// Bool filter formatter.
    /// </summary>
    internal sealed class BoolFilterFormatter : SearchFilterFormatterBase
    {
        /// <summary>
        /// </summary>
        /// <inheritdoc cref="System.Type" />
        public override string Type
        {
            get { return typeof(bool).FullName; }
        }

        /// <summary>
        /// Format internal.
        /// </summary>
        /// <param name="obj">Object to format.</param>
        /// <param name="filterOperator">Filter operator.</param>
        /// <param name="propertyDefinition">Property definition.</param>
        /// <returns></returns>
        protected override string FormatInternal(object obj, FilterOperator filterOperator, PropertyDefinition propertyDefinition)
        {
            return this.Format(
                obj.ToString(),
                filterOperator,
                propertyDefinition);
        }
    }

    /// <summary>
    /// DateTimeOffset filter formatter.
    /// </summary>
    internal sealed class DateTimeFilterFormatter : SearchFilterFormatterBase
    {
        /// <inheritdoc cref="BaseFilterFormatter.Type"/>
        public override string Type
        {
            get { return typeof(DateTime).FullName; }
        }

        /// <summary>
        /// Date time format.
        /// </summary>
        private const string dateTimeFormat = "yyyy-MM-ddThh:mm:ssZ";

        /// <inheritdoc cref="BaseFilterFormatter.FormatInternal"/>
        protected override string FormatInternal(object obj, FilterOperator filterOperator, PropertyDefinition propertyDefinition)
        {
            DateTime dateTime = (DateTime)obj;
            return this.Format(
                dateTime.Date.ToString(DateTimeFilterFormatter.dateTimeFormat),
                filterOperator,
                propertyDefinition);
        }
    }

    /// <summary>
    /// DateTimeOffset filter formatter.
    /// </summary>
    internal sealed class DateTimeOffsetFilterFormatter : SearchFilterFormatterBase
    {
        /// <inheritdoc cref="BaseFilterFormatter.Type"/>
        public override string Type
        {
            get { return typeof(DateTimeOffset).FullName; }
        }

        /// <summary>
        /// Date time format.
        /// </summary>
        private const string dateTimeFormat = "yyyy-MM-ddThh:mm:ssZ";

        /// <inheritdoc cref="BaseFilterFormatter.FormatInternal"/>
        protected override string FormatInternal(object obj, FilterOperator filterOperator, PropertyDefinition propertyDefinition)
        {
            DateTimeOffset dateTimeOffset = (DateTimeOffset)obj;
            return this.Format(
                dateTimeOffset.Date.ToString(DateTimeOffsetFilterFormatter.dateTimeFormat),
                filterOperator,
                propertyDefinition);
        }
    }

    /// <summary>
    /// Recipient filter formatter.
    /// </summary>
    internal sealed class RecipientFilterFormatter : SearchFilterFormatterBase
    {
        /// <inheritdoc cref="BaseFilterFormatter.Type"/>
        public override string Type
        {
            get { return typeof(Recipient).FullName; }
        }

        /// <inheritdoc cref="BaseFilterFormatter.QuoteRequired"/>
        protected override bool QuoteRequired
        {
            get { return true; }
        }

        /// <inheritdoc cref="BaseFilterFormatter.FormatInternal"/>
        protected override string FormatInternal(object obj, FilterOperator filterOperator, PropertyDefinition propertyDefinition)
        {
            Recipient recipient = (Recipient)obj;
            ArgumentValidator.ThrowIfNull(
                recipient.EmailAddress,
                nameof(recipient.EmailAddress));

            if (string.IsNullOrEmpty(recipient.EmailAddress.Address))
            {
                ArgumentValidator.ThrowIfNullOrEmpty(
                    recipient.EmailAddress.Name,
                    "Name");

                return this.Format(
                    recipient.EmailAddress.Name,
                    filterOperator,
                    propertyDefinition);
            }

            ArgumentValidator.ThrowIfNullOrEmpty(
                recipient.EmailAddress.Address,
                "Address");

            return this.Format(
                recipient.EmailAddress.Address,
                filterOperator,
                propertyDefinition);
        }

        /// <inheritdoc cref="BaseFilterFormatter.FormatPropertyName"/>
        protected override string FormatPropertyName(string obj, PropertyDefinition propertyDefinition)
        {
            if (EmailAddressValidator.IsValid(obj))
            {
                return $"{propertyDefinition.Name}/EmailAddress/Address";
            }

            return $"{propertyDefinition.Name}/EmailAddress/Name";
        }
    }

    /// <summary>
    /// String formatter. Should be base for all non-implemented formatters.
    /// </summary>
    internal sealed class StringFilterFormatter : SearchFilterFormatterBase
    {
        /// <inheritdoc cref="BaseFilterFormatter.Type"/>
        public override string Type
        {
            get { return typeof(string).FullName; }
        }

        /// <inheritdoc cref="BaseFilterFormatter.QuoteRequired"/>
        protected override bool QuoteRequired
        {
            get { return true; }
        }

        /// <inheritdoc cref="BaseFilterFormatter.FormatInternal"/>
        protected override string FormatInternal(object obj, FilterOperator filterOperator, PropertyDefinition propertyDefinition)
        {
            // This won't be hit, base class will take care of it
            // this class overrides quote required behavior.
            return this.FormatString(
                obj.ToString(),
                filterOperator,
                propertyDefinition.Name);
        }
    }
}
