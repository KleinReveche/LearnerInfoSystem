using System.Collections;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem;

public interface IRepo
{
    /// <summary>
    ///     Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add.</param>
    void AddUser(User user);

    /// <summary>
    ///     Removes a user from the repository.
    /// </summary>
    /// <param name="id">The id of the user to remove.</param>
    void RemoveUser(int id);

    /// <summary>
    ///     Updates a user in the repository.
    /// </summary>
    /// <param name="id">The id of the user to update.</param>
    /// <param name="user">The updated user.</param>
    void UpdateUser(int id, User user);

    /// <summary>
    ///     Gets all users from the repository.
    /// </summary>
    /// <returns>A list of all users.</returns>
    List<User> GetUsers();

    /// <summary>
    ///     Gets a user from the repository.
    /// </summary>
    /// <param name="id">The id of the user to get.</param>
    /// <returns>The user with the given id.</returns>
    User? GetUser(int id);

    /// <summary>
    ///     Adds a new program to the repository.
    /// </summary>
    /// <param name="program">The program to add.</param>
    void AddProgram(Program program);

    /// <summary>
    ///     Removes a program from the repository.
    /// </summary>
    /// <param name="id">The id of the program to remove.</param>
    void RemoveProgram(int id);

    /// <summary>
    ///     Updates a program in the repository.
    /// </summary>
    /// <param name="id">The id of the program to update.</param>
    /// <param name="program">The updated program.</param>
    void UpdateProgram(int id, Program program);

    /// <summary>
    ///     Gets all programs from the repository.
    /// </summary>
    /// <returns>A list of all programs.</returns>
    List<Program> GetPrograms();

    /// <summary>
    ///     Gets a program from the repository.
    /// </summary>
    /// <param name="id">The id of the program to get.</param>
    /// <returns>The program with the given id.</returns>
    Program? GetProgram(int id);

    /// <summary>
    ///     Adds a new course to the repository.
    /// </summary>
    /// <param name="course">The course to add.</param>
    void AddCourse(Course course);

    /// <summary>
    ///     Removes a course from the repository.
    /// </summary>
    /// <param name="id">The id of the course to remove.</param>
    void RemoveCourse(int id);

    /// <summary>
    ///     Updates a course in the repository.
    /// </summary>
    /// <param name="id">The id of the course to update.</param>
    /// <param name="course">The updated course.</param>
    void UpdateCourse(int id, Course course);

    /// <summary>
    ///     Gets all courses from the repository.
    /// </summary>
    /// <returns>A list of all courses.</returns>
    List<Course> GetCourses();

    /// <summary>
    ///     Gets a course from the repository.
    /// </summary>
    /// <param name="id">The id of the course to get.</param>
    /// <returns>The course with the given id.</returns>
    Course? GetCourse(int id);

    /// <summary>
    ///     Adds a new course completion to the repository.
    /// </summary>
    /// <param name="courseCompletion">The course completion to add.</param>
    void AddCourseCompletion(CourseCompletion courseCompletion);

    /// <summary>
    ///     Removes a course completion from the repository.
    /// </summary>
    /// <param name="id">The id of the course completion to remove.</param>
    void RemoveCourseCompletion(int id);

    /// <summary>
    ///     Updates a course completion in the repository.
    /// </summary>
    /// <param name="id">The id of the course completion to update.</param>
    /// <param name="courseCompletion">The updated course completion.</param>
    void UpdateCourseCompletion(int id, CourseCompletion courseCompletion);

    /// <summary>
    ///     Gets all course completions from the repository.
    /// </summary>
    /// <returns>A list of all course completions.</returns>
    IEnumerable<CourseCompletion> GetCourseCompletions();

    /// <summary>
    ///     Gets a course completion from the repository.
    /// </summary>
    /// <param name="id">The id of the course completion to get.</param>
    /// <returns>The course completion with the given id.</returns>
    CourseCompletion? GetCourseCompletion(int id);
    
    /// <summary>
    ///     Adds a new ProgramTracker to the repository.
    /// </summary>
    /// <param name="programTracker">The ProgramTracker to add.</param>
    void AddProgramTracker(ProgramTracker programTracker);

    /// <summary>
    ///     Removes a ProgramTracker from the repository.
    /// </summary>
    /// <param name="id">The id of the ProgramTracker to remove.</param>
    void RemoveProgramTracker(int id);

    /// <summary>
    ///     Updates a ProgramTracker in the repository.
    /// </summary>
    /// <param name="id">The id of the ProgramTracker to update.</param>
    /// <param name="programTracker">The updated ProgramTracker.</param>
    void UpdateProgramTracker(int id, ProgramTracker programTracker);

    /// <summary>
    ///     Gets all ProgramTrackers from the repository.
    /// </summary>
    /// <returns>A list of all ProgramTrackers.</returns>
    IEnumerable<ProgramTracker> GetProgramTrackers();

    /// <summary>
    ///     Gets a ProgramTracker from the repository.
    /// </summary>
    /// <param name="id">The id of the ProgramTracker to get.</param>
    /// <returns>The ProgramTracker with the given id.</returns>
    ProgramTracker? GetProgramTracker(int id);

    /// <summary>
    ///     Adds a new setting to the repository.
    /// </summary>
    /// <param name="setting">The setting to add.</param>
    void AddSetting(Setting setting);

    /// <summary>
    ///     Updates a setting in the repository.
    /// </summary>
    /// <param name="id">The id of the setting to update.</param>
    /// <param name="value">The updated value of the setting.</param>
    void UpdateSetting(int id, string value);

    /// <summary>
    ///     Gets all settings from the repository.
    /// </summary>
    /// <returns>A list of all settings.</returns>
    List<Setting> GetSettings();

    /// <summary>
    ///     Gets a setting from the repository.
    /// </summary>
    /// <param name="id">The id of the setting to get.</param>
    /// <returns>The setting with the given id.</returns>
    Setting? GetSetting(int id);

    /// <summary>
    ///     Logs a user into the system.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="email">The email of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="loggedInUser">The user that was logged in.</param>
    /// <returns>True iƒ the user is logged in, otherwise false.</returns>
    bool Login(string username, string email, string password, out User? loggedInUser);
    
    void AddCourses(IEnumerable<Course> courses);
    void AddPrograms(IEnumerable<Program> programs);
    void AddUsers(IEnumerable<User> users);
    void AddCourseCompletions(IEnumerable<CourseCompletion> courseCompletions);
    void AddProgramTrackers(IEnumerable<ProgramTracker> programTrackers);
}

public enum RepoType
{
    Json,
    Sqlite,
    Mysql
}