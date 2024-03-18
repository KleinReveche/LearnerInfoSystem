using Reveche.LearnerInfoSystem.Console.Data;
using Reveche.LearnerInfoSystem.Models;

namespace Reveche.LearnerInfoSystem.Console.Presentation;

public class CourseMenu(IRepo repo)
{
    private readonly List<Course> _courses = repo.GetCourses();

    public void ManageCourses()
    {
        Action[] actions = [AddCourse, UpdateCourse, RemoveCourse, DisplayCourses];
        MenuUtils.DisplayMenu("Courses", actions);
    }

    public void DisplayCourses()
    {
        var headers = new[] { "Code", "Title", "Description", "Instructor" };
        var users = repo.GetUsers();
        var courses = _courses.Select(x => new[]
        {
            x.Code,
            x.Title.Length > 130 ? x.Title[..127] + "..." : x.Title,
            x.Description.Length > 40 ? x.Description[..37] + "..." : x.Description,
            users.Find(u => u.Id == x.InstructorId)!.FullName
        }).ToArray();

        Boxes.CreateLazyTable(headers, courses);
        System.Console.ReadKey();
    }

    private void RemoveCourse()
    {
        if (_courses.Count == 0)
        {
            MenuUtils.NotFoundPrompt("course", true);
            System.Console.ReadKey();
        }

        var courseList = _courses.Select(x => x.Code + " - " + x.Title).ToList();
        var selectedCourses = Boxes.MultiSelectionBox(courseList);
        var coursesToRemove = _courses.Where(x => selectedCourses.Contains(x.Code + " - " + x.Title)).ToList();

        if (coursesToRemove.Count == 0)
        {
            Boxes.DrawCenteredBox("No courses selected.");
            System.Console.ReadKey();
            return;
        }

        foreach (var course in coursesToRemove)
        {
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to remove {course.Title}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) continue;

            Boxes.DrawCenteredBox($"Course {course.Code} removed from the record.");
            repo.RemoveCourse(course.Id);
        }
    }

    private void UpdateCourse()
    {
        if (_courses.Count == 0)
        {
            MenuUtils.NotFoundPrompt("course", true);
            System.Console.ReadKey();
            return;
        }

        var courseList = _courses.Select(x => x.Code + " - " + x.Title).ToList();
        var selectedCourse = Boxes.SingleSelectionBox(courseList);
        var courseToUpdate = _courses.First(x => x.Code + " - " + x.Title == selectedCourse);

        Utils.GetStringUpdate("Course Code", courseToUpdate.Code, out var courseCode);
        Utils.GetStringUpdate("Course Title", courseToUpdate.Title, out var courseTitle);
        Utils.GetStringUpdate("Course Description", courseToUpdate.Description, out var courseDescription);
        Utils.GetNumberUpdate("Course Duration (in hours)", courseToUpdate.DurationInHours.ToString(),
            out var courseDuration);
        Utils.GetNumberUpdate("Course Year", courseToUpdate.Year.ToString(), out var courseYear);
        Utils.GetStringUpdate("Course Term", courseToUpdate.Term, out var courseTerm);
        var courseType = GetUpdatedCourseType();
        Utils.GetNumberUpdate("Course Units", courseToUpdate.Units.ToString(), out var courseUnits);

        var users = repo.GetUsers();
        var instructorList = users.Where(x => x.Role == UserRole.Instructor).Select(i => i.FullName).ToList();
        var selectedInstructor = Boxes.SingleSelectionBox(instructorList);
        var courseInstructor = users.First(u => u.FullName == selectedInstructor).Id;

        var newCourse = new Course
        {
            Code = courseCode, Title = courseTitle, Id = courseToUpdate.Id,
            Description = courseDescription, DurationInHours = int.Parse(courseDuration),
            InstructorId = courseInstructor, Year = int.Parse(courseYear), Term = courseTerm,
            Type = courseType, Units = int.Parse(courseUnits)
        };
        repo.UpdateCourse(courseToUpdate.Id, newCourse);
        Boxes.DrawCenteredBox($"Course {courseCode} updated.");
        System.Console.ReadKey();

        return;

        CourseType GetUpdatedCourseType()
        {
            while (true)
            {
                System.Console.Clear();
                Boxes.DrawHeaderAndQuestionBox("Course Type (LEC, LAB, IND):", [
                    "Lecture (LEC)" + (courseToUpdate.Type == CourseType.Lecture ? " (Current)" : ""),
                    "Laboratory (LAB)" + (courseToUpdate.Type == CourseType.Laboratory ? " (Current)" : ""),
                    "Independent Study (IND)" + (courseToUpdate.Type == CourseType.IndependentStudy ? " (Current)" : "")
                ]);
                var inputProgram = System.Console.ReadLine() ?? "";
                CourseType? updatedCourseType = inputProgram switch
                {
                    "LEC" => CourseType.Lecture,
                    "LAB" => CourseType.Laboratory,
                    "IND" => CourseType.IndependentStudy,
                    _ => null
                };
                return updatedCourseType ?? courseToUpdate.Type;
            }
        }
    }

    private void AddCourse()
    {
        if (repo.GetInstructors().Count == 0)
        {
            MenuUtils.NotFoundPrompt("instructor", true);    
            System.Console.ReadKey();
            return;
        }
        
        var courseCode = Utils.GetUserStringInput("Course Code: ");
        var courseTitle = Utils.GetUserStringInput("Course Title: ", 50);
        var courseDescription = Utils.GetUserStringInput("Course Description: ", 65);
        var courseDuration = int.Parse(Utils.GetUserNumberInput("Course Duration (in hours): "));
        var courseYear = int.Parse(Utils.GetUserNumberInput("Course Year: "));
        var courseTerm = Utils.GetUserStringInput("Course Term: ");
        var courseType = GetCourseType();
        var courseUnits = int.Parse(Utils.GetUserNumberInput("Course Units: "));

        var users = repo.GetUsers();
        var instructorList = users.Where(x => x.Role == UserRole.Instructor).Select(i => i.FullName).ToList();
        var selectedInstructor = Boxes.SingleSelectionBox(instructorList);
        var courseInstructor = users.First(u => u.FullName == selectedInstructor).Id;

        if (repo.GetCourses().Any(x => x.Code == courseCode || x.Title == courseTitle))
        {
            Boxes.DrawCenteredBox("Course already exists.");
            return;
        }

        var courseId = Utils.GetUniqueId(repo.GetCourses());
        while (repo.GetCourses().Any(x => x.Id == courseId)) courseId++;

        var newCourse = new Course
        {
            Code = courseCode, Title = courseTitle, Id = courseId,
            Description = courseDescription, DurationInHours = courseDuration,
            InstructorId = courseInstructor, Year = courseYear, Term = courseTerm,
            Type = courseType, Units = courseUnits
        };
        repo.AddCourse(newCourse);
        Boxes.DrawCenteredBox($"Course {courseCode} added to the record.");
        System.Console.ReadKey();

        return;

        CourseType GetCourseType()
        {
            while (true)
            {
                System.Console.Clear();
                Boxes.DrawHeaderAndQuestionBox("Course Type (LEC, LAB, IND):", [
                    "Lecture (LEC)",
                    "Laboratory (LAB)",
                    "Independent Study (IND)"
                ]);
                var inputProgram = System.Console.ReadLine() ?? "";
                CourseType? course = inputProgram switch
                {
                    "LEC" => CourseType.Lecture,
                    "LAB" => CourseType.Laboratory,
                    "IND" => CourseType.IndependentStudy,
                    _ => null
                };

                if (course is not null) return course.Value;
            }
        }
    }
}