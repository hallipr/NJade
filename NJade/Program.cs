namespace NJade
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    class Program
    {
        static void Main(string[] args)
        {
            var runner = new JadeRunner();

            var inputPath = args.FirstOrDefault();

            inputPath = inputPath ?? "*.jade";
            inputPath = inputPath.Replace("/", @"\");

            var files = FindFiles(inputPath);
   
            foreach (var file in files)
            {
                var output = runner.Render(file.FullName);
                File.WriteAllText(Path.Combine(file.DirectoryName, file.Name.Replace(".jade", ".html")), output);
            }
        }

        public static IEnumerable<FileInfo> FindFiles(string blob)
        {
            var searchInfo = BuildSearchInfo(blob);
            var results = Enumerable.Empty<FileInfo>();

            if (searchInfo.FileName != null)
            {
                var file = new FileInfo(searchInfo.FileName);
                if (file.Exists)
                    results = new[] { file };
            }
            else
            {
                var directory = new DirectoryInfo(searchInfo.Directory);
                results = FindFiles(directory, searchInfo.Pattern);
            }

            return results;
        }

        private static IEnumerable<FileInfo> FindFiles(DirectoryInfo directory, Regex pattern)
        {
            if (directory.Exists)
            {
                try
                {
                    return directory.EnumerateFiles("*.*")
                        .Where(x =>
                        {
                            try
                            {
                                return pattern.IsMatch(x.FullName);
                            }
                            catch (PathTooLongException)
                            {
                            }
                            catch (UnauthorizedAccessException)
                            {
                            }

                            return false;
                        })
                        .Concat(directory.EnumerateDirectories().SelectMany(x => FindFiles(x, pattern)));
                }
                catch (PathTooLongException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            return Enumerable.Empty<FileInfo>();
        }

        private static SearchInfo BuildSearchInfo(string blob)
        {
            if (blob.Contains("***"))
                throw new Exception("Invalid blob string");

            var searchInfo = new SearchInfo { FileName = blob };

            var blobIndex = blob.IndexOf("*");

            if (blobIndex > -1)
            {
                var beforeBlob = blob.Substring(0, blobIndex);
                var directoryPart = blobIndex == 0 ? "." : Path.GetDirectoryName(beforeBlob);
                var patternPart = blob.Substring(directoryPart.Length).TrimStart('\\');
                var afterBlob = blob.Substring(blobIndex);

                searchInfo.FileName = null;
                searchInfo.Directory = Path.GetFullPath(blobIndex == 0 ? "." : Path.GetDirectoryName(beforeBlob));
                searchInfo.Pattern = new Regex(Regex.Escape(Path.Combine(searchInfo.Directory, patternPart)).Replace(@"\*\*", ".*").Replace(@"\*", @"[^\\]*"));
            }

            return searchInfo;
        }

        private class SearchInfo
        {
            public string FileName;
            public string Directory;
            public Regex Pattern;
        }
    }
}
