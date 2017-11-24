module Types

open Discord.WebSocket
open System.Threading.Tasks

type Result<'a,'b> = Success of 'a | Failure of 'b

type Wrapped = S of string | I of int64 | I32 of int32 | U of uint64 | B of bool | F of float

type Handler = Handler of (DiscordSocketClient -> SocketUserMessage -> Wrapped list -> Result<Task,string>)
