using System.Diagnostics;

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

        private bool _isConnected = false;

        public static async Task<DiscordChannelHelper> FromCredentials(ulong guildId, ulong channelId, string botToken)
        {
            if (guildId == 0 || channelId == 0)
            {
                throw new ArgumentException($"Some of the arguments sent to {nameof(DiscordChannelHelper)}.{nameof(FromCredentials)} are invalid.");
            }

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

            _client.Connected += async () =>
            {
                _isConnected = true;
            };

            await _client.LoginAsync(TokenType.Bot, _botToken).ConfigureAwait(false);
            await _client.StartAsync().ConfigureAwait(false);

            await WaitForConnectionAsync().ConfigureAwait(false);

            SocketGuild guild = _client.GetGuild(_guildId);

            await WaitForChannelsAsync(guild).ConfigureAwait(false);

            _channel = guild.GetTextChannel(_channelId);

            if (_channel == null)
            {
                throw new Exception("Channel not found");
            }
        }

        private static async Task WaitForChannelsAsync(SocketGuild guild)
        {
            int timeout = 0;

            while (guild.Channels.Count < 1)
            {
                Debug.WriteLine("Waiting for channels");
                await Task.Delay(1000);

                timeout++;
                if (timeout > 10)
                {
                    throw new Exception("Timeout while waiting for channels.");
                }
            }
        }

        private async Task WaitForConnectionAsync()
        {
            int timeout = 0;

            while (!_isConnected)
            {
                Debug.WriteLine("Waiting for connection");
                await Task.Delay(1000);

                timeout++;

                if (timeout > 10)
                {
                    throw new Exception("Timeout while waiting for connection.");
                }
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