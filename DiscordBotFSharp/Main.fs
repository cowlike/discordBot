module Main

open CommandHandler
open Commands
open Types
open Util
open Discord

let pLogLevel = function
    | "CRITICAL" -> LogSeverity.Critical
    | "ERROR"    -> LogSeverity.Error
    | "WARN"     -> LogSeverity.Warning
    | "VERBOSE"  -> LogSeverity.Verbose
    | "DEBUG"    -> LogSeverity.Debug
    | _          -> LogSeverity.Info

[<EntryPoint>]
let main _ =
    let logLevel = envDef "LOG_LEVEL" "INFO" |> pLogLevel
    match env "BOT_TOKEN" with
    | Some token ->
        match botCommands() |> runBot token logLevel with
        | Success _ -> 0
        | Fail msg -> printfn "%s" msg; 1
    | _ -> 
        printfn "Set environment variable BOT_TOKEN with the bot token"; 1
