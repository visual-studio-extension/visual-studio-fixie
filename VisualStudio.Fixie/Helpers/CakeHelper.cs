using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudio.Fixie.Helpers
{
    class CakeHelper
    {
        public static void ExecuteCmd(string className, string workingDir)
        {
            var process = new Process();
            process.StartInfo.FileName = "build.cmd";
            process.StartInfo.Arguments = $"-target fixie -className={className}";
            process.StartInfo.WorkingDirectory = workingDir;
            process.Start();
        }
    }
}
