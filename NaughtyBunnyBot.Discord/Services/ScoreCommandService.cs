using NaughtyBunnyBot.Database.Services.Abstractions
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

        if (leaderboard is not null)
        {
            foreach (var entry in leaderboard)
            {
                var user = _discordClient.GetUser(entry.UserId);
                embedBuilder.AddField(user?.Username ?? "Unknown", entry.Score);
            }
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
