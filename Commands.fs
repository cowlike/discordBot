module Commands

open System
open System.Threading.Tasks
open Discord.Commands
open Discord.WebSocket
open Types

let private sendMsg client msg msgOut =
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(msgOut) :> Task

let private echo = Handler <| fun client msg args ->
    String.Join(" ", args)
    |> sendMsg client msg
    |> Success

let private showArgs = Handler <| fun client msg args ->
    sprintf "arg list = %A" args
    |> sendMsg client msg
    |> Success

let private guilds = 
    let showChannel (ch: SocketGuildChannel) = 
        sprintf "(%i, %s, %A)" ch.Id ch.Name <| ch.GetType()

    Handler <| fun (client: DiscordSocketClient) msg _ -> 
        let join ch (xs: seq<string>) = String.Join(ch, xs)
        client.Guilds |>
        Seq.map (fun g -> g.Name + ": [" + (Seq.map showChannel g.Channels |> join "\n") + "]")
        |> join ", "
        |> sendMsg client msg
        |> Success

let private newChannel = Handler <| fun (client: DiscordSocketClient) _ args -> 
    try
        let guild = client.Guilds |> Seq.head
        guild.CreateTextChannelAsync(args.[0]) :> Task
        |> Success
    with ex -> Failure ex.Message

let private rmChannel = Handler <| fun (client: DiscordSocketClient) _ args ->
    try
        let guild = client.Guilds |> Seq.head
        let channel = guild.GetChannel (args.[0] |> uint64)
        channel.DeleteAsync ()
        |> Success
    with ex -> Failure ex.Message

let botCommands () = [
    ("echo", echo)
    ("showArgs", showArgs)
    ("guilds", guilds)
    ("newChannel", newChannel)
    ("rmChannel", rmChannel) ]
