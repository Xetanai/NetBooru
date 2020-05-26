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
    }
}
