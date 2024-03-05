using Reveche.SimpleLearnerInfoSystem.Console.Data;
using Reveche.SimpleLearnerInfoSystem.Data;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public partial class AdminMenu
{
    private const int StudentDialogPadding = 10;
    
    private void ManageStudent()
    {
        Action[] actions = [AddStudent, RemoveStudent, UpdateStudent];
        
        MenuUtils.DisplayMenu("Manage Students", actions);
    }
    
    private void AddStudent()
    {
        if (Programs.Count == 0)
        {
            MenuUtils.NoPrograms();
            return;
        }
        
        var barangayEnabled = bool.Parse(repo.GetSetting(6)!.Value);
        var firstName = Utils.GetUserStringInput("Student's First Name: ", padding: StudentDialogPadding);
        var middleName = Utils.GetUserStringInputOptional("Student's Middle Name [Optional]: ");
        var lastName = Utils.GetUserStringInput("Student's Last Name: ", padding: StudentDialogPadding);
        var fullName = $"{firstName} {middleName} {lastName}";
        var userId = Utils.GetId(_academicYear.YearStart, _academicYear.YearEnd, repo.GetUsers());
        var email = Utils.GetEmail(firstName, middleName, lastName, repo.GetSetting(5)!.Value, Users, repo.GetSetting(4)!.Value);
        // TODO: FIX
        var birthday = Utils.GetUserStringInput("Birthdate (YYYY-MM-DD):", required: ['-']);
        var username = Utils.GetUserDefaultUsername(firstName, lastName, Users);
        var addrStreet = Utils.GetUserStringInput("Street Address:", padding: StudentDialogPadding);
        var addrBarangay = string.Empty;
        if (barangayEnabled) addrBarangay = Utils.GetUserStringInput("Barangay:", padding: StudentDialogPadding);
        var addrCity = Utils.GetUserStringInput("City:", padding: StudentDialogPadding);
        var addrProvince = Utils.GetUserStringInput(barangayEnabled ? "Province:" : "State:");
        var addrCountryCode = Utils.GetUserStringInput("Country Code [PH]: ", requiredLength: 2);
        var addrZipCode = Utils.GetUserNumberInput("Zip/Postal Code: ");
        var phoneNumber = long.Parse(Utils.GetUserNumberInput("Student's Phone Number: ", hasIndicator: true, indicator: CountryJsonRepo.GetCountryInfo(addrCountryCode)?.Phone[0] ?? ">"));
        var defaultUserCredential = Default.DefaultUserCredential(repo.GetSetting(7)!.Value);
        var program = GetProgram("Program: ");
        var student = new User
        {
            Id = Utils.GetUniqueId(Users),
            Role = UserRole.Learner,
            UserIdStr = userId,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            FullName = fullName,
            Username = username,
            AddressStreet = addrStreet,
            AddressBarangay = addrBarangay,
            AddressCity = addrCity,
            AddressProvince = addrProvince,
            AddressCountryCode = addrCountryCode,
            Email = email,
            PhoneNumber = phoneNumber,
            BirthDate = birthday,
            RegistrationDate = DateTime.Now,
            AddressZipCode = addrZipCode,
            PasswordHash = defaultUserCredential.hash,
            PasswordSalt = defaultUserCredential.salt,
            Status = UserStatus.ActiveLearner
        };
        
        var programTracker = new ProgramTracker
        {
            UserId = student.Id,
            Programs =
            [
                new ProgramProgress
                {
                    ProgramId = Programs.Find(x => x.Code == program.Code)!.ProgramId,
                    Status = Status.InProgress,
                    DateCompleted = null
                }
            ],
            ProgramTrackerId = Utils.GetUniqueId(repo.GetProgramTrackers()),
            RemainingCourses = Utils.GetDefaultCourses(student, program, repo.GetCourseCompletions())
        };

        if (Users.Any(x => x.FullName == student.FullName && x.BirthDate == student.BirthDate))
        {
            Boxes.DrawHeaderAndQuestionBox("A student with the same full name and birthdate is found.",
                "Are you sure you want to add again? (Y/N)");
            var key = System.Console.ReadKey().Key;
            if (key == ConsoleKey.N) return;
        }
        
        repo.AddUser(student);
        repo.AddProgramTracker(programTracker);
        Boxes.DrawCenteredBox($"Student {fullName} added to the record.");
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
    
    private void RemoveStudent()
    {
        System.Console.Clear();
        
        if (Programs.Count == 0)
        {
            MenuUtils.NoPrograms();
            return;
        }
        
        if (Users.All(u => u.Role != UserRole.Learner))
        {
            MenuUtils.NoStudents();
            return;
        }
        
        var foundStudents = QueryStudentList("remove");

        if (foundStudents is null)
        {
            Boxes.DrawCenteredBox("This name doesn't appear in the records.");
            return;
        }
        
        if (foundStudents.Count == 1)
        {
            Remove(foundStudents[0]);
            return;
        }
        
        Boxes.DrawCenteredBox($"Found {foundStudents.Count} students with the same name.");
        System.Console.ReadKey();
        System.Console.Clear();
        var headers = new[] { "Index", "Name", "Student ID" };
        var data = foundStudents.Select(x => new[] { foundStudents.IndexOf(x).ToString(), x.FullName, x.UserIdStr }).ToArray();
        Boxes.CreateTable(headers, data);

        System.Console.WriteLine("\n\n[Delete] - Remove all these students. WARNING: This action is irreversible.");
        System.Console.Write("\nChoose: ");
        var key = System.Console.ReadKey();

        if (key.Key == ConsoleKey.Delete)
        {
            Boxes.DrawCenteredQuestionBox("Are you sure you want to remove all these students? (Y/N): ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) return;
            foundStudents.ForEach(Remove);
            return;
        }

        if (!int.TryParse(key.KeyChar.ToString(), out var studentIndex)) return;
        if (studentIndex < 0 || studentIndex >= foundStudents.Count) return;
        Remove(foundStudents[studentIndex]);
        System.Console.ReadKey();

        return;

        void Remove(User student)
        {
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to remove {student.FullName}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) return;
            Boxes.DrawHeaderAndQuestionBox(Application.AppName, "What kind of removal?", [ "Dropped", "Suspended", "Expelled", "Permanent" ], zeroIndexed: true);
            switch (System.Console.ReadKey().KeyChar)
            {
                case '0': 
                    Boxes.DrawCenteredQuestionBox("Are you sure you want to drop this student?");
                    System.Console.WriteLine("Y/N: ");
                    if (System.Console.ReadKey().Key != ConsoleKey.Y)
                    {
                        Boxes.DrawCenteredBox("Cancelled.");
                        System.Console.ReadKey();
                        return;
                    }
                    student.Status = UserStatus.DroppedLearner;
                    repo.UpdateUser(student.Id, student);
                    break;
                case '1':
                    Boxes.DrawCenteredQuestionBox("Are you sure you want to suspend this student?");
                    System.Console.Write("Y/N: ");
                    if (System.Console.ReadKey().Key != ConsoleKey.Y)
                    {
                        Boxes.DrawCenteredBox("Cancelled.");
                        System.Console.ReadKey();
                        return;
                    }
                    student.Status = UserStatus.SuspendedLearner;
                    repo.UpdateUser(student.Id, student);
                    break;
                case '2':
                    Boxes.DrawCenteredQuestionBox("Are you sure you want to expel this student?");
                    System.Console.Write("Y/N: ");
                    if (System.Console.ReadKey().Key != ConsoleKey.Y)
                    {
                        Boxes.DrawCenteredBox("Cancelled.");
                        System.Console.ReadKey();
                        return;
                    }
                    student.Status = UserStatus.ExpelledLearner;
                    repo.UpdateUser(student.Id, student);
                    break;
                case '3':
                    Boxes.DrawCenteredQuestionBox("Are you sure you want to permanently remove this student?");
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
                    Boxes.DrawCenteredQuestionBox("This will remove all records of this student. Are you sure?");
                    System.Console.Write("Y/N: ");
                    if (System.Console.ReadKey().Key != ConsoleKey.Y)
                    {
                        Boxes.DrawCenteredBox("Cancelled.");
                        System.Console.ReadKey();
                        return;
                    }
                    
                    repo.RemoveProgramTracker(repo.GetProgramTracker(student.Id)!.ProgramTrackerId);
                    repo.RemoveUser(student.Id);
                    Boxes.DrawCenteredBox($"Student {student.FullName} removed from the record.");
                    break;
                default:
                    Boxes.DrawCenteredBox("Cancelled.");
                    return;
            }
        }
    }
    
    private void UpdateStudent()
    {
        System.Console.Clear();
        
        if (Programs.Count == 0)
        {
            MenuUtils.NoPrograms();
            return;
        }
        
        if (Users.All(u => u.Role != UserRole.Learner))
        {
            MenuUtils.NoStudents();
            return;
        }
        
        var foundStudents = QueryStudentList("update");

        if (foundStudents is null)
        {
            Boxes.DrawCenteredBox("This name doesn't appear in the records.");
            System.Console.ReadKey();
            return;
        }
        
        if (foundStudents.Count == 1)
        {
            Update(foundStudents[0]);
            return;
        }
        
        Boxes.DrawCenteredBox($"Found {foundStudents.Count} students with the same name.");
        System.Console.ReadKey();
        System.Console.Clear();
        var headers = new[] { "Index", "Name", "Student ID" };
        var data = foundStudents.Select(x => new[] { Users.IndexOf(x).ToString(), x.FullName, x.UserIdStr }).ToArray();
        Boxes.CreateTable(headers, data);

        System.Console.WriteLine("\n\n[INSERT] - Update all these students. WARNING: This action is irreversible.");
        System.Console.Write("\nChoose: ");
        var key = System.Console.ReadKey();

        if (key.Key == ConsoleKey.Insert)
        {
            Boxes.DrawCenteredQuestionBox("Are you sure you want to update all these students? (Y/N): ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) return;
            foundStudents.ForEach(Update);
            return;
        }

        if (!int.TryParse(key.KeyChar.ToString(), out var studentIndex)) return;
        if (studentIndex < 0 || studentIndex >= foundStudents.Count) return;
        Update(foundStudents[studentIndex]);
        System.Console.ReadKey();

        return;

        void Update(User original)
        {
            Boxes.DrawCenteredQuestionBox($"Student {original.FullName}'s Information to update:");
            MenuUtils.GetStringUpdate("First Name", original.FirstName, out var firstName);
            MenuUtils.GetStringUpdate("Middle Name", original.MiddleName ?? "[None]", out var middleName);
            MenuUtils.GetStringUpdate("Last Name", original.LastName, out var lastName);
            var fullName = $"{firstName} {middleName} {lastName}";
            MenuUtils.GetStringUpdate("Student ID", original.UserIdStr, out var userIdStr);
            MenuUtils.GetStringUpdate("Username", original.Username, out var username);
            MenuUtils.GetStringUpdate("Birth Date", original.BirthDate, out var birthdate);
            MenuUtils.GetStringUpdate("Street Address", original.AddressStreet, out var addressStreet);
            var barangay = string.Empty;
            if (bool.Parse(repo.GetSetting(6)!.Value))
                MenuUtils.GetStringUpdate("Barangay", original.AddressBarangay ?? "[MenuUtils.None]", out barangay);
            MenuUtils.GetStringUpdate("City", original.AddressCity, out var addressCity);
            MenuUtils.GetStringUpdate("Province", original.AddressProvince, out var addressProvince);
            MenuUtils.GetStringUpdate("CountryCode", original.AddressCountryCode, out var addressCountryCode);
            MenuUtils.GetStringUpdate("ZipCode", original.AddressZipCode, out var addressZipCode);
            MenuUtils.GetStringUpdate("Email", original.Email, out var email);
            MenuUtils.GetNumberUpdate("Phone Number", original.PhoneNumber.ToString(), out var phoneNumber);

            Boxes.DrawCenteredQuestionBox("Save [S] or Cancel [C] ");
            switch (System.Console.ReadLine()?.ToLower())
            {
                case "s":
                    var updated = new User
                    {
                        Id = original.Id,
                        UserIdStr = userIdStr,
                        FirstName = firstName,
                        MiddleName = middleName,
                        LastName = lastName,
                        FullName = fullName,
                        Email = email,
                        PhoneNumber = long.Parse(phoneNumber),
                        Username = username,
                        PasswordHash = original.PasswordHash,
                        PasswordSalt = original.PasswordSalt,
                        BirthDate = birthdate,
                        AddressStreet = addressStreet,
                        AddressBarangay = barangay,
                        AddressCity = addressCity,
                        AddressProvince = addressProvince,
                        AddressCountryCode = addressCountryCode,
                        AddressZipCode = addressZipCode,
                        Role = UserRole.Learner,
                        RegistrationDate = original.RegistrationDate,
                        Status = UserStatus.ActiveLearner
                    };
                    repo.UpdateUser(original.Id, updated);
                    Boxes.DrawCenteredBox("Successfully updated the record.");
                    return;
                case "c":
                    Boxes.DrawCenteredBox("Update cancelled.");
                    return;
                default:
                    Boxes.DrawCenteredBox("Cancelled.");
                    return;
            }
        }
    }
    
    private void SearchStudent()
    {
        System.Console.Clear();
        
        if (Programs.Count == 0)
        {
            MenuUtils.NoPrograms();
            return;
        }
        
        if (Users.Any(u => u.Role == UserRole.Learner))
        {
            MenuUtils.NoStudents();
            return;
        }
        
        var foundStudents = QueryStudentList("search");
        
        if (foundStudents is null)
        {
            Boxes.DrawCenteredBox("This name doesn't appear in the records.");
            return;
        }
        
        System.Console.Clear();

        var headers = new[] { "Student ID", "Name", "Age", "Program", "Year", "Email", "Phone Number" };
        var data = foundStudents.Select(x => new[]
        {
            x.UserIdStr, x.FullName, Utils.GetAge(x.BirthDate), GetLastProgram(repo.GetProgramTracker(x.Id)!.Programs), 
            x.Email, x.PhoneNumber.ToString()
        }).ToArray();
        Boxes.CreateTable(headers, data);

        System.Console.WriteLine("\n");
        System.Console.ReadKey();
        
        return;

        string GetLastProgram(IReadOnlyCollection<ProgramProgress> programProgresses)
        {
            if (programProgresses.Count == 0) return "MenuUtils.No Program Yet.";
            var inProgressProgram = programProgresses.FirstOrDefault(x => x.Status == Status.InProgress);
            if (inProgressProgram != default) return Programs.Find(x => x.ProgramId == inProgressProgram.ProgramId)!.Code;
            var completedPrograms = programProgresses.Where(x => x.Status == Status.Completed).OrderBy(d => d);
            return Programs.Find(x => x.ProgramId == completedPrograms.Last().ProgramId)!.Code;
        }
    }

    private List<User>? QueryStudentList(string queryType)
    {
        var students = Users.Where(x => x.Role == UserRole.Learner).ToList();
        var names = new List<User>();
        Boxes.DrawCenteredQuestionBox($"Enter Student to {queryType}: ");
        var input = System.Console.ReadLine() ?? "";

        if (int.TryParse(input, out var id))
        {
            try
            {
                var user = students.Find(x => x.Id == id);
                if (user is null) throw new ArgumentException("Invalid User Id.");
                names.Add(user);
                return names;
            }
            catch
            {
                Boxes.DrawCenteredBox("Invalid User ID.");
                return null;
            }
        }

        names = students.FindAll(x => x.FullName.Contains(input, StringComparison.OrdinalIgnoreCase) || x.UserIdStr.Contains(input, StringComparison.OrdinalIgnoreCase));
        return names.Count > 0 ? names : null;
    }
}