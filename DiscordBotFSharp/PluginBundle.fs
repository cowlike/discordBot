module PluginBundle

open Plugins
open Types
open Commands
open Util

module EchoPlugin =
    let private echo client msg = function
        | [S s] -> sendMsg client msg (s + " " + s) |> Success
        | _ -> Fail "echoTwice takes a single string"

    [<NamedCommandAttribute("echoTwice")>]
    type T() = interface ICommand with member t.Execute = echo

module VersionPlugin =
    let private version client msg _ = 
        version() |> sendMsg client msg |> Success

    [<NamedCommandAttribute("version")>]
    type T() = interface ICommand with member t.Execute = version