﻿module FooStringUnitTests 

open System
open NUnit.Framework
open FsUnit

open Combinator
open StringCombinator
open StringMatchers.FooSample


[<Test>]
let preturnTest () = 
    let target = new StringStreamP("foofighters")

    let band = test target band
        
    match band with
        | FooFighter -> Assert.IsTrue true
        | _ -> Assert.IsFalse true

[<Test>]
let manyTest () = 
        
    let manyFooStr = test (new StringStreamP("foofoofoofoofob")) manyFoo

    Assert.IsTrue (List.length manyFooStr = 4)

[<Test>]
let fooString () = 
    let target = new StringStreamP("foofighters")
        
    let fString = test target fooString

    fString = "foo" |> Assert.IsTrue 

[<Test>]
let fightString () = 
    let target = new StringStreamP("foofighters")

    let fightString = test target fighterString
        
    fightString = "fighter" |> Assert.IsTrue 

[<Test>]
let testTuples () = 
    let target = new StringStreamP("foofighters")

    let (foo, fighters) = test target fighterTuples

    foo = "foo" |> Assert.IsTrue
    fighters = "fighter" |> Assert.IsTrue

        
[<Test>]
let options () = 
    let target = new StringStreamP("foofighters")
        
    test target opts = "foo" |> Assert.IsTrue

    test target optsC = "foo" |> Assert.IsTrue

[<Test>]
let manyOptions () = 
    let target = new StringStreamP("foofighters") :> IStreamP<string, string>
        
    test target (many opts) = ["foo";"fighter"] |> Assert.IsTrue
    test target (many optsC) = ["foo";"fighter"] |> Assert.IsTrue

[<Test>]
let regex () = 
    let target = new StringStreamP("foofighters")
        
    test target fRegex = "foof" |> Assert.IsTrue

[<Test>]
let regexes () = 
    let target = new StringStreamP("      foofighters           foofighters")
        
    let result = test target fooFightersWithSpaces
        
    result |> List.length = 4 |> Assert.IsTrue

[<Test>]
let anyOfChars () = 
    let target = new StringStreamP("      foofighters           foofighters") :> IStreamP<string, string>
        
    let result = test target allFooCharacters |> List.fold (+) ""
        
    result = target.state |> Assert.IsTrue

[<Test>]
let newLine () = 
    let fullNewline = new StringStreamP("\r\n")  :> IStreamP<string, string>
    let carriageReturn = new StringStreamP("\r") :> IStreamP<string, string>
    let newLine = new StringStreamP("\n")  :> IStreamP<string, string>
    let nl = @"
"
    let newLine2 = new StringStreamP(nl) :> IStreamP<string, string>

    test fullNewline newline = fullNewline.state |> Assert.IsTrue
    test carriageReturn newline = carriageReturn.state |> Assert.IsTrue
    test newLine newline = newLine.state |> Assert.IsTrue
    test newLine2 newline = newLine2.state |> Assert.IsTrue

[<Test>]
let attempt () = 
    let target = new StringStreamP("foofighters")
        
    match test target parseWithErrorAttempt with
        | FooFighter -> Assert.IsTrue true

[<Test>]
let manyTillTest () =
    let target = new StringStreamP("abc abc def abc")

    let abc = matchStr "abc" .>> ws

    let def = matchStr "def"

    let line = (manyTill abc def .>> ws) .>>. abc .>> eof

    let result = test target line

    result |> should equal (["abc";"abc"],"abc")

[<Test>]
[<ExpectedException>]
let manyTillOneOrMore () =
    let target = new StringStreamP("x abc def abc")

    let abc = matchStr "abc" .>> ws

    let def = matchStr "def"

    let line = (manyTill1 abc def .>> ws) .>>. abc .>> eof

    let result = test target line

    result |> should equal (["abc";"abc"],"abc")

[<Test>]
let lookaheadTest () =
    let target = new StringStreamP("abc abc def abc") :> IStreamP<string, string>
    
    let abc = lookahead (matchStr "abc" .>> ws) >>= fun r -> 
        if r = "abc" then preturn "found"
        else preturn "not found"

    match abc target with 
        | Some(m), state -> 
            m |> should equal "found"
            state.state |> should equal target.state
        | None, state -> 
            false |> should equal true

[<Test>]
[<ExpectedException>]
let many1TestFail () = 
    let target = new StringStreamP("abc abc def abc") :> IStreamP<string, string>
    
    let foo = matchStr "foo"

    let manyFoo = many1 foo

    test target manyFoo |> should equal false

[<Test>]
let many1Test () = 
    let target = new StringStreamP("abc abc def abc") :> IStreamP<string, string>
    
    let abc = ws >>. matchStr "abc"

    let manyAbc = many1 abc

    test target manyAbc |> should equal ["abc";"abc"]