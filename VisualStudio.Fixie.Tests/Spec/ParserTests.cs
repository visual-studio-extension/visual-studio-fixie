using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualStudio.Fixie.Helpers;
using FluentAssertions;

namespace VisualStudio.Fixie.Tests.Spec
{
    public class ParserTests
    {
        public void ShortParseFile()
        {
            var input = @"Z:\Source\project\extension\VisualStudio.Fixie\build.cake";
            var files = SimpleParser.ParseFile(new FileInfo(input));

            files.Count().Should().BeGreaterThan(1);

            files.ToList().ForEach(x => {
                Console.WriteLine(x.Path);
            });
        }
    }
}
