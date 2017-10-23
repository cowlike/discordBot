module CommandHandler

open Discord.Commands
open Discord.WebSocket
open System.Threading.Tasks
open Commands

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

