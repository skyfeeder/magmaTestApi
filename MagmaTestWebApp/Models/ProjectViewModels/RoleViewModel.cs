namespace MagmaTestWebApp.Models
{
    public class RoleViewModel
    {
        public RoleItemViewModel? Member { get; set; }
        public RoleItemViewModel? Editor { get; set; }
        public RoleItemViewModel? Admin { get; set; }
        public RoleItemViewModel? OrganizationAdmin { get; set; }
        public string? SimpleRole { get; set; }
        public ReferenceViewModel? User { get; set; }
    }
}
