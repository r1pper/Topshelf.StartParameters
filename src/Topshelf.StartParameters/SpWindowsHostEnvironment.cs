using System;
using System.Configuration.Install;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;
using Topshelf.Runtime.Windows;

namespace Topshelf.StartParameters
{
    public class SpWindowsHostEnvironment :
        HostEnvironment
    {
        private readonly HostConfigurator _hostConfigurator;
        private readonly WindowsHostEnvironment _environment;

        public SpWindowsHostEnvironment(HostConfigurator configurator)
        {
            _hostConfigurator = configurator;
            _environment=new WindowsHostEnvironment(configurator);
        }

        public bool IsServiceInstalled(string serviceName)
        {
            return _environment.IsServiceInstalled(serviceName);
        }

        public bool IsServiceStopped(string serviceName)
        {
            return _environment.IsServiceStopped(serviceName);
        }

        public void StartService(string serviceName, TimeSpan startTimeOut)
        {
            _environment.StartService(serviceName,startTimeOut);
        }

        public void StopService(string serviceName, TimeSpan stopTimeOut)
        {
            _environment.StopService(serviceName,stopTimeOut);
        }

        public void InstallService(
            InstallHostSettings settings, 
            Action<InstallHostSettings> beforeInstall, 
            Action afterInstall, 
            Action beforeRollback,
            Action afterRollback)
        {
            using (var installer = new SpHostServiceInstaller(settings, _hostConfigurator))
            {
                Action<InstallEventArgs> before = x =>
                {
                    beforeInstall?.Invoke(settings);
                };

                Action<InstallEventArgs> after = x =>
                {
                    afterInstall?.Invoke();
                };

                Action<InstallEventArgs> before2 = x =>
                {
                    beforeRollback?.Invoke();
                };

                Action<InstallEventArgs> after2 = x =>
                {
                    afterRollback?.Invoke();
                };

                installer.InstallService(before, after, before2, after2);
            }
        }

        public string CommandLine
        {
            get { return _environment.CommandLine; }
        }

        public bool IsAdministrator
        {
            get { return _environment.IsAdministrator; }
        }

        public bool IsRunningAsAService
        {
            get { return _environment.IsRunningAsAService; }
        }

        public bool RunAsAdministrator()
        {
            return _environment.RunAsAdministrator();
        }

        public Host CreateServiceHost(HostSettings settings, ServiceHandle serviceHandle)
        {
            return _environment.CreateServiceHost(settings, serviceHandle);
        }

        public void SendServiceCommand(string serviceName, int command)
        {
            _environment.SendServiceCommand(serviceName,command);
        }

        public void InstallService(
            InstallHostSettings settings, 
            Action beforeInstall, 
            Action afterInstall, 
            Action beforeRollback, 
            Action afterRollback)
        {
            using (var installer = new SpHostServiceInstaller(settings,_hostConfigurator))
            {
                Action<InstallEventArgs> before = x =>
                {
                    beforeInstall?.Invoke();
                };

                Action<InstallEventArgs> after = x =>
                {
                    afterInstall?.Invoke();
                };

                Action<InstallEventArgs> before2 = x =>
                {
                    beforeRollback?.Invoke();
                };

                Action<InstallEventArgs> after2 = x =>
                {
                    afterRollback?.Invoke();
                };

                installer.InstallService(before, after, before2, after2);
            }
        }

        public void UninstallService(HostSettings settings, Action beforeUninstall, Action afterUninstall)
        {
            _environment.UninstallService(settings,beforeUninstall,afterUninstall);
        }
    }
}