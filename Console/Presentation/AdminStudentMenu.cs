using Reveche.SimpleLearnerInfoSystem.Console.Data;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public partial class AdminMenu
{
    private const int StudentDialogPadding = 30;
    
    private void ManageStudent()
    {
        Action[] actions = [AddStudent, RemoveStudent, UpdateStudent, ResetStudentPassword, DisplayStudentList];
        
        MenuUtils.DisplayMenu("Manage Students", actions);
    }
    
    private void AddStudent()
    {
        if (Programs.Count == 0)
        {
            MenuUtils.NotFoundPrompt("program", true);
            return;
        }

        AddUser(UserRole.Learner, out var student);
        if (student is null) return;
        
        var program = GetProgram("Program: ");
        var programTracker = new ProgramTracker
        {
            UserId = student.Id,
            Programs =
            [
                new ProgramProgress
                {
                    ProgramId = Programs.Find(x => x.Code == program.Code)!.Id,
                    Status = Status.InProgress,
                    DateCompleted = null
                }
            ],
            Id = Utils.GetUniqueId(repo.GetProgramTrackers()),
            Courses = Utils.GetDefaultCourses(student, program, repo.GetCourseCompletions())
        };
        
        repo.AddProgramTracker(programTracker);
        System.Console.ReadKey();
        
        return;

        Program GetProgram(string prompt)
        {
            while (true)
            {
                System.Console.Clear();
                var programs = Programs;
                System.Console.WriteLine("\nAvailable Programs:");
                var programList = programs.Select(x => x.Code).ToList();
                Application.Print(programList, true);
                Boxes.DrawCenteredQuestionBox(prompt, clear: false);
                var inputProgram = System.Console.ReadLine() ?? "";
                var p = programs.FirstOrDefault(x => x.Code.Equals(inputProgram, StringComparison.OrdinalIgnoreCase));
                if (p is not null) return p;
            }
        }
    }
    
    private void RemoveStudent() => RemoveUser(UserRole.Learner);
    private void ResetStudentPassword() => ResetPassword(UserRole.Learner);
    private void UpdateStudent() => UpdateUser(UserRole.Learner);
    private void SearchStudent() => SearchUser(UserRole.Learner);
}