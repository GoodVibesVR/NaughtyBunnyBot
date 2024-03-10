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
using NaughtyBunnyBot.Lovense.Abstractions;

namespace NaughtyBunnyBot.Service.Extensions
{
    public static class DependencyExtensions
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Config
            var lovenseSection = configuration.GetSection("Lovense");
            var lovenseConfig = lovenseSection.Get<LovenseConfig>();
            services.AddHttpClient(
                lovenseConfig?.ClientName ?? "LovenseClient",
                client =>
                {
                    // Set the base address of the named client.
                    client.BaseAddress = new Uri(lovenseConfig!.ApiRoot!);

                    // Add a user-agent default request header.
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
                }
            );

            services.Configure<LovenseConfig>(lovenseSection);
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

            // Transients
            services.AddTransient<ILovenseClient, ILovenseClient>();

            // Hosted services
            services.AddHostedService<DiscordClient>();

            return services;
        }
    }
}
