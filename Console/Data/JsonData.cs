using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Data;

public class JsonData
{
    public List<User> Users { get; set; } = [];
    public List<Program> Programs { get; set; } = [];
    public List<Course> Courses { get; set; } = [];
    public List<CourseCompletion> CourseCompletions { get; set; } = [];
    public List<ProgramTracker> ProgramTrackers { get; set; } = [];
    public required List<Setting> Settings { get; set; }
}