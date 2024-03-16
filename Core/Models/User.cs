using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reveche.SimpleLearnerInfoSystem.Models;

/// <summary>
///     Represents the role of a user in the learning system.
/// </summary>
public enum UserRole
{
    Learner,
    Instructor,
    Administrator
}

public enum LearnerYear
{
    NotApplicable,
    FirstYear,
    SecondYear,
    ThirdYear,
    FourthYear,
    FifthYear,
    SixthYear,
    SeventhYear,
}

/// <summary>
///     Represents the status of a user in the learning system.
/// </summary>
public enum UserStatus
{
    ActiveLearner,
    GraduatedLearner,
    DroppedLearner,
    SuspendedLearner,
    ExpelledLearner,
    Instructing,
    Retired,
    Administrator
}

/// <summary>
///     Represents a user in the learning system.
/// </summary>
public class User
{
    /// <summary>
    ///     The unique identifier for the user.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(255)] public required int Id { get; set; }
   
    /// <summary>
    ///     The string identifier for the user.
    /// </summary>
    [StringLength(255)] public required string UserIdStr { get; set; }

    /// <summary>
    ///     The username of the user.
    /// </summary>
    [StringLength(255)] public required string Username { get; set; }

    /// <summary>
    ///     The hashed password of the user.
    /// </summary>
    [StringLength(255)] public required string PasswordHash { get; set; }

    /// <summary>
    ///     The salt used for the user's password hash.
    /// </summary>
    public required byte[] PasswordSalt { get; set; }

    /// <summary>
    ///     The email of the user.
    /// </summary>
    [StringLength(255)] public required string Email { get; set; }

    /// <summary>
    ///     The first name of the user.
    /// </summary>
    [StringLength(255)] public required string FirstName { get; set; }

    /// <summary>
    ///     The middle name of the user.
    /// </summary>
    [StringLength(255)] public string? MiddleName { get; set; }

    /// <summary>
    ///     The last name of the user.
    /// </summary>
    [StringLength(255)] public required string LastName { get; set; }

    /// <summary>
    ///     The full name of the user.
    /// </summary>
    [StringLength(255)] public required string FullName { get; set; }

    /// <summary>
    ///     The birthdate of the user.
    /// </summary>
    [StringLength(10)] public required string BirthDate { get; set; }

    /// <summary>
    ///     The street address of the user.
    /// </summary>
    [StringLength(255)] public required string AddressStreet { get; set; }
    
    /// <summary>
    ///     The barangay of the user's address.
    /// </summary>
    [StringLength(255)] public string? AddressBarangay { get; set; }

    /// <summary>
    ///     The city of the user's address.
    /// </summary>
    [StringLength(255)] public required string AddressCity { get; set; }

    /// <summary>
    ///     The state of the user's address.
    /// </summary>
    [StringLength(255)] public required string AddressProvince { get; set; }

    /// <summary>
    ///     The country code of the user's address.
    /// </summary>
    [StringLength(2)] public required string AddressCountryCode { get; set; }

    /// <summary>
    ///     The zip code of the user's address.
    /// </summary>
    [StringLength(10)] public required string AddressZipCode { get; set; }

    /// <summary>
    ///     The phone number of the user.
    /// </summary>
    public required long PhoneNumber { get; set; }

    /// <summary>
    ///     The role of the user in the learning system.
    /// </summary>
    public required UserRole Role { get; set; }

    /// <summary>
    ///     The date when the user registered in the learning system.
    /// </summary>
    public required DateTime RegistrationDate { get; set; }
    
    /// <summary>
    ///     The status of the user in the learning system.
    /// </summary>
    public required UserStatus Status { get; set; }
    
    /// <summary>
    ///     The year level of the user in the learning system.
    /// </summary>
    public required LearnerYear YearLevel { get; set; }
}