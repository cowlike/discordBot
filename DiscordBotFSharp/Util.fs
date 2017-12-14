module Util

///Some helper functions

let version () = "v1.4.5-b3"

let lowerCase (s: string) = s.ToLower()

let env = 
  let envVars = 
    System.Environment.GetEnvironmentVariables()
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.map (fun d -> string d.Key, string d.Value)
    |> Map.ofSeq

  fun key -> Map.tryFind key envVars

let envDef key defVal = Option.defaultValue defVal (env key)

/// a "normal" Applicative operator would be applying a wrapped function
/// to a wrapped value: 
///     ('a -> 'b) option -> 'a option -> 'b option
/// but here we are applying a wrapped function to a simple value:
///     ('a -> 'b) option -> 'a -> 'b option

let (<**>) mf x = 
    match mf with 
    | Some f -> Option.map f (Some x) 
    | _      -> None

let now (fmt: string) = System.DateTime.Now.ToString(fmt)

let stampMsg msg = now "MM/dd HH:mm:ss:FFF" + ": " + msg