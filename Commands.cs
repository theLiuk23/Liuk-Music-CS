using System;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Liuk_Music_CS
{
	public class Commands : ModuleBase<SocketCommandContext>
	{
		[Command("ping")]
		[Summary("It makes sure the bot is running properly.")]
		public async Task Ping()
		{
			await ReplyAsync($"Latency: {Program.Client.Latency} ms.");
		}


		[Command("help")]
		[Summary("It gives a simple explanation about each command.")]
		public async Task Help()
		{
			var embed = new EmbedBuilder()
			{
				Title = "HELP",
				// Description = "It gives a simple explanation about each command.",
				Timestamp = DateTime.Now
			};
			foreach (var command in Program.CommandsService.Commands)
			{
				embed.AddField($"__{command.Name}__", command.Summary, true);
			}

			await ReplyAsync(embed: embed.Build());
		}


		[Command("play")]
		[Summary("It plays the specified song in the user's voice channel.")]
		public async Task Play()
		{

		}
	}
}