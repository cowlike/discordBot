module Commands

open System
open System.Threading.Tasks
open Discord.Commands

let private sendMsg client msg msgOut =
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(msgOut) :> Task

let private echo client msg (args: string seq) =
    String.Join(" ", args)
    |> sendMsg client msg

let private foo client msg args =
    sprintf "arg list = %A" args
    |> sendMsg client msg

let public botCommands () = [
    ("echo", echo)
    ("foo", foo)
    ]
