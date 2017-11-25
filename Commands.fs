module Commands

open System
open System.Threading.Tasks
open Discord.Commands
open Discord.WebSocket
open Types

let private sendMsg client msg msgOut =
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(msgOut) :> Task

let private echo client msg = function
    | [S s] -> sendMsg client msg s |> Success
    | _ -> Failure "Echo takes a single string"

let private showArgs client msg args =
    sprintf "arg list = %A" args
    |> sendMsg client msg
    |> Success

let private showGuilds (client: DiscordSocketClient) msg _ = 
    let showChannel (ch: SocketGuildChannel) = 
        sprintf "(%i, %s, %A)" ch.Id ch.Name <| ch.GetType()

    let join ch (xs: seq<string>) = String.Join(ch, xs)
    client.Guilds |>
    Seq.map (fun g -> g.Name + ": [" + (Seq.map showChannel g.Channels |> join "\n") + "]")
    |> join ", "
    |> sendMsg client msg
    |> Success

let private newChannel (client: DiscordSocketClient) _ = function
    | S channelName :: _ -> 
        try
            let guild = client.Guilds |> Seq.head
            guild.CreateTextChannelAsync(channelName) :> Task
            |> Success
        with ex -> Failure ex.Message
    | _ -> Failure "Missing channel name"

let private rmChannel (client: DiscordSocketClient) (msg: SocketUserMessage) args = 
    let rm (ch: SocketGuildChannel) = ch.DeleteAsync() |> Success
    try
        let guild = client.Guilds |> Seq.head
        match args with
        | U64 id :: _ -> 
            guild.GetChannel id |> rm
        | S name :: _ ->
            guild.Channels
            |> Seq.find (fun (ch: SocketGuildChannel) -> ch.Name = name)
            |> rm
        | _ -> Failure "Missing channel id"
    with ex -> Failure ex.Message

let botCommands () = [
    ("echo", Handler echo)
    ("showArgs", Handler showArgs)
    ("showGuilds", Handler showGuilds)
    ("newChannel", Handler newChannel)
    ("rmChannel", Handler rmChannel) ]
