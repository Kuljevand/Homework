using Microsoft.CodeAnalysis.CSharp.Syntax;
using PasswordGenerator;

namespace Homework1.Helpers
{
    public class Shared
    {
        public static string GeneratePassword(int PasswordLength)
        {
            var password = new Password(PasswordLength).IncludeLowercase().IncludeUppercase().IncludeSpecial().Next();
            return password;

        }
    }
}
