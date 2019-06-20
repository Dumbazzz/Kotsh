namespace Kotsh.Modules.Console
{
    /// <summary>
    /// Level of debugging/message throw
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// Informative message
        /// </summary>
        INFO,

        /// <summary>
        /// Warning message, most of time it's small errors that's does not impact the program
        /// </summary>
        WARNING,

        /// <summary>
        /// Danger message is a important message, like an error
        /// </summary>
        DANGER,

        /// <summary>
        /// Success message show you a successful task
        /// </summary>
        SUCCESS
    }
}
