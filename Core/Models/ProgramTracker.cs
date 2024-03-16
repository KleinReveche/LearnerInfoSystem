﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Reveche.SimpleLearnerInfoSystem.Models;

public class ProgramTracker
{
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public List<ProgramProgress> Programs { get; set; } = [];
    public List<CourseCompletion> Courses { get; set; } = [];
}

public class ProgramProgress
{
    public int Id { get; set; }
    public required int ProgramId { get; set; }
    public required Status Status { get; set; }
    public required DateTime? DateCompleted { get; set; }
}