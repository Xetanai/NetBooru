using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a user's role.
    /// </summary>
    public class Role : IdentityRole<ulong>
    {
        /// <summary>
        /// The color for the role
        /// </summary>
        public uint Color { get; set; }

        /// <summary>
        /// True if the role can be deleted, false otherwise
        /// </summary>
        public bool Deletable { get; set; }

        /// <summary>
        /// True if the role is locked.
        /// Locked roles can only have their color changed.
        /// They cannot be given to users and cannot have permissions changed.
        /// </summary>
        public bool Locked { get; set; }
    }
}
