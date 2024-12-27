using System;
using System.Collections.Generic;

namespace NetCoreSpace.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string PermissionName { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
