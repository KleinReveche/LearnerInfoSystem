using Reveche.LearnerInfoSystem.Console.Data;
using Reveche.LearnerInfoSystem.Data;
using Reveche.LearnerInfoSystem.Models;

namespace Reveche.LearnerInfoSystem.Console.Presentation;

public partial class AdminMenu
{
    private void ManageInstructor()
    {
        Action[] actions =
        [
            SearchInstructor, AddInstructor, UpdateInstructor, RemoveInstructor, ResetInstructorPassword,
            DisplayInstructorList
        ];
        MenuUtils.DisplayMenu("Manage Instructor", actions);
    }

    private void AddInstructor() => AddUser(UserRole.Instructor, out _);
    private void UpdateInstructor() => UpdateUser(UserRole.Instructor);
    private void RemoveInstructor() => RemoveUser(UserRole.Instructor);
    private void ResetInstructorPassword() => ResetPassword(UserRole.Instructor);
    private void SearchInstructor() => SearchUser(UserRole.Instructor);

    private void DisplayInstructorList()
    {
        if (repo.GetInstructors().Count == 0)
        {
            Boxes.DrawCenteredBox("No instructor found. Please add an instructor first.");
            System.Console.ReadKey();
            return;
        }

        var countryInfos = CountryJsonRepo.GetCountryInfos().Countries;
        var headers = new[] { "ID", "Name", "Email", "Phone Number" };
        var instructor = repo.GetInstructors()
            .Select(x => new[]
            {
                x.UserIdStr, x.FullName, x.BirthDate, Utils.GetAge(x.BirthDate),
                $"{countryInfos.GetValueOrDefault(x.AddressCountryCode)?.Phone[0]} {x.PhoneNumber}",
                $"{x.AddressCity}, {x.AddressProvince}, {x.AddressCountryCode}"
            }).ToArray();

        Boxes.CreateLazyTable(headers, instructor);
        System.Console.ReadKey();
    }
}