using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualStudio.Fixie.Helpers
{
    public class SolutionHelper
    {
        static DTE GetCurrentDTE(IServiceProvider provider)
        {
            var vs = (DTE)provider.GetService(typeof(DTE));
            if (vs == null) throw new InvalidOperationException("DTE not found.");
            return vs;
        }

        public static DirectoryInfo GetSolutionDir()
        {
            var dte = GetCurrentDTE(ServiceProvider.GlobalProvider);
            var dir = new FileInfo(dte.Solution.FullName).Directory;
            return dir;
        }

        public static IEnumerable<string> GetCSharpNames()
        {
            var dte = GetCurrentDTE(ServiceProvider.GlobalProvider);
            var projects = dte.Solution.Projects;

            if (projects.Count == 0)
                return Enumerable.Empty<string>();

            var csFiles = Enumerable.Range(1, projects.Count).ToList().Select(x =>
            {
                var project = projects.Item(x);
                var files = new FileInfo(project.FullName).Directory.GetFiles("*.cs", SearchOption.AllDirectories);
                return files;
            });

            return csFiles.SelectMany(x => x).Select(x => x.Name);
        }
    }
}
