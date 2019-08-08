namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Defines the MAPI type of an extended property.
    /// Partially copied from Ews managed api.
    /// </summary>
    public enum MapiPropertyType
    {
        /// <summary>
        /// The property is of type ApplicationTime.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        ApplicationTime,

        /// <summary>
        /// The property is of type ApplicationTimeArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        ApplicationTimeArray,

        /// <summary>
        /// The property is of type Binary.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Binary,

        /// <summary>
        /// The property is of type BinaryArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        BinaryArray,

        /// <summary>
        /// The property is of type Boolean.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Boolean,

        /// <summary>
        /// The property is of type CLSID.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        CLSID,

        /// <summary>
        /// The property is of type CLSIDArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        CLSIDArray,

        /// <summary>
        /// The property is of type Currency.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Currency,

        /// <summary>
        /// The property is of type CurrencyArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        CurrencyArray,

        /// <summary>
        /// The property is of type Double.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Double,

        /// <summary>
        /// The property is of type DoubleArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        DoubleArray,

        /// <summary>
        /// The property is of type Error.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Error,

        /// <summary>
        /// The property is of type Float.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Float,

        /// <summary>
        /// The property is of type FloatArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        FloatArray,

        /// <summary>
        /// The property is of type Integer.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Integer,

        /// <summary>
        /// The property is of type IntegerArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        IntegerArray,

        /// <summary>
        /// The property is of type Long.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Long,

        /// <summary>
        /// The property is of type LongArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        LongArray,

        /// <summary>
        /// The property is of type Null.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Null,

        /// <summary>
        /// The property is of type Object.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Object,

        /// <summary>
        /// The property is of type ObjectArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        ObjectArray,

        /// <summary>
        /// The property is of type Short.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        Short,

        /// <summary>
        /// The property is of type ShortArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        ShortArray,

        /// <summary>
        /// The property is of type SystemTime.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        SystemTime,

        /// <summary>
        /// The property is of type SystemTimeArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        SystemTimeArray,

        /// <summary>
        /// The property is of type String.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.SingleValueExtendedProperties)]
        String,

        /// <summary>
        /// The property is of type StringArray.
        /// </summary>
        [MapiPropertyTypeValueAttribute(MapiPropertyValueType.MultiValueExtendedProperties)]
        StringArray
    }
}
