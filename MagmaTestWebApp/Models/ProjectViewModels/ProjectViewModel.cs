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
        public Guid? Guid { get; set; }
        public string? Handle { get; set; }
        public int IconId { get; set; }
    }
}