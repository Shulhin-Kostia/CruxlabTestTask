using System.Text.RegularExpressions;

namespace CruxlabTestTask
{
    /// <summary>
    /// Toolbox to validate passwords
    /// </summary>
    class PasswordValidator
    {
        //NOTE: I assume that whitespaces are prohibited in passwords. I also assume that if password requirements provided in invalid format - the password is incorrect

        //Regex to check if data provided in correct format. All whitespaces need to be deleted before check. 
        //Because the same pattern used to check all lines, it is better to compile it during initialization of program
        //instead of interpreting it in runtime.
        private readonly Regex stringFormatPattern = new Regex(@"^(.{1})(\d+)-(\d+):(.+)$", RegexOptions.Compiled);

        /// <summary>
        /// Method calculates amount of valid passwords in file using LINQ query
        /// </summary>
        /// <param name="fileData">Text file represented as byte array</param>
        /// <returns>Number of valid passwords</returns>
        public int CountValidPasswordsInFileWithLinq(byte[] fileData)
        {
            using (StreamReader reader = new StreamReader(new MemoryStream(fileData)))
            {
                return reader.ReadToEnd().Split("\r\n").Select(IsValidWithLinq).Count(isValid => isValid);
            }
        }

        /// <summary>
        /// Method calculates amount of valid passwords in file using LINQ query in parallel
        /// </summary>
        /// <param name="fileData">Text file represented as byte array</param>
        /// <returns>Number of valid passwords</returns>
        public int CountValidPasswordsInFileWithLinqParallel(byte[] fileData)
        {
            using (StreamReader reader = new StreamReader(new MemoryStream(fileData)))
            {
                return reader.ReadToEnd().Split("\r\n").AsParallel().Select(IsValidWithLinq).Count(isValid => isValid);
            }
        }
        
        /// <summary>
        /// Method calculates amount of valid passwords in file using LINQ query
        /// </summary>
        /// <param name="fileData">Text file represented as byte array</param>
        /// <returns>Number of valid passwords</returns>
        public int CountValidPasswordsInFileWithRegex(byte[] fileData)
        {
            using (StreamReader reader = new StreamReader(new MemoryStream(fileData)))
            {
                return reader.ReadToEnd().Split("\r\n").Select(IsValidWithRegex).Count(isValid => isValid);
            }
        }

        /// <summary>
        /// Method calculates amount of valid passwords in file using LINQ query in parallel
        /// </summary>
        /// <param name="fileData">Text file represented as byte array</param>
        /// <returns>Number of valid passwords</returns>
        public int CountValidPasswordsInFileWithRegexParallel(byte[] fileData)
        {
            using (StreamReader reader = new StreamReader(new MemoryStream(fileData)))
            {
                return reader.ReadToEnd().Split("\r\n").AsParallel().Select(IsValidWithRegex).Count(isValid => isValid);
            }
        }

        /// <summary>
        /// Parses line from file to obtain next info:
        /// - required symbol;
        /// - minimum and maximum allowed amount of this symbol occurrences;
        /// - password to validate;
        /// </summary>
        /// <param name="entry">Line from file to retrieve password info</param>
        /// <returns>Password info object</returns>
        private PasswordInfo GetPasswordInfoFromString(string entry)
        {
            Match requirementsAndPasswordMatch = stringFormatPattern.Match(entry.Replace(" ", ""));

            if (!requirementsAndPasswordMatch.Success)
                return null;

            PasswordInfo passwordInfo = new PasswordInfo()
            {
                RequiredSymbol = requirementsAndPasswordMatch.Groups[1].Value[0],
                MinAmount = Convert.ToInt32(requirementsAndPasswordMatch.Groups[2].Value),
                MaxAmount = Convert.ToInt32(requirementsAndPasswordMatch.Groups[3].Value),
                Password = requirementsAndPasswordMatch.Groups[4].Value
            };

            if(passwordInfo.MinAmount > passwordInfo.MaxAmount)
                return null;

            return passwordInfo;
        }

        /// <summary>
        /// Validates password using LINQ query
        /// </summary>
        /// <param name="passwordInfo">Info for password validation</param>
        /// <returns><see langword="true"/> if password is valid; otherwise, <see langword="false"/></returns>
        private bool IsValidWithLinq(string line)
        {
            PasswordInfo passwordInfo = GetPasswordInfoFromString(line);

            if (passwordInfo == null)
                return false;

            int amount = (from ch in passwordInfo.Password
                          where ch.Equals(passwordInfo.RequiredSymbol)
                          select ch).Count();

            return amount >= passwordInfo.MinAmount && amount <= passwordInfo.MaxAmount;
        }

        /// <summary>
        /// Validates password using regular expression
        /// </summary>
        /// <param name="passwordInfo">Info for password validation</param>
        /// <returns><see langword="true"/> if password is valid; otherwise, <see langword="false"/></returns>
        private bool IsValidWithRegex(string line)
        {
            PasswordInfo passwordInfo = GetPasswordInfoFromString(line);

            if (passwordInfo == null) return false;
            //Regex explanation
            //I'm trying to find such pattern: any number (0 or more) of any symbol except required + one required symbol.
            //This pattern has to occur in password given amount of times (range min-max)
            //After last required symbol pattern allows any number of any symbol except required
            string validPasswordPattern = $"\\b(?:[^{passwordInfo.RequiredSymbol}]*{passwordInfo.RequiredSymbol}){{{passwordInfo.MinAmount},{passwordInfo.MaxAmount}}}[^{passwordInfo.RequiredSymbol}]*\\b";

            return Regex.IsMatch(passwordInfo.Password, validPasswordPattern);
        }
    }
}
