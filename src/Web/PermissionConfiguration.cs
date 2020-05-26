using System.Collections;
using System.Collections.Generic;

namespace NetBooru.Web
{
    public class PermissionConfiguration
    {
        // These are the *internal* names for the roles.
        // Changing these may cause things to break.
        public const string AnonymousRole = "Anonymous";
        public const string BannedRole = "Banned";
        public const string OwnerRole = "Owner";

        // Permission types. Mapped to claims when managing roles/permissions.
        public static IEnumerable<Permission> Permissions =>
            new List<Permission>
            {
                new Permission
                {
                    Claim = "CanPostFiles",
                    Name = "Create Posts",
                    Description = "Upload and create new posts."
                },

                new Permission
                {
                    Claim = "CanDeleteFiles",
                    Name = "Delete Posts",
                    Description = "Delete posts which don't belong to them."
                },

                new Permission
                {
                    Claim = "CanCreateTags",
                    Name = "Create Tags",
                    Description = "Apply tags for the first time."
                },

                new Permission
                {
                    Claim = "CanDeleteTags",
                    Name = "Delete Tags",
                    Description = "Remove a tag from its last post."
                },

                new Permission
                {
                    Claim = "CanCreateAliases",
                    Name = "Alias Tags",
                    Description = "Set tag aliases."
                },

                new Permission
                {
                    Claim = "CanCreateImplications",
                    Name = "Imply Tags",
                    Description = "Set tag implications."
                },

                new Permission
                {
                    Claim = "CanBanHash",
                    Name = "Ban Posts",
                    Description = "Delete a post and prevent it from being reposted."
                }
            };

        public class Permission
        {
            public string Claim { get; set; } = null!;

            public string Name { get; set; } = null!;

            public string Description { get; set; } = null!;
        }
    }
}
