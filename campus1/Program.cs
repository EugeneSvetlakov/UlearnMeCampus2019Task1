using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GitTask
{
    class Program
    {
        static void Main(string[] args)
        {

            #region TestMyFunc

            Git newGit = new Git(10);
            try
            {
            Console.WriteLine($"Checkout: Commit=0, File=2, Data = {newGit.Checkout(0, 2)}; Expect = 0, {newGit.CommitCount}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine($"Commit # {newGit.Commit()}; Expect: 0, {newGit.CommitCount}");
            Console.WriteLine($"Checkout: Commit=0, File=2, Data = {newGit.Checkout(0, 2)}; Expect = 0, {newGit.CommitCount}");
            newGit.Update(2, 5);
            Console.WriteLine($"Checkout: Commit=0, File=2, Data = {newGit.Checkout(0, 2)}; Expect = 0, {newGit.CommitCount}");
            try
            {
                Console.WriteLine($"Checkout: Commit=0, File=2, Data = {newGit.Checkout(1, 2)}; Expect = ArgumentException");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine($"Commit # {newGit.Commit()}; Expect = 1, {newGit.CommitCount}");
            Console.WriteLine($"Checkout: Commit=0, File=2, Data = {newGit.Checkout(0, 2)}; Expect = 0, {newGit.CommitCount}");
            Console.WriteLine($"Checkout: Commit=0, File=2, Data = {newGit.Checkout(1, 2)}; Expect = 5, { newGit.CommitCount}");

            #endregion

            var commandNames = Regex.Matches(Console.ReadLine(), @"\w+").Cast<Match>().Select(x => x.Value).ToArray();
            var commandArgs = Regex.Matches(Console.ReadLine(), @"\[([\d,]*)\]").Cast<Match>()
                .Select(
                    x => x.Groups[1].Value
                        .Split(',')
                        .Where(y => !string.IsNullOrEmpty(y))
                        .Select(int.Parse)
                        .ToArray()
                ).ToArray();


            var resultArray = new int?[commandArgs.Length];

            Git git = null;

            for (int i = 0; i < commandNames.Length; i++)
            {
                switch (commandNames[i].ToLower())
                {
                    case "git":
                        git = new Git(commandArgs[i][0]);
                        resultArray[i] = null;
                        break;
                    case "commit":
                        resultArray[i] = git.Commit();
                        break;
                    case "update":
                        git.Update(commandArgs[i][0], commandArgs[i][1]);
                        resultArray[i] = null;
                        break;
                    case "checkout":
                        resultArray[i] = git.Checkout(commandArgs[i][0], commandArgs[i][1]);
                        break;
                    default:
                        throw new ArgumentException("lol");
                }
            }

            Console.WriteLine(
                $"[{string.Join<string>(",", resultArray.Select(x => x.HasValue ? x.Value.ToString() : "null"))}]");
            Console.ReadKey();
        }
    }
}