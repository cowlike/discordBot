module Types

open Discord.WebSocket
open System.Threading.Tasks

type Handler = DiscordSocketClient -> SocketUserMessage -> string list -> Task

type Result<'a,'b> = Success of 'a | Failure of 'b

