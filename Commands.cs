using System;
using Discord.Commands;
using System.Threading.Tasks;

namespace Liuk_Music_CS
{
	public class Commands : ModuleBase<SocketCommandContext>
	{
		Program program = new Program();

		[Command("ping")]
		public async Task Ping()
		{
			await ReplyAsync($"Latency: {program.Client.Latency} ms.");
		}
	}
}