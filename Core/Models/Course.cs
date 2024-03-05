using System.ComponentModel.DataAnnotations;

namespace Reveche.SimpleLearnerInfoSystem.Models;

/// <summary>
///     Represents a course in the learning system.
/// </summary>
public class Course
{
    /// <summary>
    ///     The unique identifier for the course.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    ///     The title of the course. The maximum length is 255 characters.
    /// </summary>
    [StringLength(255)]
    public required string Title { get; set; }

    /// <summary>
    ///     The code of the course. The maximum length is 255 characters.
    /// </summary>
    [StringLength(255)]
    public required string Code { get; set; }

    /// <summary>
    ///     The description of the course. The maximum length is 255 characters.
    /// </summary>
    [StringLength(255)]
    public required string Description { get; set; }

    /// <summary>
    ///     The unique identifier of the instructor for the course.
    /// </summary>
    public required int InstructorId { get; set; }

    /// <summary>
    ///     The duration of the course in hours.
    /// </summary>
    public required int DurationInHours { get; set; }
}

/// <summary>
///     Represents the completion status of a course by a student.
/// </summary>
public class CourseCompletion
{
    /// <summary>
    ///     The unique identifier for the course completion record.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    ///     The unique identifier of the student.
    /// </summary>
    public required int UserId { get; set; }

    /// <summary>
    ///     The unique identifier of the course.
    /// </summary>
    public required int CourseId { get; set; }
    
    /// <summary>
    ///     The unique identifier of the instructor for the course.
    /// </summary>
    public required int InstructorId { get; set; }

    /// <summary>
    ///     The status of the course (NotStarted, InProgress, Completed).
    /// </summary>
    public required Status Status { get; set; }

    /// <summary>
    ///     The date when the course was completed. Null if the course is not yet completed.
    /// </summary>
    public DateTime? DateCompleted { get; set; }

    /// <summary>
    ///     The grade of the student for the course. Null if the course is not yet graded.
    /// </summary>
    public double? Grade { get; set; }
}

/// <summary>
///     Represents the current status
/// </summary>
public enum Status
{
    /// <summary>
    ///     It has not been started yet by the student.
    /// </summary>
    NotStarted,

    /// <summary>
    ///     It is currently in progress by the student.
    /// </summary>
    InProgress,

    /// <summary>
    ///     It has been completed by the student.
    /// </summary>
    Completed
}