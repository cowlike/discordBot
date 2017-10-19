module Entry

open System
open System.Threading.Tasks
open Discord
open Discord.WebSocket
open CommandHandler

let env = 
  let envVars = 
    System.Environment.GetEnvironmentVariables()
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.map (fun d -> string d.Key, string d.Value)
    |> Map.ofSeq

  fun key -> Map.tryFind key envVars

let envDef key defVal = Option.defaultValue defVal (env key)

let asyncClient token = async {
    let client = new DiscordSocketClient()
    do buildHandler(client)
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
    | _ -> 1
