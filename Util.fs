module Util

///Some helper functions

let lowerCase (s: string) = s.ToLower()

let env = 
  let envVars = 
    System.Environment.GetEnvironmentVariables()
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.map (fun d -> string d.Key, string d.Value)
    |> Map.ofSeq

  fun key -> Map.tryFind key envVars

let envDef key defVal = Option.defaultValue defVal (env key)
