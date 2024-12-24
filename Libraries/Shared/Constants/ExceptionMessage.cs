namespace Shared.Constants;

internal class ExceptionMessage
{
    #region Enum
    public const string EnumNumericValueOutOfRange = "Cannot convert numeric value '{0}' to enum type '{1}'. The value is outside the range of defined enum values.";
    public const string EnumInvalidStringValue = "Cannot convert string value '{0}' to enum type '{1}'. The value must be a valid numeric value within the enum's range or one of the defined enum names.";
    public const string EnumConversionFailed = "Cannot convert value '{0}' to enum type '{1}'.";
    public const string EnumInvalidValueType = "Cannot convert value '{0}' to enum type '{1}'. The value must be a string or a numeric type.";
    #endregion

    #region Property
    public const string PropertyNotFound = "Property '{0}' not found in type '{1}'.";
    #endregion

    public const string UnableToCreateInstance = "Unable to create an instance of type {0}.";
    public const string CannotConvertValueToType = "Cannot convert value of type '{0}' to '{1}'.";
}
