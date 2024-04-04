namespace NaughtyBunnyBot.Discord.Constants
{
    public static class SlashCommandConstants
    {
        public const string ChannelAdd = "addchannel";
        public const string ChannelAddDescription = "[Admin] Adds a channel to the approved list";

        public const string ChannelRemove = "removechannel";
        public const string ChannelRemoveDescription = "[Admin] Removes a channel from the approved list";

        public const string ChannelList = "listchannels";
        public const string ChannelListDescription = "[Admin] Lists all approved channels";


        public const string Enable = "enable";
        public const string EnableDescription = "[Admin] Enables Egg Hunting";

        public const string Disable = "disable";
        public const string DisableDescription = "[Admin] Disables Egg Hunting";


        public const string Leaderboard = "leaderboard";
        public const string LeaderboardDescription = "Shows top 10 places";
        
        public const string Profile = "profile";
        public const string ProfileDescription = "Shows user's current standing";

        public const string Join = "join";
        public const string JoinDescription = "Join the current ongoing hunt";

        public const string Leave = "leave";
        public const string LeaveDescription = "Leave the current ongoing hunt";
    }
}
