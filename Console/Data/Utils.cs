using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Reveche.LearnerInfoSystem.Console.Presentation;
using Reveche.LearnerInfoSystem.Models;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
namespace Reveche.LearnerInfoSystem.Console.Data;

public static partial class Utils
{
    /// <summary>
    ///     Generates a unique ID for a learner based on the academic year and a format string.
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
    ///     - ####: A unique number for each learner, incremented for each new learner.
    /// </remarks>
    public static string GetLearnerId(int startYear, int endYear, IEnumerable<User> studentList,
        string format = "SY-EY-####")
    {
        var id = new StringBuilder(format);

        id.Replace("SY", startYear.ToString()[2..]);
        id.Replace("EY", endYear.ToString()[2..]);
        id.Replace("SYYY", startYear.ToString());
        id.Replace("EYYY", endYear.ToString());

        var count = format.Count(x => x == '#');
        var lastStudent =
            studentList.LastOrDefault(x => x.UserIdStr.StartsWith(id.ToString().Replace(new string('#', count), "")));
        var lastStudentId = lastStudent?.UserIdStr.Split('-').Last() ?? "0";
        var idNumber = int.Parse(lastStudentId) + 1;
        id.Replace(new string('#', count), idNumber.ToString().PadLeft(count, '0'));

        return id.ToString();
    }

    /// <summary>
    ///     Generates a unique ID for an instructor based on a format string.
    /// </summary>
    /// <param name="firstname">The first name of the instructor.</param>
    /// <param name="middlename">The middle name of the instructor.</param>
    /// <param name="lastname">The last name of the instructor.</param>
    /// <param name="users">List of Users to check for duplicates.</param>
    /// <param name="format">A string representing the format of the ID. Default is "SY-EY-####".</param>
    /// <returns>A string representing the generated ID.</returns>
    /// <remarks>
    ///     The format string can contain the following placeholders:
    ///     - FI: First initial of the instructor's first name.
    ///     - MI: First initial of the instructor's middle name.
    ///     - LI: First initial of the instructor's last name.
    ///     - FN: Full first name of the instructor.
    ///     - MN: Full middle name of the instructor.
    ///     - LN: Full last name of the instructor.
    ///     - ####: A unique number for each instructor, incremented for each new instructor.
    /// </remarks>
    public static string GetInstructorId(string firstname, string middlename, string lastname, List<User> users,
        string format = "FILNfaculty")
    {
        var id = new StringBuilder(format);
        id.Replace("FI", firstname[0].ToString());
        id.Replace("MI", middlename[0].ToString());
        id.Replace("LI", lastname[0].ToString());
        id.Replace("FN", firstname);
        id.Replace("MN", middlename);
        id.Replace("LN", lastname);

        var count = format.Count(x => x == '#');
        var idNumber = users.Count(i => i.Role == UserRole.Instructor) + 1;
        if (count > 0) id.Replace(new string('#', count), idNumber.ToString().PadLeft(count, '0'));

        var duplicateCount = 0;
        var formattedId = id + (duplicateCount > 0 ? duplicateCount.ToString("D2") : "");
        while (users.Any(x => x.Username == formattedId)) duplicateCount++;
        return formattedId.ToLower();
    }

    /// <summary>
    ///     Generates an email for a learner based on their first name, last name, and a format string.
    /// </summary>
    /// <param name="firstname">The first name of the learner.</param>
    /// <param name="middlename">The middle name of the learner.</param>
    /// <param name="lastname">The last name of the learner.</param>
    /// <param name="emailDomain">The domain for the email.</param>
    /// <param name="users">List of Users to check for duplicates.</param>
    /// <param name="format">A string representing the format of the email. Default is "FILN@ED".</param>
    /// <returns>A string representing the generated email.</returns>
    /// <remarks>
    ///     The format string can contain the following placeholders:
    ///     - FI: First initial of the learner's first name.
    ///     - MI: First initial of the learner's middle name.
    ///     - LI: First initial of the learner's last name.
    ///     - FN: Full first name of the learner.
    ///     - MN: Full middle name of the learner.
    ///     - LN: Full last name of the learner.
    ///     - ED: Email domain.
    /// </remarks>
    public static string GetEmail(string firstname, string middlename, string lastname, string emailDomain,
        List<User> users, string format = "FILN@ED")
    {
        var email = new StringBuilder(format);
        email.Replace("FI", firstname[0].ToString());
        email.Replace("MI", middlename[0].ToString());
        email.Replace("LI", lastname[0].ToString());
        email.Replace("FN", firstname);
        email.Replace("MN", middlename);
        email.Replace("LN", lastname);
        email.Replace("ED", emailDomain);

        var count = 0;
        var formattedEmail = email.ToString().ToLower().Replace("@", count > 0 ? count.ToString("D2") + "@" : "@");
        while (users.Any(x => x.Username == formattedEmail)) count++;
        return formattedEmail;
    }

    public static string GetUserStringInput(string prompt, int padding = 0, int requiredLength = -1,
        params char[] required)
    {
        string input;
        do
        {
            if (padding < 12) Boxes.DrawCenteredQuestionBox(prompt);
            else Boxes.DrawCenteredQuestionBox(prompt, padding: padding);
            input = System.Console.ReadLine() ?? "";

            if (required.Length == 0 || required.All(input.Contains) ||
                (requiredLength != -1 && requiredLength == input.Length)) break;
        } while (string.IsNullOrEmpty(input));

        return input;
    }

    public static string GetUserBirthDateInput(string prompt, int padding = 0)
    {
        while (true)
        {
            if (padding < 12) Boxes.DrawCenteredQuestionBox(prompt);
            else Boxes.DrawCenteredQuestionBox(prompt, padding: padding);
            var input = System.Console.ReadLine() ?? "";
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                return input;
        }
    }

    public static void GetUserBirthDateUpdate(string prompt, string old, out string updated)
    {
        while (true)
        {
            Boxes.DrawCenteredQuestionBox($"{prompt} [{old}]: ");
            var input = System.Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(input))
            {
                updated = old;
                break;
            }

            if (!DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out _)) continue;
            updated = input;
            break;
        }
    }

    public static void GetStringUpdate(string type, string old, out string updated)
    {
        Boxes.DrawCenteredQuestionBox($"{type} [{old}]: ");
        updated = System.Console.ReadLine() ?? "";
        if (string.IsNullOrEmpty(updated)) updated = old;
    }

    public static void GetNumberUpdate(string prompt, string old, out string updated)
    {
        Boxes.DrawCenteredQuestionBox($"{prompt} [{old}]: ");
        var input = System.Console.ReadLine();
        if (string.IsNullOrEmpty(input)) updated = old;

        if (long.TryParse(input, out var n) && n > 0) updated = input;
        updated = old;
    }


    public static void GetDoubleUpdate(string prompt, string old, out string updated)
    {
        Boxes.DrawCenteredQuestionBox($"{prompt} [{old}]: ");
        var input = System.Console.ReadLine();
        if (string.IsNullOrEmpty(input)) updated = old;

        if (double.TryParse(input, out var n) && n > 0) updated = input;
        updated = old;
    }

    public static void GetEnumUpdate<T>(string prompt, T old, out T updated) where T : struct, Enum
    {
        Boxes.DrawCenteredQuestionBox($"{prompt} [{old}]: ");
        var input = System.Console.ReadLine();
        if (string.IsNullOrEmpty(input)) updated = old;

        if (Enum.TryParse<T>(input, true, out var result)) updated = result;
        updated = old;
    }

    public static void GetAcademicYear(out (int YearStart, int YearEnd) academicYear)
    {
        var academicYearStr = $"{DateTime.Now.Year}-{DateTime.Now.Year + 1}";
        var inputParsed = Array.Empty<int>();

        while (true)
        {
            Boxes.DrawHeaderAndQuestionBox(Application.AppName, $"Enter the Academic Year [{academicYearStr}]: ");
            var input = System.Console.ReadLine() ?? "";

            try
            {
                inputParsed = input.Split('-').Select(int.Parse).ToArray();
            }
            catch
            {
                if (inputParsed.Length == 0)
                {
                    System.Console.Clear();
                    break;
                }

                System.Console.WriteLine("Invalid input.");
            }

            if (inputParsed is not { Length: 2 }) continue;
            System.Console.Clear();
            break;
        }

        try
        {
            academicYear = (inputParsed[0], inputParsed[1]);
        }
        catch
        {
            academicYear = (DateTime.Now.Year, DateTime.Now.Year + 1);
        }
    }

    public static string GetUserStringInputOptional(string prompt)
    {
        Boxes.DrawCenteredQuestionBox(prompt);
        var input = System.Console.ReadLine() ?? "";
        return string.IsNullOrEmpty(input) ? "" : input;
    }

    public static string GetUserNumberInput(string prompt, long upperBound = long.MaxValue, bool hasIndicator = false,
        string indicator = ">")
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
        foreach (var itemId in items.Select(item => (int)propertyInfo.GetValue(item)!).Where(itemId => itemId == id))
            id = itemId + 1;
        return id;
    }

    public static List<CourseCompletion> GetDefaultCourses(User learner, Program program,
        IEnumerable<CourseCompletion> courseCompletions)
        => program.Courses.Select(course => new CourseCompletion
        {
            Id = GetUniqueId(courseCompletions),
            CourseId = course.Id,
            InstructorId = course.InstructorId,
            UserId = learner.Id,
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