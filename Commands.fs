module Commands

open System
open CommandHandler
open Types

let private echo client msg (args: string array) =
    String.Join(" ", args)
    |> sendMsg client msg

let private foo client msg args =
    sprintf "arg list = %A" args
    |> sendMsg client msg

let public botCommands () : (string * Handler) list
    = [("echo", echo); ("foo", foo)]
