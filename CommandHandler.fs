module CommandHandler

open Discord.Commands
open Discord.WebSocket
open System.Threading.Tasks

let messageRecieved (client: DiscordSocketClient) (service: CommandService) (sm: SocketMessage) = 
    let message = sm :?> SocketUserMessage
    let context = SocketCommandContext(client, message)
    if message.Author.IsBot || isNull message then Task.CompletedTask
    else context.Channel.SendMessageAsync(message.Content) :> Task

let initServer (client: DiscordSocketClient) = Task.CompletedTask

let buildHandler (client: DiscordSocketClient) = 
    let service = CommandService()

    client.add_MessageReceived(fun sm -> messageRecieved client service sm)
    client.add_Ready(fun () -> initServer client)

