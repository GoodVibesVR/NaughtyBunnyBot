using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using NaughtyBunnyBot.Discord.Constants;
using NaughtyBunnyBot.Discord.Services.Abstractions;

namespace NaughtyBunnyBot.Discord.Services;

public class SlashCommandService : ISlashCommandService
{
    private readonly ILogger<SlashCommandService> _logger;
    private readonly DiscordSocketClient _discordClient;

    public SlashCommandService(ILogger<SlashCommandService> logger, DiscordSocketClient discordClient)
    {
        _logger = logger;
        _discordClient = discordClient;
    }

    public async Task BuildSlashCommandsAsync()
    {
        var slashCommands = new[]
        {
            // Game server commands
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.BooliTest)
                .WithDescription(SlashCommandConstants.BooliTestDescription),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.BooliList)
                .WithDescription(SlashCommandConstants.BooliListDescription),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.BooliPerformance)
                .WithDescription(SlashCommandConstants.BooliPerformanceDescription),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.BooliGet)
                .WithDescription(SlashCommandConstants.BooliGetDescription)
                .AddOption("id", ApplicationCommandOptionType.String, "The identifier of the game server", isRequired: true),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.BooliRestart)
                .WithDescription(SlashCommandConstants.BooliRestartDescription)
                .AddOption("id", ApplicationCommandOptionType.String, "The identifier of the game server", isRequired: true),

            // Bully commands
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.Booli)
                .WithDescription(SlashCommandConstants.BooliDescription)
                .AddOption("user", ApplicationCommandOptionType.User, "The friend you want to booli!", isRequired: false)
        };

        foreach (var slashCommand in slashCommands)
        {
            _logger.LogDebug($"Registering SlashCommand: {slashCommand.Name}");
            await _discordClient.CreateGlobalApplicationCommandAsync(slashCommand.Build());
        }
    }

    public async Task HandleTestCommandAsync(SocketSlashCommand command)
    {
        await command.RespondAsync($"Bot is responding from SlashCommandService");
    }
}