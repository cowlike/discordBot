module Main

open CommandHandler
open Commands
open Util

let main _ =
    match env "BOT_TOKEN" with
    | Some token ->
        runBot token (botCommands())
        0
    | _ -> 
        printfn "Set environment variable BOT_TOKEN with the bot token"
        1
