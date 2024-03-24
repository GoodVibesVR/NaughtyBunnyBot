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

        var guildId = command.GuildId.Value;
        var leaderboard = await _leaderBoardService.GetGuildTopLeaderboardAsync(guildId.ToString());

        var embedBuilder = new EmbedBuilder()
            .WithTitle("Leaderboard")
            .WithDescription("Top 10 leaderboard")
            .WithColor(Color.Blue)
            .WithCurrentTimestamp()
            .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

        if (leaderboard is not null && leaderboard.Count() > 0)
        {
            foreach (var entry in leaderboard)
            {
                string userName = "Unknown";
                var sUser = _discordClient.GetUser(entry.UserId);

                if (sUser is not null)
                {
                    userName = sUser.Username;
                }
                else
                {
                    // Fetch user from REST API
                    var user = await _discordClient.Rest.GetUserAsync((ulong)Int64.Parse(entry.UserId));
                    if (user is not null)
                    {
                        userName = user.Username;
                    }
                }

                embedBuilder.AddField(userName, entry.Score);
            }
        }
        else
        {
            embedBuilder.AddField("No entries", "No entries found");
        }

        await command.RespondAsync(embed: embedBuilder.Build());
    }

    public async Task HandleProfileCommandAsync(SocketSlashCommand command)
    {
        if (!command.GuildId.HasValue)
        {
            await command.RespondAsync("This command can only be used in a guild.");
            return;
        }

        var guildId = command.GuildId.Value;
        var userId = command.User.Id;

        var leaderboard = await _leaderBoardService.GetLeaderboardEntryByGuildAndUser(guildId.ToString(), userId.ToString());

        var embedBuilder = new EmbedBuilder()
            .WithTitle("Profile")
            .WithDescription("Your profile")
            .WithColor(Color.Blue)
            .WithCurrentTimestamp()
            .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

        if (leaderboard is not null)
        {
            embedBuilder.AddField("Score", leaderboard.Score);
        }
        else
        {
            embedBuilder.AddField("Score", 0);
        }

        await command.RespondAsync(embed: embedBuilder.Build());
    }
}
