module FSharpPDev.HigherPlaneOfExistence

// Functors and Monads

// Map

(*
Takes a function in the normal world and transforms it into a corresponding function in the elevated world.
In other words, it will apply a function to a elevated type as if it were a normal type and return an elevated result.
Sig: E<(a -> b)> -> E<a> -> E<b>
*)

// Example impl for Option
let mapOption f opt =
    match opt with
    | None -> None
    | Some x ->
        Some (f x) // apply function to inner value

// using a map

let add1 x = x + 1

// Partial application of Option which returns new function taking and option as an argument and returning a function
let add1IfSomething = Option.map add1
let result = add1IfSomething (Some 1)

// Or...
let terseOptionMap = Some 1 |>  Option.map add1





