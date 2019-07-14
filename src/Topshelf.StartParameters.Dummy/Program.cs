namespace Topshelf.StartParameters.Dummy
{
    class Program
    {
        private static string _path;

        private static string _action;
        private static string _manual;
        private static string _auto;
        private static string _constant;

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.EnableStartParameters();
                x.Service<Bootstrapper>(sc =>
                {
                    sc.ConstructUsing(hs => new Bootstrapper(_path)
                    {
                        ActionSample = _action,
                        AutoSample = _auto,
                        ManualSample = _manual,
                        ConstantSample = _constant
                    });
                    sc.WhenStarted((s, h) => s.Start(h));
                    sc.WhenStopped((s, h) => s.Stop(h));
                });

                x.WithStartParameter("setpath", "path", n => _path = n);

                x.WithStartParameter("setaction", "action", (n) => n.ToUpperInvariant(), n => _action = n);
                x.WithStartParameter("setmanual", "manual", n => _manual = n);
                x.WithStartParameter("auto", n => _auto = n);
                x.WithConstantStartParameter("constant","hello", n => _constant = n);

                x.SetServiceName("TSSP");
                x.SetDisplayName("TS SP Dummy");
                x.SetDescription("TopShelf Start Paramter Dummy Test Service");
                x.StartAutomatically();
                x.RunAsLocalSystem();
                x.EnableServiceRecovery(r =>
                {
                    r.OnCrashOnly();
                    r.RestartService(1); //first
                    r.RestartService(1); //second
                    r.RestartService(1); //subsequents
                    r.SetResetPeriod(0);
                });
            });
        }
    }
}
