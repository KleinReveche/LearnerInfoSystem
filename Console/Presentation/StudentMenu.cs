using Reveche.SimpleLearnerInfoSystem.Data;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public class StudentMenu(IRepo repo, User loggedInUser)
{
    // TODO: ADD YEAR LEVEL
    public void DisplayMenu()
    {
        Action[] actions = [ViewProfile, ViewCourses, ViewPrograms, Enroll, ViewGrades, Settings];
        
        MenuUtils.DisplayMenu("Student Menu", actions);
    }

    private void Settings()
    {
        
    }

    private void ViewGrades()
    {
        throw new NotImplementedException();
    }

    private void Enroll()
    {
        throw new NotImplementedException();
    }

    private void ViewPrograms()
    {
        throw new NotImplementedException();
    }

    private void ViewCourses()
    {
        throw new NotImplementedException();
    }

    private void ViewProfile()
    {
        string[] userDetails =
        [
            "Student ID: " + loggedInUser.UserIdStr,
            "Name: " + loggedInUser.FullName,
            "Birthdate: " + loggedInUser.BirthDate,
            $"Address: {loggedInUser.AddressStreet}, {(!string.IsNullOrEmpty(loggedInUser.AddressBarangay) ? loggedInUser.AddressBarangay + ", " : "")}{loggedInUser.AddressCity}, " +
            $"{loggedInUser.AddressProvince}, {loggedInUser.AddressCountryCode}",
            $"Phone Number: {CountryJsonRepo.GetCountryInfo(loggedInUser.AddressCountryCode)?.Phone[0]} {loggedInUser.PhoneNumber}",
            "Email: " + loggedInUser.Email,
            "Username: " + loggedInUser.Username
        ];
        Boxes.DrawCenteredBox(userDetails);
        System.Console.ReadKey();
    }
}