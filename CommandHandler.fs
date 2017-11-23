module CommandHandler

open System.Threading.Tasks
open FParsec
open Discord.Commands
open Discord.WebSocket
open Types

let private commandPrefix = "//"

let errorMsg error client msg = 
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(error) :> Task

let inline mkStr s = Seq.fold (fun acc v -> acc + string v) "" s

let quoted = 
    let nonQuote = satisfy ((<>) '"')
    between (pchar '"') (pchar '"') (manyChars nonQuote)

let word = 
    let nonSpace = satisfy ((<>) ' ')
    spaces >>. many1 nonSpace .>> spaces |>> mkStr

let argument = quoted <|> word

let msgParser = skipString commandPrefix >>. tuple2 word (many argument)

let private doCommand commands client (msg: SocketUserMessage) =
    match run msgParser msg.Content with
    | ParserResult.Success ((cmdName,args), _, _) ->
        let (Handler handler) = commands cmdName  
        (client, msg, args) 
        |||> handler
        |> function
            | Success task -> task
            | Failure err -> errorMsg err client msg
    | ParserResult.Failure (error, _, _) -> errorMsg error client msg

let private messageReceived commands client _ (sm: SocketMessage) = 
    let message = sm :?> SocketUserMessage
    if isNull message 
        || message.Author.IsBot 
        || not (message.Content.StartsWith(commandPrefix)) then
        Task.CompletedTask
    else
        doCommand commands client message

let private mkCommandRetriever commandList = 
    let unknown msg = (fun _ _ _ -> Failure msg) |> Handler
    let cmds = commandList |> Map.ofList
    fun cmdName -> Option.defaultValue (unknown "Unknown command") <| Map.tryFind cmdName cmds

let private logTask msg = 
    fun () -> printfn "%s" msg
    |> Task.Factory.StartNew

let private buildHandler (client: DiscordSocketClient) msgReceiver = 
    let service = CommandService()
    client.add_MessageReceived(fun sm -> msgReceiver client service sm)
    client.add_LoggedIn(fun() -> logTask "bot logged in...")
    client.add_Ready(fun() -> logTask "bot is ready")
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
    with ex -> Failure ex.Message
