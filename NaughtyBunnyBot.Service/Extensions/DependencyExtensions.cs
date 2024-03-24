using NaughtyBunnyBot.Lovense.Settings;
using Discord.WebSocket;
using Discord;
using NaughtyBunnyBot.Database.Repositories;
using NaughtyBunnyBot.Database.Repositories.Abstractions;
using NaughtyBunnyBot.Database.Services;
using NaughtyBunnyBot.Database.Services.Abstractions;
using DiscordConfig = NaughtyBunnyBot.Discord.Settings.DiscordConfig;
using NaughtyBunnyBot.Discord.Handlers;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using NaughtyBunnyBot.Discord.Services;
using NaughtyBunnyBot.Lovense.Services.Abstractions;
using NaughtyBunnyBot.Lovense.Services;
using NaughtyBunnyBot.Discord;
using NaughtyBunnyBot.Egg.Services;
using NaughtyBunnyBot.Egg.Services.Abstractions;
using NaughtyBunnyBot.Egg.Settings;
using NaughtyBunnyBot.Lovense;
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
            services.Configure<EggConfig>(configuration.GetSection("Egg"));
            var discordConfig = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.None
            };

            // Singletons
            services.AddSingleton(_ => discordConfig);
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<SlashCommandHandler>();
            services.AddSingleton<ISlashCommandService, SlashCommandService>();
            services.AddSingleton<IEnableCommandService, EnableCommandService>();
            services.AddSingleton<IScoreCommandService, ScoreCommandService>();
            services.AddSingleton<IChannelCommandService, ChannelCommandService>();

            services.AddSingleton<ILovenseService, LovenseService>();
            services.AddSingleton<IEggService, EggService>();
            services.AddSingleton<ILeaderboardService, LeaderboardService>();
            services.AddSingleton<IApprovedChannelsService, ApprovedChannelsService>();
            services.AddSingleton<ILeaderboardRepository>(_ =>
                new LeaderboardRepository(configuration.GetValue<string>("ConnectionStrings:Leaderboard") ??
                                          throw new ArgumentNullException("connectionString")));
            services.AddSingleton<IApprovedChannelsRepository>(_ =>
                new ApprovedChannelsRepository(configuration.GetValue<string>("ConnectionStrings:Leaderboard") ??
                                          throw new ArgumentNullException("connectionString")));

            // Transients
            services.AddTransient<ILovenseClient, LovenseClient>();

        // Hosted services
            services.AddHostedService<DiscordClient>();

            return services;
        }
    }
}
