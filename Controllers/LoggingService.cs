using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Interactions;

namespace DiscordBot.Controllers
{
    public class LoggingService
    {
		public LoggingService(DiscordSocketClient client, InteractionService interactionService)
		{
			client.Log += LogAsync;
			interactionService.Log += LogAsync;
		}

		private Task LogAsync(LogMessage message)
		{
			if (message.Exception is CommandException cmdException)
			{
				Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
					+ $" failed to execute in {cmdException.Context.Channel}.");
				Console.WriteLine(cmdException);
			}
			else
				Console.WriteLine($"[General/{message.Severity}] {message}");

			return Task.CompletedTask;
		}
	}
}
