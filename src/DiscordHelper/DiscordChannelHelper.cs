using Discord;
using Discord.WebSocket;

namespace DiscordHelper
{
    public class DiscordChannelHelper : IDisposable
    {
        private readonly ulong _guildId;
        private readonly ulong _channelId;
        private readonly string _botToken;

        private SocketTextChannel? _channel;
        private bool disposedValue;

        private DiscordSocketClient? _client;

        public static async Task<DiscordChannelHelper> FromCredentials(ulong guildId, ulong channelId, string botToken)
        {
            var instance = new DiscordChannelHelper(guildId, channelId, botToken);
            await instance.LoginAsync().ConfigureAwait(false);
            return instance;
        }

        private DiscordChannelHelper(ulong guildId, ulong channelId, string botToken)
        {
            _guildId = guildId;
            _channelId = channelId;
            _botToken = botToken;
        }

        private async Task LoginAsync()
        {
            _client = new DiscordSocketClient();
            bool isConnected = false;

            _client.Connected += async () => isConnected = true;

            await _client.LoginAsync(TokenType.Bot, _botToken).ConfigureAwait(false);
            await _client.StartAsync().ConfigureAwait(false);

            while (!isConnected)
            {
                await Task.Delay(1000);
            }

            SocketGuild guild = _client.GetGuild(_guildId);

            while (guild.Channels.Count < 1)
            {
                await Task.Delay(1000);
            }

            _channel = guild.GetTextChannel(_channelId);

            if (_channel == null)
            {
                throw new Exception("Channel not found");
            }
        }

        public async Task SendMessagesAsync(params string[] messages)
        {
            if (_channel == null)
            {
                throw new Exception("Channel not found");
            }

            foreach (string message in messages)
            {
                await _channel.SendMessageAsync(message);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}