using Faker;
using Reveche.LearnerInfoSystem.Models;
using Country = Faker.Country;

namespace Reveche.LearnerInfoSystem.Console.Data;

public class RandomData(IRepo repo)
{
    public void GenerateData()
    {
        if (!File.Exists("CourseTestingData.csv") || !File.Exists("ProgramTestingData.csv")) return;
        GenerateInstructors(50);
        GenerateCourses();
        GeneratePrograms();
        GenerateStudents(100);
    }

    private void GenerateCourses()
    {
        using var file = new StreamReader("CourseTestingData.csv");
        var instructors = repo.GetUsers().Where(u => u.Role == UserRole.Instructor).ToList();
        var generatedCourses = new List<Course>();
        var units = new List<int> { 1, 2, 3, 4, 5 };
        var counter = 0;

        foreach (var line in file.ReadToEnd().Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Skip(1))
        {
            var data = line.Split(",");
            var code = data[2] + data[03];
            var random = new Random();

            if (generatedCourses.Any(c => c.Code == code)) continue;
            var desc = data[5].Replace("&comma", ",").Replace("&amp;", "&");
            var course = new Course
            {
                Id = counter,
                Code = code,
                Title = data[4].Replace("&comma", ",").Replace("&amp;", "&"),
                Description = desc.Length > 255 ? desc[..252] + "..." : desc,
                InstructorId = instructors[random.Next(0, instructors.Count - 1)].Id,
                Year = int.Parse(data[0]),
                Term = data[1],
                DurationInHours = random.Next(1, 4) * 15,
                Type = data[7] switch
                {
                    "LEC" => CourseType.Lecture,
                    "LAB" => CourseType.Laboratory,
                    "IND" => CourseType.IndependentStudy,
                    _ => CourseType.Lecture
                },
                Units = units[random.Next(0, units.Count - 1)]
            };
            generatedCourses.Add(course);
            counter++;
        }

        repo.AddCourses(generatedCourses);
    }

    private void GeneratePrograms()
    {
        using var file = new StreamReader("ProgramTestingData.csv");
        var generatedPrograms = new List<Program>();
        var courses = repo.GetCourses();
        foreach (var line in file.ReadToEnd().Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Skip(1))
        {
            var data = line.Split(",");
            var randomCourses = new List<Course>();

            for (var i = 0; i < 10; i++)
            {
                var random = new Random();
                var randomCourse = courses[random.Next(0, courses.Count - 1)];
                if (!randomCourses.Contains(randomCourse)) randomCourses.Add(randomCourse);
            }

            var program = new Program
            {
                Id = int.Parse(data[0]),
                Code = data[1],
                Description = data[1],
                Title = data[2],
                Status = ProgramStatus.Active,
                Courses = randomCourses
            };
            generatedPrograms.Add(program);
        }

        repo.AddPrograms(generatedPrograms);
    }

    private void GenerateStudents(int count)
    {
        var users = repo.GetUsers();
        var learners = new List<User>();
        var courseCompletions = new List<CourseCompletion>();
        var programTrackers = new List<ProgramTracker>();
        for (var i = 0; i < count; i++)
        {
            var random = new Random();
            var firstName = Name.First();
            var middleName = Name.Middle();
            var lastName = Name.Last();
            var fullName = $"{firstName} {middleName} {lastName}";
            var yearInt = random.Next(1, 7);
            var year = yearInt switch
            {
                1 => LearnerYear.FirstYear,
                2 => LearnerYear.SecondYear,
                3 => LearnerYear.ThirdYear,
                4 => LearnerYear.FourthYear,
                5 => LearnerYear.FifthYear,
                6 => LearnerYear.SixthYear,
                7 => LearnerYear.SeventhYear,
                _ => LearnerYear.FirstYear
            };
            var statusInt = random.Next(0, 4);
            var status = statusInt switch
            {
                0 => UserStatus.ActiveLearner,
                1 => UserStatus.GraduatedLearner,
                2 => UserStatus.DroppedLearner,
                3 => UserStatus.SuspendedLearner,
                4 => UserStatus.ExpelledLearner,
                _ => UserStatus.ActiveLearner
            };
            users.AddRange(learners);
            var learner = GenerateUser(firstName, middleName, lastName, fullName, year, status, users);
            learners.Add(learner);

            GenerateProgramForLearner(learner, out var courseCompletionsT, out var programTracker);
            courseCompletions.AddRange(courseCompletionsT);
            programTrackers.Add(programTracker);
        }

        repo.AddUsers(learners);
        repo.AddCourseCompletions(courseCompletions);
        repo.AddProgramTrackers(programTrackers);
    }

    private void GenerateInstructors(int count)
    {
        var users = repo.GetUsers();
        var instructors = new List<User>();
        for (var i = 0; i < count; i++)
        {
            var firstName = Name.First();
            var middleName = Name.Middle();
            var lastName = Name.Last();
            var fullName = $"{firstName} {middleName} {lastName}";
            users.AddRange(instructors);
            instructors.Add(GenerateUser(firstName, middleName, lastName, fullName, LearnerYear.NotApplicable,
                UserStatus.Instructing, users, false));
        }

        repo.AddUsers(instructors);
    }

    private User GenerateUser(string firstName, string middleName, string lastName, string fullName, LearnerYear year,
        UserStatus status, List<User> users, bool isLearner = true)
    {
        var (hash, salt) = Default.DefaultUserCredential();
        var random = new Random();
        var user = new User
        {
            Id = Utils.GetUniqueId(users),
            Username = Utils.GetUserDefaultUsername(firstName, lastName, users),
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = isLearner ? UserRole.Learner : UserRole.Instructor,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            FullName = fullName,
            BirthDate = Identification.DateOfBirth().ToString("yyyy-MM-dd"),
            AddressStreet = Address.StreetAddress(),
            AddressBarangay = Address.SecondaryAddress(),
            AddressCity = Address.City(),
            AddressProvince = Address.UsState(),
            AddressCountryCode = Country.TwoLetterCode(),
            PhoneNumber = random.Next(1000, 1000000),
            Email = Utils.GetEmail(firstName, middleName, lastName, repo.GetSetting(5)!.Value, users),
            RegistrationDate = DateTime.Now,
            UserIdStr = isLearner
                ? Utils.GetLearnerId(2023, 2024, users)
                : Utils.GetInstructorId(firstName, middleName, lastName, users),
            YearLevel = year,
            Status = status,
            AddressZipCode = Address.ZipCode()
        };

        return user;
    }


    private void GenerateProgramForLearner(User learner, out List<CourseCompletion> courseCompletions,
        out ProgramTracker programTracker)
    {
        var randomProgram = repo.GetPrograms().OrderBy(_ => Guid.NewGuid()).First();
        var courseCompletionsTemp = (from course in randomProgram.Courses
            let random = new Random()
            let isDone = random.Next(0, 100) > 50
            select new CourseCompletion
            {
                Id = Utils.GetUniqueId(repo.GetCourseCompletions()),
                UserId = learner.Id,
                CourseId = course.Id,
                InstructorId = course.InstructorId,
                Status = isDone ? Status.Completed : Status.InProgress,
                DateCompleted = isDone ? DateTime.Now : null,
                Grade = isDone ? RandomNumber.Next(75, 100) : null
            }).ToList();

        var learnerCourses = repo.GetCourseCompletions().Where(c => c.UserId == learner.Id).ToList();
        var programDone = learnerCourses.All(c => c.Status == Status.Completed);
        var programProgress = new ProgramProgress
        {
            ProgramId = randomProgram.Id,
            Status = programDone ? Status.Completed : Status.InProgress,
            DateCompleted = programDone ? DateTime.Now : null
        };

        var tracker = new ProgramTracker
        {
            UserId = learner.Id,
            Id = Utils.GetUniqueId(repo.GetProgramTrackers()),
            Programs = [programProgress],
            Courses = learnerCourses
        };

        programTracker = tracker;
        courseCompletions = courseCompletionsTemp;
    }
}