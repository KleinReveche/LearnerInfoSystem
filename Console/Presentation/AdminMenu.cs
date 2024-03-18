using Reveche.LearnerInfoSystem.Console.Data;
using Reveche.LearnerInfoSystem.Data;
using Reveche.LearnerInfoSystem.Models;

namespace Reveche.LearnerInfoSystem.Console.Presentation;

public partial class AdminMenu(IRepo repo)
{
    private readonly CourseMenu _courseMenu = new(repo);
    private readonly ProgramMenu _programMenu = new(repo);
    private (int YearStart, int YearEnd) _academicYear = (DateTime.Now.Year, DateTime.Now.Year + 1);

    private List<Program> Programs => repo.GetPrograms();
    private List<User> Users => repo.GetUsers();

    public void DisplayMenu()
    {
        Utils.GetAcademicYear(out _academicYear);
        Action[] actions =
        [
            SearchStudent, ManageStudent, DisplayStudentList,
            ManageInstructor, DisplayInstructorList, _programMenu.ManagePrograms,
            _programMenu.DisplayPrograms, _courseMenu.ManageCourses, _courseMenu.DisplayCourses,
            AdminSettings
        ];
        MenuUtils.DisplayMenu("Admin Menu", actions);
    }

    private void AdminSettings()
    {
        Action[] actions = [ChangeAdminCredential, ChangeIdFormat, ChangeEmailFormat, ChangeEmailDomain, ShowSettings];
        MenuUtils.DisplayMenu("Admin Settings", actions);
    }

    private List<Setting> GetSettings() => repo.GetSettings();

    private void AddUser(UserRole role, out User? user)
    {
        var userType = role == UserRole.Learner ? "Learner" : "Instructor";
        var status = role == UserRole.Learner ? UserStatus.ActiveLearner : UserStatus.Instructing;
        var barangayEnabled = bool.Parse(repo.GetSetting(6)!.Value);
        var firstName = Utils.GetUserStringInput(userType + "'s First Name: ", StudentDialogPadding);
        var middleName = Utils.GetUserStringInputOptional(userType + "'s Middle Name [Optional]: ");
        var lastName = Utils.GetUserStringInput(userType + "s Last Name: ", StudentDialogPadding);
        var fullName = $"{firstName} {middleName} {lastName}";
        var learnerUserIdFormat = repo.GetSetting(3)!.Value;
        var learnerUserId =
            Utils.GetLearnerId(_academicYear.YearStart, _academicYear.YearEnd, Users, learnerUserIdFormat);
        var instructorUserIdFormat = repo.GetSetting(8)!.Value;
        var instructorUserId = Utils.GetInstructorId(firstName, middleName, lastName, Users, instructorUserIdFormat);
        var email = Utils.GetEmail(firstName, middleName, lastName, repo.GetSetting(5)!.Value, Users,
            repo.GetSetting(4)!.Value);
        var birthday = Utils.GetUserBirthDateInput("Birthdate (YYYY-mm-dd):");
        var username = Utils.GetUserDefaultUsername(firstName, lastName, Users);
        var addrStreet = Utils.GetUserStringInput("Street Address:", StudentDialogPadding);
        var addrBarangay = string.Empty;
        if (barangayEnabled) addrBarangay = Utils.GetUserStringInput("Barangay:", StudentDialogPadding);
        var addrCity = Utils.GetUserStringInput("City:", StudentDialogPadding);
        var addrProvince = Utils.GetUserStringInput(barangayEnabled ? "Province:" : "State:");
        var addrCountryCode = Utils.GetUserStringInput("Country Code [PH]: ", requiredLength: 2);
        var addrZipCode = Utils.GetUserNumberInput("Zip/Postal Code: ");
        var phoneNumber = long.Parse(Utils.GetUserNumberInput(userType + "'s Phone Number: ", hasIndicator: true,
            indicator: CountryJsonRepo.GetCountryInfo(addrCountryCode)?.Phone[0] ?? ">"));
        var defaultUserCredential = Default.DefaultUserCredential(repo.GetSetting(7)!.Value);
        var userTmp = new User
        {
            Id = Utils.GetUniqueId(Users),
            Role = role,
            UserIdStr = role == UserRole.Learner ? learnerUserId : instructorUserId,
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
            Status = status,
            YearLevel = role == UserRole.Learner ? LearnerYear.FirstYear : LearnerYear.NotApplicable
        };

        if (Users.Any(x => x.FullName == userTmp.FullName && x.BirthDate == userTmp.BirthDate))
        {
            Boxes.DrawHeaderAndQuestionBox($"A {userType} with the same full name and birthdate is found.",
                "Are you sure you want to add again? (Y/N)");
            var key = System.Console.ReadKey().Key;
            user = null;
            if (key == ConsoleKey.N) return;
        }

        repo.AddUser(userTmp);
        user = userTmp;
        
        if (role == UserRole.Learner) return;
        Boxes.DrawCenteredBox($"{userType} {fullName} added to the record.");
        System.Console.ReadKey();
    }

    private void RemoveUser(UserRole role)
    {
        System.Console.Clear();
        if (Users.All(u => u.Role != role))
        {
            switch (role)
            {
                case UserRole.Learner:
                    Boxes.DrawCenteredBox("No students found.");
                    break;
                case UserRole.Instructor:
                    Boxes.DrawCenteredBox("No instructors found.");
                    break;
                case UserRole.Administrator:
                    throw new ArgumentOutOfRangeException(nameof(role), role, "Admin cannot be removed.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, "Invalid Role.");
            }

            return;
        }

        var userType = role == UserRole.Learner ? "Learner" : "Instructor";
        var foundUsers = QueryUserList("remove", UserRole.Learner);

        if (foundUsers is null)
        {
            Boxes.DrawCenteredBox("This name doesn't appear in the records.");
            return;
        }

        if (foundUsers.Count == 1)
        {
            Remove(foundUsers[0]);
            return;
        }

        Boxes.DrawCenteredBox($"Found {foundUsers.Count} {userType} with the same name.");
        System.Console.ReadKey();
        System.Console.Clear();
        var headers = new[] { "Index", "Name", userType + " ID" };
        var data = foundUsers.Select(x => new[] { foundUsers.IndexOf(x).ToString(), x.FullName, x.UserIdStr })
            .ToArray();
        Boxes.CreateTable(headers, data);

        System.Console.WriteLine($"\n\n[Delete] - Remove all these {userType}. WARNING: This action is irreversible.");
        System.Console.Write("\nChoose: ");
        var key = System.Console.ReadKey();

        if (key.Key == ConsoleKey.Delete)
        {
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to remove all these {userType}? (Y/N): ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) 
            {
                Boxes.DrawCenteredBox("Cancelled.");
                return;
            }
            foundUsers.ForEach(Remove);
            return;
        }

        if (!int.TryParse(key.KeyChar.ToString(), out var studentIndex)) return;
        if (studentIndex < 0 || studentIndex >= foundUsers.Count) return;
        Remove(foundUsers[studentIndex]);
        System.Console.ReadKey();

        return;

        void Remove(User user)
        {
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to remove {user.FullName}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) 
            {
                Boxes.DrawCenteredBox("Cancelled.");
                return;
            }

            if (role == UserRole.Learner)
            {
                Boxes.DrawHeaderAndQuestionBox(Application.AppName, "What kind of removal?",
                    ["Dropped", "Suspended", "Expelled", "Permanent"]);
                switch (System.Console.ReadKey().KeyChar)
                {
                    case '1':
                        Boxes.DrawCenteredQuestionBox("Are you sure you want to drop this learner?");
                        System.Console.WriteLine("Y/N: ");
                        if (System.Console.ReadKey().Key != ConsoleKey.Y)
                        {
                            Boxes.DrawCenteredBox("Cancelled.");
                            System.Console.ReadKey();
                            return;
                        }

                        user.Status = UserStatus.DroppedLearner;
                        repo.UpdateUser(user.Id, user);
                        break;
                    case '2':
                        Boxes.DrawCenteredQuestionBox("Are you sure you want to suspend this learner?");
                        System.Console.Write("Y/N: ");
                        if (System.Console.ReadKey().Key != ConsoleKey.Y)
                        {
                            Boxes.DrawCenteredBox("Cancelled.");
                            System.Console.ReadKey();
                            return;
                        }

                        user.Status = UserStatus.SuspendedLearner;
                        repo.UpdateUser(user.Id, user);
                        break;
                    case '3':
                        Boxes.DrawCenteredQuestionBox("Are you sure you want to expel this learner?");
                        System.Console.Write("Y/N: ");
                        if (System.Console.ReadKey().Key != ConsoleKey.Y)
                        {
                            Boxes.DrawCenteredBox("Cancelled.");
                            System.Console.ReadKey();
                            return;
                        }

                        user.Status = UserStatus.ExpelledLearner;
                        repo.UpdateUser(user.Id, user);
                        break;
                    case '4':
                        Boxes.DrawCenteredQuestionBox("Are you sure you want to permanently remove this learner?");
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

                        Boxes.DrawCenteredQuestionBox("This will remove all records of this learner. Are you sure?");
                        System.Console.Write("Y/N: ");
                        if (System.Console.ReadKey().Key != ConsoleKey.Y)
                        {
                            Boxes.DrawCenteredBox("Cancelled.");
                            System.Console.ReadKey();
                            return;
                        }

                        repo.RemoveProgramTracker(repo.GetProgramTracker(user.Id)!.Id);
                        repo.RemoveUser(user.Id);
                        Boxes.DrawCenteredBox($"Learner {user.FullName} removed from the record.");
                        break;
                    default:
                        Boxes.DrawCenteredBox("Cancelled.");
                        return;
                }
            }
            else
            {
                Boxes.DrawHeaderAndQuestionBox(Application.AppName, "What kind of removal?",
                    ["Instructing", "Retired", "Permanent"]);
                switch (System.Console.ReadKey().KeyChar)
                {
                    case '1':
                        Boxes.DrawCenteredQuestionBox(
                            "Are you sure you want to change this instructor's status to instructing?");
                        System.Console.WriteLine("Y/N: ");
                        if (System.Console.ReadKey().Key != ConsoleKey.Y)
                        {
                            Boxes.DrawCenteredBox("Cancelled.");
                            System.Console.ReadKey();
                            return;
                        }

                        user.Status = UserStatus.Instructing;
                        repo.UpdateUser(user.Id, user);
                        break;
                    case '2':
                        Boxes.DrawCenteredQuestionBox(
                            "Are you sure you want to change this instructor's status to retired?");
                        System.Console.WriteLine("Y/N: ");
                        if (System.Console.ReadKey().Key != ConsoleKey.Y)
                        {
                            Boxes.DrawCenteredBox("Cancelled.");
                            System.Console.ReadKey();
                            return;
                        }

                        user.Status = UserStatus.Retired;
                        repo.UpdateUser(user.Id, user);
                        break;
                    case '3':

                        Boxes.DrawCenteredQuestionBox("Are you sure you want to permanently remove this instructor?");
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

                        Boxes.DrawCenteredQuestionBox("This will remove all records of this instructor. Are you sure?");
                        System.Console.Write("Y/N: ");
                        if (System.Console.ReadKey().Key != ConsoleKey.Y)
                        {
                            Boxes.DrawCenteredBox("Cancelled.");
                            System.Console.ReadKey();
                            return;
                        }

                        foreach (var course in repo.GetCourses().Where(c => c.InstructorId == user.Id).ToArray())
                        {
                            course.InstructorId = int.MaxValue;
                            repo.UpdateCourse(course.Id, course);
                        }

                        repo.RemoveUser(user.Id);
                        Boxes.DrawCenteredBox($"Instructor {user.FullName} removed from the record.");
                        break;
                    default:
                        Boxes.DrawCenteredBox("Cancelled.");
                        return;
                }
            }
        }
    }

    private void UpdateUser(UserRole role)
    {
        System.Console.Clear();

        if (Users.All(u => u.Role != role))
        {
            switch (role)
            {
                case UserRole.Learner:
                    Boxes.DrawCenteredBox("No students found.");
                    break;
                case UserRole.Instructor:
                    Boxes.DrawCenteredBox("No instructors found.");
                    break;
                case UserRole.Administrator:
                    throw new ArgumentOutOfRangeException(nameof(role), role, "Admin cannot be removed.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, "Invalid Role.");
            }

            return;
        }

        var userType = Enum.GetName(role);
        var foundUsers = QueryUserList("update", role);

        if (foundUsers is null)
        {
            Boxes.DrawCenteredBox("This name doesn't appear in the records.");
            System.Console.ReadKey();
            return;
        }

        if (foundUsers.Count == 1)
        {
            Update(foundUsers[0]);
            return;
        }

        Boxes.DrawCenteredBox($"Found {foundUsers.Count} {userType}s with the same name.");
        System.Console.ReadKey();
        System.Console.Clear();
        var headers = new[] { "Index", "Name", "Learner ID" };
        var data = foundUsers.Select(x => new[] { Users.IndexOf(x).ToString(), x.FullName, x.UserIdStr }).ToArray();
        Boxes.CreateTable(headers, data);

        System.Console.WriteLine($"\n\n[INSERT] - Update all these {userType}. WARNING: This action is irreversible.");
        System.Console.Write("\nChoose: ");
        var key = System.Console.ReadKey();

        if (key.Key == ConsoleKey.Insert)
        {
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to update all these {userType}? (Y/N): ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) return;
            foundUsers.ForEach(Update);
            return;
        }

        if (!int.TryParse(key.KeyChar.ToString(), out var studentIndex)) return;
        if (studentIndex < 0 || studentIndex >= foundUsers.Count) return;
        Update(foundUsers[studentIndex]);
        System.Console.ReadKey();

        return;

        void Update(User original)
        {
            Boxes.DrawCenteredQuestionBox($"{userType} {original.FullName}'s Information to update:");
            Utils.GetStringUpdate("First Name", original.FirstName, out var firstName);
            Utils.GetStringUpdate("Middle Name", original.MiddleName ?? "[None]", out var middleName);
            Utils.GetStringUpdate("Last Name", original.LastName, out var lastName);
            var fullName = $"{firstName} {middleName} {lastName}";
            Utils.GetStringUpdate($"{userType} ID", original.UserIdStr, out var userIdStr);
            Utils.GetStringUpdate("Username", original.Username, out var username);
            Utils.GetUserBirthDateUpdate("Birth Date", original.BirthDate, out var birthdate);
            Utils.GetStringUpdate("Street Address", original.AddressStreet, out var addressStreet);
            var barangay = string.Empty;
            if (bool.Parse(repo.GetSetting(6)!.Value))
                Utils.GetStringUpdate("Barangay", original.AddressBarangay ?? "[MenuUtils.None]", out barangay);
            Utils.GetStringUpdate("City", original.AddressCity, out var addressCity);
            Utils.GetStringUpdate("Province", original.AddressProvince, out var addressProvince);
            Utils.GetStringUpdate("CountryCode", original.AddressCountryCode, out var addressCountryCode);
            Utils.GetStringUpdate("ZipCode", original.AddressZipCode, out var addressZipCode);
            Utils.GetStringUpdate("Email", original.Email, out var email);
            Utils.GetNumberUpdate("Phone Number", original.PhoneNumber.ToString(), out var phoneNumber);
            Utils.GetEnumUpdate("Status", original.Status, out var status);
            var year = original.Role == UserRole.Instructor
                ? "NotApplicable"
                : Boxes
                    .SingleSelectionBox(Enum.GetNames(typeof(LearnerYear)).ToList()
                        .Where(x => x != "NotApplicable" && role != UserRole.Learner)
                        .Select(x => x.Replace("Year", " Year"))
                        .ToList()).Replace(" Year", "Year");

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
                        Role = original.Role,
                        RegistrationDate = original.RegistrationDate,
                        Status = status,
                        YearLevel = Enum.Parse<LearnerYear>(year)
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

    private void SearchUser(UserRole role)
    {
        System.Console.Clear();

        if (Users.All(u => u.Role != role))
        {
            switch (role)
            {
                case UserRole.Learner:
                    Boxes.DrawCenteredBox("No students found.");
                    break;
                case UserRole.Instructor:
                    Boxes.DrawCenteredBox("No instructors found.");
                    break;
                case UserRole.Administrator:
                    throw new ArgumentOutOfRangeException(nameof(role), role, "Admin cannot be searched.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, "Invalid Role.");
            }

            return;
        }

        if (Programs.Count == 0 && role == UserRole.Learner)
        {
            MenuUtils.NotFoundPrompt("program", true);
            return;
        }

        var foundUsers = QueryUserList("search", role);

        if (foundUsers is null)
        {
            Boxes.DrawCenteredBox("This name doesn't appear in the records.");
            return;
        }

        System.Console.Clear();
        var countryInfos = CountryJsonRepo.GetCountryInfos().Countries;

        switch (role)
        {
            case UserRole.Learner:
            {
                var headers = new[] { "{ ID", "Name", "Birthdate", "Age", "Program", "Year", "Email", "Phone Number" };
                var data = foundUsers.Select(x => new[]
                {
                    x.UserIdStr, x.FullName, x.BirthDate, Utils.GetAge(x.BirthDate),
                    GetLastProgram(repo.GetProgramTracker(x.Id)?.Programs ?? []),
                    x.Email, $"{countryInfos.GetValueOrDefault(x.AddressCountryCode)?.Phone[0]} {x.PhoneNumber}",
                    $"{x.AddressCity}, {x.AddressProvince}, {x.AddressCountryCode}"
                }).ToArray();
                Boxes.CreateLazyTable(headers, data);
                break;
            }
            case UserRole.Instructor:
            {
                var headers = new[] { "ID", "Name", "Email", "Phone Number" };
                var data = foundUsers.Select(x => new[]
                {
                    x.UserIdStr, x.FullName, x.BirthDate, Utils.GetAge(x.BirthDate),
                    $"{countryInfos.GetValueOrDefault(x.AddressCountryCode)?.Phone[0]} {x.PhoneNumber}",
                    $"{x.AddressCity}, {x.AddressProvince}, {x.AddressCountryCode}"
                }).ToArray();
                Boxes.CreateTable(headers, data);
                break;
            }
            case UserRole.Administrator:
            {
                Boxes.DrawCenteredBox("Admin cannot be searched.");
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(role), role, "Invalid Role.");
        }

        System.Console.WriteLine("\n");
        System.Console.ReadKey();

        return;

        string GetLastProgram(IReadOnlyCollection<ProgramProgress> programProgresses)
        {
            if (programProgresses.Count == 0) return "No Program Yet.";
            var inProgressProgram = programProgresses.FirstOrDefault(x => x.Status == Status.InProgress);
            if (inProgressProgram != default)
                return Programs.Find(x => x.Id == inProgressProgram.ProgramId)!.Code;
            var completedPrograms = programProgresses.Where(x => x.Status == Status.Completed).OrderBy(d => d);
            return Programs.Find(x => x.Id == completedPrograms.Last().ProgramId)!.Code;
        }
    }

    private void ResetPassword(UserRole role)
    {
        System.Console.Clear();
        var users = Users.Where(x => x.Role == role).ToArray();
        if (users.Length == 0)
        {
            Boxes.DrawCenteredBox("No users found.");
            return;
        }

        var names = QueryUserList("Reset password for", role);
        if (names is null)
        {
            Boxes.DrawCenteredBox("This name doesn't appear in the records.");
            return;
        }

        if (names.Count == 1)
        {
            Reset(names[0]);
            return;
        }

        Boxes.DrawCenteredBox($"Found {names.Count} users with the same name.");
        System.Console.ReadKey();
        System.Console.Clear();
        var headers = new[] { "Index", "Name", "User ID" };
        var data = names.Select(x => new[] { names.IndexOf(x).ToString(), x.FullName, x.UserIdStr }).ToArray();
        Boxes.CreateTable(headers, data);

        System.Console.WriteLine(
            "\n\n[INSERT] - Reset password for all these users. WARNING: This action is irreversible.");
        System.Console.Write("\nChoose: ");
        var key = System.Console.ReadKey();

        if (key.Key == ConsoleKey.Insert)
        {
            Boxes.DrawCenteredQuestionBox("Are you sure you want to reset password for all these users? (Y/N): ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) return;
            names.ForEach(Reset);
            return;
        }

        if (!int.TryParse(key.KeyChar.ToString(), out var studentIndex)) return;
        if (studentIndex < 0 || studentIndex >= names.Count) return;
        Reset(names[studentIndex]);
        System.Console.ReadKey();

        return;

        void Reset(User user)
        {
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to reset password for {user.FullName}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) 
            {
                Boxes.DrawCenteredBox("Cancelled.");
                return;
            }
            var defaultUserCredential = Default.DefaultUserCredential(repo.GetSetting(7)!.Value);
            user.PasswordHash = defaultUserCredential.hash;
            user.PasswordSalt = defaultUserCredential.salt;
            repo.UpdateUser(user.Id, user);
            Boxes.DrawCenteredBox($"Password for {user.FullName} reset.");
        }
    }

    private List<User>? QueryUserList(string queryType, UserRole queryRole)
    {
        var students = Users.Where(x => x.Role == queryRole).ToList();
        var names = new List<User>();
        Boxes.DrawCenteredQuestionBox($"Enter {Enum.GetName(queryRole)} to {queryType}: ");
        var input = System.Console.ReadLine() ?? "";

        if (int.TryParse(input, out var id))
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

        names = students.FindAll(x =>
            x.FullName.Contains(input, StringComparison.OrdinalIgnoreCase) ||
            x.UserIdStr.Contains(input, StringComparison.OrdinalIgnoreCase));
        return names.Count > 0 ? names : null;
    }

    private void ChangeAdminCredential()
    {
        while (true)
        {
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup Admin User", "Enter new Username: ");
            var newUsername = System.Console.ReadLine();
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup Admin User", "Repeat new Username: ");
            var repeatUsername = System.Console.ReadLine();

            if (newUsername != repeatUsername) continue;

            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup Admin User", "Enter new Password: ");
            var newPassword = Utils.GetHiddenConsoleInput();

            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Setup Admin User", "Repeat new Password: ");
            var repeatPassword = Utils.GetHiddenConsoleInput();

            if (newPassword != repeatPassword) continue;
            var credentials = new Credentials();
            var settings = GetSettings();
            var usernameSetting = settings.First(x => x.Key == "AdminUsername");
            var passwordSetting = settings.First(x => x.Key == "AdminPasswordHash");
            var saltSetting = settings.First(x => x.Key == "AdminSalt");
            repo.UpdateSetting(usernameSetting.Id, newUsername ?? usernameSetting.Value);
            repo.UpdateSetting(passwordSetting.Id, credentials.HashPassword(newPassword, out var salt));
            repo.UpdateSetting(saltSetting.Id, Convert.ToBase64String(salt));

            Boxes.DrawHeaderAndQuestionBox("StudentBook", "New credential saved.", 6);
            break;
        }
    }

    private void ChangeIdFormat()
    {
        while (true)
        {
            var idFormatSetting = GetSettings().First(x => x.Key == "AdminIdFormatting");
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Enter new ID Format: ", [
                "Legend: ",
                "SY - Start Year Shortened [23]",
                "EY - End Year Shortened [24]",
                "SYYY - Start Year Full [2023]",
                "EYYY - End Year Full [2024]",
                "# - Represents a number",
                "Example: SY-EY-#### for 23-24-0001"
            ], hasIndicator: true, defaultInput: idFormatSetting.Value);
            var newIdFormat = System.Console.ReadLine();

            if (string.IsNullOrEmpty(newIdFormat)) newIdFormat = idFormatSetting.Value;

            Boxes.DrawCenteredQuestionBox($"Are you sure you want to change ID format to {newIdFormat}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) continue;
            repo.UpdateSetting(idFormatSetting.Id, newIdFormat);
            break;
        }
    }

    private void ChangeEmailFormat()
    {
        while (true)
        {
            var emailFormatSetting = GetSettings().First(x => x.Key == "AdminEmailFormatting");
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Enter new Email Format: ", [
                "Legend: ",
                "FN - First Name",
                "MN - Middle Name",
                "LN - Last Name",
                "FI - First Name Initial",
                "MI - Middle Name Initial",
                "LI - Last Name Initial",
                "ED - Email Domain",
                "Example: FNLN@ED for juandelacruz@institution.edu.ph"
            ], hasIndicator: true, defaultInput: emailFormatSetting.Value);
            var newEmailFormat = System.Console.ReadLine();

            if (string.IsNullOrEmpty(newEmailFormat)) newEmailFormat = emailFormatSetting.Value;
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to change Email format to {newEmailFormat}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) continue;
            repo.UpdateSetting(emailFormatSetting.Id, newEmailFormat);
            break;
        }
    }

    private void ChangeEmailDomain()
    {
        while (true)
        {
            var emailDomainSetting = GetSettings().First(x => x.Key == "AdminEmailDomain");
            System.Console.Clear();
            Boxes.DrawHeaderAndQuestionBox("Enter institution's Email Domain: ", [
                "Example: institution.com.edu"
            ], hasIndicator: true, defaultInput: emailDomainSetting.Value);
            var emailDomain = System.Console.ReadLine();

            if (string.IsNullOrEmpty(emailDomain)) emailDomain = emailDomainSetting.Value;
            Boxes.DrawCenteredQuestionBox($"Are you sure you want to change Email domain to {emailDomain}?");
            System.Console.Write("Y/N: ");
            if (System.Console.ReadKey().Key != ConsoleKey.Y) continue;
            repo.UpdateSetting(emailDomainSetting.Id, emailDomain);
            break;
        }
    }

    private void ShowSettings()
    {
        var settings = GetSettings().ToDictionary(x => x.Key, x => x.Value);
        var settingsList = new[]
        {
            new[] { "Username", settings["AdminUsername"] },
            ["Email Formatting", settings["AdminEmailFormatting"]],
            ["ID Formatting", settings["AdminIdFormatting"]],
            ["Email Domain", settings["AdminEmailDomain"]]
        };
        Boxes.CreateTable(["Setting", "Value"], settingsList);
        System.Console.ReadKey(true);
    }


    private void DisplayStudentList()
    {
        var countryInfos = CountryJsonRepo.GetCountryInfos();
        var headers = new[] { "ID", "Name", "Birthday", "Phone Number", "Truncated Address" };
        var students = repo.GetLearners()
            .Select(x => new[]
            {
                x.UserIdStr, x.FullName, x.BirthDate,
                $"{countryInfos.Countries.GetValueOrDefault(x.AddressCountryCode)?.Phone[0]} {x.PhoneNumber}",
                $"{x.AddressCity}, {x.AddressProvince}, {x.AddressCountryCode}"
            }).ToArray();

        Boxes.CreateLazyTable(headers, students);
        System.Console.ReadKey();
    }
}