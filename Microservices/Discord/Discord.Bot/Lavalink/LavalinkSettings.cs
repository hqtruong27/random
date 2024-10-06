using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bot.Lavalink;

public record LavalinkSettings(string Host, int Port, string Password);