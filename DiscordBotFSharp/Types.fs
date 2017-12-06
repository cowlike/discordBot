module Types

open Discord.WebSocket
open System.Threading.Tasks

type Result<'a,'b> = Success of 'a | Fail of 'b

type Wrapped = 
  S of string 
  | I32 of int32 
  | I64 of int64 
  | U32 of uint32 
  | U64 of uint64 
  | B of bool 
  | F of float

type CommandName = Name of string
type Command = DiscordSocketClient -> SocketUserMessage -> Wrapped list -> Result<Task,string>
type ICommand =
    abstract member Execute: Command

type Handler = Handler of Command
