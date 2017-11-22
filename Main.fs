module Main

open CommandHandler
open Commands
open Types
open Util

[<EntryPoint>]
let main _ =
    match env "BOT_TOKEN" with
    | Some token ->
        match botCommands() |> runBot token with
        | Success _ -> 0
        | Failure msg -> printfn "%s" msg; 1
    | _ -> 
        printfn "Set environment variable BOT_TOKEN with the bot token"; 1
