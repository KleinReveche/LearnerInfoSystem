using Reveche.SimpleLearnerInfoSystem.Console.Data;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public class ProgramMenu(IRepo repo)
{
    private List<Program> GetPrograms => repo.GetPrograms();

    public void ManagePrograms()
    {
        Action[] actions = [AddProgram, UpdateProgram, RemoveProgram, DisplayPrograms, ChangeProgramCourses];

        MenuUtils.DisplayMenu("Programs", actions);
    }

    private void AddProgram()
    {
        var courses = repo.GetCourses().ToList();
        var programCode = Utils.GetUserStringInput("Program Code: ");
        var programTitle = Utils.GetUserStringInput("Program Title: ", 50);
        var programDescription = Utils.GetUserStringInput("Program Description: ", 65);
        var programCoursesSelectedCode = Boxes.MultiSelectionBox(courses.Select(x => x.Code).ToList());
        var programCourses = courses.Where(x => programCoursesSelectedCode.Contains(x.Code)).ToList();
        
        if (GetPrograms.Any(x => x.Code == programCode || x.Title == programTitle))
        {
            Boxes.DrawCenteredBox("Program already exists.");
            return;
        }

        var programId = GetPrograms.Count + 1;
        while (GetPrograms.Any(x => x.Id == programId)) programId++;

        var newProgram = new Program
        {
            Code = programCode, 
            Title = programTitle, 
            Id = programId, 
            Description = programDescription, 
            Status = ProgramStatus.Active,
            Courses = programCourses
        };
        
        repo.AddProgram(newProgram);
        Boxes.DrawCenteredBox($"Program {programCode} added to the record.");
        System.Console.ReadKey();
    }

    public void DisplayPrograms()
    {
        if (GetPrograms.Count == 0)
        {
            MenuUtils.NotFoundPrompt("program", true);
            return;
        }
        
        var headers = new[] { "Code", "Program" };
        var programs = GetPrograms.Select(x => new[] { x.Code, x.Title }).ToArray();
        Boxes.CreateLazyTable(headers, programs);
        System.Console.ReadKey();
    }

    private void UpdateProgram()
    {
        if (GetPrograms.Count == 0)
        {
            MenuUtils.NotFoundPrompt("program", true);
            return;
        }
        
        var program = SearchProgram();
        if (program is null) return;

        Utils.GetStringUpdate("Program Code", program.Code, out var newProgramCode);
        Utils.GetStringUpdate("Program Name", program.Title, out var newProgramName);
        Utils.GetStringUpdate("Program Description", program.Description, out var newProgramDescription);
        Utils.GetEnumUpdate("Program Status", program.Status, out var newProgramStatus);

        if (newProgramCode == program.Code && newProgramName == program.Title)
        {
            Boxes.DrawCenteredBox("No changes made.");
            return;
        }

        System.Console.Clear();
        Boxes.DrawCenteredQuestionBox($"Are you sure you want to update {program.Code} to {newProgramCode}?");
        System.Console.Write("Y/N: ");
        var key = System.Console.ReadKey();
        if (key.Key != ConsoleKey.Y) return;

        repo.UpdateProgram(program.Id, new Program
        {
            Code = newProgramCode,
            Title = newProgramName,
            Id = program.Id,
            Description = newProgramDescription,
            Courses = program.Courses,
            Status = newProgramStatus
        });
        
        Boxes.DrawCenteredBox($"Program {program.Code} updated to {newProgramCode}.");
        System.Console.ReadKey();
    }

    private void RemoveProgram()
    {
        if (GetPrograms.Count == 0)
        {
            MenuUtils.NotFoundPrompt("program", true);
            return;
        }
        
        var program = SearchProgram();

        if (program is null)
        {
            MenuUtils.NotFoundPrompt("program", false);
            return;
        }

        System.Console.Clear();
        Boxes.DrawCenteredQuestionBox($"Are you sure you want to remove {program.Code}?");
        System.Console.Write("Y/N: ");
        var key = System.Console.ReadKey();
        if (key.Key != ConsoleKey.Y) return;
        
        Boxes.DrawHeaderAndQuestionBox(Application.AppName, "What kind of removal?", [ "Suspended", "Discontinued", "Permanent" ], zeroIndexed: true);
        switch (System.Console.ReadKey().KeyChar)
        {
            case '0': 
                Boxes.DrawCenteredQuestionBox("Are you sure you want to suspend this program?");
                System.Console.Write("Y/N: ");
                if (System.Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Boxes.DrawCenteredBox("Cancelled.");
                    System.Console.ReadKey();
                    return;
                }
                program.Status = ProgramStatus.Suspended;
                repo.UpdateProgram(program.Id, program);
                break;
            case '1':
                Boxes.DrawCenteredQuestionBox("Are you sure you want to discontinue this program?");
                System.Console.Write("Y/N: ");
                if (System.Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Boxes.DrawCenteredBox("Cancelled.");
                    System.Console.ReadKey();
                    return;
                }
                program.Status = ProgramStatus.Discontinued;
                repo.UpdateProgram(program.Id, program);
                break;
            case '2':
                Boxes.DrawCenteredQuestionBox("Are you sure you want to permanently remove this program?");
                System.Console.Write("Y/N: ");
                if (System.Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Boxes.DrawCenteredBox("Cancelled.");
                    System.Console.ReadKey();
                    return;
                }
                Boxes.DrawCenteredQuestionBox("Only do this if you are sure. This action is irreversible.");
                System.Console.Write("Y/N: ");
                if (System.Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Boxes.DrawCenteredBox("Cancelled.");
                    System.Console.ReadKey();
                    return;
                }
                Boxes.DrawCenteredQuestionBox("This will remove all records of this program. Are you sure?");
                System.Console.Write("Y/N: ");
                if (System.Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Boxes.DrawCenteredBox("Cancelled.");
                    System.Console.ReadKey();
                    return;
                }
                
        
                repo.RemoveProgram(program.Id);
                Boxes.DrawCenteredBox($"Program {program.Code} removed from the record.");
                break;
            default:
                Boxes.DrawCenteredBox("Cancelled.");
                return;
        }
        System.Console.ReadKey();
    }

    private Program? SearchProgram()
    {
        var programName = Utils.GetUserStringInput("Program Code or Name: ").ToLower();
        var programs = GetPrograms.Where(x => x.Title.Contains(programName, StringComparison.OrdinalIgnoreCase) ||
                                              x.Code.Contains(programName, StringComparison.OrdinalIgnoreCase)).ToArray();

        switch (programs.Length)
        {
            case 0:
                Boxes.DrawCenteredBox("Program not found.");
                return null;
            case 1:
                return programs[0];
        }

        var headers = new[] { "Abbrev.", "Name of Program " };
        var programList = programs.Select(x => new[] { x.Code, x.Title }).ToArray();
        Boxes.CreateTable(headers, programList);
        System.Console.Write("Select Program: ");
        var programCode = System.Console.ReadLine();
        var selectedProgram = programs.FirstOrDefault(x => x.Code == programCode);
        if (selectedProgram is null) Boxes.DrawCenteredBox("Program not found.");
        return selectedProgram;
    }

    private void ChangeProgramCourses()
    {
        var program = SearchProgram();
        if (program is null) return;
        var courses = repo.GetCourses().ToList();
        var programCourses = program.Courses.Select(x => x.Code).ToArray();
        var availableCourses = courses.Where(x => !programCourses.Contains(x.Code)).ToArray();
        var selectedCoursesCode = Boxes.MultiSelectionBox(availableCourses.Select(x => x.Code).ToList());
        var selectedCourses = courses.Where(x => selectedCoursesCode.Contains(x.Code)).ToArray();
        program.Courses.AddRange(selectedCourses);
        repo.UpdateProgram(program.Id, program);
    }
}