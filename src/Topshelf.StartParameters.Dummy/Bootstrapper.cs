using System.IO;
using System.Text;

namespace Topshelf.StartParameters.Dummy
{
    class Bootstrapper
    {
        private readonly string _path;

        public string ActionSample { get; set; }
        public string ManualSample { get; set; }
        public string AutoSample { get; set; }
        public string ConstantSample { get; set; }

        public Bootstrapper(string path) => _path = path;

        public bool Start(HostControl hostControl)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder
                .AppendLine(ActionSample)
                .AppendLine(ManualSample)
                .AppendLine(AutoSample)
                .AppendLine(ConstantSample);

            File.WriteAllText(_path,stringBuilder.ToString());

            hostControl.Stop(TopshelfExitCode.Ok);

            return true;
        }

        public bool Stop(HostControl hostControl) => true;

    }
}