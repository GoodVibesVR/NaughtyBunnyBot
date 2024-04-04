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
                .WithName(SlashCommandConstants.ChannelAdd)
                .WithDescription(SlashCommandConstants.ChannelAddDescription),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.ChannelRemove)
                .WithDescription(SlashCommandConstants.ChannelRemoveDescription),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.ChannelList)
                .WithDescription(SlashCommandConstants.ChannelListDescription),

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
                .WithDescription(SlashCommandConstants.ProfileDescription),

            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.Join)
                .WithDescription(SlashCommandConstants.JoinDescription),
            new SlashCommandBuilder()
                .WithName(SlashCommandConstants.Leave)
                .WithDescription(SlashCommandConstants.LeaveDescription)
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