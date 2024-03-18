using Reveche.LearnerInfoSystem.Console.Data;
using Reveche.LearnerInfoSystem.Models;

namespace Reveche.LearnerInfoSystem.Console.Presentation;

public class Login(IRepo repo)
{
    public void Run(out User loggedInUser)
    {
        while (true)
        {
            Boxes.DrawHeaderAndQuestionBox(Application.AppName, "Enter your Username/Email: ");
            var username = System.Console.ReadLine();
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox(Application.AppName, "Enter your Password: ");
            var password = Utils.GetHiddenConsoleInput();
            System.Console.Clear();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Boxes.DrawHeaderAndQuestionBox(Application.AppName, "Please enter a valid username and password.", 6);
                System.Console.ReadKey();
                System.Console.Clear();
                continue;
            }

            if (!repo.Login(username, username, password, out var user)) continue;
            if (user is null)
            {
                Boxes.DrawHeaderAndQuestionBox(Application.AppName, "Wrong Credentials.", 6);
                System.Console.ReadKey();
                System.Console.Clear();
                continue;
            }

            loggedInUser = user;
            break;
        }
    }
}