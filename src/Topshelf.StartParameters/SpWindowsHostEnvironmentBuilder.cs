using Topshelf.Builders;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;

namespace Topshelf.StartParameters
{
    public class SpWindowsHostEnvironmentBuilder :
        EnvironmentBuilder
    {
        private readonly HostConfigurator _hostConfigurator;

        public SpWindowsHostEnvironmentBuilder(HostConfigurator configurator)
        {
            _hostConfigurator = configurator;
        }

        public HostEnvironment Build()
        {
            return new SpWindowsHostEnvironment(_hostConfigurator);
        }
    }
}