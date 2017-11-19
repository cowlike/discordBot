module Commands

open System
open System.Threading.Tasks
open Discord.Commands
open Discord.WebSocket

type Handler = DiscordSocketClient -> SocketUserMessage -> string list -> Task

let private sendMsg client msg msgOut =
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(msgOut) :> Task

let private echo : Handler = fun client msg args ->
    String.Join(" ", args)
    |> sendMsg client msg

let private foo : Handler = fun client msg args ->
    sprintf "arg list = %A" args
    |> sendMsg client msg

let private ch : Handler = fun _ _ _ -> 
    Task.CompletedTask

let public botCommands () : (string * Handler) list = [
    ("echo", echo)
    ("foo", foo)
    ("ch", ch)
    ]
