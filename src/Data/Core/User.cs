using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User : IdentityUser<ulong>
    {
    }
}
