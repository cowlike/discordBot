module Util

///Some helper functions

type Success<'a,'b> = Success of 'a | Failure of 'b

let lowerCase (s: string) = s.ToLower()

let env = 
  let envVars = 
    System.Environment.GetEnvironmentVariables()
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.map (fun d -> string d.Key, string d.Value)
    |> Map.ofSeq

  fun key -> Map.tryFind key envVars

let envDef key defVal = Option.defaultValue defVal (env key)
