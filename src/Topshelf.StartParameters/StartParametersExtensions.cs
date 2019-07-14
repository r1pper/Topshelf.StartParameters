﻿using System;
using System.Collections.Generic;
using Topshelf.HostConfigurators;

namespace Topshelf.StartParameters
{
    public static class StartParametersExtensions
    {
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
            configurator.AddCommandLineDefinition(name, action);
            configurator.AddCommandLineDefinition(name, s => Add(configurator, name, s));

            return configurator;
        }

        public static HostConfigurator WithStartParameter(this HostConfigurator configurator, string name, string value,
                                                          Action<string> action)
        {
            Add(configurator, name, value);
            configurator.AddCommandLineDefinition(name, action);

            return configurator;
        }

        public static HostConfigurator WithCustomStartParameter(this HostConfigurator configurator, string argName,
                                                                string paramName, Action<string> action)
        {
            configurator.AddCommandLineDefinition(paramName, action);
            configurator.AddCommandLineDefinition(argName, s => Add(configurator, paramName, s));

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
