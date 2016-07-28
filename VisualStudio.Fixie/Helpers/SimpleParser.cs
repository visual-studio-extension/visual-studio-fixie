using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VisualStudio.Fixie.Helpers
{
    public class SimpleParser
    {
        public static IEnumerable<Task> ParseFile(FileInfo file)
        {
            if (!file.Exists) return Enumerable.Empty<Task>();

            var lines = File.ReadAllLines(file.FullName);
            var tasks = lines.Select(x => x.Trim()).Where(x => x.StartsWith("Task(\""));

            var  paths = tasks.Select(x =>
            {
                var name = x.Split('\"')[1];
                return name;
            })
            .Where(x => x.StartsWith("fixie:"))
            .Select(x => x.Replace("fixie:", String.Empty).Trim());

            var files = paths.Select(x => {
                var fs = new DirectoryInfo(x).GetFiles("*.cs", SearchOption.AllDirectories);
                return fs;
            });

            return files.SelectMany(x => x).Select(x => new Task { Path = x.Name });
        }
    }
}
