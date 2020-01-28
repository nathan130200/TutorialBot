using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Lavalink;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Tutorial
{
    public class TutorialBot
    {
        public static TutorialBot Instance { get; private set; }
        public DiscordClient Discord { get; private set; }
        public IServiceProvider Services { get; private set; }
        public CommandsNextExtension CommandsNext { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public LavalinkExtension Lavalink { get; private set; }

        public TutorialBot()
        {
            Instance = this;

            this.Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Environment.GetEnvironmentVariable("TUTORIAL_BOT_TOKEN"),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Error,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                UseInternalLogHandler = true
            });

            this.Lavalink = this.Discord.UseLavalink();

            this.Interactivity = this.Discord.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                PaginationDeletion = PaginationDeletion.DeleteMessage,
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromMinutes(5d)
            });

            this.Services = new ServiceCollection()
                .AddSingleton(this)
                .BuildServiceProvider(true);

            this.CommandsNext = this.Discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "!" },
                EnableDms = false,
                Services = this.Services
            });

            this.CommandsNext.RegisterCommands(typeof(TutorialBot).Assembly);

            this.CommandsNext.CommandErrored += (e) =>
            {
                Console.WriteLine(e.Exception);
                return Task.CompletedTask;
            };
        }

        public Task StartAsync()
            => this.Discord.ConnectAsync();

        public Task StopAsync()
            => this.Discord.DisconnectAsync();
    }
}
