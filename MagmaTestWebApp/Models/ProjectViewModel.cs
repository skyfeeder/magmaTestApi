using System;
using System.Collections.Generic;

namespace MagmaTestWebApp.Models
{
    public class ProjectViewModel
    {
        public string? Customer { get; set; }
        public double Readiness { get; set; }
        public double MaximumSize { get; set; }
        public double Size { get; set; }
        public DateTime ExecuteBefore { get; set; }
        public RoleViewModel? UserRole { get; set; }
        public ReferenceViewModel? Status { get; set; }
        public string? DiskLetter { get; set; }
        public DateTime CreateTime { get; set; }
        public bool Private { get; set; }
        public ReferenceViewModel? ModifyUser { get; set; }
        public DateTime ModifyTime { get; set; }
        public bool Locked { get; set; }
        public bool Deleted { get; set; }
        public int LockType { get; set; }
        public string? ObjectType { get; set; }
        public string? Description { get; set; }
        public string? GUID { get; set; } // С guid не работает, только string
        public string? Handle { get; set; }
        public int IconId { get; set; }
    }

    public class RoleViewModel
    {
        public RoleItemViewModel? Member { get; set; }
        public RoleItemViewModel? Editor { get; set; }
        public RoleItemViewModel? Admin { get; set; }
        public RoleItemViewModel? OrganizationAdmin { get; set; }
        public string? SimpleRole { get; set; }
        public ReferenceViewModel? User { get; set; }
    }

    public class RoleItemViewModel
    {
        public bool HasRole { get; set; }
        public bool HasWorkgroupRole { get; set; }
        public List<ReferenceViewModel>? Groups { get; set; }
    }

    public class ReferenceViewModel
    {
        public string? Description { get; set; }
        public string? Handle { get; set; }
        public string? SysId { get; set; }
        public string? Type { get; set; }
    }
} 