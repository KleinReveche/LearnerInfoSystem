using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem;

public static class Default
{
    private static readonly Credentials Credentials = new();
    private static readonly (string username, string hash, byte[] salt) DefaultAdmin = ("root", Credentials.HashPassword("root", out var defaultSalt), defaultSalt);
    public static readonly List<Setting> DefaultSettings = 
    [
        new Setting { Id = 0, Key = "AdminUsername", Value = DefaultAdmin.username, Scope = UserRole.Administrator, IsString = true },
        new Setting { Id = 1, Key = "AdminPasswordHash", Value = DefaultAdmin.hash, Scope = UserRole.Administrator, IsString = true },
        new Setting { Id = 2, Key = "AdminSalt", Value = Convert.ToBase64String(DefaultAdmin.salt), Scope = UserRole.Administrator, IsString = true },
        new Setting { Id = 3, Key = "AdminIdFormatting", Value = "SY-EY-####", Scope = UserRole.Administrator, IsString = true },
        new Setting { Id = 4, Key = "AdminEmailFormatting", Value = "FILN@ED", Scope = UserRole.Administrator, IsString = true },
        new Setting { Id = 5, Key = "AdminEmailDomain", Value = "institution.com.edu", Scope = UserRole.Administrator, IsString = true },
        new Setting { Id = 6, Key = "AdminIsBarangayEnabled", Value = "True", Scope = UserRole.Administrator, IsBool = true },
        new Setting { Id = 7, Key = "DefaultUserPassword", Value = "defaultpassword", Scope = UserRole.Administrator, IsString = true },
    ];

    public static (string hash, byte[] salt) DefaultUserCredential(string defaultPassword) =>
        (Credentials.HashPassword(defaultPassword, out var defaultSalt), defaultSalt);
}