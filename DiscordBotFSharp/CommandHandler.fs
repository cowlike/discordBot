module CommandHandler

open System.Threading.Tasks
open FParsec
open Discord.Commands
open Discord.WebSocket
open Types
open Parsers
open Util
open Discord

let errorMsg error client msg = 
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(stampMsg error) :> Task

let inline mkStr s = Seq.fold (fun acc v -> acc + string v) "" s

let private hasPrefix (msg: SocketUserMessage) = msg.Content.StartsWith(commandPrefix)

let private doCommand commands client (msg: SocketUserMessage) =
    let content = 
        if hasPrefix msg 
        then msg.Content.Substring(String.length commandPrefix) 
        else msg.Content
    match run pCommand content with
    | ParserResult.Success ((cmdName,args), _, _) ->
        let (Handler handler) = commands cmdName  
        (client, msg, args) 
        |||> handler
        |> function
            | Success task -> task
            | Fail err -> errorMsg err client msg
    | ParserResult.Failure (error, _, _) -> errorMsg error client msg

let private messageReceived commands client _ (sm: SocketMessage) = 
    let message = sm :?> SocketUserMessage
    if (not << isNull) message
        && not message.Author.IsBot
        && (message.Channel.Name.StartsWith("@") || hasPrefix message)
    then doCommand commands client message
    else Task.CompletedTask

let private mkCommandRetriever commandList = 
    let unknown msg = (fun _ _ _ -> Fail msg) |> Handler
    let cmds = commandList |> Map.ofList
    fun cmdName -> Option.defaultValue (unknown "Unknown command") <| Map.tryFind cmdName cmds

let private logTask msg = 
    fun () -> printfn "%s" <| stampMsg msg
    |> Task.Factory.StartNew

// let private setOnline (client: DiscordSocketClient) =
//     client.SetStatusAsync(UserStatus.Online) 
//     |> Async.AwaitTask 
//     |> Async.RunSynchronously

let private buildHandler (client: DiscordSocketClient) msgReceiver = 
    let service = CommandService()
    client.add_MessageReceived(fun sm -> msgReceiver client service sm)

    client.add_Connected(fun() -> version() |> sprintf "bot connected (%s)" |> logTask)
    client.add_Disconnected(fun e -> logTask <| "bot disconnected: " + e.Message)
    client.add_LoggedIn(fun() -> logTask "bot logged in")
    client.add_LoggedOut(fun() -> logTask "bot logged out")
    client.add_Ready(fun() -> logTask "bot is ready")
    client.add_LatencyUpdated(fun low hi -> logTask <| sprintf "latency (%d,%d); connect state (%A)" low hi client.ConnectionState)
    client.add_ChannelCreated(fun ch -> logTask <| sprintf "channel %s created" (ch.ToString()))
    client.add_ChannelDestroyed(fun ch -> logTask <| sprintf "channel %s destroyed" (ch.ToString()))

/// Public API

let public runBot token botCommands = 
    try
        let msgReceiver = botCommands |> mkCommandRetriever |> messageReceived
        let client = new DiscordSocketClient()
        buildHandler client msgReceiver
        
        async {
            do! client.LoginAsync(Discord.TokenType.Bot, token) |> Async.AwaitTask
            do! client.StartAsync() |> Async.AwaitTask
            return! Task.Delay -1 |> Async.AwaitTask 
        } |> Async.RunSynchronously

        Success 0
    with ex -> Fail ex.Message
