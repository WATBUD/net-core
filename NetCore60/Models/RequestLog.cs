﻿using System;
using System.Collections.Generic;

namespace NetCoreSpace.Models;

public partial class RequestLog
{
    public int Id { get; set; }

    public string Path { get; set; } = null!;

    public string Method { get; set; } = null!;

    public string ClientIp { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string BackendLanguage { get; set; } = null!;
}
