module FSharpPDev.HigherPlaneOfExistence

// Functors and Monads

//--> Map <--//

(*
What?
Takes a function in the normal world and transforms it into a corresponding function in the elevated world.
In other words, it will apply a function to a elevated type as if it were a normal type and return an elevated result.

Must:
Create a new function which is identical to the non-lifted one.
A composed function, once lifted, should be the same as a composed function, 'unlifted'
Sig: E<(a -> b)> -> E<a> -> E<b>
*)

// Example impl 
let mapOption f opt =
    match opt with
    | None -> None
    | Some x ->
        Some (f x) // apply function to inner value

// Using Map

let add1 x = x + 1

// Partial application of Option which returns new function taking and option as an argument and returning a function
let add1IfSomething = Option.map add1
let result = add1IfSomething (Some 1)

// Or...
let terseOptionMap = Some 1 |>  Option.map add1

// --> Return <--//

(*
Doesn't need much of an explanation: lifts a value 
Sig: a -> E<a>
*)

// Example impl
let returnOption x = Some x

// --> Apply <--//

(*
What?
Unpacks a function wrapped inside a elevated value into a lifted function.
Can be used to apply lifted values to a lifted function - or to use lifted values as arguments for a lifted function
Sig: E<(a -> b)> -> E<a> -> E<b>
Thanks to partial application and currying, apply can be applied in succession to lift a function with an
arbitrary number of parameters.
Sig: E<(a -> b -> c)> -> E<a> -> E<b> -> E<c>
*)

// Example impl

let apply fOpt xOpt =
    match fOpt, xOpt with
    | Some f, Some x -> Some (f x) // Apply first argument to second 
    | _ -> None
    
// Using Apply

// Long curried version 
let add x y = x + y
let someAdd = Some add
let someAddPartialOne = apply someAdd (Some 1)
let someAddPartialTwo = apply someAddPartialOne (Some 1) // Some 2

// Infix version
let (<*>) = apply
let infixResult = (Some add) <*> Some 1 <*> Some 1 // Some 2



    
    










