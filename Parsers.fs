module Parsers

open FParsec
open Types

let commandPrefix = "//"

let inline mkStr s = Seq.fold (fun acc v -> acc + string v) "" s

let pquoted = 
    let nonQuote = satisfy ((<>) '"')
    between (pchar '"') (pchar '"') (manyChars nonQuote) .>> spaces

let pword = 
    let nonSpace = satisfy ((<>) ' ')
    many1 nonSpace |>> mkStr .>> spaces

let pargument = pquoted <|> pword

let pwString = pargument |>> S

let pwInt32 = skipString "'I32 " >>. pint32 .>> spaces |>> I32

let pwInt64 = skipString "'I64 " >>. pint64 .>> spaces |>> I64

let pwUint32 = skipString "'U32 " >>. puint32 .>> spaces |>> U32

let pwUint64 = skipString "'U64 " >>. puint64 .>> spaces |>> U64

let pwFloat = pfloat .>> spaces |>> F
        
let pwBool = 
    (pstring "True" <|> pstring "False") .>> spaces
    |>> function | "True" -> B true | _ -> B false

let pwrapped = 
    pwInt32
    <|> pwInt64
    <|> pwUint32
    <|> pwUint64 
    <|> pwBool 
    <|> pwFloat 
    <|> pwString

let pwrappedMany = many pwrapped

/// Parse bot command

let pCommand: Parser<(string * Wrapped list), unit> = 
  spaces >>. skipString commandPrefix >>. tuple2 pword pwrappedMany
