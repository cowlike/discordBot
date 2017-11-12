module Commands

open Discord.Commands
open Discord.WebSocket
open System
open System.Threading.Tasks

type Handler = DiscordSocketClient -> SocketUserMessage -> string array -> Task

let sendMsg client msg msgOut =
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(msgOut) :> Task

let echo client msg (args: string array)  =
    String.Join(" ", args)
    |> sendMsg client msg

let foo client msg args =
    sprintf "arg list = %A" args
    |> sendMsg client msg

let unknown client msg _ = 
    "Unknown command"
    |> sendMsg client msg

let commands: string -> Handler = 
    let cmds =
        [ ("echo", echo)
          ("foo", foo)
        ] |> Map.ofList
    fun cmdName -> Option.defaultValue unknown <| Map.tryFind cmdName cmds
