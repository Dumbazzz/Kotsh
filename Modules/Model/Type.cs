namespace Kotsh.Modules.Model
{
    public enum Type
    {
        /// <summary>
        /// Working account
        /// This is saved into a single file
        /// </summary>
        HIT,

        /// <summary>
        /// Working account but free or useless
        /// This is saved into a single file
        /// </summary>
        FREE,

        /// <summary>
        /// Not working account
        /// </summary>
        FAIL,

        /// <summary>
        /// Most of time working account with specific category
        /// This is saved into a single file
        /// </summary>
        CUSTOM,

        /// <summary>
        /// Working account but with expired subscription
        /// This is saved into a single file
        /// </summary>
        EXPIRED,

        /// <summary>
        /// The worker has been banned and can't continue the check
        /// </summary>
        BANNED,

        /// <summary>
        /// The worker meet an error and must restart
        /// </summary>
        RETRY,

        /// <summary>
        /// This response will be ignored everywhere
        /// </summary>
        IGNORED
    }
}
