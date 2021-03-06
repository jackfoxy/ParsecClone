﻿namespace StringCombinator

open Combinator
open StringCombinator
open System.Text.RegularExpressions
open System

[<AutoOpen>]
module StringP = 

    type ParseState = State<string, string>
    
    let sumChars = List.fold (+) ""

    let foldChars = fun chars -> preturn (sumChars chars)

    let isMatch regex item = Regex.IsMatch(item, regex)

    let private getStringStream (state:ParseState) = (state :?> StringStreamP)

    let private isEof (input:ParseState) target = not (input.hasMore())

    let private invertRegexMatch (input:ParseState) target = (input |> getStringStream).invertRegexMatch input target 1
    
    let private startsWith (input:ParseState) target = (input |> getStringStream).startsWith input target

    let private regexMatch (input:ParseState) target = (input |> getStringStream).regexMatch input target 

    let regexStr pattern = matcher regexMatch pattern

    let matchStr str = matcher startsWith str

    let invertRegex pattern = matcher invertRegexMatch pattern 
        
    let anyBut<'a> = invertRegex

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

    let isDigit = isMatch "[0-9]"

    let isUpper = isMatch "[A-Z]"

    let isLower = isMatch "[a-z]"  

    let any<'a> = regexStr "."

    let isChar = isMatch "[A-z]"

    let isSpace = function 
                    | " "
                    | "\t" -> true
                    | _ -> false

    let ws<'a> = opt (many (satisfy isSpace any))
                    >>= function
                        | Some(i) -> preturn (sumChars i)
                        | None -> preturn ""

    let isNewLine i = isMatch "\r\n" i || isMatch "\r" i || isMatch "\n" i