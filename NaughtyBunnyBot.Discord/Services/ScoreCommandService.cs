using NaughtyBunnyBot.Database.Services.Abstractions;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace NaughtyBunnyBot.Discord.Services;

public class ScoreCommandService : IScoreCommandService
{
    private readonly ILogger<ScoreCommandService> _logger;
    private readonly DiscordSocketClient _discordClient;
    private readonly ILeaderboardService _leaderBoardService;


    public ScoreCommandService(ILogger<ScoreCommandService> logger, DiscordSocketClient discordClient, ILeaderboardService leaderboardService)
    {
        _logger = logger;
        _discordClient = discordClient;
        _leaderBoardService = leaderboardService;
    }

    public async Task HandleLeaderboardCommandAsync(SocketSlashCommand command)
    {
        if (!command.GuildId.HasValue)
        {
            await command.RespondAsync("This command can only be used in a guild.");
            return;
        }

        await command.DeferAsync();

        var guildId = command.GuildId.Value;
        var leaderboard = await _leaderBoardService.GetGuildTopLeaderboardAsync(guildId.ToString());

        var embedBuilder = new EmbedBuilder()
            .WithTitle("Top 10 Leaderboard")
            .WithColor(Color.Blue)
            .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

        if (leaderboard is not null && leaderboard.Count() > 0)
        {
            var desc = new System.Text.StringBuilder();

            var index = 0;
            foreach(var entry in leaderboard)
            {
                index++;

                desc.AppendLine($"{index}. **<@{entry.UserId}>** - {entry.Score} Eggs");
            }
            
            embedBuilder.WithDescription(desc.ToString());
        }
        else
        {
            embedBuilder.AddField("No entries", "No entries found");
        }

        await command.FollowupAsync(embed: embedBuilder.Build(), allowedMentions: new AllowedMentions(AllowedMentionTypes.None));
    }

    public async Task HandleProfileCommandAsync(SocketSlashCommand command)
    {
        if (!command.GuildId.HasValue)
        {
            await command.RespondAsync("This command can only be used in a guild.");
            return;
        }

        await command.DeferAsync();

        var guildId = command.GuildId.Value;
        var userId = command.User.Id;

        var leaderboard = await _leaderBoardService.GetLeaderboardEntryByGuildAndUser(guildId.ToString(), userId.ToString());

        var embedBuilder = new EmbedBuilder()
            .WithTitle("Profile")
            .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
            .WithColor(Color.DarkPurple)
            .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

        if (leaderboard is not null)
        {
            embedBuilder.AddField("Score", leaderboard.Score);
        }
        else
        {
            embedBuilder.AddField("Score", 0);
        }

        await command.FollowupAsync(embed: embedBuilder.Build(), allowedMentions: new AllowedMentions(AllowedMentionTypes.None));
    }
}
