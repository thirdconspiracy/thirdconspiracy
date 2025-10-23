using poker.api.Repos;
using poker.api.Services;

namespace poker.api.ExtensionMethods;

public static class PokerExtensions
{
    public static IServiceCollection AddPoker(this IServiceCollection services)
    {
        services.AddSignalR(o => o.EnableDetailedErrors = true);
        services.AddSingleton<IPokerRepository, PokerRepository>();
        services.AddScoped<IPokerService, PokerService>();
        return services;
    }
}