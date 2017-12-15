module PluginBundle

open System
open System.Threading.Tasks
open Discord.WebSocket

open Plugins
open Types
open Commands
open Util
open Discord

module HelpPlugin =
    let private help client msg _ = 
        botCommands() 
        |> List.map (fun (k, _) -> k)
        |> List.sort
        |> sprintf "%A" 
        |> sendMsg client msg 
        |> Success

    [<NamedCommandAttribute("help")>]
    type T() = interface ICommand with member t.Execute = withFail help


module ChannelPlugins =
    let private join ch (xs: seq<string>) = String.Join(ch, xs)


    let private showChannels channels =
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

    let private newChannel _ (msg: SocketMessage) = function
        | S channelName :: _ -> 
            try
                let guild = let c = (msg.Channel :?> IGuildChannel) in c.Guild
                guild.CreateTextChannelAsync(channelName) :> Task
                |> Success
            with ex -> Fail ex.Message
        | _ -> Fail <| sprintf "Missing channel name"

    let private rmChannel _ (msg: SocketUserMessage) args = 
        let rm (ch: SocketGuildChannel) = ch.DeleteAsync() |> Success
        try
            let guild = let c = (msg.Channel :?> SocketGuildChannel) in c.Guild
            match args with
            | U64 id :: _ -> 
                guild.GetChannel id |> rm
            | S name :: _ ->
                guild.Channels
                |> Seq.find (fun (ch: SocketGuildChannel) -> ch.Name = name)
                |> rm
            | _ -> Fail "Missing channel id"
        with ex -> Fail ex.Message

    [<NamedCommandAttribute("showGuilds")>]
    type T1() = interface ICommand with member t.Execute = withFail showGuilds

    [<NamedCommandAttribute("showDMChannels")>]
    type T2() = interface ICommand with member t.Execute = withFail showDMChannels

    [<NamedCommandAttribute("newChannel")>]
    type T3() = interface ICommand with member t.Execute = withFail newChannel

    [<NamedCommandAttribute("rmChannel")>]
    type T4() = interface ICommand with member t.Execute = withFail rmChannel

