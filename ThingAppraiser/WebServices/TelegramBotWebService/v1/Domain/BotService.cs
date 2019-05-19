﻿using Microsoft.Extensions.Options;
using MihaZupan;
using Telegram.Bot;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public class BotService : IBotService
    {
        private readonly BotConfiguration _config;

        #region IBotService Implementation

        public TelegramBotClient Client { get; }

        #endregion


        public BotService()
        {
        }

        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value.ThrowIfNull(nameof(config));

            // Use proxy if configured in appsettings.*.json
            Client = string.IsNullOrWhiteSpace(_config.Socks5Host)
                ? new TelegramBotClient(_config.BotToken)
                : new TelegramBotClient(
                    _config.BotToken,
                    new HttpToSocks5Proxy(_config.Socks5Host, _config.Socks5Port));
        }

    }
}
