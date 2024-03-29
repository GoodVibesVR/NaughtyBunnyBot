using NaughtyBunnyBot.Database.Services.Abstractions;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Text;

namespace NaughtyBunnyBot.Discord.Services;

public class ChannelCommandService : IChannelCommandService
{
    private readonly ILogger<ScoreCommandService> _logger;
    private readonly DiscordSocketClient _discordClient;
    private readonly IApprovedChannelsService _approvedChannelsService;


    public ChannelCommandService(ILogger<ScoreCommandService> logger, DiscordSocketClient discordClient, IApprovedChannelsService approvedChannelsService)
    {
        _logger = logger;
        _discordClient = discordClient;
        _approvedChannelsService = approvedChannelsService;
    }

    public async Task HandleAddChannelCommandAsync(SocketSlashCommand command) 
    {
        if (!command.GuildId.HasValue)
        {
            await command.RespondAsync("This command can only be used in a guild.");
            return;
        }

        var guildId = command.GuildId.Value;
        var channelId = command.ChannelId!.Value;

        var approvedChannel = await _approvedChannelsService.GetApprovedChannelAsync(guildId.ToString(), channelId.ToString());
        if (approvedChannel is not null)
        {
            await command.RespondAsync($"Channel <#{channelId}> ({channelId}) is **already** in the approved channels list.");
            return;
        }

        await _approvedChannelsService.AddApprovedChannelAsync(guildId.ToString(), channelId.ToString());
        await command.RespondAsync($"Channel <#{channelId}> ({channelId}) has been added to the approved channels list.");
    }

    public async Task HandleRemoveChannelCommandAsync(SocketSlashCommand command)
    {
        if (!command.GuildId.HasValue)
        {
            await command.RespondAsync("This command can only be used in a guild.");
            return;
        }

        var guildId = command.GuildId.Value;
        var channelId = command.ChannelId!.Value;

        var approvedChannel = await _approvedChannelsService.GetApprovedChannelAsync(guildId.ToString(), channelId.ToString());
        if (approvedChannel == null)
        {
            await command.RespondAsync($"Channel <#{channelId}> ({channelId}) is **not** in the approved channels list.");
            return;
        }

        await _approvedChannelsService.RemoveApprovedChannelAsync(guildId.ToString(), channelId.ToString());

        await command.RespondAsync($"Channel <#{channelId}> ({channelId}) has been removed from the approved channels list.");
    }

    public async Task HandleListChannelsCommandAsync(SocketSlashCommand command)
    {
        if (!command.GuildId.HasValue)
        {
            await command.RespondAsync("This command can only be used in a guild.");
            return;
        }

        var guildId = command.GuildId.Value;

        var approvedChannels = await _approvedChannelsService.GetApprovedChannelByGuildAsync(guildId.ToString());

        var embedBuilder = new EmbedBuilder()
            .WithTitle("Approved Channels")
            .WithDescription("List of approved channels")
            .WithColor(Color.Blue)
            .WithCurrentTimestamp()
            .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

        var description = new StringBuilder();
        foreach (var channel in approvedChannels)
        {
            description.AppendLine($"<#{channel.ChannelId}> ({channel.ChannelId})");
        }

        embedBuilder.WithDescription(description.ToString());

        await command.RespondAsync(embed: embedBuilder.Build());
    }
}
