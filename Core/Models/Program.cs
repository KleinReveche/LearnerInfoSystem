﻿using System.ComponentModel.DataAnnotations;

namespace Reveche.SimpleLearnerInfoSystem.Models;

/// <summary>
///     Represents a program in the learning system.
/// </summary>
public class Program
{
    /// <summary>
    ///     The unique identifier for the program.
    /// </summary>
    public required int ProgramId { get; set; }

    /// <summary>
    ///     The title of the program. The maximum length is 255 characters.
    /// </summary>
    [StringLength(255)]
    public required string Title { get; set; }

    /// <summary>
    ///     The code of the program. The maximum length is 255 characters.
    /// </summary>
    [StringLength(255)]
    public required string Code { get; set; }

    /// <summary>
    ///     The description of the program. The maximum length is 255 characters.
    /// </summary>
    [StringLength(255)]
    public required string Description { get; set; }

    /// <summary>
    ///     The list of courses that are part of the program.
    /// </summary>
    public List<Course> Courses { get; set; } = [];
    
    /// <summary>
    ///     The status of the program.
    /// </summary>
    public required ProgramStatus Status { get; set; }
}

public enum ProgramStatus
{
    Active,
    Suspended,
    Discontinued
}