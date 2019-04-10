using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using Topshelf.HostConfigurators;
using Topshelf.Logging;
using Topshelf.Runtime;
using Topshelf.Runtime.Windows;

namespace Topshelf.StartParameters
{
    public class SpWindowsHostEnvironment :
        HostEnvironment
    {
        readonly LogWriter _log = HostLogger.Get(typeof(WindowsHostEnvironment));
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
                    if (beforeInstall != null)
                    {
                        beforeInstall(settings);
                        installer.ServiceProcessInstaller.Username = settings.Credentials.Username;
                        installer.ServiceProcessInstaller.Account = settings.Credentials.Account;

                        bool gMSA = false;
                        // Group Managed Service Account (gMSA) workaround per
                        // https://connect.microsoft.com/VisualStudio/feedback/details/795196/service-process-installer-should-support-virtual-service-accounts
                        if (settings.Credentials.Account == ServiceAccount.User &&
                            settings.Credentials.Username != null &&
                            ((gMSA = settings.Credentials.Username.EndsWith("$", StringComparison.InvariantCulture)) ||
                            string.Equals(settings.Credentials.Username, "NT SERVICE\\" + settings.ServiceName, StringComparison.InvariantCulture)))
                        {
                            _log.InfoFormat(gMSA ? "Installing as gMSA {0}." : "Installing as virtual service account", settings.Credentials.Username);
                            installer.ServiceProcessInstaller.Password = null;
                            installer.ServiceProcessInstaller
                                .GetType()
                                .GetField("haveLoginInfo", BindingFlags.Instance | BindingFlags.NonPublic)
                                .SetValue(installer.ServiceProcessInstaller, true);
                        }
                        else
                        {
                            installer.ServiceProcessInstaller.Password = settings.Credentials.Password;
                        }
                    }
                };

                Action<InstallEventArgs> after = x =>
                {
                    if (afterInstall != null)
                        afterInstall();
                };

                Action<InstallEventArgs> before2 = x =>
                {
                    if (beforeRollback != null)
                        beforeRollback();
                };

                Action<InstallEventArgs> after2 = x =>
                {
                    if (afterRollback != null)
                        afterRollback();
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
            _environment.SendServiceCommand(serviceName, command);
        }

        public void UninstallService(HostSettings settings, Action beforeUninstall, Action afterUninstall)
        {
            _environment.UninstallService(settings, beforeUninstall, afterUninstall);
        }
    }
}