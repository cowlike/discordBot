module Commands

open System.Threading.Tasks
open Discord.WebSocket
open Types
open Plugins
open Util

/// not private so it can be used by plugins
let sendMsg _ (msg: SocketUserMessage) msgOut =
    msg.Channel.SendMessageAsync(stampMsg msgOut) :> Task

let withFail f client msg args = 
    try f client msg args 
    with exn -> exn.Message |> Fail

/// private built-in Handler functions
let private version client msg _ = 
    version() |> sendMsg client msg |> Success

let private echo client msg = function
    | [S s] -> sendMsg client msg s |> Success
    | _ -> Fail "Echo takes a single string"

let private showArgs client msg args =
    sprintf "arg list = %A" args
    |> sendMsg client msg
    |> Success

let botCommands = 
    let builtins = [
        ("version", Handler version)
        ("echo", Handler echo)
        ("showArgs", Handler showArgs) ]
    let plugins = 
        plugins() 
        |> Map.toList
        |> List.map (fun (Name k, v) -> k, Handler v)

    fun () -> List.append builtins plugins
