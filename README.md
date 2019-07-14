Topshelf.StartParameters
===================

Topshelf.StartParameters is a small extension based on the amazing [Topshelf project](http://topshelf-project.com/), with the sole purpose of adding Customizable Start Parameters to Topshelf based services.

**NOTE:** *this extension only works for windows based services.*

How to use
-------------
The only dependency is Topshelf *(the extension is written based on Version 4.2.0)* itself, so we can simply add a reference to Topshelf.StartParameters.dll or copy the project and reference it directly.

Reference
------------

###EnableStartParameters###

An extension method that should be called to activate StartParameters.

*Usage:*
```c#
HostFactory.Run(x =>
            {
                x.EnableStartParameters();
            });
```

AddParameters is taking advantage of custom Topshelf environment, most of the code for the custom `SpWindowsHostEnvironmentBuilder` is borrowed from the Topshelf project itself with some subtle changes to add Start Parameters Support.

The following code is equivalent to calling `EnableStartParameters`:

```c#
HostFactory.Run(x =>
            {
                x.EnableStartParameters();
            });
```
  
###WithStartParameter###

The method which we can be used to set a start parameter for the service. it comes in two flavors `WithStartParameter(string name, Action<string> action)` and `WithStartParameter(string name, string value)`, the former can be used to set a start parameter value during service installation and the latter can be used to set a constant value as a start parameter.

####WithStartParameter(string name, Action<string> action)####
*Usage:*

```c#
HostFactory.Run(x =>
            {
                x.WithStartParameter("config",
                    a => HostLogger.Get("StartParameters").InfoFormat("parameter: {0}, value: {1}", "config", a));
            });
```

Under the hood it will create command line definitions **`config`** and **`tsspconfig`**, the former would be used during the installation, and the latter would be passed automatically to the service during startup.

####WithStartParameter(string name, string value, Action<string> action)####
*Usage:*

```c#
HostFactory.Run(x =>
            {
                x.WithStartParameter("test", "hello world from start parameter!",
                    a => HostLogger.Get("StartParameters").InfoFormat("constant parameter: {0}, value: {1}", "test", a));
            });
```

**Note:** Be careful to not to define a parameter or command line definition with the name prefixed with **`tssp`** and similar to other defined parameters or command line definitions. (e.g never define both parameters **`test`** and **`tssptest`** at the same time because the extension is using **`tssptest`** under the hood for the **`test`**).

**Note:** Never define a command line definition with the same name as a parameter (or parameter name with **`tssp`** prefix), the extension will use the parameter name as a command line definition under the hood.

**Note:** The command line defined for a constant parameter should not be used during installation, it will be added to the service parameters to be called automatically during service startup.

###WithCustomStartParameter(string argName,string paramName, string value, Action<string> action)###

Similar to `WithStartParameter` but with complete control over configuration and runtime naming.
`argName` specifies configuration name for the parameter which can be used during installation process.
`paramName` specifies runtime name for the parameter which can be used with the start command, and will be automatically passed to the service during startup.

*Usage:*

####WithStartParameter(string name, string value, Action<string> action)####
*Usage:*

```c#
HostFactory.Run(x =>
            {
                x.WithCustomStartParameter("setmyparam", "myparam",
                    a => HostLogger.Get("StartParameters").InfoFormat("custom parameter: {0}, value: {1}", "myparam", a));
            });
```

Code Sample
---------------

Let's dive into the code!

```c#
HostFactory.Run(x =>
            {
                x.EnableStartParameters();
                x.UseNLog();
                x.Service<MyService>(sc =>
                {
                    sc.ConstructUsing(hs => new MyService(hs));
                    sc.WhenStarted((s, h) => s.Start(h));
                    sc.WhenStopped((s, h) => s.Stop(h));
                });

                x.WithStartParameter("config",
                    a => HostLogger.Get("StartParameters").InfoFormat("parameter: {0}, value: {1}", "config", a));

                x.WithStartParameter("test", "hello world from start parameter!",
                    a => HostLogger.Get("StartParameters").InfoFormat("constant parameter: {0}, value: {1}", "test", a));

                x.WithCustomStartParameter("setmyparam", "myparam",
                    a => HostLogger.Get("StartParameters").InfoFormat("custom parameter: {0}, value: {1}", "myparam", a));
                    
                x.SetServiceName("MyService");
                x.SetDisplayName("My Service");
                x.SetDescription("Sample Service");
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

```

Command Samples
---------------

An installation command based on the sample code:

    MyService.exe install -config "standard" -setmyparam "customparam"

As with Topshelf command arguments, we can use a parameter multiple times.

    MyService.exe install -config "standard" -config "custom" -setmyparam "customparam1" -setmyparam "customparam2"
    
and of course we can use them in instances

    MyService.exe install -instance "i00" -config "standard0" -config "custom0"
    
    MyService.exe install -instance "i01" -config "standard1" -config "custom1" -setmyparam "customparam2"
    