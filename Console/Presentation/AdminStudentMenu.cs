using Reveche.LearnerInfoSystem.Console.Data;
using Reveche.LearnerInfoSystem.Models;

namespace Reveche.LearnerInfoSystem.Console.Presentation;

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

        AddUser(UserRole.Learner, out var learner);
        if (learner is null) return;

        var programs = Programs.Select(x => x.Code).ToList();
        var programCode = Boxes.SingleSelectionBox(programs);
        var program = Programs.Find(x => x.Code == programCode)!;
        var programTracker = new ProgramTracker
        {
            UserId = learner.Id,
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
            Courses = Utils.GetDefaultCourses(learner, program, repo.GetCourseCompletions())
        };

        repo.AddProgramTracker(programTracker);
        Boxes.DrawCenteredBox($"Student {learner.FullName} added to the record.");
        System.Console.ReadKey();
    }

    private void RemoveStudent() => RemoveUser(UserRole.Learner);
    private void ResetStudentPassword() => ResetPassword(UserRole.Learner);
    private void UpdateStudent() => UpdateUser(UserRole.Learner);
    private void SearchStudent() => SearchUser(UserRole.Learner);
}