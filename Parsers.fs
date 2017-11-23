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

let pwInt = skipString "'I" >>. pint64 .>> spaces |>> I

let pwUint = skipString "'U" >>. puint64 .>> spaces |>> U

let pwFloat = pfloat .>> spaces |>> F
        
let pwBool = 
    (pstring "True" <|> pstring "False") .>> spaces
    |>> function | "True" -> B true | _ -> B false

let pwrapped = pwInt <|> pwUint <|> pwBool <|> pwFloat <|> pwString

let pwrappedMany = many pwrapped

/// Another way using the combinator primitives

let pCommand: Parser<(string * Wrapped list), unit> = 
  spaces >>. skipString "//" >>. tuple2 pword pwrappedMany

// simple test
// let args = "  //echo aString False 11 \"a quoted string\" 'I42 True true 98.4"
// let tupled = run pCommand args
