﻿using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class LogModel
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTimeOffset TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }
}
