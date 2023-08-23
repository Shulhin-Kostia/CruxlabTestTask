using CruxlabTestTask;

PasswordValidator validator = new();

//I've decided to implement solution in two ways: using LINQ and using Regex.
//However, because every password has different requirements from the others Regex is considerably worse in performance than linq due to generation of pattern and interpreting it in runtime
//But Regex method can be useful if password requirements will be more complex

//I've also implemented serial and parallel file processing methods, because files can be large and if fast processing is needed parallel processing can be used.
//If processor resources are limited or execution time is not a deal, serial processing can be used.

//I've used direct access to file in application build folder to test application,
//but since CountValidPasswordsInFile methods accept byte array - any data source (Amazon S3 file, data obtained by HTTP request etc.) can be used
int validPasswordsCount = validator.CountValidPasswordsInFileWithLinqParallel(File.ReadAllBytes("passwords.txt"));

if (validPasswordsCount >= 0)
    Console.WriteLine($"There're {validPasswordsCount} valid passwords in file.");

Console.WriteLine("File processing finished.");
