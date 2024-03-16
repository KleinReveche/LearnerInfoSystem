using Reveche.SimpleLearnerInfoSystem.Console.Data;
using Reveche.SimpleLearnerInfoSystem.Data;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public partial class AdminMenu
{
    private void ManageInstructor()
    {
        Action[] actions = [SearchInstructor, AddInstructor, UpdateInstructor, RemoveInstructor, ResetInstructorPassword, DisplayInstructorList];
        MenuUtils.DisplayMenu("Manage Instructor", actions);
    }
    
    private void AddInstructor() => AddUser(UserRole.Instructor, out _);
    private void UpdateInstructor() => UpdateUser(UserRole.Instructor);
    private void RemoveInstructor() => RemoveUser(UserRole.Instructor);
    private void ResetInstructorPassword() => ResetPassword(UserRole.Instructor);
    private void SearchInstructor() => SearchUser(UserRole.Instructor);

    private void DisplayInstructorList()
    {
        if (!Users.Any(x => x.Role is UserRole.Instructor or UserRole.Administrator))
        {
            Boxes.DrawCenteredBox("No instructor found. Please add an instructor first.");
            System.Console.ReadKey();
            return;
        }
        var countryInfos = CountryJsonRepo.GetCountryInfos().Countries;
        var headers = new[] { "ID", "Name", "Email", "Phone Number" };
        var instructor = Users.Where(x => x.Role is UserRole.Instructor or UserRole.Administrator)
            .Select(x => new[] { 
                x.UserIdStr, x.FullName, x.BirthDate, Utils.GetAge(x.BirthDate), 
                $"{countryInfos.GetValueOrDefault(x.AddressCountryCode)?.Phone[0]} {x.PhoneNumber}",
                $"{x.AddressCity}, {x.AddressProvince}, {x.AddressCountryCode}"
            }).ToArray();
        
        Boxes.CreateLazyTable(headers, instructor);
        System.Console.ReadKey();
    }
}