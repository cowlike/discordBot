module Types

open Discord.WebSocket
open System.Threading.Tasks

type Handler = DiscordSocketClient -> SocketUserMessage -> string array -> Task

