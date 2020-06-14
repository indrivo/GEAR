using System;

namespace GR.EmailTwoFactorAuth.Helpers
{
    public static class CodeGenerator
    {
        /// <summary>
        /// Generate code
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateCode(int length)
        {
            var generator = new Random();
            var code = generator.Next(0, 999999).ToString($"D{length}");
            return code;
        }
    }
}