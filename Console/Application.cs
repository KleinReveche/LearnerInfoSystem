using CommandLine;
using Reveche.SimpleLearnerInfoSystem.Console.Data;
using Reveche.SimpleLearnerInfoSystem.Console.Presentation;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console;

public static class Application
{
    public const string AppName = "StudentBook";
    private static RepoType RepoType { get; set; }
    public static (string Host, string Port, string Database, string Username, string Password) SqlCredentials { get; private set; }
    public static string JsonFileName { get; private set; } = "StudentRecord.dat";

    public static void Main(string[] args)
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        var parserResult = Parser.Default.ParseArguments<ApplicationOptions>(args);
        var firstTime = !File.Exists(JsonFileName) && !File.Exists("SimpleLearnerInfoSystem.db");
        
        try
        {
            parserResult.WithParsed(options =>
            {
                RepoType = options.RepoType switch
                {
                    "sqlite" => RepoType.Sqlite,
                    "mysql" => RepoType.Mysql,
                    "json" => RepoType.Json,
                    _ => throw new ArgumentOutOfRangeException(nameof(args),
                        "Invalid argument. Please use 'mysql', 'sqlite', or 'json")
                };
                SqlCredentials = (options.Host, options.Port, options.Database, options.Username, options.Password);
                JsonFileName = options.JsonFileName;
            });
        }
        catch
        {
            System.Console.WriteLine("Invalid repo. Please use 'mysql', 'sqlite', or 'json'");
            Environment.Exit(1);
        }
        
        parserResult.WithNotParsed(errors =>
        {
            foreach (var error in errors)
            {
                if (error is HelpRequestedError or VersionRequestedError) continue;
                System.Console.WriteLine($"Error: {error}");
            }
            Environment.Exit(1);
        });

        if (SqlCredentials is { Host: "none", Port: "3306", Username: "none", Password: "none" } && (int) RepoType > 1) 
            GetSqlCredentials();
        
        var repo = RepoType switch
        {
            RepoType.Json => new JsonRepo() as IRepo,
            // TODO: ADD MIGRATION FROM JSON TO SQL
            RepoType.Sqlite => new SqlRepo(new SqliteDbContext()),
            RepoType.Mysql => new SqlRepo(new MySqlDbContext()),
            _ => throw new ArgumentOutOfRangeException(nameof(args),
                "Invalid argument. Please use 'mysql', 'sqlite', or 'json'")
        };
        
#if DEBUG
        if (firstTime)
        {
            var randomData = new RandomData(repo);
            randomData.GenerateData();
        }
#endif
        
        var login = new Login(repo);
        login.Run(out var loggedInUser);
        switch (loggedInUser.Role)
        {
            case UserRole.Learner:
                var student = new StudentMenu(repo, loggedInUser);
                student.DisplayMenu();
                break;
            case UserRole.Instructor:
                var instructor = new InstructorMenu(repo, loggedInUser);
                instructor.DisplayMenu();
                break;
            case UserRole.Administrator:
                var admin = new AdminMenu(repo);
                admin.DisplayMenu();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args), "Invalid Role");
        }
    }

    private static void GetSqlCredentials()
    {
        while (true)
        {
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup SQL Server", "Enter SQL Server Address: ");
            var server = System.Console.ReadLine();
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup SQL Server", "Enter SQL Server Port [Optional]: ");
            var port = System.Console.ReadLine();
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup SQL Server", "Enter SQL Server Username: ");
            var username = System.Console.ReadLine();
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup SQL Server", "Enter SQL Server Password: ");
            var password = Utils.GetHiddenConsoleInput();
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup SQL Server", "Enter Database Name: ");
            var database = System.Console.ReadLine();

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database)) continue;

            SqlCredentials = (server, port ?? "3306", database, username, password);
            break;
        }
    }

    public static void Print<T>(IEnumerable<T> list, bool comma = false)
    {
        var enumerable = list as T[] ?? list.ToArray();
        foreach (var t in enumerable)
        {
            if (t?.ToString() == "NONE") continue;
            System.Console.Write(t);
            if (comma) System.Console.Write(", ");
        }

        System.Console.Write("\b \n");
    }
}


// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
public class ApplicationOptions
{
    [Option('r', "repo", Default = "json", Required = false,
        HelpText = "The type of repository to use. Options: 'json' [default], 'sqlite', 'mysql'")]
    public string RepoType { get; set; } = "json";

    [Option('h', "host", Default = "none", Required = false, HelpText = "The host of the SQL server")]
    public string Host { get; set; } = "none";

    [Option('t', "port", Default = "3306", Required = false, HelpText = "The port of the SQL server")]
    public string Port { get; set; } = "3306";

    [Option('d', "database", Default = "none", Required = false, HelpText = "The name of the database")]
    public string Database { get; set; } = "none";

    [Option('u', "none", Default = "none", Required = false, HelpText = "The username of the SQL server")]
    public string Username { get; set; } = "none";

    [Option('p', "password", Default = "none", Required = false, HelpText = "The password of the SQL server")]
    public string Password { get; set; } = "none";
    
    [Option('j', "jsonName", Default = "StudentRecord.dat", Required = false, HelpText = "The name of the JSON file")]
    public string JsonFileName { get; set; } = "StudentRecord.dat";
}