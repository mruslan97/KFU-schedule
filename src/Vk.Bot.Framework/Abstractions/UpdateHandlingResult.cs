namespace Vk.Bot.Framework.Abstractions
{
    public enum UpdateHandlingResult
    {
        /// <summary>
        /// Handling the update should continue with the next available handler
        /// </summary>
        Continue,

        /// <summary>
        /// Update is handled completely
        /// </summary>
        Handled,
    }
}