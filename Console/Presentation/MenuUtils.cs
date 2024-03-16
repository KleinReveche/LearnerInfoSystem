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
            Boxes.DrawHeaderAndQuestionBox(title, "Choose your Action: ", choices, padding: 20, zeroIndexed: true);
            System.Console.ResetColor();
            System.Console.Write("\nAction: ");
            var key = System.Console.ReadKey();
            System.Console.Clear();

            if (key.Key == ConsoleKey.Backspace) break;
            if (!actionDictionary.TryGetValue(key.KeyChar, out var action)) continue;
            action();
        }
    }
    
    public static void NotFoundPrompt(string type, bool add)
    {
        var addMsg = add ? $" Please add a {type} first." : "";
        Boxes.DrawCenteredBox($"No {type} found.{addMsg}");
        System.Console.ReadKey();
    }
    
    private static Dictionary<char, Action> GetActions(IEnumerable<Action> actions)
    {
        var actionList = actions.ToList();
        var actionDict = new Dictionary<char, Action>();
        for (var i = 0; i < actionList.Count; i++)
        {
            var asciiCode = i switch
            {
                < 10 => Convert.ToChar(i + 48), // Numbers 0-9
                < 36 => Convert.ToChar(i + 55), // Uppercase A-Z
                _ => throw new InvalidOperationException("Too many actions. Cannot assign a unique key to each action.")
            };

            actionDict.Add(asciiCode, actionList[i]);
        }
        return actionDict;
    }
}