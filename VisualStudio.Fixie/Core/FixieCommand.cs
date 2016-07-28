using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Collections.Generic;
using VisualStudio.Fixie.Helpers;
using System.Linq;

namespace VisualStudio.Fixie.Core
{
    class Guids
    {
        public const int FixieStartButton = 0x2005;
        public static readonly Guid CommandSet = new Guid("3d71a661-1f01-42a5-aa88-a6989d6f9ebf");
    }

    internal sealed class FixieCommand
    {
        private readonly Package _package;
        private static List<OleMenuCommand> _commands;

        public static FixieCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get
            {
                return this._package;
            }
        }

        public static void Initialize(Package package)
        {
            Instance = new FixieCommand(package);
        }

        private FixieCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this._package = package;

            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var tasksId = new CommandID(Guids.CommandSet, Guids.FixieStartButton);
                var tasksCommand = new OleMenuCommand((x, y) => { }, tasksId);
                tasksCommand.Visible = false;
                tasksCommand.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(tasksCommand);
            }
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var currentCommand = sender as OleMenuCommand;
            currentCommand.Visible = true;
            currentCommand.Enabled = false;

            CreateCommands();
        }

        private OleMenuCommandService GetService()
        {
            return this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
        }

        private void CreateCommands()
        {
            var output = WindowHelper.Ouput();
            var mcs = GetService();
            if (_commands == null)
                _commands = new List<OleMenuCommand>();

            foreach (var cmd in _commands)
                mcs.RemoveCommand(cmd);

            var list = Enumerable.Empty<string>();

            try
            {
                list = SolutionHelper.GetCSharpNames();
            }
            catch (Exception ex)
            {
                output(ex.Message);
                output(ex.StackTrace);
            }

            var index = 1;

            foreach (var ele in list)
            {
                var className = Path.GetFileNameWithoutExtension(ele);
                var menuCommandID = new CommandID(Guids.CommandSet, Guids.FixieStartButton + index++);
                var command = new OleMenuCommand(this.TestCallback, menuCommandID);
                command.Text = $"Fixie: {className}";
                command.BeforeQueryStatus += (x, y) => { (x as OleMenuCommand).Visible = true; };
                _commands.Add(command);
                mcs.AddCommand(command);
            }
        }

        private void TestCallback(object sender, EventArgs e)
        {
            var cmd = (OleMenuCommand)sender;
            var text = cmd.Text;
            var task = text.Substring(text.IndexOf(':') + 1).Trim();

            System.Threading.Tasks.Task.Run(() =>
            {
                var dir = SolutionHelper.GetSolutionDir();
                CakeHelper.ExecuteCmd(task, dir.FullName);
            });
        }

    }
}
