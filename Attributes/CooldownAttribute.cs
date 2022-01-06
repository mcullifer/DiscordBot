using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Attributes
{
    class CooldownAttribute : PreconditionAttribute
    {
        public float Time { get; }

        public static Dictionary<(string,ulong), DateTime> LastCommandTime = new();

        public CooldownAttribute(float time)
        {
            Time = time;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if ( !LastCommandTime.ContainsKey((command.Name, context.User.Id)) ) 
            {
                LastCommandTime.Add((command.Name, context.User.Id), DateTime.Now);
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            DateTime lastTime = LastCommandTime[(command.Name, context.User.Id)];
            
            if ( (DateTime.Now - lastTime).Seconds < Time )
            {
                return Task.FromResult(PreconditionResult.FromError("You are using commands too quickly"));
            }
            
            LastCommandTime[(command.Name, context.User.Id)] = DateTime.Now;
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
