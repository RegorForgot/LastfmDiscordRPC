using System.Reflection;
using Autofac;
using DiscordRPC.Logging;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.RPC;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.ViewModels.Panes;
using LastfmDiscordRPC2.ViewModels.Update;
using LastfmDiscordRPC2.Views;

namespace LastfmDiscordRPC2;

public static class ContainerConfigurator
{
    public static IContainer Configure()
    {
        ContainerBuilder builder = new ContainerBuilder();

        builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
        
        builder.RegisterAssemblyTypes(Assembly.Load(nameof(LastfmDiscordRPC2)))
            .Where(t => t.Namespace.Contains("ViewModels.Panes"))
            .As<AbstractPaneViewModel>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(Assembly.Load(nameof(LastfmDiscordRPC2)))
            .Where(t => t.Namespace.Contains("ViewModels.Controls"))
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterType<UIContext>().AsSelf().SingleInstance();

        builder.RegisterType<DialogWindow>().AsSelf().SingleInstance();
        
        builder.RegisterType<LoggingService>().AsSelf().SingleInstance();

        builder.RegisterType<ViewLogger>().As<IRPCLogger>().SingleInstance().WithParameter("level", LogLevel.Info);
        builder.RegisterType<TextLogger>().As<IRPCLogger>().SingleInstance().WithParameter("level", LogLevel.Warning);

        builder.RegisterType<LastfmAPIService>().AsSelf().SingleInstance();
        
        builder.RegisterType<SignatureAPIService>().As<ISignatureAPIService>().SingleInstance();

        builder.RegisterType<DiscordClient>().As<IDiscordClient>().SingleInstance();
        builder.RegisterType<PresenceService>().As<IPresenceService>().SingleInstance();

        builder.RegisterType<ViewModelSetter>().As<IViewModelSetter>().SingleInstance();

        builder.RegisterType<LogIOService>().AsSelf().SingleInstance();
        builder.RegisterType<SaveCfgIOService>().AsSelf().SingleInstance();
            
        return builder.Build();
    }
}