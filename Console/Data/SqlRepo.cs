using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Reveche.LearnerInfoSystem.Models;

namespace Reveche.LearnerInfoSystem.Console.Data;

public class SqlRepo : IRepo
{
    private readonly DatabaseContext _db;

    public SqlRepo(DatabaseContext db)
    {
        _db = db;
        if (!db.Database.GetService<IRelationalDatabaseCreator>().Exists()) InitializeRepo();
        if (db is not MySqlDbContext) return;
        try
        {
            _ = _db.Settings.First();
        }
        catch
        {
            InitializeRepo();
        }
    }

    public void AddUser(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public void RemoveUser(int id)
    {
        var user = _db.Users.Find(id);
        _db.Users.Remove(user!);
        _db.SaveChanges();
    }

    public void UpdateUser(int id, User user)
    {
        var userToUpdate = _db.Users.Find(id);
        userToUpdate!.Username = user.Username;
        userToUpdate.Role = user.Role;
        userToUpdate.UserIdStr = user.UserIdStr;
        userToUpdate.PasswordHash = user.PasswordHash;
        userToUpdate.PasswordSalt = user.PasswordSalt;
        userToUpdate.Email = user.Email;
        userToUpdate.FirstName = user.FirstName;
        userToUpdate.MiddleName = user.MiddleName;
        userToUpdate.LastName = user.LastName;
        userToUpdate.FullName = user.FullName;
        userToUpdate.BirthDate = user.BirthDate;
        userToUpdate.AddressStreet = user.AddressStreet;
        userToUpdate.AddressBarangay = user.AddressBarangay;
        userToUpdate.AddressCity = user.AddressCity;
        userToUpdate.AddressProvince = user.AddressProvince;
        userToUpdate.AddressCountryCode = user.AddressCountryCode;
        userToUpdate.AddressZipCode = user.AddressZipCode;
        userToUpdate.PhoneNumber = user.PhoneNumber;
        userToUpdate.Status = user.Status;
        userToUpdate.RegistrationDate = user.RegistrationDate;
        userToUpdate.YearLevel = user.YearLevel;
        _db.Users.Update(userToUpdate);
        _db.SaveChanges();
    }

    public List<User> GetUsers() => _db.Users.ToList();

    public User? GetUser(int id) => _db.Users.Find(id);

    public void AddProgram(Program program)
    {
        _db.Programs.Add(program);
        _db.SaveChanges();
    }

    public void RemoveProgram(int id)
    {
        var program = _db.Programs.Find(id);
        _db.Programs.Remove(program!);
        _db.SaveChanges();
    }

    public void UpdateProgram(int id, Program program)
    {
        var programToUpdate = _db.Programs.Find(id);
        programToUpdate!.Code = program.Code;
        programToUpdate.Title = program.Title;
        programToUpdate.Description = program.Description;
        programToUpdate.Status = program.Status;
        programToUpdate.Courses = program.Courses;
        _db.Programs.Update(programToUpdate);
        _db.SaveChanges();
    }

    public List<Program> GetPrograms() => _db.Programs.ToList();

    public Program? GetProgram(int id) => _db.Programs.Find(id);

    public void AddCourse(Course course)
    {
        _db.Courses.Add(course);
        _db.SaveChanges();
    }

    public void RemoveCourse(int id)
    {
        var course = _db.Courses.Find(id);
        _db.Courses.Remove(course!);
        _db.SaveChanges();
    }

    public void UpdateCourse(int id, Course course)
    {
        var courseToUpdate = _db.Courses.Find(id);
        courseToUpdate!.Code = course.Code;
        courseToUpdate.Title = course.Title;
        courseToUpdate.Description = course.Description;
        courseToUpdate.Type = course.Type;
        courseToUpdate.DurationInHours = course.DurationInHours;
        courseToUpdate.InstructorId = course.InstructorId;
        courseToUpdate.Year = course.Year;
        courseToUpdate.Term = course.Term;
        courseToUpdate.Units = course.Units;
        
        _db.Courses.Update(courseToUpdate);
        _db.SaveChanges();
    }

    public List<Course> GetCourses() => _db.Courses.ToList();

    public Course? GetCourse(int id) => _db.Courses.Find(id);

    public void AddCourseCompletion(CourseCompletion courseCompletion)
    {
        _db.CourseCompletions.Add(courseCompletion);
        _db.SaveChanges();
    }

    public void RemoveCourseCompletion(int id)
    {
        var courseCompletion = _db.CourseCompletions.Find(id);
        _db.CourseCompletions.Remove(courseCompletion!);
        _db.SaveChanges();
    }

    public void UpdateCourseCompletion(int id, CourseCompletion courseCompletion)
    {
        var courseCompletionToUpdate = _db.CourseCompletions.Find(id);
        courseCompletionToUpdate!.CourseId = courseCompletion.CourseId;
        courseCompletionToUpdate.UserId = courseCompletion.UserId;
        courseCompletionToUpdate.DateCompleted = courseCompletion.DateCompleted;
        courseCompletionToUpdate.Status = courseCompletion.Status;
        courseCompletionToUpdate.InstructorId = courseCompletion.InstructorId;
        courseCompletionToUpdate.Grade = courseCompletion.Grade;
        _db.CourseCompletions.Update(courseCompletionToUpdate);
        _db.SaveChanges();
    }

    public IEnumerable<CourseCompletion> GetCourseCompletions() => _db.CourseCompletions.ToList();

    public CourseCompletion? GetCourseCompletion(int id) => _db.CourseCompletions.Find(id);

    public void AddProgramTracker(ProgramTracker programTracker)
    {
        _db.ProgramTrackers.Add(programTracker);
        _db.SaveChanges();
    }

    public void RemoveProgramTracker(int id)
    {
        var programTracker = _db.ProgramTrackers.Find(id);
        _db.ProgramTrackers.Remove(programTracker!);
        _db.SaveChanges();
    }

    public void UpdateProgramTracker(int id, ProgramTracker programTracker)
    {
        var programTrackerToUpdate = _db.ProgramTrackers.Find(id);
        programTrackerToUpdate!.UserId = programTracker.UserId;
        programTrackerToUpdate.Programs = programTracker.Programs;
        programTrackerToUpdate.Courses = programTracker.Courses;
        _db.ProgramTrackers.Update(programTrackerToUpdate);
        _db.SaveChanges();
    }

    public IEnumerable<ProgramTracker> GetProgramTrackers() => _db.ProgramTrackers.ToList();

    public ProgramTracker? GetProgramTracker(int id) => _db.ProgramTrackers.Find(id);

    public void AddSetting(Setting setting)
    {
        _db.Settings.Add(setting);
        _db.SaveChanges();
    }

    public void UpdateSetting(int id, string value)
    {
        var setting = _db.Settings.Find(id)!;
        setting.Value = value;
        _db.Settings.Update(setting);
        _db.SaveChanges();
    }

    public List<Setting> GetSettings() => _db.Settings.ToList();

    public Setting? GetSetting(int id) => _db.Settings.Find(id);

    public bool Login(string username, string email, string password, out User? loggedInUser)
    {
        var adminUsername = GetSetting(0)!.Value;
        var adminPassword = GetSetting(1)!.Value;
        var adminSalt = GetSetting(2)!.Value;
        var credentials = new Credentials();

        if (username == adminUsername && credentials.VerifyPassword(password, adminPassword,
                Convert.FromBase64String(adminSalt)))
        {
            loggedInUser = new User
            {
                Id = -1,
                Username = adminUsername,
                Role = UserRole.Administrator,
                UserIdStr = "",
                PasswordHash = adminPassword,
                PasswordSalt = Convert.FromBase64String(adminSalt),
                Email = "",
                FirstName = "",
                MiddleName = "",
                LastName = "",
                FullName = "",
                BirthDate = "",
                AddressStreet = "",
                AddressBarangay = "",
                AddressCity = "",
                AddressProvince = "",
                AddressCountryCode = "",
                AddressZipCode = "",
                PhoneNumber = 0,
                Status = UserStatus.Administrator,
                RegistrationDate = default,
                YearLevel = LearnerYear.NotApplicable
            };
            return true;
        }


        loggedInUser = _db.Users.First(user => user.Username == username || user.Email == email);
        if (credentials.VerifyPassword(password, loggedInUser.PasswordHash, loggedInUser.PasswordSalt))
            return true;

        loggedInUser = null;
        return false;
    }

    public void AddCourses(IEnumerable<Course> courses)
    {
        _db.Courses.AddRange(courses);
        _db.SaveChanges();
    }

    public void AddPrograms(IEnumerable<Program> programs)
    {
        _db.Programs.AddRange(programs);
        _db.SaveChanges();
    }

    public void AddUsers(IEnumerable<User> users)
    {
        _db.Users.AddRange(users);
        _db.SaveChanges();
    }

    public void AddCourseCompletions(IEnumerable<CourseCompletion> courseCompletions)
    {
        _db.CourseCompletions.AddRange(courseCompletions);
        _db.SaveChanges();
    }

    public void AddProgramTrackers(IEnumerable<ProgramTracker> programTrackers)
    {
        _db.ProgramTrackers.AddRange(programTrackers);
        _db.SaveChanges();
    }

    public List<User> GetLearners() => _db.Users.Where(user => user.Role == UserRole.Learner).ToList();
    public List<User> GetInstructors() => _db.Users.Where(user => user.Role == UserRole.Instructor).ToList();

    public List<CourseCompletion> GetCourseCompletionsByUser(int userId) =>
        _db.CourseCompletions.Where(x => x.UserId == userId).ToList();

    public List<ProgramTracker> GetProgramTrackersByUser(int userId) =>
        _db.ProgramTrackers.Where(x => x.UserId == userId).ToList();

    public List<Course> GetCoursesByInstructor(int instructorId) =>
        _db.Courses.Where(x => x.InstructorId == instructorId).ToList();

    public List<User> GetStudentsByCourse(int courseId) =>
        _db.Users.Where(x => GetCourseCompletionsByCourse(courseId).Any(y => y.UserId == x.Id)).ToList();

    public User? GetUserByFullName(string fullName) => _db.Users.FirstOrDefault(x => x.FullName == fullName);

    private void InitializeRepo()
    {
        _db.Database.Migrate();
        _db.Settings.AddRange(Default.DefaultSettings);
        _db.SaveChanges();
    }

    public List<CourseCompletion> GetCourseCompletionsByCourse(int courseId) =>
        _db.CourseCompletions.Where(x => x.CourseId == courseId).ToList();

    public List<ProgramTracker> GetProgramTrackersByProgram(int programId) =>
        _db.ProgramTrackers.Where(x => x.Programs.Any(y => y.ProgramId == programId)).ToList();

    public List<Course> GetCoursesByCourseCompletions(IEnumerable<CourseCompletion> courseCompletions) =>
        _db.Courses.Where(x => courseCompletions.Any(y => y.CourseId == x.Id)).ToList();
}