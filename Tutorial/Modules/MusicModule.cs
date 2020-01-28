using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Modules
{
    [RequireGuild]
    public class MusicModule : BaseCommandModule
    {
        private LavalinkNodeConnection node;
        private LavalinkGuildConnection connection;
        private ConnectionEndpoint endpoint;
        private LavalinkConfiguration configuration;

        public MusicModule()
        {
            this.endpoint = new ConnectionEndpoint("127.0.0.1", 2333);
            this.configuration = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = this.endpoint,
                SocketEndpoint = this.endpoint,
                ResumeKey = "tutorial-bot"
            };
        }

        public override async Task BeforeExecutionAsync(CommandContext ctx)
        {
            var lavalink = ctx.Client.GetLavalink();

            if (this.node == null || !this.node.IsConnected)
                this.node = await lavalink.ConnectAsync(this.configuration);

            if (this.node?.IsConnected == false)
            {
                await ctx.RespondAsync($"{ctx.User.Mention} :x: Conexão com o lavalink falhou!");
                throw new InvalidOperationException();
            }
        }

        [Command]
        public async Task Join(CommandContext ctx)
        {
            if (this.node?.IsConnected == false)
            {
                await ctx.RespondAsync($"{ctx.User.Mention} :x: Conexão com o lavalink falhou!");
                return;
            }

            if (this.connection?.IsConnected == true)
            {
                await ctx.RespondAsync($"{ctx.User.Mention} :x: Estou no canal de voz: `{this.connection.Channel.Name}`.");
                return;
            }

            if (ctx.Member.VoiceState?.Channel == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention} :x: Você não está no canal de voz!");
                return;
            }

            this.connection = await this.node.ConnectAsync(ctx.Member.VoiceState.Channel);

            if (this.connection?.IsConnected == false)
            {
                await ctx.RespondAsync($"{ctx.User.Mention} :x: Conexão com o canal de voz falhou.");
                return;
            }

            await ctx.RespondAsync($"{ctx.User.Mention} :white_check_mark: Conectado no canal de voz `{this.connection.Channel.Name}`");
        }

        [Command]
        public async Task Play(CommandContext ctx, [RemainingText] Uri url)
        {
            var result = await this.node.Rest.GetTracksAsync(url);

            if (!result.Tracks.Any())
            {
                await ctx.RespondAsync($"{ctx.User.Mention} :x: Nenhuma música foi encontrada.");
                return;
            }

            var track = result.Tracks.First();
            await ctx.RespondAsync($"{ctx.User.Mention} :notes: Tocando agora `{Formatter.Sanitize(track.Title)}`");
            this.connection.Play(track);
        }
    }
}
