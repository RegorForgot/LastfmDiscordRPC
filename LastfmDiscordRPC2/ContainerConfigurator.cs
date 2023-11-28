using System.Reflection;
using Autofac;
using DiscordRPC.Logging;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2;

public static class ContainerConfigurator
{
    public static IContainer Configure()
    {
        ContainerBuilder builder = new ContainerBuilder();

        builder.RegisterAssemblyTypes(Assembly.Load(nameof(LastfmDiscordRPC2)))
            .Where(t => t.Namespace.Contains("ViewModels.Panes"))
            .As(typeof(IPaneViewModel))
            .InstancePerLifetimeScope();

        builder.RegisterType<MainViewModel>().As<IWindowViewModel>().SingleInstance();
        builder.RegisterType<SettingsConsoleViewModel>().As<ILoggingControlViewModel>().SingleInstance();
        builder.RegisterType<TextLogger>().As<IRPCLogger>().SingleInstance().WithParameter("level", LogLevel.Trace);
        builder.RegisterType<ViewLogger>().SingleInstance().WithParameter("level", LogLevel.Trace);
        builder.RegisterType<LastfmAPIClient>().SingleInstance();
        builder.RegisterType<SignatureLocalClient>().As<ISignatureAPIClient>().SingleInstance();
            
        return builder.Build();
    }
}