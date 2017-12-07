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

///==========================
let private version client msg = function
    | [S s] -> sendMsg client msg (s + " " + s) |> Success
    | _ -> Fail "V1.0.0"

[<NamedCommandAttribute("version")>]
type U() = 
    interface ICommand with 
        member t.Execute = version