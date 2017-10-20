module CommandHandler

open Discord.Commands
open Discord.WebSocket
open System.Threading.Tasks

let messageRecieved (client: DiscordSocketClient) (service: CommandService) (sm: SocketMessage) = 
    let message = sm :?> SocketUserMessage
    if isNull message || message.Author.IsBot || not (message.Content.StartsWith("//")) then
        Task.CompletedTask
    else
        let context = SocketCommandContext(client, message)
        context.Channel.SendMessageAsync(message.Content) :> Task

let initServer (client: DiscordSocketClient) = Task.CompletedTask

let buildHandler (client: DiscordSocketClient) = 
    let service = CommandService()

    client.add_MessageReceived(fun sm -> messageRecieved client service sm)
    client.add_Ready(fun () -> initServer client)

