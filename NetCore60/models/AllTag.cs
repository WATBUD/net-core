using System;
using System.Collections.Generic;

namespace NetCoreSpace.Models;

public partial class AllTag
{
    public int? TagId { get; set; }

    public int TagGroupId { get; set; }

    public string? TagName { get; set; }
}
