using System.Text.Json;
using System.Text.Json.Serialization;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Data;

public class JsonRepo : IRepo
{
    private static readonly JsonSerializerOptions SourceGenOptions = new()
    {
        TypeInfoResolver = LearnerInfoSystemJsonContext.Default,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };

    private static readonly LearnerInfoSystemJsonContext Context = new(SourceGenOptions);
    private readonly string _jsonPath = Application.JsonFileName;

    public JsonRepo()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (File.Exists(_jsonPath)) return;
        var data = new JsonData { Settings = Default.DefaultSettings };
        var json = JsonSerializer.Serialize(data, Context.JsonData);
        File.WriteAllText(_jsonPath, json);
    }

    public void AddUser(User user)
    {
        var data = GetInfo();
        data.Users.Add(user);
        SaveData(data);
    }

    public void RemoveUser(int id)
    {
        var data = GetInfo();
        var user = data.Users.Find(u => u.Id == id);
        if (user is not null && user.Role == UserRole.Learner)
        {
            data.Users.RemoveAll(u => u.Id == id);
            data.ProgramTrackers.RemoveAll(p => p.UserId == id);
            data.CourseCompletions.RemoveAll(c => c.UserId == id);
        }
        SaveData(data);
    }

    public void UpdateUser(int id, User user)
    {
        var data = GetInfo();
        var index = data.Users.FindIndex(u => u.Id == id);
        data.Users[index] = user;
        SaveData(data);
    }

    public List<User> GetUsers()
    {
        return GetInfo().Users;
    }

    public User? GetUser(int id)
    {
        return GetInfo().Users.Find(u => u.Id == id);
    }

    public void AddProgram(Program program)
    {
        var data = GetInfo();
        data.Programs.Add(program);
        SaveData(data);
    }

    public void RemoveProgram(int id)
    {
        var data = GetInfo();
        var program = data.Programs.Find(p => p.ProgramId == id);
        if (program is not null)
        {
            data.Programs.RemoveAll(p => p.ProgramId == id);
        }
        SaveData(data);
    }

    public void UpdateProgram(int id, Program program)
    {
        var data = GetInfo();
        var index = data.Programs.FindIndex(p => p.ProgramId == id);
        data.Programs[index] = program;
        SaveData(data);
    }

    public List<Program> GetPrograms()
    {
        return GetInfo().Programs;
    }

    public Program? GetProgram(int id)
    {
        return GetInfo().Programs.Find(p => p.ProgramId == id);
    }

    public void AddCourse(Course course)
    {
        var data = GetInfo();
        data.Courses.Add(course);
        SaveData(data);
    }

    public void RemoveCourse(int id)
    {
        var data = GetInfo();
        data.Courses.RemoveAll(c => c.Id == id);
        SaveData(data);
    }

    public void UpdateCourse(int id, Course course)
    {
        var data = GetInfo();
        var index = data.Courses.FindIndex(c => c.Id == id);
        data.Courses[index] = course;
        SaveData(data);
    }

    public IEnumerable<Course> GetCourses()
    {
        return GetInfo().Courses;
    }

    public Course? GetCourse(int id)
    {
        return GetInfo().Courses.Find(c => c.Id == id);
    }

    public void AddCourseCompletion(CourseCompletion courseCompletion)
    {
        var data = GetInfo();
        data.CourseCompletions.Add(courseCompletion);
        SaveData(data);
    }

    public void RemoveCourseCompletion(int id)
    {
        var data = GetInfo();
        data.CourseCompletions.RemoveAll(cc => cc.Id == id);
        SaveData(data);
    }

    public void UpdateCourseCompletion(int id, CourseCompletion courseCompletion)
    {
        var data = GetInfo();
        var index = data.CourseCompletions.FindIndex(cc => cc.Id == id);
        data.CourseCompletions[index] = courseCompletion;
        SaveData(data);
    }

    public List<CourseCompletion> GetCourseCompletions()
    {
        return GetInfo().CourseCompletions;
    }

    public CourseCompletion? GetCourseCompletion(int id)
    {
        return GetInfo().CourseCompletions.Find(cc => cc.Id == id);
    }

    public void AddProgramTracker(ProgramTracker programTracker)
    {
        var data = GetInfo();
        data.ProgramTrackers.Add(programTracker);
        SaveData(data);
    }

    public void RemoveProgramTracker(int id)
    {
        var data = GetInfo();
        data.ProgramTrackers.RemoveAll(pt => pt.UserId == id);
        SaveData(data);
    }

    public void UpdateProgramTracker(int id, ProgramTracker programTracker)
    {
        var data = GetInfo();
        var index = data.ProgramTrackers.FindIndex(pt => pt.UserId == id);
        data.ProgramTrackers[index] = programTracker;
        SaveData(data);
    }

    public List<ProgramTracker> GetProgramTrackers()
    {
        return GetInfo().ProgramTrackers;
    }

    public ProgramTracker? GetProgramTracker(int id)
    {
        return GetInfo().ProgramTrackers.Find(pt => pt.UserId == id);
    }

    public void AddSetting(Setting setting)
    {
        var data = GetInfo();
        data.Settings.Add(setting);
        SaveData(data);
    }

    public void UpdateSetting(int id, string value)
    {
        var data = GetInfo();
        var index = data.Settings.FindIndex(s => s.Id == id);
        data.Settings[index].Value = value;
        SaveData(data);
    }

    public List<Setting> GetSettings()
    {
        return GetInfo().Settings;
    }

    public Setting? GetSetting(int id)
    {
        return GetInfo().Settings.Find(s => s.Id == id);
    }


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
                RegistrationDate = default
            };
            return true;
        }

        loggedInUser = users.Find(user =>
            (user.Username == username || user.Email == email) &&
            credentials.VerifyPassword(password, user.PasswordHash, user.PasswordSalt));
        return loggedInUser != null;
    }

    private JsonData GetInfo()
    {
        var json = File.ReadAllText(_jsonPath);
        return JsonSerializer.Deserialize(json, Context.JsonData) ??
               new JsonData { Settings = Default.DefaultSettings };
    }

    private void SaveData(JsonData data)
    {
        var json = JsonSerializer.Serialize(data, Context.JsonData);
        File.WriteAllText(_jsonPath, json);
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(JsonData))]
internal partial class LearnerInfoSystemJsonContext : JsonSerializerContext;