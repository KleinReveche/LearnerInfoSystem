using Reveche.SimpleLearnerInfoSystem.Console.Data;
using Reveche.SimpleLearnerInfoSystem.Data;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public partial class AdminMenu(IRepo repo)
{
    private readonly ProgramMenu _programMenu = new(repo);
    private readonly CourseMenu _courseMenu = new(repo);
    private (int YearStart, int YearEnd) _academicYear = (DateTime.Now.Year, DateTime.Now.Year + 1);

    private List<Program> Programs => repo.GetPrograms();
    private List<User> Users => repo.GetUsers();
    
    public void DisplayMenu()
    {
        MenuUtils.GetAcademicYear(out _academicYear);
        Action[] actions = [SearchStudent, ManageStudent, DisplayStudentList, ManageStaff, _programMenu.ManagePrograms, _courseMenu.ManageCourses, AdminSettings];
        MenuUtils.DisplayMenu("Admin Menu", actions);
    }
    
    private void AdminSettings()
    {
        Action[] actions = [ChangeAdminCredential, ChangeIdFormat, ChangeEmailFormat, ChangeEmailDomain, ShowSettings];
        MenuUtils.DisplayMenu("Admin Settings", actions);
    }
    
    private List<Setting> GetSettings() => repo.GetSettings();
    
    private void ChangeAdminCredential()
    {
        while (true)
        {
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup Admin User", "Enter new Username: ");
            var newUsername = System.Console.ReadLine();
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup Admin User", "Repeat new Username: ");
            var repeatUsername = System.Console.ReadLine();

            if (newUsername != repeatUsername) continue;

            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup Admin User", "Enter new Password: ");
            var newPassword = Utils.GetHiddenConsoleInput();

            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup Admin User", "Repeat new Password: ");
            var repeatPassword = Utils.GetHiddenConsoleInput();

            if (newPassword != repeatPassword) continue;
            var credentials = new Credentials();
            var settings = GetSettings();
            var usernameSetting = settings.First(x => x.Key == "AdminUsername");
            var passwordSetting = settings.First(x => x.Key == "AdminPasswordHash");
            var saltSetting = settings.First(x => x.Key == "AdminSalt");
            repo.UpdateSetting(usernameSetting.Id, newUsername ?? usernameSetting.Value);
            repo.UpdateSetting(passwordSetting.Id, credentials.HashPassword(newPassword, out var salt));
            repo.UpdateSetting(saltSetting.Id, Convert.ToBase64String(salt));
            
            Boxes.DrawHeaderAndQuestionBox("StudentBook", "New credential saved.", 6);
            break;
        }
    }
    
    private void ChangeIdFormat()
    {
        while (true)
        {
            var idFormatSetting = GetSettings().First(x => x.Key == "AdminIdFormatting");
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Enter new ID Format: ", [
                "Legend: ",
                "SY - Start Year Shortened [23]",
                "EY - End Year Shortened [24]",
                "SYYY - Start Year Full [2023]",
                "EYYY - End Year Full [2024]",
                "# - Represents a number",
                "Example: SY-EY-#### for 23-24-0001"
            ], hasIndicator: true, defaultInput: idFormatSetting.Value);
            var newIdFormat = System.Console.ReadLine();

            if (string.IsNullOrEmpty(newIdFormat)) newIdFormat = idFormatSetting.Value;
            
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to change ID format to {newIdFormat}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) continue;
            repo.UpdateSetting(idFormatSetting.Id, newIdFormat);
            break;
        }
    }

    private void ChangeEmailFormat()
    {
        while (true)
        {
            var emailFormatSetting = GetSettings().First(x => x.Key == "AdminEmailFormatting");
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Enter new Email Format: ", [
                "Legend: ",
                "FN - First Name",
                "MN - Middle Name",
                "LN - Last Name",
                "FI - First Name Initial",
                "MI - Middle Name Initial",
                "LI - Last Name Initial",
                "ED - Email Domain",
                "Example: FNLN@ED for juandelacruz@institution.edu.ph"
            ], hasIndicator: true, defaultInput: emailFormatSetting.Value);
            var newEmailFormat = System.Console.ReadLine();

            if (string.IsNullOrEmpty(newEmailFormat)) newEmailFormat = emailFormatSetting.Value;
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to change Email format to {newEmailFormat}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) continue;
            repo.UpdateSetting(emailFormatSetting.Id, newEmailFormat);
            break;
        }
    }

    private void ChangeEmailDomain()
    {
        while (true)
        {
            var emailDomainSetting = GetSettings().First(x => x.Key == "AdminEmailDomain");
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Enter institution's Email Domain: ", [
                "Example: institution.com.edu"
            ], hasIndicator: true, defaultInput: emailDomainSetting.Value);
            var emailDomain = System.Console.ReadLine();

            if (string.IsNullOrEmpty(emailDomain)) emailDomain = emailDomainSetting.Value;
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to change Email domain to {emailDomain}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) continue;
            repo.UpdateSetting(emailDomainSetting.Id, emailDomain);
            break;
        }
    }
    
    private void ShowSettings()
    {
        var settings = GetSettings().ToDictionary(x => x.Key, x => x.Value);
        var settingsList = new[]
        {
            new[] { "Username", settings["AdminUsername"] },
            ["Email Formatting", settings["AdminEmailFormatting"]],
            ["ID Formatting", settings["AdminIdFormatting"]],
            ["Email Domain", settings["AdminEmailDomain"]]
        };
        Boxes.CreateTable(["Setting", "Value"], settingsList);
        System.Console.ReadKey(true);
    }
    

    private void DisplayStudentList()
    {
        var countryInfos = CountryJsonRepo.GetCountryInfos();
        var headers = new[] { "ID", "Name", "Birthday", "Phone Number", "Truncated Address" };
        var students = Users.Where(x => x.Role == UserRole.Learner)
            .Select(x => new[]
            {
                x.UserIdStr, x.FullName, x.BirthDate, $"{$"{countryInfos.Countries.GetValueOrDefault(x.AddressCountryCode)?.Phone[0]} " ?? ""}{x.PhoneNumber}",
                $"{x.AddressCity}, {x.AddressProvince}, {x.AddressCountryCode}"
            }).ToArray();

        Boxes.CreateLazyTable(headers, students);
        System.Console.ReadKey();
    }
}