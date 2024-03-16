using Reveche.SimpleLearnerInfoSystem.Console.Data;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

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
            x.Title, 
            x.Description.Length > 40 ? x.Description[..40] + "..." : x.Description, 
            users.Find(u => u.Id == x.InstructorId)!.FullName
        }).ToArray();
        
        Boxes.CreateLazyTable(headers, courses);
        System.Console.ReadKey();
    }
    
    // TODO: FINISH THIS!
    
    private void RemoveCourse()
    {
        if (_courses.Count == 0)
        {
            MenuUtils.NotFoundPrompt("course", true);
        }
    }

    private void UpdateCourse()
    {
        throw new NotImplementedException();
    }

    private void AddCourse()
    {
        var courseCode = Utils.GetUserStringInput("Course Code: ");
        var courseTitle = Utils.GetUserStringInput("Course Title: ", 50);
        var courseDescription = Utils.GetUserStringInput("Course Description: ", 65);
        var courseDuration = int.Parse(Utils.GetUserNumberInput("Course Duration (in hours): "));
        var courseYear = int.Parse(Utils.GetUserNumberInput("Course Year: "));
        var courseTerm = Utils.GetUserStringInput("Course Term: ");
        var courseType = GetCourseType();
        
        
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
                Type = courseType
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