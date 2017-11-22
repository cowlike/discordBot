module Types

open Discord.WebSocket
open System.Threading.Tasks

type Result<'a,'b> = Success of 'a | Failure of 'b

type Handler = Handler of (DiscordSocketClient -> SocketUserMessage -> string list -> Result<Task,string>)
