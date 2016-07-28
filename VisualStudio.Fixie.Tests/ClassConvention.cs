using Fixie;
using System.Configuration;

namespace VisualStudio.Fixie.Tests
{
    public class ClassConvention : Convention
    {
        public ClassConvention()
        {
            var target = ConfigurationManager.AppSettings.Get("fixie");
            Classes.Where(x => x.Name == target);
        }
    }
}
