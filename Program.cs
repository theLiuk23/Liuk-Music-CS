using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Liuk_Music_CS
{
	public static class Program
	{
		public static DiscordSocketClient Client { get; set; }
		public static CommandService CommandsService { get; set; }
		public static IServiceProvider Services { get; set; }

		static void Main(string[] args) => RunBotAsync().GetAwaiter().GetResult();

		public static async Task RunBotAsync()
		{
			var _config = new DiscordSocketConfig
			{
				LogLevel = LogSeverity.Info,
				MessageCacheSize = 1000,
				GatewayIntents = GatewayIntents.All
			};
			Client = new DiscordSocketClient(_config);
			CommandsService = new CommandService();
			Services = new ServiceCollection()
				.AddSingleton(Client)
				.AddSingleton(CommandsService)
				.BuildServiceProvider();

			await InstallCommandsAsync();
			await Client.LoginAsync(Discord.TokenType.Bot, secrets.Default.token);
			await Client.StartAsync();

			Client.Log += Client_Log;
			Client.MessageUpdated += MessageUpdated;
			Client.Ready += () => { return Task.CompletedTask; };

			await Task.Delay(-1);
		}

		private static Task Client_Log(LogMessage arg)
		{
			Console.WriteLine(arg);
			return Task.CompletedTask;
		}

		//Register Commands Async
		public static async Task InstallCommandsAsync()
		{
			// Hook the MessageReceived event into our command handler
			Client.MessageReceived += HandleCommandAsync;

			// Here we discover all of the command modules in the entry 
			// assembly and load them. Starting from Discord.NET 2.0, a
			// service provider is required to be passed into the
			// module registration method to inject the 
			// required dependencies.
			//
			// If you do not use Dependency Injection, pass null.
			// See Dependency Injection guide for more information.
			await CommandsService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), Services);
			Console.WriteLine(string.Join("\n", CommandsService.Commands));
		}

		// Read Input and get Output ( RECEIVE AND DO SOMETHING )
		private static async Task HandleCommandAsync(SocketMessage messageParam)
		{
			// Don't process the command if it was a system message
			var message = messageParam as SocketUserMessage;
			if (message == null)
				return;

			// Create a number to track where the prefix ends and the command begins
			int argPos = 0;

			// Determine if the message is a command based on the prefix and make sure no bots trigger commands
			if (!(message.HasCharPrefix(secrets.Default.prefix, ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos)) || message.Author.IsBot)
				return;

			// Create a WebSocket-based command context based on the message
			var context = new SocketCommandContext(Client, message);

			// Execute the command with the command context we just
			// created, along with the service provider for precondition checks.
			var result = await CommandsService.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: null);

			if (!result.IsSuccess)
				Console.WriteLine(result.ErrorReason);
		}

		private static async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
		{
			// If the message was not in the cache, downloading it will result in getting a copy of `after`.
			var message = await before.GetOrDownloadAsync();
			Console.WriteLine($"{message} -> {after}");
		}
	}
}
