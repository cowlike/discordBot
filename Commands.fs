module Commands

open System
open System.Threading.Tasks
open Discord.Commands
open Discord.WebSocket
open Types

let private sendMsg client msg msgOut =
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(msgOut) :> Task

let private echo : Handler = fun client msg args ->
    String.Join(" ", args)
    |> sendMsg client msg

let private foo : Handler = fun client msg args ->
    sprintf "arg list = %A" args
    |> sendMsg client msg

let showChannel (ch: SocketGuildChannel) = 
    sprintf "(%i, %s, %A)" ch.Id ch.Name <| ch.GetType()

let private guilds : Handler = fun client msg _ -> 
    let join ch (xs: seq<string>) = String.Join(ch, xs)
    client.Guilds |>
    Seq.map (fun g -> g.Name + ": [" + (Seq.map showChannel g.Channels |> join "\n") + "]")
    |> join ", "
    |> sendMsg client msg

let private newChannel : Handler = fun client _ args -> 
    let guild = client.Guilds |> Seq.head
    guild.CreateTextChannelAsync(args.[0]) :> Task

let private rmChannel : Handler = fun client _ args -> 
    try
        let guild = client.Guilds |> Seq.head
        let channel = guild.GetChannel (args.[0] |> uint64)
        channel.DeleteAsync ()
    with ex ->
        printfn "failed: %s" ex.Message
        Task.CompletedTask

let botCommands () : (string * Handler) list = [
    ("echo", echo)
    ("foo", foo)
    ("guilds", guilds)
    ("newChannel", newChannel)
    ("rmChannel", rmChannel)
    ]
