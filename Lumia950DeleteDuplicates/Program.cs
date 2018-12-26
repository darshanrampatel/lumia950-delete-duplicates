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
            var newFolderPath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), @"Downloads\Duplicates");
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

            var groupedPictures = filesHash.GroupBy(p => p.Substring(0, 20)).Select(g => new KeyValuePair<string, HashSet<string>>(g.Key, g.ToHashSet())).ToList();
            Console.WriteLine($"Found {groupedPictures.Count} grouped files");

            groupedPictures = groupedPictures.Where(g => g.Value.Count > 1).ToList();
            Console.WriteLine($"Found {groupedPictures.Count} grouped files with duplicates");

            var filteredGroupedPictures = new Dictionary<string, HashSet<string>>();
            foreach (var group in groupedPictures)
            {
                if (group.Value.Any(p => Path.GetExtension(p) == ".jpg") && group.Value.Count > 1)
                {
                    filteredGroupedPictures.Add(group.Key, group.Value);
                }
            }
            Console.WriteLine($"Found {filteredGroupedPictures.Count} filtered grouped files with duplicates");

            var filesToMove = new HashSet<string>();
            foreach (var group in filteredGroupedPictures)
            {
                var jpgFile = group.Value.OrderBy(p => p.Length).FirstOrDefault(p => Path.GetExtension(p) == ".jpg");
                var jpgFileNameWithoutExtension = Path.GetFileNameWithoutExtension(jpgFile);
                var matchingDNGFile = $"{jpgFileNameWithoutExtension}__highres.dng";
                var matchingDNGFile_shortexp = $"{jpgFileNameWithoutExtension}__highres.shortexp.dng";
                var matchingDNGFile_longexp = $"{jpgFileNameWithoutExtension}__highres.longexp.dng";
                var matchingDNGFile_noflash = $"{jpgFileNameWithoutExtension}__highres.noflash.dng";
                var matchingDNGFile_flash = $"{jpgFileNameWithoutExtension}__highres.flash.dng";
                var matchingDNGFile_ev0 = $"{jpgFileNameWithoutExtension}__highres.ev0.dng";
                if (group.Value.Contains(matchingDNGFile)
                    || group.Value.Contains(matchingDNGFile_shortexp)
                    || group.Value.Contains(matchingDNGFile_longexp)
                    || group.Value.Contains(matchingDNGFile_noflash)
                    || group.Value.Contains(matchingDNGFile_flash)
                    || group.Value.Contains(matchingDNGFile_ev0))
                {
                    filesToMove.Add(jpgFile);
                }        
            }
            Console.WriteLine($"Found {filesToMove.Count} pictures to move");

            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
            }
            foreach (var file in filesToMove)
            {
                var oldFilePath = Path.Combine(folderPath, file);
                var newFilePath = Path.Combine(newFolderPath, file);
                File.Move(oldFilePath, newFilePath);
                Console.WriteLine($"Moved {oldFilePath} > {newFilePath}");
            }

            Console.WriteLine($"All files moved");
            Console.WriteLine("Press [Enter] to exit...");
            Console.ReadLine();
        }
    }
}

