using Microsoft.Extensions.DependencyInjection;
using YAOCr.Core.Providers;
using YAOCr.Services;
using YAOCr.ViewModels;

namespace YAOCr.Bootstrap;
public static class IocConfiguration {

    public static IServiceCollection RegisterViewModels(this IServiceCollection services) {
        services.AddTransient<ConversationsViewModel>();

        return services;
    }

    public static IServiceCollection RegisterProviders(this IServiceCollection services) {
        services.AddTransient<IConversationProvider, FakeConversationProvider>();
        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services) {
        services.AddSingleton<ThemeService>();
        return services;
    }
}
