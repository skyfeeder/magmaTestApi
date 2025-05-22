using System.Collections.Generic;

namespace MagmaTestWebApp.Models
{
    public class TreeItemViewModel
    {
        public List<TreeItemViewModel>? Children { get; set; }
        public string? ObjectType { get; set; }
        public string? Description { get; set; }
        public string? GUID { get; set; }
        public string? Handle { get; set; }
        public int IconId { get; set; }
    }
} 