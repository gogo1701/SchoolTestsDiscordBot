using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBusinessLayer
{
    public class RoleManager
    {
        private readonly ulong editorRoleId;
        public RoleManager(ulong editorRoleId)
        {
            this.editorRoleId = editorRoleId;
        }
        public Predicate<DiscordMember> DoesUserHaveEditorRole => x => !x.Roles.Any(role => role.Id == editorRoleId);

        public Predicate<DiscordMember> DoesUserHaveAdminPerms => x => !x.Permissions.HasPermission(Permissions.Administrator);
    }
}
