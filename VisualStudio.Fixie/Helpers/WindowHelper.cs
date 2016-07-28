using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudio.Fixie.Helpers
{
    class WindowHelper
    {
        static Action<string> _output = (x) => { };
        static WindowHelper()
        {
            var outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var customGuid = new Guid("D515D633-6E6A-4644-880F-C569D85C3157");

            IVsOutputWindowPane outputPane;
            outWindow.CreatePane(ref customGuid, "Fixie", 1, 1);
            outWindow.GetPane(ref customGuid, out outputPane);
            outputPane.Activate();
            _output = (message) => outputPane.OutputString(message + Environment.NewLine);
        }

        public static Action<String> Ouput()
        {
            return _output;
        }
    }
}
