using System.Text;

namespace Reveche.LearnerInfoSystem.Console.Presentation;

/// <summary>
///     Utility class that provides methods for drawing various types of boxes and tables in the console.
/// </summary>
public static class Boxes
{
    /// <summary>
    ///     Draws a centered box with a question in the console.
    /// </summary>
    /// <param name="question">The question to be displayed in the box.</param>
    /// <param name="height">The height of the box. Default is 4.</param>
    /// <param name="padding">The padding for the box. Default is 12.</param>
    /// <param name="clear">Whether to clear the console before drawing the box. Default is true.</param>
    /// <param name="hasIndicator">Whether to add an indicator to the box. Default is false.</param>
    /// <param name="indicator">Indicator to add. Default is ">"</param>
    public static void DrawCenteredQuestionBox(string question, int height = 4, int padding = 12, bool clear = true,
        bool hasIndicator = false, string indicator = ">")
    {
        var width = question.Length + padding;
        var x = (System.Console.WindowWidth - width) / 2;
        var y = (System.Console.WindowHeight - height) / 2;
        if (clear) System.Console.Clear();
        DrawBox(x, y, width, height + 2);
        System.Console.SetCursorPosition(x + 2, y + 2);
        var questionPadding = (width - question.Length - 4) / 2;
        System.Console.WriteLine(new string(' ', questionPadding) + question);
        if (height == 3) return;
        var indicatorPadding = 0;
        if (hasIndicator)
        {
            System.Console.SetCursorPosition(x + 2, y + 3);
            System.Console.Write(indicator);
            indicatorPadding = indicator.Length + 1;
        }

        System.Console.SetCursorPosition(x + 2 + indicatorPadding, y + 3);
    }

    /// <summary>
    ///     Draws a centered box with a message in the console.
    /// </summary>
    /// <param name="message">The message to be displayed in the box.</param>
    /// <param name="height">The height of the box. Default is 3.</param>
    /// <param name="padding">The padding for the box. Default is 12.</param>
    /// <param name="clear">Whether to clear the console before drawing the box. Default is true.</param>
    /// <param name="hasIndicator">Whether to add an indicator to the box. Default is false.</param>
    public static void DrawCenteredBox(string message, int height = 3, int padding = 12, bool clear = true,
        bool hasIndicator = false)
    {
        var width = message.Length + padding;
        var x = (System.Console.WindowWidth - width) / 2;
        var y = (System.Console.WindowHeight - height) / 2;
        if (clear) System.Console.Clear();
        DrawBox(x, y, width, height + 2);
        System.Console.SetCursorPosition(x + 2, y + 2);
        var messagePadding = (width - message.Length - 4) / 2;
        if (messagePadding % 2 != 0) messagePadding++;
        System.Console.WriteLine(new string(' ', messagePadding) + message);
        if (height == 3) return;

        var indicatorPadding = 0;

        if (hasIndicator)
        {
            System.Console.SetCursorPosition(x + 2, y + 3);
            System.Console.Write(">");
            indicatorPadding = 2;
        }

        System.Console.SetCursorPosition(x + 2 + indicatorPadding, y + 3);
    }

    /// <summary>
    ///     Draws a centered box that displays the strings from the `info` array.
    /// </summary>
    /// <param name="info">
    ///     An array of strings to be displayed in the box. Each string will be printed on a new line inside the
    ///     box.
    /// </param>
    /// <remarks>
    ///     This method calculates the maximum length of the strings in the array to determine the width of the box.
    ///     It then calculates the `x` and `y` coordinates for the box to center it in the console window.
    /// </remarks>
    public static void DrawCenteredBox(string[] info)
    {
        var maxLength = info.Max(s => s.Length);
        var totalWidth = maxLength + 4; // Add 4 for padding
        var x = (System.Console.WindowWidth - totalWidth) / 2;
        var y = (System.Console.WindowHeight - info.Length - 2) / 2; // Subtract 2 for the box borders

        DrawBox(x, y, totalWidth, info.Length + 2); // Add 2 for the box borders

        for (var i = 0; i < info.Length; i++)
        {
            System.Console.SetCursorPosition(x + 2, y + i + 1); // Add 2 for the box borders
            System.Console.WriteLine(info[i]);
        }
    }

    /// <summary>
    ///     Draws a box with a header and a question in the console.
    /// </summary>
    /// <param name="header">The header to be displayed at the top of the box.</param>
    /// <param name="question">The question to be displayed in the box.</param>
    /// <param name="height">The height of the box. Default is 7.</param>
    /// <param name="padding">The padding for the box. Default is 12.</param>
    /// <param name="hasIndicator">Whether to add an indicator to the box. Default is false.</param>
    public static void DrawHeaderAndQuestionBox(string header, string question, int height = 7, int padding = 12,
        bool hasIndicator = false)
    {
        var width = Math.Max(header.Length, question.Length) + padding;
        var x = (System.Console.WindowWidth - width) / 2;
        var y = (System.Console.WindowHeight - height) / 2;

        System.Console.Clear();
        DrawHeaderBox(x, y, width, height + 1, header);
        System.Console.SetCursorPosition(x + (width - question.Length) / 2, y + 4);
        System.Console.WriteLine(question);

        if (height == 5) return;

        var indicatorPadding = 0;
        if (hasIndicator)
        {
            System.Console.SetCursorPosition(x + 2, y + 5);
            System.Console.Write(">");
            indicatorPadding = 2;
        }

        System.Console.SetCursorPosition(x + 2 + indicatorPadding, y + 5);
    }

    /// <summary>
    ///     Draws a box with a header, a question, and a list of choices in the console.
    /// </summary>
    /// <param name="header">The header to be displayed at the top of the box.</param>
    /// <param name="question">The question to be displayed in the box.</param>
    /// <param name="choices">The choices to be displayed in the box.</param>
    /// <param name="height">The height of the box. Default is 7.</param>
    /// <param name="padding">The padding for the box. Default is 12.</param>
    /// <param name="zeroIndexed">Whether the choices are zero-indexed. Default is false.</param>
    /// <param name="hasIndicator">Whether to add an indicator to the box. Default is false.</param>
    public static void DrawHeaderAndQuestionBox(string header, string question, string[] choices, int height = 7,
        int padding = 12, bool zeroIndexed = false, bool hasIndicator = false)
    {
        var width = Math.Max(header.Length, question.Length) + padding;
        var x = (System.Console.WindowWidth - width) / 2;
        var y = (System.Console.WindowHeight - height) / 2 - choices.Length;
        if (y < 0) y = 0;

        System.Console.Clear();
        DrawHeaderBox(x, y, width, height + choices.Length + 1, header);
        System.Console.SetCursorPosition(x + (width - question.Length) / 2, y + 4);
        System.Console.WriteLine(question);
        for (var i = 0; i < choices.Length; i++)
        {
            if (i == 9)
            {
                System.Console.SetCursorPosition(x + padding / 2, y + 6 + i);
                System.Console.WriteLine($"[{(zeroIndexed ? 9 : 1)}] {choices[i]}");
                continue;
            }

            if (choices[i] == "Return")
            {
                System.Console.SetCursorPosition(x + padding / 2, y + 6 + i);
                System.Console.WriteLine($"[BCKSPCE] {choices[i]}");
                continue;
            }

            if (i > 9)
            {
                System.Console.SetCursorPosition(x + padding / 2, y + 6 + i);
                System.Console.WriteLine($"[{Convert.ToChar(87 + i)}] {choices[i]}");
                continue;
            }

            System.Console.SetCursorPosition(x + padding / 2, y + 6 + i);
            System.Console.WriteLine($"[{i + (zeroIndexed ? 0 : 1)}] {choices[i]}");
        }

        var indicatorPadding = 0;
        if (hasIndicator)
        {
            System.Console.SetCursorPosition(x + padding / 2, y + 6 + choices.Length);
            System.Console.Write(">");
            indicatorPadding = 2;
        }

        System.Console.SetCursorPosition(x + padding / 2 + indicatorPadding, y + 6 + choices.Length);
    }

    /// <summary>
    ///     Draws a box with a header and a list of questions in the console.
    /// </summary>
    /// <param name="header">The header to be displayed at the top of the box.</param>
    /// <param name="question">The list of questions to be displayed in the box.</param>
    /// <param name="height">The height of the box. Default is 7.</param>
    /// <param name="padding">The padding for the box. Default is 12.</param>
    /// <param name="hasIndicator">Whether to add an indicator to the box. Default is false.</param>
    /// <param name="defaultInput">The default input for the questions. Default is an empty string.</param>
    public static void DrawHeaderAndQuestionBox(string header, string[] question, int height = 7, int padding = 12,
        bool hasIndicator = false, string defaultInput = "")
    {
        var width = Math.Max(header.Length, question.Max(x => x.Length)) + padding + defaultInput.Length / 2;
        var x = (System.Console.WindowWidth - width) / 2;
        var y = (System.Console.WindowHeight - height) / 2 - question.Length;

        System.Console.Clear();
        DrawHeaderBox(x, y, width, height + question.Length + 1, header);
        for (var i = 0; i < question.Length; i++)
        {
            System.Console.SetCursorPosition(x + (width - question[i].Length) / 2, y + 4 + i);
            System.Console.WriteLine(question[i]);
        }

        var indicatorPadding = 0;
        if (hasIndicator)
        {
            System.Console.SetCursorPosition(x + 2, y + 5 + question.Length);
            indicatorPadding = 2;
            if (defaultInput != "")
            {
                System.Console.Write($"[{defaultInput}] ");
                indicatorPadding = defaultInput.Length + 5;
            }

            System.Console.Write(">");
        }

        System.Console.SetCursorPosition(x + 2 + indicatorPadding, y + 5 + question.Length);
    }

    /// <summary>
    ///     Draws a box at the specified coordinates with the specified width and height.
    /// </summary>
    /// <param name="x">The x-coordinate of the top-left corner of the box.</param>
    /// <param name="y">The y-coordinate of the top-left corner of the box.</param>
    /// <param name="width">The width of the box.</param>
    /// <param name="height">The height of the box.</param>
    private static void DrawBox(int x, int y, int width, int height)
    {
        System.Console.SetCursorPosition(x, y);
        System.Console.Write("╔");
        for (var i = 0; i < width - 2; i++) System.Console.Write("═");
        System.Console.Write("╗");
        for (var i = 0; i < height - 2; i++)
        {
            System.Console.SetCursorPosition(x, y + i + 1);
            System.Console.Write("║");
            System.Console.SetCursorPosition(x + width - 1, y + i + 1);
            System.Console.Write("║");
        }

        System.Console.SetCursorPosition(x, y + height - 1);
        System.Console.Write("╚");
        for (var i = 0; i < width - 2; i++) System.Console.Write("═");
        System.Console.Write("╝");
    }

    /// <summary>
    ///     Draws a box with a header at the specified coordinates with the specified width and height.
    /// </summary>
    /// <param name="x">The x-coordinate of the top-left corner of the box.</param>
    /// <param name="y">The y-coordinate of the top-left corner of the box.</param>
    /// <param name="width">The width of the box.</param>
    /// <param name="height">The height of the box.</param>
    /// <param name="header">The header to be displayed at the top of the box.</param>
    private static void DrawHeaderBox(int x, int y, int width, int height, string header)
    {
        var padding = (int)Math.Ceiling((width - header.Length - 4) / 2.0);
        var additionalPadding = (width - header.Length - 4) % 2;
        System.Console.SetCursorPosition(x, y);
        System.Console.Write("╔");
        for (var i = 0; i < width - 2 + additionalPadding; i++) System.Console.Write("═");
        System.Console.Write("╗");

        System.Console.SetCursorPosition(x, y + 1);
        System.Console.Write("║" + new string(' ', padding + 1) + header + new string(' ', padding + 1) + "║");

        System.Console.SetCursorPosition(x, y + 2);
        System.Console.Write("╠");
        for (var i = 0; i < width - 2 + additionalPadding; i++) System.Console.Write("═");
        System.Console.Write("╣");

        for (var i = 0; i < height - 4; i++)
        {
            System.Console.SetCursorPosition(x, y + i + 3);
            System.Console.Write("║");
            System.Console.SetCursorPosition(x + width - 1 + additionalPadding, y + i + 3);
            System.Console.Write("║");
        }

        System.Console.SetCursorPosition(x, y + height - 1);
        System.Console.Write("╚");
        for (var i = 0; i < width - 2 + additionalPadding; i++) System.Console.Write("═");
        System.Console.Write("╝");
    }

    /// <summary>
    ///     Creates a table with the specified headers and data.
    /// </summary>
    /// <param name="headers">The headers to be displayed at the top of the table.</param>
    /// <param name="data">The data to be displayed in the table.</param>
    public static void CreateTable(string[] headers, string[][] data)
    {
        var columnWidths = new int[headers.Length];
        for (var i = 0; i < headers.Length; i++)
        {
            columnWidths[i] = headers[i].Length;
            foreach (var row in data)
                if (row[i].Length > columnWidths[i])
                    columnWidths[i] = row[i].Length;
        }

        var totalWidth = columnWidths.Sum() + columnWidths.Length * 3 + 1;
        var x = (System.Console.WindowWidth - totalWidth) / 2;
        var y = (System.Console.WindowHeight - data.Length - 2) / 2;
        DrawTable(headers, data, columnWidths);
    }

    /// <summary>
    ///     Creates a table with the specified headers and data.
    /// </summary>
    /// <param name="headers">An array of strings representing the headers to be displayed at the top of the table.</param>
    /// <param name="data">
    ///     A 2D array of strings representing the data to be displayed in the table. Each sub-array represents
    ///     a row of data.
    /// </param>
    /// <param name="maxRows">Maximum amount of rows to show in the table.</param>
    /// <remarks>
    ///     This method calculates the width of each column based on the longest string in each column (including headers).
    ///     It then draws the table centered in the console window.
    /// </remarks>
    public static void CreateLazyTable(string[] headers, string[][] data, int maxRows = 20)
    {
        if (maxRows == 20 && data.Length > 20) maxRows = System.Console.WindowHeight - 10;

        var columnWidths = new int[headers.Length];
        for (var i = 0; i < headers.Length; i++)
        {
            columnWidths[i] = headers[i].Length;
            foreach (var row in data)
                if (row[i].Length > columnWidths[i])
                    columnWidths[i] = row[i].Length;
        }

        var totalWidth = columnWidths.Sum() + columnWidths.Length * 3 + 1;
        var x = (System.Console.WindowWidth - totalWidth) / 2;
        var y = (System.Console.WindowHeight - maxRows - 2) / 2;
        var topRow = 0;

        System.Console.CursorVisible = false;
        while (true)
        {
            System.Console.Clear();
            DrawTable(headers, data.Skip(topRow).Take(maxRows).ToArray(), columnWidths);
            System.Console.WriteLine("\n\n(Use the arrow keys to navigate the table)");

            var key = System.Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow && topRow > 0) topRow--;
            else if (key == ConsoleKey.DownArrow && topRow < data.Length - maxRows) topRow++;
            else if (key is ConsoleKey.Escape or ConsoleKey.Backspace or ConsoleKey.Enter or ConsoleKey.Spacebar) break;
        }

        System.Console.CursorVisible = true;
    }

    /// <summary>
    ///     Draws a table at the specified coordinates with the specified headers and data.
    /// </summary>
    /// <param name="headers">The headers to be displayed at the top of the table.</param>
    /// <param name="data">The data to be displayed in the table.</param>
    /// <param name="columnWidths">The widths of the columns of the table.</param>
    private static void DrawTable(IReadOnlyList<string> headers, IEnumerable<string[]> data,
        IReadOnlyList<int> columnWidths)
    {
        var sb = new StringBuilder();

        sb.Append('╔');
        for (var i = 0; i < headers.Count; i++)
        {
            var additionalPadding = i == headers.Count - 1 ? 1 : 0;
            sb.Append("═".PadRight(columnWidths[i] + additionalPadding, '═'));
            if (i != headers.Count - 1) sb.Append("═╦═");
        }

        sb.Append('╗');

        sb.AppendLine();
        sb.Append('║');
        for (var i = 0; i < headers.Count; i++)
        {
            var additionalPadding = i == headers.Count - 1 ? 1 : 0;
            sb.Append(headers[i].PadRight(columnWidths[i] + additionalPadding));
            if (i != headers.Count - 1) sb.Append(" ║ ");
        }

        sb.Append('║');

        sb.AppendLine();
        sb.Append('╠');
        for (var i = 0; i < headers.Count; i++)
        {
            var additionalPadding = i == headers.Count - 1 ? 1 : 0;
            sb.Append("═".PadRight(columnWidths[i] + additionalPadding, '═'));
            if (i != headers.Count - 1) sb.Append("═╬═");
        }

        sb.Append('╣');

        foreach (var t in data)
        {
            sb.AppendLine();
            sb.Append('║');
            for (var j = 0; j < headers.Count; j++)
            {
                var additionalPadding = j == headers.Count - 1 ? 1 : 0;
                sb.Append(t[j].PadRight(columnWidths[j] + additionalPadding));
                if (j != headers.Count - 1) sb.Append(" ║ ");
            }

            sb.Append('║');
        }

        sb.AppendLine();
        sb.Append('╚');
        for (var i = 0; i < headers.Count; i++)
        {
            var additionalPadding = i == headers.Count - 1 ? 1 : 0;
            sb.Append("═".PadRight(columnWidths[i] + additionalPadding, '═'));
            if (i != headers.Count - 1) sb.Append("═╩═");
        }

        sb.Append('╝');
        System.Console.Write(sb.ToString());
    }

    public static IEnumerable<string> MultiSelectionBox(List<string> options, int maxRows = 10)
    {
        var selectedOptions = new List<string>();
        var currentIndex = 0;
        var topRow = 0;

        System.Console.CursorVisible = false;
        while (true)
        {
            System.Console.Clear();
            var displayOptions = options.Skip(topRow).Take(maxRows).ToList();
            var longestOptionLength = displayOptions.Max(option => option.Length);
            var boxWidth = longestOptionLength + 20;
            var boxHeight = displayOptions.Count + 2;
            var boxX = (System.Console.WindowWidth - boxWidth) / 2;
            var boxY = (System.Console.WindowHeight - boxHeight) / 2;

            DrawBox(boxX, boxY, boxWidth, boxHeight);

            for (var i = 0; i < displayOptions.Count; i++)
            {
                var checkbox = selectedOptions.Contains(displayOptions[i]) ? "[✓]" : "[ ]";
                var selector = i + topRow == currentIndex ? ">> " : "   ";
                System.Console.SetCursorPosition(boxX + 2, boxY + i + 1);
                System.Console.WriteLine($"{selector} {checkbox} {displayOptions[i]}");
            }

            if (options.Count > maxRows) System.Console.WriteLine("\n(Use the arrow keys to navigate the list)");
            System.Console.WriteLine("\nPress [Space] to select an option. Press [Enter] to confirm selection.");

            var keyInfo = System.Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentIndex > 0) currentIndex--;
                    if (currentIndex < topRow) topRow--;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentIndex < options.Count - 1) currentIndex++;
                    if (currentIndex >= topRow + maxRows) topRow++;
                    break;
                case ConsoleKey.PageUp:
                    currentIndex = Math.Max(0, currentIndex - maxRows);
                    topRow = Math.Max(0, topRow - maxRows);
                    break;
                case ConsoleKey.PageDown:
                    currentIndex = Math.Min(options.Count - 1, currentIndex + maxRows);
                    topRow = Math.Min(options.Count - maxRows, topRow + maxRows);
                    break;
                case ConsoleKey.Spacebar:
                    var selectedOption = options[currentIndex];
                    if (!selectedOptions.Remove(selectedOption)) selectedOptions.Add(selectedOption);
                    break;
                case ConsoleKey.Enter:
                    System.Console.CursorVisible = true;
                    return selectedOptions;
                default:
                    continue;
            }
        }
    }

    public static string SingleSelectionBox(List<string> options, int maxRows = 10)
    {
        var selectedOption = "";
        var currentIndex = 0;
        var topRow = 0;

        System.Console.CursorVisible = false;
        while (true)
        {
            System.Console.Clear();
            var displayOptions = options.Skip(topRow).Take(maxRows).ToList();
            var longestOptionLength = displayOptions.Max(option => option.Length);
            var boxWidth = longestOptionLength + 20;
            var boxHeight = displayOptions.Count + 2;
            var boxX = (System.Console.WindowWidth - boxWidth) / 2;
            var boxY = (System.Console.WindowHeight - boxHeight) / 2;

            DrawBox(boxX, boxY, boxWidth, boxHeight);

            for (var i = 0; i < displayOptions.Count; i++)
            {
                var checkbox = selectedOption.Contains(displayOptions[i]) ? "[✓]" : "[ ]";
                var selector = i + topRow == currentIndex ? ">> " : "   ";
                System.Console.SetCursorPosition(boxX + 2, boxY + i + 1);
                System.Console.WriteLine($"{selector} {checkbox} {displayOptions[i]}");
            }

            if (options.Count > maxRows) System.Console.WriteLine("\n(Use the arrow keys to navigate the list)");
            System.Console.WriteLine("\nPress [Space] to select an option. Press [Enter] to confirm selection.");

            var keyInfo = System.Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentIndex > 0) currentIndex--;
                    if (currentIndex < topRow) topRow--;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentIndex < options.Count - 1) currentIndex++;
                    if (currentIndex >= topRow + maxRows) topRow++;
                    break;
                case ConsoleKey.PageUp:
                    currentIndex = Math.Max(0, currentIndex - maxRows);
                    topRow = Math.Max(0, topRow - maxRows);
                    break;
                case ConsoleKey.PageDown:
                    currentIndex = Math.Min(options.Count - 1, currentIndex + maxRows);
                    topRow = Math.Min(options.Count - maxRows, topRow + maxRows);
                    break;
                case ConsoleKey.Spacebar:
                    selectedOption = options[currentIndex];
                    break;
                case ConsoleKey.Enter:
                    System.Console.CursorVisible = true;
                    return selectedOption;
                default:
                    continue;
            }
        }
    }
}