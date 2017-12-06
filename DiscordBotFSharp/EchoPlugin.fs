module EchoPlugin

open Plugins
open Types
open Commands

let private doSomething client msg = function
    | [S s] -> sendMsg client msg (s + " " + s) |> Success
    | _ -> Fail "echoTwice takes a single string"

[<NamedCommandAttribute("echoTwice")>]
type T() = 
    interface ICommand with 
        member t.Execute = doSomething