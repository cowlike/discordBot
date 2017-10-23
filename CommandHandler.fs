module CommandHandler

open Discord.Commands
open Discord.WebSocket
open System.Threading.Tasks
open Commands
open Util

let commandPrefix = "//"

let skipPrefix (s: string) =
    if s.StartsWith(commandPrefix) 
    then s.Substring(String.length commandPrefix) 
    else s

let doCommand (client: DiscordSocketClient) (msg: SocketUserMessage) =
    let msgParts = msg.Content.Split(" ")
    let cmdName = msgParts |> Array.head |> skipPrefix |> lowerCase
    let args = Array.tail msgParts
    (client, msg, args) |||> commands cmdName

let messageRecieved (client: DiscordSocketClient) _ (sm: SocketMessage) = 
    let message = sm :?> SocketUserMessage
    if isNull message || message.Author.IsBot || not (message.Content.StartsWith(commandPrefix)) then
        Task.CompletedTask
    else
        doCommand client message

let initServer (client: DiscordSocketClient) = Task.CompletedTask

let buildHandler (client: DiscordSocketClient) = 
    let service = CommandService()

    client.add_MessageReceived(fun sm -> messageRecieved client service sm)
    client.add_Ready(fun () -> initServer client)

