using System.ComponentModel.DataAnnotations.Schema;

namespace Reveche.LearnerInfoSystem.Models;

/// <summary>
///     Represents a setting in the learning system.
/// </summary>
public class Setting
{
    /// <summary>
    ///     The unique identifier for the setting.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; init; }

    /// <summary>
    ///     The name of the setting.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    ///     The value of the setting.
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    ///     The type of the setting.
    /// </summary>
    public bool IsBool { get; init; }

    /// <summary>
    ///     The type of the setting.
    /// </summary>
    public bool IsInt { get; init; }

    /// <summary>
    ///     The type of the setting.
    /// </summary>
    public bool IsLong { get; init; }

    /// <summary>
    ///     The type of the setting.
    /// </summary>
    public bool IsString { get; init; }

    /// <summary>
    ///     The scope of the setting. Determines which user roles this setting applies to.
    ///     If the setting is system-wide, the scope is set to Administrator.
    ///     If the setting is for instructors, the scope is set to Instructor.
    ///     If the setting is for students, the scope is set to Learner.
    /// </summary>
    public required UserRole Scope { get; init; }
}