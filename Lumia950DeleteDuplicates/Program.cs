using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lumia950DeleteDuplicates
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running...");
            Console.WriteLine();
            var folderPath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), @"OneDrive\Pictures\Camera Roll");
            var files = Directory.EnumerateFiles(folderPath, "*");
            var filesHash = new HashSet<string>();
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("WP"))
                {
                    filesHash.Add(fileName);
                }
            }
            Console.WriteLine($"Found {filesHash.Count} WP files in {folderPath}");

            Console.WriteLine("Press [Enter] to exit...");
            Console.ReadLine();
        }
    }
}

