module CommandHandler

open Discord.Commands
open Discord.WebSocket
open System.Threading.Tasks
open Util
open Types

let private commandPrefix = "//"

let private skipPrefix (s: string) =
    if s.StartsWith(commandPrefix) 
    then s.Substring(String.length commandPrefix) 
    else s

let private doCommand (commands: string -> Handler) (client: DiscordSocketClient) (msg: SocketUserMessage) =
    let msgParts = msg.Content.Split(" ")
    let cmdName = msgParts |> Array.head |> skipPrefix |> lowerCase
    let args = Array.tail msgParts
    (client, msg, args) |||> commands cmdName

let private messageReceived commands (client: DiscordSocketClient) _ (sm: SocketMessage) = 
    let message = sm :?> SocketUserMessage
    if isNull message || message.Author.IsBot || not (message.Content.StartsWith(commandPrefix)) then
        Task.CompletedTask
    else
        doCommand commands client message

let private initServer (_: DiscordSocketClient) = Task.CompletedTask

let public buildHandler (client: DiscordSocketClient) commands = 
    let service = CommandService()

    client.add_MessageReceived(fun sm -> messageReceived commands client service sm)
    client.add_Ready(fun () -> initServer client)

let public sendMsg client msg msgOut =
    let context = SocketCommandContext(client, msg)
    context.Channel.SendMessageAsync(msgOut) :> Task

let public unknown client msg _ = sendMsg client msg "Unknown command"

let private commands commandList = 
    let cmds = commandList |> Map.ofList
    fun cmdName -> Option.defaultValue unknown <| Map.tryFind cmdName cmds

let public runBot token botCommands = 
    let client = new DiscordSocketClient()
    buildHandler client (commands <| botCommands) |> ignore
    
    async {
        do! client.LoginAsync(Discord.TokenType.Bot, token) |> Async.AwaitTask
        do! client.StartAsync() |> Async.AwaitTask
        do  printfn "bot running..."
        return! Task.Delay -1 |> Async.AwaitTask 
    } |> Async.RunSynchronously


