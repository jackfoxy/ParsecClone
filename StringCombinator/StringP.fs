﻿namespace Combinator

open Combinator.Combinator
open System.Text.RegularExpressions
open System

module StringP = 

    type ParseState = State<string>

    let (|RegexStr|_|) (pattern:string) (input:ParseState) =
        if String.IsNullOrEmpty input.state then None
        else
            let m = Regex.Match(input.state, "^" + pattern)
            if m.Success then 
                Some ([ for g in m.Groups -> g.Value ]
                            |> List.filter (String.IsNullOrEmpty >> not)
                            |> List.head) 
            else 
                None
    
    let private startsWith target (input:State<string>) = if input.state.StartsWith target then Some target else None

    let private regexMatch target (input:ParseState) = 
        match input with 
            | RegexStr target result -> Some(result)
            | _ -> None
           
    let matchStr str = matcher startsWith str

    let regexStr pattern = matcher regexMatch pattern
        
    let char<'a> = regexStr "[a-z]"

    let chars<'a> = regexStr "[a-z]+"

    let digit<'a> = regexStr "[0-9]"

    let digits<'a> = regexStr "[0-9]+"
   
    let newline<'a> = regexStr "\r\n" <|> regexStr "\r" <|> regexStr "\n"

    let whitespace<'a> = regexStr "\s"

    let whitespaces<'a> = regexStr "\s+"
    
    let space<'a> = regexStr " "

    let spaces<'a> = regexStr " +"

    let tab<'a> = regexStr "\t"

    let tabs<'a> = regexStr "\t+"