module Commands

open System
open System.Threading.Tasks
open Discord.Commands
open Discord.WebSocket
open Types
open Plugins

/// not private so it can be used by plugins
let sendMsg client msg msgOut =
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(msgOut) :> Task

let private echo client msg = function
    | [S s] -> sendMsg client msg s |> Success
    | _ -> Fail "Echo takes a single string"

let private showArgs client msg args =
    sprintf "arg list = %A" args
    |> sendMsg client msg
    |> Success

let private join ch (xs: seq<string>) = String.Join(ch, xs)

let showChannels channels =
    let showChannel (ch: SocketChannel) = 
        sprintf "(%A, %s, %A)" (ch.Id) (ch.ToString()) (ch.GetType())
    "[" + (Seq.map showChannel channels |> join "\n") + "]\n"

let private showGuilds (client: DiscordSocketClient) msg _ = 
    client.Guilds |>
    Seq.map (fun (g: SocketGuild) -> g.Name + ": " + showChannels g.Channels)
    |> join ", "
    |> sendMsg client msg
    |> Success

let private showDMChannels (client: DiscordSocketClient) msg _ = 
    showChannels client.DMChannels
    |> sendMsg client msg
    |> Success

let private newChannel (client: DiscordSocketClient) _ = function
    | S channelName :: _ -> 
        try
            let guild = client.Guilds |> Seq.head
            guild.CreateTextChannelAsync(channelName) :> Task
            |> Success
        with ex -> Fail ex.Message
    | _ -> Fail "Missing channel name"

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
        | _ -> Fail "Missing channel id"
    with ex -> Fail ex.Message

let botCommands = 
    let builtins = [
        ("echo", Handler echo)
        ("showArgs", Handler showArgs)
        ("showGuilds", Handler showGuilds)
        ("showDMChannels", Handler showDMChannels)
        ("newChannel", Handler newChannel)
        ("rmChannel", Handler rmChannel) ]
    let plugins = 
        plugins() 
        |> Map.toList
        |> List.map (fun (Name k, v) -> k, Handler v)

    fun () -> List.append builtins plugins
