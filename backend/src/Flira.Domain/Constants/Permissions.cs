namespace Flira.Domain.Constants;

public static class Permissions
{
    public const string ProjectCreate = "Project.Create";
    public const string ProjectRead = "Project.Read";
    public const string ProjectUpdate = "Project.Update";
    public const string ProjectDelete = "Project.Delete";

    public const string TaskCreate = "Task.Create";
    public const string TaskRead = "Task.Read";
    public const string TaskUpdate = "Task.Update";
    public const string TaskDelete = "Task.Delete";

    public const string TeamManage = "Team.Manage";
    public const string OrganizationManage = "Organization.Manage";
    public const string OrganizationUpdate = "Organization.Update";
    public const string OrganizationDelete = "Organization.Delete";
    public const string OrganizationMembersRead = "Organization.MembersRead";
    public const string OrganizationMembersManage = "Organization.MembersManage";
    public const string OrganizationMemberRolesManage = "Organization.MemberRolesManage";
}
