using Reveche.LearnerInfoSystem.Console.Data;
using Reveche.LearnerInfoSystem.Data;
using Reveche.LearnerInfoSystem.Models;

namespace Reveche.LearnerInfoSystem.Console.Presentation;

public class InstructorMenu(IRepo repo, User loggedInUser)
{
    public void DisplayMenu()
    {
        Action[] actions = [ViewProfile, ViewCourses, EditGrades, Settings];

        MenuUtils.DisplayMenu("Instructor Menu", actions);
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

    private void EditGrades()
    {
        var course = SelectCourse();
        var students = repo.GetStudentsByCourse(course.Id);
        var studentList = students.Select(s => s.FullName).ToList();
        var selectedStudent = Boxes.SingleSelectionBox(studentList);
        var student = students.First(s => s.FullName == selectedStudent);
        var completion = repo.GetCourseCompletions().First(c => c.UserId == student.Id && c.CourseId == course.Id);
        Utils.GetDoubleUpdate("Grade", completion.Grade.ToString() ?? "0", out var newGrade);

        Boxes.DrawCenteredQuestionBox("Are you sure you want to update the grade?");
        System.Console.Write("Y/N: ");
        if (System.Console.ReadKey().Key != ConsoleKey.Y)
        {
            Boxes.DrawCenteredBox("Grade not updated");
            System.Console.ReadKey();
        }

        repo.UpdateCourseCompletion(completion.Id, new CourseCompletion
        {
            Id = completion.Id,
            UserId = completion.UserId,
            CourseId = completion.CourseId,
            InstructorId = completion.InstructorId,
            Status = Status.Completed,
            DateCompleted = DateTime.Now,
            Grade = double.Parse(newGrade)
        });
        Boxes.DrawCenteredBox("Grade updated");
        System.Console.ReadKey();
    }

    private Course SelectCourse()
    {
        var courses = repo.GetCoursesByInstructor(loggedInUser.Id);
        var courseList = courses.Select(c => c.Title).ToList();
        var selectedCourse = Boxes.SingleSelectionBox(courseList);
        return courses.First(c => c.Title == selectedCourse);
    }

    private void ViewCourses()
    {
        var headers = new[] { "Code", "Title", "Description", "Year", "Term", "Duration", "Type", "Units" };
        var courses = repo.GetCoursesByInstructor(loggedInUser.Id).Select(x => new[]
        {
            x.Code,
            x.Title.Length > 130 ? x.Title[..127] + "..." : x.Title,
            x.Description.Length > 40 ? x.Description[..37] + "..." : x.Description,
            x.Year.ToString(),
            x.Term,
            x.DurationInHours.ToString(),
            x.Type.ToString(),
            x.Units.ToString()
        }).ToArray();
        Boxes.CreateLazyTable(headers, courses);
    }

    private void ViewProfile()
    {
        string[] userDetails =
        [
            "Instructor ID: " + loggedInUser.UserIdStr,
            "Name: " + loggedInUser.FullName,
            "Birthdate: " + loggedInUser.BirthDate,
            $"Address: {loggedInUser.AddressStreet}, {(!string.IsNullOrEmpty(loggedInUser.AddressBarangay) ? loggedInUser.AddressBarangay + ", " : "")}{loggedInUser.AddressCity}, " +
            $"{loggedInUser.AddressProvince}, {loggedInUser.AddressCountryCode}",
            $"Phone Number: {CountryJsonRepo.GetCountryInfo(loggedInUser.AddressCountryCode)?.Phone[0]} {loggedInUser.PhoneNumber}",
            "Email: " + loggedInUser.Email,
            "Username: " + loggedInUser.Username
        ];
        Boxes.DrawCenteredBox(userDetails);
        System.Console.ReadKey();
    }
}