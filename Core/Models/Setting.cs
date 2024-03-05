namespace Reveche.SimpleLearnerInfoSystem.Models;

/// <summary>
///     Represents a setting in the learning system.
/// </summary>
public class Setting
{
    /// <summary>
    ///     The unique identifier for the setting.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    ///     The name of the setting.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    ///     The value of the setting.
    /// </summary>
    public required string Value { get; set; }
    
    /// <summary>
    ///     The type of the setting.
    /// </summary>
    public bool IsBool { get; set; }
    
    /// <summary>
    ///     The type of the setting.
    /// </summary>
    public bool IsInt { get; set; }
    
    /// <summary>
    ///     The type of the setting.
    /// </summary>
    public bool IsLong { get; set; }
    
    /// <summary>
    ///     The type of the setting.
    /// </summary>
    public bool IsString { get; set; }

    /// <summary>
    ///     The scope of the setting. Determines which user roles this setting applies to.
    ///     If the setting is system-wide, the scope is set to Administrator.
    ///     If the setting is for instructors, the scope is set to Instructor.
    ///     If the setting is for students, the scope is set to Learner.
    /// </summary>
    public required UserRole Scope { get; set; }
}