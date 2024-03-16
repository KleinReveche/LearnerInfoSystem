using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Data;

public class SqlRepo : IRepo
{
    private readonly DatabaseContext _db;
    public SqlRepo(DatabaseContext db)
    {
        _db = db;
        if (!db.Database.GetService<IRelationalDatabaseCreator>().Exists()) InitializeRepo();
    }

    private void InitializeRepo()
    {
        _db.Database.EnsureCreated();
        _db.Settings.AddRange(Default.DefaultSettings);
        _db.SaveChanges();
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
        _db.Users.Update(user);
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
        _db.Programs.Update(program);
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
        _db.Courses.Update(course);
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
        _db.CourseCompletions.Update(courseCompletion);
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
        _db.ProgramTrackers.Update(programTracker);
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
        var users = GetUsers();
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

        loggedInUser = users.Find(user =>
            (user.Username == username || user.Email == email) &&
            credentials.VerifyPassword(password, user.PasswordHash, user.PasswordSalt));
        return loggedInUser != null;
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
}