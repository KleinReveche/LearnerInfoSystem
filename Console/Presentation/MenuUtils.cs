using Reveche.SimpleLearnerInfoSystem.Console.Data;

namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public static class MenuUtils
{
    public static void DisplayMenu(string title, Action[] actions)
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.ForegroundColor = ConsoleColor.Green;
            var choices = actions.Select(x => Utils.NameRegex().Replace(x.Method.Name, " $1")).Append("Return").ToArray();
            var actionDictionary = GetActions(actions);
            Boxes.DrawHeaderAndQuestionBox(title, "Choose your Action: ", choices, padding: 20);
            System.Console.ResetColor();
            System.Console.Write("\nAction: ");
            var key = System.Console.ReadKey();
            System.Console.Clear();

            if (key.Key == ConsoleKey.Backspace) break;
            if (!actionDictionary.TryGetValue(key.KeyChar, out var action)) continue;
            action();
        }
    }
    
    public static void NoPrograms()
    {
        Boxes.DrawCenteredBox("No programs found. Please add a program first.");
        System.Console.ReadKey();
    }

    public static void NoStudents()
    {
        Boxes.DrawCenteredBox("No students found. Please add a student first.");
        System.Console.ReadKey();
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

    public static Dictionary<char, Action> GetActions(IEnumerable<Action> actions)
    {
        var actionList = actions.ToList();
        var actionDict = new Dictionary<char, Action>();
        for (var i = 0; i < actionList.Count; i++)
        {
            var index = i + 49;
            if (i == 9) i = 48;
            if (i > 9) index += 7;
            
            actionDict.Add((char) index, actionList[i]);
        }
        return actionDict;
    }
}