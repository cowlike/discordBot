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

//32-bit int
let pwInt32 = attempt (pint32 .>> skipString "I32" .>> spaces |>> I32)

let pwInt32' = attempt (pint32 .>> skipChar 'I' .>> spaces |>> I32)

//64-bit int
let pwInt64 = attempt (pint64 .>> skipString "I64" .>> spaces |>> I64)

let pwInt64' = attempt (pint64 .>> skipChar 'L' .>> spaces |>> I64)

//32-bit unsigned int
let pwUInt32 = attempt (puint32 .>> skipString "U32" .>> spaces |>> U32)

let pwUInt32' = attempt (puint32 .>> skipChar 'U' .>> spaces |>> U32)

//64-bit unsigned int
let pwUInt64 = attempt (puint64 .>> skipString "U64" .>> spaces |>> U64)

let pwUInt64' = attempt (puint64 .>> skipString "UL" .>> spaces |>> U64)

//float
let pwFloat = pfloat .>> spaces |>> F

let pwBool = 
    (pstring "True" <|> pstring "False") .>> spaces
    |>> function | "True" -> B true | _ -> B false

let pwrapped = 
    choice [
        pwInt32
        pwInt64
        pwUInt32
        pwUInt64
        pwUInt64'
        pwInt32'
        pwInt64'
        pwUInt32'
        pwBool 
        pwFloat 
        pwString ]

let pwrappedMany = many pwrapped

/// Parse bot command

let pCommand: Parser<(string * Wrapped list), unit> = 
  spaces >>. skipString commandPrefix >>. tuple2 pword pwrappedMany
