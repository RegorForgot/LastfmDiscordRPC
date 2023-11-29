using System.Reflection;
using Autofac;
using DiscordRPC.Logging;
using LastfmDiscordRPC2.Assets;
using LastfmDiscordRPC2.IO;
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

        builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
        
        builder.RegisterAssemblyTypes(Assembly.Load(nameof(LastfmDiscordRPC2)))
            .Where(t => t.Namespace.Contains("ViewModels.Panes"))
            .As(typeof(AbstractPaneViewModel))
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(Assembly.Load(nameof(LastfmDiscordRPC2)))
            .Where(t => t.Namespace.Contains("ViewModels.Controls"))
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<LoggingService>().AsSelf().SingleInstance();
        
        builder.RegisterType<ViewLogger>().As<IRPCLogger>().SingleInstance().WithParameter("level", LogLevel.Info);
        builder.RegisterType<TextLogger>().As<IRPCLogger>().SingleInstance().WithParameter("level", LogLevel.Warning);

        builder.RegisterType<LastfmAPIService>().AsSelf().SingleInstance();
        
        builder.RegisterType<SecretKey>().As<ISecretKey>().InstancePerLifetimeScope();
        builder.RegisterType<SignatureLocalAPIService>().As<ISignatureAPIService>().SingleInstance();
        builder.RegisterType<SignatureAPIService>().As<ISignatureAPIService>().SingleInstance().PreserveExistingDefaults();

        builder.RegisterType<DiscordClient>().As<IDiscordClient>().SingleInstance();
        builder.RegisterType<PresenceService>().As<IPresenceService>().SingleInstance();

        builder.RegisterType<LogIOService>().AsSelf().SingleInstance();
        builder.RegisterType<SaveCfgIOService>().AsSelf().SingleInstance();
            
        return builder.Build();
    }
}