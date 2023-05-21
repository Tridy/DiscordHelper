using Microsoft.Extensions.Configuration;

using Xunit;

namespace DiscordHelper.Tests
{
    public partial class DiscordChannelHelperTests
    {
        public ulong GuildId = 0;
        public ulong ChannelId = 0;
        public string BotToken = "";

        public DiscordChannelHelperTests()
        {
            IConfigurationRoot conf = new ConfigurationBuilder()
                        .AddJsonFile("test.settings.json", optional: false)
                        .AddJsonFile("test.settings.local.json", optional: true)
                        .Build();

            GuildId = ulong.Parse(conf["GuildId"]);
            ChannelId = ulong.Parse(conf["ChannelId"]);
            BotToken = conf["BotToken"];
        }

        [Fact]
        public async Task CanSendDiscordMessage()
        {
            using (var helper = await DiscordChannelHelper.FromCredentials(GuildId, ChannelId, BotToken))
            {
                await helper.SendMessagesAsync("Test Message");
            }
        }
    }
}