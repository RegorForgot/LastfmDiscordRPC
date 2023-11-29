using System.Reflection;
using Autofac;
using DiscordRPC.Logging;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.IO.Schema;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.RPC;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.ViewModels.Controls;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2;

public static class ContainerConfigurator
{
    public static IContainer Configure()
    {
        ContainerBuilder builder = new ContainerBuilder();

        builder.RegisterAssemblyTypes(Assembly.Load(nameof(LastfmDiscordRPC2)))
            .Where(t => t.Namespace.Contains("ViewModels.Panes"))
            .As(typeof(AbstractPaneViewModel))
            .InstancePerLifetimeScope();

        builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<SettingsConsoleViewModel>().As<AbstractLoggingControlViewModel>().SingleInstance();

        builder.RegisterType<ViewLogger>().As<IRPCLogger>().SingleInstance().WithParameter("level", LogLevel.Info);
        builder.RegisterType<TextLogger>().As<IRPCLogger>().SingleInstance().WithParameter("level", LogLevel.Warning);
        
        builder.RegisterType<LoggingService>().As<AbstractLoggingService>().SingleInstance();

        builder.RegisterType<DiscordClient>().As<IDiscordClient>().SingleInstance();
        builder.RegisterType<PresenceService>().As<IPresenceService>().SingleInstance();
        
        builder.RegisterType<LastfmService>().AsSelf().SingleInstance();
        builder.RegisterType<SignatureLocalClient>().As<ISignatureAPIClient>().SingleInstance();

        builder.RegisterType<LogFileIO>().AsSelf().SingleInstance();
        builder.RegisterType<SaveDataFileIO>().As<AbstractConfigFileIO<SaveData>>().SingleInstance();
            
        return builder.Build();
    }
}