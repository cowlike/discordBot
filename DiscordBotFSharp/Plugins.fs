module Plugins

open System
open System.Collections.Generic
open System.Reflection
open System.Composition
open System.Composition.Hosting
open System.Composition.Convention
open Types

[<MetadataAttribute>]
[<AttributeUsageAttribute(AttributeTargets.All)>]
type NamedCommandAttribute (commandName: string) =
    inherit ExportAttribute(typeof<ICommand>)

    member this.CommandName = commandName

type private NamedCommandMetadata(data: IDictionary<string, obj>) =
    member this.CommandName = data.Item("CommandName").ToString()

let private commandParser (item: ExportFactory<ICommand, NamedCommandMetadata>) =
    let commandFunc = item.CreateExport().Value.Execute
    let commandName = Name item.Metadata.CommandName
    (commandName, commandFunc)

let plugins : unit -> Map<CommandName, Command> =
    let commandMap = 
        let conventions = ConventionBuilder()
        conventions
            .ForTypesDerivedFrom<ICommand>()
            .Export<ICommand>()
            .Shared() |> ignore

        let assemblies = [Assembly.GetExecutingAssembly()]

        use container = ContainerConfiguration()
                            .WithAssemblies(assemblies, conventions)
                            .CreateContainer()
        
        container.GetExports() 
        |> Seq.map commandParser
        |> Map.ofSeq
    fun () -> commandMap

let getPlugin name =
    plugins() |> Map.tryFind (Name name)
