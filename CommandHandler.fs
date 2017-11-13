module CommandHandler

open System.Threading.Tasks
open FParsec
open Discord.Commands
open Discord.WebSocket

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
    | Success ((cmdName,args), _, _) -> 
        (client, msg, args) |||> commands cmdName
    | Failure (error, _, _) -> errorMsg error client msg

let private messageReceived commands client _ (sm: SocketMessage) = 
    let message = sm :?> SocketUserMessage
    if isNull message 
        || message.Author.IsBot 
        || not (message.Content.StartsWith(commandPrefix)) then
        Task.CompletedTask
    else
        doCommand commands client message

let private mkCommandRetriever commandList = 
    let unknown msg = fun c m _ -> errorMsg msg c m
    let cmds = commandList |> Map.ofList
    fun cmdName -> Option.defaultValue (unknown "Unknown command") <| Map.tryFind cmdName cmds

let private initServer (_: DiscordSocketClient) = Task.CompletedTask

/// Public API

let public buildHandler (client: DiscordSocketClient) msgReceiver = 
    let service = CommandService()
    client.add_MessageReceived(fun sm -> msgReceiver client service sm)
    client.add_Ready(fun () -> initServer client)

let public runBot token botCommands = 
    let msgReceiver = botCommands |> mkCommandRetriever |> messageReceived
    let client = new DiscordSocketClient()
    buildHandler client msgReceiver
    
    async {
        do! client.LoginAsync(Discord.TokenType.Bot, token) |> Async.AwaitTask
        do! client.StartAsync() |> Async.AwaitTask
        do  printfn "bot running..."
        return! Task.Delay -1 |> Async.AwaitTask 
    } |> Async.RunSynchronously
