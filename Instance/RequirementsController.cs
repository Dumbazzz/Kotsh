using Kotsh.Models;
using System.Linq;

namespace Kotsh.Instance
{
    /// <summary>
    /// Helper class for Tasker
    /// Checks if a part (user, pass) contains a requirement
    /// </summary>
    public class RequirementsController
    {
        /// <summary>
        /// Requirements
        /// </summary>
        private Objects.Requirements requirements;

        /// <summary>
        /// Build a requirements rules
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public RequirementsController BuildRequirements(string part)
        {
            // Initialize requirements
            requirements = new Objects.Requirements();

            // Set part name
            requirements.part = part;

            // Return instance
            return this;
        }

        /// <summary>
        /// Add a requirement
        /// </summary>
        /// <param name="req">Requirement Type</param>
        /// <param name="value">Value (if int, put it as string)</param>
        public RequirementsController AddRequirements(Rules req, string value)
        {
            // Add to dictionary
            requirements.rules.Add(req, value);

            // Return instance
            return this;
        }

        /// <summary>
        /// Return the dictionary of requirements once it has been built
        /// </summary>
        /// <returns>Dictionary</returns>
        public Objects.Requirements GetRequirements()
            => requirements;

        /// <summary>
        /// Check every requirements
        /// </summary>
        /// <param name="combo">Combo as user:pass</param>
        /// <param name="requirements">Rules</param>
        /// <returns>Boolean</returns>
        public bool CheckRequirements(string combo, Objects.Requirements requirements)
        {
            // Setting up temporary variable
            bool result = true;

            // Split the combo
            string user = combo.Split(':')[0];
            string pass = combo.Split(':')[1];

            // Define haystack
            string haystack;
            if (requirements.part == "user")
                haystack = user;
            else
                haystack = pass;

            // Foreach rules
            foreach (Rules req in requirements.rules.Keys)
            {
                // Check every rules
                switch (req)
                {
                    case Rules.MinLength:
                        result = CheckMinimumLength(haystack, int.Parse(requirements.rules[req]));
                        break;

                    case Rules.MaxLength:
                        result = CheckMaximumLength(haystack, int.Parse(requirements.rules[req]));
                        break;

                    case Rules.MustContains:
                        result = ContainsChar(haystack, requirements.rules[req]);
                        break;

                    case Rules.MustNotContains:
                        result = !ContainsChar(haystack, requirements.rules[req]);
                        break;

                    case Rules.MustContainsDigit:
                        result = ContainsDigit(haystack);
                        break;

                    case Rules.MustContainsLowercase:
                        result = ContainsLowercase(haystack);
                        break;

                    case Rules.MustContainsUppercase:
                        result = ContainsUppercase(haystack);
                        break;

                    case Rules.MustContainsSymbol:
                        result = ContainsSymbol(haystack);
                        break;
                }
            }

            // Return result
            return result;
        }

        #region Helpers

        /// <summary>
        /// Check if the haystack contains the needle string
        /// </summary>
        /// <param name="haystack">Full string</param>
        /// <param name="needle">String to find</param>
        /// <returns>Boolean</returns>
        private bool ContainsChar(string haystack, string needle)
            => haystack.Contains(needle);

        /// <summary>
        /// Check if the haystack contains at least one digit
        /// </summary>
        /// <param name="haystack">Full string</param>
        /// <returns>Boolean</returns>
        private bool ContainsDigit(string haystack)
            => haystack.Any(char.IsDigit);

        /// <summary>
        /// Check if the haystack contains at least one lowercase char
        /// </summary>
        /// <param name="haystack">Full string</param>
        /// <returns>Boolean</returns>
        private bool ContainsLowercase(string haystack)
            => haystack.Any(char.IsLower);

        /// <summary>
        /// Check if the haystack contains at least one uppercase case
        /// </summary>
        /// <param name="haystack">Full string</param>
        /// <returns>Boolean</returns>
        private bool ContainsUppercase(string haystack)
            => haystack.Any(char.IsUpper);

        /// <summary>
        /// Check if the haystack contains at least one symbol
        /// </summary>
        /// <param name="haystack">Full string</param>
        /// <returns>Boolean</returns>
        private bool ContainsSymbol(string haystack)
            => haystack.Any(char.IsSymbol);

        /// <summary>
        /// Check if haystack has minimum length
        /// </summary>
        /// <param name="haystack">Full string</param>
        /// <param name="length">Minimum length</param>
        /// <returns>Boolean</returns>
        private bool CheckMinimumLength(string haystack, int length)
            => haystack.Length >= length;

        /// <summary>
        /// Check if haystack does not exceed the maximum length
        /// </summary>
        /// <param name="haystack">Full string</param>
        /// <param name="length">Maximum length</param>
        /// <returns>Boolean</returns>
        private bool CheckMaximumLength(string haystack, int length)
            => haystack.Length <= length;

        #endregion
    }
}
