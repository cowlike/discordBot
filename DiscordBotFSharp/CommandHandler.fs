module CommandHandler

open System.Threading.Tasks
open FParsec
open Discord
open Discord.Commands
open Discord.WebSocket
open Types
open Parsers
open Util

let errorMsg error _ (msg: SocketUserMessage) = 
    msg.Channel.SendMessageAsync(stampMsg error) :> Task

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
    | Failure (error, _, _) -> errorMsg error client msg

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

let private logTask (msg: LogMessage) = 
    let msgOut = sprintf "[%A] %s" msg.Severity <| msg.ToString(null, true, false)
    fun () -> stampMsg msgOut |> printfn "%s"
    |> Task.Factory.StartNew

let private buildHandler (client: DiscordSocketClient) msgReceiver = 
    let service = CommandService()
    client.add_Log(fun msg -> logTask msg)
    client.add_MessageReceived(fun sm -> msgReceiver client service sm)

/// Public API

let public runBot token logLevel botCommands = 
    try
        let msgReceiver = botCommands |> mkCommandRetriever |> messageReceived

        let cfg = DiscordSocketConfig()
        cfg.LogLevel <- logLevel
        let client = new DiscordSocketClient(cfg)

        buildHandler client msgReceiver
        version() |> printfn "Starting server (%s)"
        
        async {
            do! client.LoginAsync(TokenType.Bot, token) |> Async.AwaitTask
            do! client.StartAsync() |> Async.AwaitTask
            return! Task.Delay -1 |> Async.AwaitTask 
        } |> Async.RunSynchronously

        Success 0
    with ex -> Fail ex.Message
