using Reveche.LearnerInfoSystem.Console.Data;
using Reveche.LearnerInfoSystem.Data;
using Reveche.LearnerInfoSystem.Models;

namespace Reveche.LearnerInfoSystem.Console.Presentation;

public class LearnerMenu(IRepo repo, User loggedInUser)
{
    public void DisplayMenu()
    {
        Action[] actions = [ViewProfile, ViewCourses, ViewPrograms, ViewGrades, Settings];

        MenuUtils.DisplayMenu("Learner Menu", actions);
    }

    private void Settings()
    {
        Action[] actions = [ChangePassword, ChangePhoneNumber, ChangeAddress];
        MenuUtils.DisplayMenu("Settings", actions);
    }

    private void ChangeAddress()
    {
        var barangay = loggedInUser.AddressBarangay;
        Utils.GetStringUpdate("Street", loggedInUser.AddressStreet, out var street);
        if (bool.Parse(repo.GetSetting(6)!.Value))
            Utils.GetStringUpdate("Barangay", loggedInUser.AddressBarangay ?? "[MenuUtils.None]", out barangay);
        Utils.GetStringUpdate("City", loggedInUser.AddressCity, out var addressCity);
        Utils.GetStringUpdate("Province", loggedInUser.AddressProvince, out var addressProvince);
        Utils.GetStringUpdate("CountryCode", loggedInUser.AddressCountryCode, out var addressCountryCode);
        Utils.GetStringUpdate("ZipCode", loggedInUser.AddressZipCode, out var addressZipCode);

        repo.UpdateUser(loggedInUser.Id, new User
        {
            Id = loggedInUser.Id,
            Email = loggedInUser.Email,
            Role = loggedInUser.Role,
            BirthDate = loggedInUser.BirthDate,
            Status = loggedInUser.Status,
            Username = loggedInUser.Username,
            UserIdStr = loggedInUser.UserIdStr,
            PasswordHash = loggedInUser.PasswordHash,
            PasswordSalt = loggedInUser.PasswordSalt,
            FirstName = loggedInUser.FirstName,
            LastName = loggedInUser.LastName,
            FullName = loggedInUser.FullName,
            PhoneNumber = loggedInUser.PhoneNumber,
            RegistrationDate = loggedInUser.RegistrationDate,
            YearLevel = loggedInUser.YearLevel,
            AddressStreet = street,
            AddressBarangay = barangay,
            AddressCity = addressCity,
            AddressProvince = addressProvince,
            AddressCountryCode = addressCountryCode,
            AddressZipCode = addressZipCode
        });
    }

    private void ChangePhoneNumber()
    {
        Utils.GetStringUpdate("Phone Number", loggedInUser.PhoneNumber.ToString(), out var phoneNumber);
        repo.UpdateUser(loggedInUser.Id, new User
        {
            Id = loggedInUser.Id,
            Email = loggedInUser.Email,
            Role = loggedInUser.Role,
            BirthDate = loggedInUser.BirthDate,
            Status = loggedInUser.Status,
            Username = loggedInUser.Username,
            UserIdStr = loggedInUser.UserIdStr,
            PasswordHash = loggedInUser.PasswordHash,
            PasswordSalt = loggedInUser.PasswordSalt,
            FirstName = loggedInUser.FirstName,
            LastName = loggedInUser.LastName,
            FullName = loggedInUser.FullName,
            PhoneNumber = int.Parse(phoneNumber),
            RegistrationDate = loggedInUser.RegistrationDate,
            YearLevel = loggedInUser.YearLevel,
            AddressStreet = loggedInUser.AddressStreet,
            AddressBarangay = loggedInUser.AddressBarangay,
            AddressCity = loggedInUser.AddressCity,
            AddressProvince = loggedInUser.AddressProvince,
            AddressCountryCode = loggedInUser.AddressCountryCode,
            AddressZipCode = loggedInUser.AddressZipCode
        });
    }

    private void ChangePassword()
    {
        var credentials = new Credentials();
        var oldPassword = Utils.GetUserStringInput("Old Password");

        if (!credentials.VerifyPassword(oldPassword, loggedInUser.PasswordHash, loggedInUser.PasswordSalt))
        {
            Boxes.DrawCenteredBox("Incorrect Password");
            System.Console.ReadKey();
            return;
        }

        var newPassword = Utils.GetUserStringInput("New Password");
        var confirmNewPassword = Utils.GetUserStringInput("Confirm New Password");
        if (newPassword != confirmNewPassword)
        {
            Boxes.DrawCenteredBox("Passwords do not match");
            System.Console.ReadKey();
            return;
        }

        var hash = credentials.HashPassword(newPassword, out var salt);
        repo.UpdateUser(loggedInUser.Id, new User
        {
            Id = loggedInUser.Id,
            Email = loggedInUser.Email,
            Role = loggedInUser.Role,
            BirthDate = loggedInUser.BirthDate,
            Status = loggedInUser.Status,
            Username = loggedInUser.Username,
            UserIdStr = loggedInUser.UserIdStr,
            PasswordHash = hash,
            PasswordSalt = salt,
            FirstName = loggedInUser.FirstName,
            LastName = loggedInUser.LastName,
            FullName = loggedInUser.FullName,
            PhoneNumber = loggedInUser.PhoneNumber,
            RegistrationDate = loggedInUser.RegistrationDate,
            YearLevel = loggedInUser.YearLevel,
            AddressStreet = loggedInUser.AddressStreet,
            AddressBarangay = loggedInUser.AddressBarangay,
            AddressCity = loggedInUser.AddressCity,
            AddressProvince = loggedInUser.AddressProvince,
            AddressCountryCode = loggedInUser.AddressCountryCode,
            AddressZipCode = loggedInUser.AddressZipCode
        });
    }

    private void ViewGrades()
    {
        var headers = new[] { "Course", "Status", "Grade" };
        var courseCompletions = repo.GetCourseCompletionsByUser(loggedInUser.Id);
        var data = courseCompletions.Select(x => new[]
        {
            repo.GetCourse(x.CourseId)!.Code + repo.GetCourse(x.CourseId)!.Title,
            Enum.GetName(x.Status)!,
            x.Grade.ToString() ?? "N/A"
        }).ToArray();
        Boxes.CreateLazyTable(headers, data);
    }

    private void ViewPrograms()
    {
        var headers = new[] { "ID", "Program Name", "Description" };
        var programTrackers = repo.GetProgramTrackersByUser(loggedInUser.Id);
        var programs = new List<(int, int)>();
        foreach (var programTracker in programTrackers)
            programTracker.Programs.ForEach(x => programs.Add((programTracker.Id, x.ProgramId)));
        var programInfo = programs.Select(x => new[]
        {
            x.Item2.ToString(), repo.GetProgram(x.Item2)!.Title,
            repo.GetProgram(x.Item2)!.Description.Length > 40
                ? repo.GetProgram(x.Item2)!.Description[..37] + "..."
                : repo.GetProgram(x.Item2)!.Description
        }).ToArray();
        Boxes.CreateLazyTable(headers, programInfo);
    }

    private void ViewCourses()
    {
        var headers = new[] { "ID", "Course Name", "Description", "Units" };
        var courseCompletions = repo.GetCourseCompletionsByUser(loggedInUser.Id);
        var learnerCourses = courseCompletions.Select(courseCompletion => repo.GetCourse(courseCompletion.CourseId)!)
            .ToList();
        var courseInfo = learnerCourses.Select(x => new[]
        {
            x.Code, x.Title, x.Description.Length > 40 ? x.Description[..37] + "..." : x.Description, x.Units.ToString()
        }).ToArray();
        Boxes.CreateLazyTable(headers, courseInfo);
    }

    private void ViewProfile()
    {
        string[] userDetails =
        [
            "Learner ID: " + loggedInUser.UserIdStr,
            "Name: " + loggedInUser.FullName,
            "Birthdate: " + loggedInUser.BirthDate,
            $"Address: {loggedInUser.AddressStreet}, {(!string.IsNullOrEmpty(loggedInUser.AddressBarangay) ? loggedInUser.AddressBarangay + ", " : "")}{loggedInUser.AddressCity}, " +
            $"{loggedInUser.AddressProvince}, {loggedInUser.AddressCountryCode}",
            $"Phone Number: {CountryJsonRepo.GetCountryInfo(loggedInUser.AddressCountryCode)?.Phone[0]} {loggedInUser.PhoneNumber}",
            "Email: " + loggedInUser.Email,
            "Username: " + loggedInUser.Username,
            "Year: " + Enum.GetName(loggedInUser.YearLevel)!.Replace("Year", " Year")
        ];
        Boxes.DrawCenteredBox(userDetails);
        System.Console.ReadKey();
    }
}