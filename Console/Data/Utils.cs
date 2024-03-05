using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Reveche.SimpleLearnerInfoSystem.Console.Presentation;
using Reveche.SimpleLearnerInfoSystem.Models;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
namespace Reveche.SimpleLearnerInfoSystem.Console.Data;

public static partial class Utils
{
    /// <summary>
    ///     Generates a unique ID for a student based on the academic year and a format string.
    /// </summary>
    /// <param name="startYear">The start year of the academic year.</param>
    /// <param name="endYear">The end year of the academic year.</param>
    /// <param name="studentList">List of students.</param>
    /// <param name="format">A string representing the format of the ID. Default is "SY-EY-####".</param>
    /// <returns>A string representing the generated ID.</returns>
    /// <remarks>
    ///     The format string can contain the following placeholders:
    ///     - SY: Last two digits of the start year.
    ///     - EY: Last two digits of the end year.
    ///     - SYYY: Full start year.
    ///     - EYYY: Full end year.
    ///     - ####: A unique number for each student, incremented for each new student.
    /// </remarks>
    public static string GetId(int startYear, int endYear, IEnumerable<User> studentList, string format = "SY-EY-####")
    {
        var id = new StringBuilder(format);

        id.Replace("SY", startYear.ToString()[2..]);
        id.Replace("EY", endYear.ToString()[2..]);
        id.Replace("SYYY", startYear.ToString());
        id.Replace("EYYY", endYear.ToString());
        
        var count = format.Count(x => x == '#');
        var lastStudent = studentList.LastOrDefault(x => x.UserIdStr.StartsWith(id.ToString().Replace(new string('#', count), "")));
        var lastStudentId = lastStudent?.UserIdStr!.Split('-').Last() ?? "0";
        var idNumber = int.Parse(lastStudentId) + 1;
        id.Replace(new string('#', count), idNumber.ToString().PadLeft(count, '0'));

        return id.ToString();
    }
    
    /// <summary>
    ///     Generates an email for a student based on their first name, last name, and a format string.
    /// </summary>
    /// <param name="firstname">The first name of the student.</param>
    /// <param name="middleName">The middle name of the student.</param>
    /// <param name="lastname">The last name of the student.</param>
    /// <param name="emailDomain">The domain for the email.</param>
    /// <param name="users">List of Users to check for duplicates.</param>
    /// <param name="format">A string representing the format of the email. Default is "FILN@ED".</param>
    /// <returns>A string representing the generated email.</returns>
    /// <remarks>
    ///     The format string can contain the following placeholders:
    ///     - FI: First initial of the student's first name.
    ///     - MI: First initial of the student's middle name.
    ///     - LI: First initial of the student's last name.
    ///     - FN: Full first name of the student.
    ///     - MN: Full middle name of the student.
    ///     - LN: Full last name of the student.
    ///     - ED: Email domain.
    /// </remarks>
    public static string GetEmail(string firstname, string middleName, string lastname, string emailDomain, List<User> users, string format = "FILN@ED")
    {
        var email = new StringBuilder(format);
        email.Replace("FI", firstname[0].ToString());
        email.Replace("MI", middleName[0].ToString());
        email.Replace("LI", lastname[0].ToString());
        email.Replace("FN", firstname);
        email.Replace("MN", middleName);
        email.Replace("LN", lastname);
        email.Replace("ED", emailDomain);
        
        var count = 0;
        var formattedEmail = email.ToString().ToLower().Replace("@", count > 0 ? count.ToString("D2") : "");
        while (users.Any(x => x.Username == formattedEmail)) count++;
        return formattedEmail;
    }

    public static string GetUserStringInput(string prompt, int padding = 0, int requiredLength = -1, params char[] required)
    {
        string input;
        do
        {
            if (padding < 12) Boxes.DrawCenteredQuestionBox(prompt);
            else Boxes.DrawCenteredQuestionBox(prompt, padding: padding);
            input = System.Console.ReadLine() ?? "";

            if (required.Length == 0 || required.All(input.Contains) || (requiredLength != -1 && requiredLength == input.Length)) break;
        } while (string.IsNullOrEmpty(input));
        //TODO: FIX EMPTY STRING
        return input;
    }

    public static string GetUserStringInputOptional(string prompt)
    {
        Boxes.DrawCenteredQuestionBox(prompt);
        var input = System.Console.ReadLine() ?? "";
        return string.IsNullOrEmpty(input) ? "" : input;
    }

    public static string GetUserNumberInput(string prompt, long upperBound = long.MaxValue, bool hasIndicator = false, string indicator = ">")
    {
        while (true)
        {
            Boxes.DrawCenteredQuestionBox(prompt, hasIndicator: hasIndicator, indicator: indicator);
            var input = System.Console.ReadLine() ?? "";
            if (long.TryParse(input, out var number) && number > 0 && number < upperBound) return input;
        }
    }
    public static int GetUniqueId<T>(IEnumerable<T> items)
    {
        var id = 0;
        var propertyInfo = typeof(T).GetProperty("Id");
        if (propertyInfo == null) return id;
        foreach (var itemId in items.Select(item => (int)propertyInfo.GetValue(item)!).Where(itemId => itemId == id)) id = itemId + 1;
        return id;
    }

    public static List<CourseCompletion> GetDefaultCourses(User student, Program program, IEnumerable<CourseCompletion> courseCompletions)
        => program.Courses.Select(course => new CourseCompletion
        {
            Id = GetUniqueId(courseCompletions),
            CourseId = course.Id,
            InstructorId = course.InstructorId,
            UserId = student.Id,
            Status = Status.NotStarted
        }).ToList();

    public static string GetUserDefaultUsername(string firstName, string lastName, List<User> users)
    {
        var name = firstName[0] + lastName;
        var count = 0;

        var username = name + (count > 0 ? count.ToString("D2") : "");

        while (users.Any(x => x.Username == username)) count++;
        return username.ToLower();
    }
    
    public static string GetAge(string birthDateStr)
    {
        var birthDate = DateTime.ParseExact(birthDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var now = DateTime.Now;
            
        var age = now.Year - birthDate.Year;
        if (birthDate > now.AddYears(-age)) 
            age--;

        return age.ToString();
    }
    
    /// <summary>
    ///     Retrieves a string input from the console without displaying the entered characters.
    /// </summary>
    /// <returns>A string representing the user's input.</returns>
    /// <remarks>
    ///     This method is used for sensitive input such as passwords where the entered characters should not be displayed.
    ///     The input is masked with asterisks (*) for each character entered.
    ///     The Backspace key is handled to allow the user to correct mistakes.
    ///     The input ends when the user presses the Enter key.
    /// </remarks>
    public static string GetHiddenConsoleInput()
    {
        var input = new StringBuilder();
        while (true)
        {
            var key = System.Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) break;
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                System.Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Backspace)
            {
                input.Append(key.KeyChar);
                System.Console.Write('*');
            }
        }

        return input.ToString();
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    public static partial Regex NameRegex();
}