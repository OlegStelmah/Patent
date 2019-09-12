using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatentNS
{
    /// <summary>
    /// Статический класс, имеющий набор методов для проверки валидности разных типов полей
    /// Для проверки используються регулярные выражения Regex
    /// </summary>
    static class RegExpr
    {
        public static bool CheckFIO(string value)
        {
            if (value == null) return false;

            var pattern =new Regex(@"^[\p{L}\p{M}' \.\-]+$");
            return pattern.IsMatch(value);
        }

        public static bool CheckEmail(string value)
        {
            if (value == null) return false;

            var pattern = new Regex("[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}");
            return pattern.IsMatch(value);
        }

        public static bool CheckPassword(string value)
        {
            if (value == null) return false;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            bool isValidated = hasNumber.IsMatch(value) && hasUpperChar.IsMatch(value) && hasMinimum8Chars.IsMatch(value);
            return isValidated;
        }
        public static bool CheckCard(string value)
        {
            if (value == null) return false;

            var pattern = new Regex("^[1-9][0-9]{3}( -*[0-9]{4}){3}$");
            return pattern.IsMatch(value);
        }
        public static bool CheckCVV(string value)
        {
            if (value == null) return false;

            var pattern = new Regex("^[0-9]{3,4}$");
            return pattern.IsMatch(value);
        }
    }
}
