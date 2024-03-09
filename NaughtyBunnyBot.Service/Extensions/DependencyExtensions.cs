using NaughtyBunnyBot.Lovense.Settings;
using Discord.WebSocket;
using Discord;
using DiscordConfig = NaughtyBunnyBot.Discord.Settings.DiscordConfig;
using NaughtyBunnyBot.Discord.Handlers;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using NaughtyBunnyBot.Discord.Services;
using NaughtyBunnyBot.Lovense.Services.Abstractions;
using NaughtyBunnyBot.Lovense.Services;
using NaughtyBunnyBot.Discord;

namespace NaughtyBunnyBot.Service.Extensions
{
    public static class DependencyExtensions
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<LovenseConfig>(configuration.GetSection("Lovense"));
            services.Configure<DiscordConfig>(configuration.GetSection("Discord"));

            var discordConfig = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.None
            };

            // Singletons
            services.AddSingleton(_ => discordConfig);
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<SlashCommandHandler>();
            services.AddSingleton<ISlashCommandService, SlashCommandService>();
            services.AddSingleton<ILovenseService, LovenseService>();

            // Hosted services
            services.AddHostedService<DiscordClient>();

            return services;
        }
    }
}
