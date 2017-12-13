module PluginBundle

open Plugins
open Types
open Commands
open Util
open System.Threading
open System.Threading.Tasks

let withFail f client msg args = 
    try f client msg args 
    with exn -> exn.Message |> Fail

module EchoPlugin =
    let private echo client msg = function
        | [S s] -> sendMsg client msg (s + " " + s) |> Success
        | _ -> Fail "echoTwice takes a single string"

    [<NamedCommandAttribute("echoTwice")>]
    type T() = interface ICommand with member t.Execute = withFail echo

module VersionPlugin =
    let private version client msg _ = 
        version() |> sendMsg client msg |> Success

    [<NamedCommandAttribute("version")>]
    type T() = interface ICommand with member t.Execute = withFail version

module HelpPlugin =
    let private help client msg _ = 
        botCommands() 
        |> List.map (fun (k, _) -> k)
        |> List.sort
        |> sprintf "%A" 
        |> sendMsg client msg 
        |> Success

    [<NamedCommandAttribute("help")>]
    type T() = interface ICommand with member t.Execute = withFail help
