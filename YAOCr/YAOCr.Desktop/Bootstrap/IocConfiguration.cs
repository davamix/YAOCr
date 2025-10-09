using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YAOCr.Core.Providers;
using YAOCr.Core.Services;
using YAOCr.Services;
using YAOCr.ViewModels;

namespace YAOCr.Bootstrap;
public static class IocConfiguration {

    public static IServiceCollection RegisterViewModels(this IServiceCollection services) {
        services.AddTransient<ConversationsViewModel>();
        services.AddTransient<SettingsViewModel>();

        return services;
    }

    public static IServiceCollection RegisterProviders(this IServiceCollection services) {
        services.AddTransient<IConversationProvider, FakeConversationProvider>();
        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services) {
        services.AddSingleton<ThemeService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<ILlmProvider, LlamaCppProvider>();
        services.AddSingleton<IConversationsService, ConversationsService>();

        return services;
    }

    public static IServiceCollection RegisterConfiguration(this IServiceCollection services) {
        services.AddSingleton(typeof(IConfiguration), sp => new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build());

        return services;
    }
}
