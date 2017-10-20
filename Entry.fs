module Entry

open System
open System.Threading.Tasks
open Discord
open Discord.WebSocket
open CommandHandler
open Util

let asyncClient token = 
    let client = new DiscordSocketClient()
    buildHandler(client) |> ignore
    
    async {
        do! client.LoginAsync(TokenType.Bot, token) |> Async.AwaitTask
        do! client.StartAsync() |> Async.AwaitTask
        return! Task.Delay -1 |> Async.AwaitTask 
    }

[<EntryPoint>]
let main argv =
    match env "BOT_TOKEN" with
    | Some token ->
        asyncClient token |> Async.RunSynchronously
        0
    | _ -> 
        printfn "Set environment variable BOT_TOKEN with the bot token"
        1
