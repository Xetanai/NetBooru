using System.ComponentModel.DataAnnotations.Schema;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique internal ID of the user.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// User's display name.
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// True if the user has a custom avatar. False otherwise.
        /// </summary>
        public bool HasAvatar { get; set; }

        /// <summary>
        /// <c>true</c> if the user is using the dark mode theme to view the
        /// site.
        /// </summary>
        public bool UseDarkMode { get; set; }

    }
}
