module Entry

open System
open System.Threading.Tasks
open Discord
open Discord.WebSocket
open CommandHandler

let asyncClient =
    let client = new DiscordSocketClient()
    buildHandler(client)
    client.LoginAsync(TokenType.Bot, "MzI0MzQ5NDIyOTI0MzMzMDU2.DMLdcQ.fx8MAUXhrAtrCrYdUt9qe8d6fnE") |> ignore
    client.StartAsync() |> ignore
    Task.Delay -1 |> Async.AwaitTask 


[<EntryPoint>]
let main argv =
    asyncClient |> Async.RunSynchronously
    0
