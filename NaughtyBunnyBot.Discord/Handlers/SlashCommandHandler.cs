using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NaughtyBunnyBot.Discord.Constants;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using DiscordConfig = NaughtyBunnyBot.Discord.Settings.DiscordConfig;

namespace NaughtyBunnyBot.Discord.Handlers;

public class SlashCommandHandler
{
    private readonly ILogger<SlashCommandHandler> _logger;
    private readonly DiscordConfig _discordSettings;
    private readonly ISlashCommandService _commandService;
    private readonly IEnableCommandService _enableCommandService;
    private readonly IScoreCommandService _scoreCommandService;
    private readonly IChannelCommandService _channelCommandService;

    public SlashCommandHandler(ILogger<SlashCommandHandler> logger, IOptions<DiscordConfig> discordSettings, 
        ISlashCommandService commandService, IEnableCommandService enableCommandService, IScoreCommandService scoreCommandService, IChannelCommandService channelCommandService)
    {
        _logger = logger;
        _discordSettings = discordSettings.Value;
        _commandService = commandService;

        _enableCommandService = enableCommandService;
        _scoreCommandService = scoreCommandService;
        _channelCommandService = channelCommandService;
    }

    public async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        try
        {
            switch (command.Data.Name)
            {
                case SlashCommandConstants.ChannelAdd:
                    if (!await IsUserAuthorized(command)) return;
                    await _channelCommandService.HandleAddChannelCommandAsync(command);
                    break;
                case SlashCommandConstants.ChannelRemove:
                    if (!await IsUserAuthorized(command)) return;
                    await _channelCommandService.HandleRemoveChannelCommandAsync(command);
                    break;
                case SlashCommandConstants.ChannelList:
                    if (!await IsUserAuthorized(command)) return;
                    await _channelCommandService.HandleListChannelsCommandAsync(command);
                    break;
                
                case SlashCommandConstants.Enable:
                    if (!await IsUserAuthorized(command)) return;
                    await _enableCommandService.HandleEnableCommandAsync(command);
                    break;
                case SlashCommandConstants.Disable:
                    if (!await IsUserAuthorized(command)) return;
                    await _enableCommandService.HandleDisableCommandAsync(command);
                    break;
                    
                case SlashCommandConstants.Leaderboard:
                    await _scoreCommandService.HandleLeaderboardCommandAsync(command);
                    break;
                case SlashCommandConstants.Profile:
                    await _scoreCommandService.HandleProfileCommandAsync(command);
                    break;
                default:
                    _logger.LogWarning($"Unknown command: {command.Data.Name}");
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred executing slash command.");
            await SendErrorResponseAsync(command, e);
        }
    }

    private async Task<bool> IsUserAuthorized(SocketSlashCommand command)
    {
        var isAdmin = _discordSettings.Admins!.Contains(command.User.Id);
        if (!isAdmin)
        {
            await command.RespondAsync($"You're not authorized to perform this action...", ephemeral: true);
        }

        return isAdmin;
    }

    private async Task SendErrorResponseAsync(SocketSlashCommand command, Exception e)
    {
        var embedBuilder = new EmbedBuilder()
            .WithAuthor(command.User.ToString(), command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl())
            .WithTitle($"Exception occurred")
            .WithDescription("I encountered an error while performing my task")
            .AddField("Command", command.Data.Name)
            .AddField("Message", e.Message)
            .WithColor(Color.Red)
            .WithCurrentTimestamp();

        // Check if defered
        if (command.HasResponded)
        {
            await command.FollowupAsync(embed: embedBuilder.Build());
            return;
        }
        else {
            await command.RespondAsync(embed: embedBuilder.Build());
        }
    }
}