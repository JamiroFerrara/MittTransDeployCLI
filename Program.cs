using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mitttransdeploycli
{
    internal class Program
    {
        private static string workDir = @"C:\TransDeployWorkingDir\";
        static void Main(string[] args)
        {
            if (ValidateArgs(args))
            {
                BuildFile(args[0], args[1]);
            }
        }

        private static void BuildFile(string filePath, string outFileName)
        {
            var fileName = filePath.Remove(0, filePath.LastIndexOf("\\") + 1);
            var path = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);

            if (filePath != null || filePath != "")
            {
                new CDatFile(workDir, outFileName, (byte)3).createTrans(path, fileName);
                Console.WriteLine(workDir + outFileName + ".dat");
            }
        }

        private static bool ValidateArgs(string[] args)
        {
            if (args.Length == 0)
            {
                return false;
            }
            else if (args.Length == 2)
            {
                return true;
            }
            return false;
        }
    }

}
