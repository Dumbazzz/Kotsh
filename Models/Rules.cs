namespace Kotsh.Models
{
    /// <summary>
    /// Available rules for a string
    /// </summary>
    public enum Rules
    {
        /// <summary>
        /// Part minimum defined length
        /// </summary>
        MinLength,

        /// <summary>
        /// Part maximum defined length
        /// </summary>
        MaxLength,

        /// <summary>
        /// Part must contains defined caracters
        /// </summary>
        MustContains,

        /// <summary>
        /// Part must not contains defined caracters
        /// </summary>
        MustNotContains,

        /// <summary>
        /// Part must contains at least one lowercase
        /// </summary>
        MustContainsLowercase,

        /// <summary>
        /// Part must contains at least one uppercase
        /// </summary>
        MustContainsUppercase,

        /// <summary>
        /// Part must contains at least one digit
        /// </summary>
        MustContainsDigit,

        /// <summary>
        /// Part must contains at least one symbol
        /// </summary>
        MustContainsSymbol
    }
}
