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
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.Enable)
                .WithDescription(SlashCommandConstants.EnableDescription),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.Disable)
                .WithDescription(SlashCommandConstants.DisableDescription),
                
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.Leaderboard)
                .WithDescription(SlashCommandConstants.LeaderboardDescription),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.Profile)
                .WithDescription(SlashCommandConstants.ProfileDescription)
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