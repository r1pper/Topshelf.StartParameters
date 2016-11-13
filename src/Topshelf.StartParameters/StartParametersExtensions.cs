using System;
using System.Collections.Generic;
using Topshelf.HostConfigurators;

namespace Topshelf.StartParameters
{
    public static class StartParametersExtensions
    {
        private const string Prefix = "tssp";

        private static readonly Dictionary<HostConfigurator, List<Tuple<string, string>>> ActionTable =
            new Dictionary<HostConfigurator, List<Tuple<string, string>>>();

        public static HostConfigurator EnableStartParameters(this HostConfigurator configurator)
        {
            configurator.UseEnvironmentBuilder(e => new SpWindowsHostEnvironmentBuilder(e));
            return configurator;
        }

        public static HostConfigurator WithStartParameter(this HostConfigurator configurator, string name,
            Action<string> action)
        {
            configurator.AddCommandLineDefinition(Prefix + name, action);
            configurator.AddCommandLineDefinition(name, s => Add(configurator, Prefix + name, s));

            return configurator;
        }

        public static HostConfigurator WithCustomStartParameter(this HostConfigurator configurator, string installName,string runtimeName,
    Action<string> action)
        {
            configurator.AddCommandLineDefinition(runtimeName, action);
            configurator.AddCommandLineDefinition(installName, s => Add(configurator, runtimeName, s));

            return configurator;
        }

        public static HostConfigurator WithStartParameter(this HostConfigurator configurator, string name,string value,
    Action<string> action)
        {
            Add(configurator,name,value);
            configurator.AddCommandLineDefinition(name, action);

            return configurator;
        }

        public static HostConfigurator WithStartParameter(this HostConfigurator configurator, string name, Func<string, string> installAction,
    Action<string> runtimeAction)
        {
            configurator.AddCommandLineDefinition(Prefix + name, runtimeAction);
            configurator.AddCommandLineDefinition(name, s => Add(configurator, Prefix + name, installAction(s)));

            return configurator;
        }

        public static HostConfigurator WithCustomStartParameter(this HostConfigurator configurator, string installName, string runtimeName,
    Func<string, string> installAction, Action<string> runtimeAction)
        {
            configurator.AddCommandLineDefinition(runtimeName, runtimeAction);
            configurator.AddCommandLineDefinition(installName, s => Add(configurator, runtimeName, installAction(s)));

            return configurator;
        }

        private static void Add(HostConfigurator configurator, string name, string value)
        {
            List<Tuple<string, string>> pairs;
            if (!ActionTable.TryGetValue(configurator, out pairs))
            {
                pairs = new List<Tuple<string, string>>();
                ActionTable.Add(configurator, pairs);
            }

            pairs.Add(new Tuple<string, string>(name, value));
        }

        internal static List<Tuple<string, string>> Commands(HostConfigurator configurator)
        {
            List<Tuple<string, string>> pairs;
            ActionTable.TryGetValue(configurator, out pairs);

            return pairs;
        } 
    }
}
