namespace MagmaTestWebApp.Models
{
    public class RoleItemViewModel
    {
        public bool HasRole { get; set; }
        public bool HasWorkgroupRole { get; set; }
        public List<ReferenceViewModel>? Groups { get; set; }
    }
}
