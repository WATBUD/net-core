using System;
using System.Collections.Generic;

namespace NetCoreSpace.Models;

public partial class RecordLogTable
{
    public int Id { get; set; }

    public string DataText { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
