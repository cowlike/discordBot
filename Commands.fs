module Commands

open Discord.Commands
open Discord.WebSocket
open System
open System.Threading.Tasks

type Handler = DiscordSocketClient -> SocketUserMessage -> string array -> Task

let echo client msg (args: string array)  =
    let context = SocketCommandContext(client, msg)
    let msgOut = String.Join(" ", args)
    context.Channel.SendMessageAsync(msgOut) :> Task

let foo client msg args = 
    let context = SocketCommandContext(client, msg)
    let msgOut = sprintf "arg list = %A" args
    context.Channel.SendMessageAsync(msgOut) :> Task

let unknown client msg _ = 
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync("Unknown command") :> Task

let commands: string -> Handler = 
    let cmds =
        [ ("echo", echo)
          ("foo", foo)
        ] |> Map.ofList
    fun cmdName -> Option.defaultValue unknown <| Map.tryFind cmdName cmds
