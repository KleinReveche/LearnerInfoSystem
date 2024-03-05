namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public class CourseMenu(IRepo repo)
{
    public void ManageCourses()
    {
        Action[] actions = [AddCourse, UpdateCourse, RemoveCourse, DisplayCourses];
        MenuUtils.DisplayMenu("Courses", actions);
    }
    
    // TODO: FINISH THIS!

    private void DisplayCourses()
    {
        throw new NotImplementedException();
    }

    private void RemoveCourse()
    {
        throw new NotImplementedException();
    }

    private void UpdateCourse()
    {
        throw new NotImplementedException();
    }

    private void AddCourse()
    {
        throw new NotImplementedException();
    }
}