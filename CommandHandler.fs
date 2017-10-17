module CommandHandler

open Discord.Commands
open Discord.WebSocket
open System.Threading.Tasks

let messageRecieved (client: DiscordSocketClient) (service: CommandService) (sm: SocketMessage) = 
    let message = sm :?> SocketUserMessage
    if isNull message 
    then Task.CompletedTask
    else
        let context = SocketCommandContext(client, message)
        let argPos = 0
        if message.HasCharPrefix('!', ref argPos)
        then
            async {
                let! result = service.ExecuteAsync(context, argPos) |> Async.AwaitTask
                if not result.IsSuccess
                then 
                    context.Channel.SendMessageAsync("test") |> ignore
                    Task.CompletedTask |> ignore
                else 
                    Task.CompletedTask |> ignore
            } |> ignore
            Task.CompletedTask
        else Task.CompletedTask

let initServer (client: DiscordSocketClient) = Task.CompletedTask

let buildHandler (client: DiscordSocketClient) = 
    let service = CommandService()

    client.add_MessageReceived(fun sm -> messageRecieved client service sm)
    client.add_Ready(fun () -> initServer client)

