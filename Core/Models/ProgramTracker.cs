namespace Reveche.SimpleLearnerInfoSystem.Models;

public class ProgramTracker
{
    public required int ProgramTrackerId { get; set; }
    public required int UserId { get; set; }
    public List<ProgramProgress> Programs { get; set; } = [];
    public List<CourseCompletion> CompletedCourses { get; set; } = [];
    public List<CourseCompletion> InProgressCourses { get; set; } = [];
    public List<CourseCompletion> RemainingCourses { get; set; } = [];
    public List<CourseCompletion> DroppedCourses { get; set; } = [];
}

public class ProgramProgress
{
    public required int ProgramId { get; set; }
    public required Status Status { get; set; }
    public required DateTime? DateCompleted { get; set; }
}