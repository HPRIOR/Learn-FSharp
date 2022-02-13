open System.IO
open System.Runtime.Serialization

/// Random language information:
/// Statically typed with type inference by default
/// Immutability by default
/// Functional first language
/// Source code must be written from top to bottom (no circular dependencies, read like a book)

//--> functions <--//
/// Expression based language so last expression of a function is returned implicitly
/// Functions can be passed arguments without parens
let addCurry a b = a + b // named after Haskell Curry not the food

// Or with parens
let addTuples (a, b) = a + b

/// These have different meanings thanks to currying and partial application. The 'curried' function is 'under the hood'
/// two functions in series; (int -> int -> int); a function which takes an integer which passes this to another function
/// which returns an integer. Internally; the compiler writes this as:
let addA a =
    let addB b = a + b
    addB

/// This enables the partial application of functions
let addFive = addCurry 5 // int -> int -> int becomes 5 -> int -> int
let ten = addFive 5 // -> 10

/// This allows for a functions behaviour to be changed by partially applying them to create new functions
/// It also allows for dependencies to be easily injected
let printSomething print something = print something
let toConsole str = printf $"{str}"
let toFile str = File.WriteAllText("path", str)
let printSomethingToConsole: (string -> unit) = printSomething toConsole
let printSomethingToFile: (string -> unit) = printSomething toFile

printSomethingToConsole "Hello World"
printSomethingToFile "Hello World"

/// If you want to constrain a function so that it must take a sequence of arguments (you don't want partial application)
/// you can use the tupled form: (arg1; arg2)

/// Curried functions also allow for some cool syntax stuff. The outputs of a function can be piped into the input of another
/// allowing you to compose multiple functions together
/// To achieve the same thing in C# you can write many extension methods

let add a b = a + b
let minus a b = b - a
let divideBy a b = b / a

/// The right hand argument is used to form the arguments 'input' from the pipe
let pipeExample =
    5
    |> add 5 //--> 10
    |> minus 1 //--> 9
    |> divideBy 3 //--> 1

/// Functions with compatible types also be composed together to form one function

let double n = n * 2
let half n = n / 2
let doNothing = double >> half

/// Backwards composition allows you to reverse the order
let doNothingInReverse = double << half

//--> collections and LINQ style stuff <--//
let array = [| 1; 2; 3; 4; 5; 6; 7; 8; 9; 10 |]
let list = [ 1; 2; 3; 4; 5; 6; 7; 8; 9; 10 ]

let sequence =
    seq {
        1
        2
        3
        4
        5
        6
        7
        8
        9
        10
    }

/// collections can can be created with handy syntax sugars and list comprehensions
let tenArray = [| 1 .. 10 |] //--> 1, 2, 3...10

let tenSquares =
    [| for n in 1 .. 10 do
           yield n * n |] //--> 1, 4, 9...100

/// LINQ style methods are available on collections (and many more)
/// The are usually applied using pipes

let result =
    array
    |> Array.map (fun x -> x * x) // Select
    |> Array.filter (fun x -> x % 2 = 0) // Where
    |> Array.reduce (fun x y -> x + y) // Aggregate


//--> types <--//

/// You can declare records with the type keyword

type Computer =
    { CPU: string
      Memory: string
      GPU: string
      HD: string }

/// These can be inferred by the type system
let returnMeAComputer =
    { CPU = "i9"
      Memory = "16gb"
      GPU = "1060 gtx"
      HD = "1tb" }

/// Can do OO if you want - most types can have members (even records and DUs like Rust structs and enums)
type Class(constructArg: int) =
    member this.Number = constructArg
    member this.square = this.Number * this.Number

//--> Discriminated Union / Algebraic Data Types / Enums <--//

/// These are like enums in C# but contain arbitrary values/types instead of integers
type Disk =
    | HardDisk of RPM: int
    | SSD
    | Tape of Length: int

/// Disk represents all the possible states that can represent a disk
/// To create a disk:
let hardDisk = HardDisk 10
let ssd = SSD
let tape = Tape 100

/// Each have the type of Disk and can be consumed as such:
let consume (disk: Disk) = printf $"%A{disk}"

consume hardDisk
consume ssd
consume tape

/// To safely use consume a DU you need to match over all possible values that it could be:

let matchOverDisk disk =
    match disk with
    | HardDisk rpm -> $"Hard disk of rpm: {rpm}"
    | SSD -> "An SSD"
    | Tape length -> $"tape with length: {length}"

/// DU's can contain arbitrary values (Types, classes, functions, records...) and can be thought of as
/// flexible interfaces for a set of different values.
/// As opposed to rigid contract defined by ordinary C# interfaces:
/// IHardDrive(DiskDrive, SSD, TapeDrive) --> SomeMethodAcceptingAHardDrive(IHardDrive hardDrive)
/// All Hard drives in this case must conform to the same method signatures


/// They allow for really cool semantics

type Option<'T> =
    | Some of 'T
    | None

/// You can remove null values in favour of Options. A value can be passed around which may or may not contain something
/// It is consumed safely by matching across its possible states and reacting accordingly

let consumeOption option =
    match option with
    | Some thing -> $"This {thing} is not null"
    | None -> "Null"

/// Errors and error handling can become part of the type system rather than a separate thing (throw/catch/try)
type Result<'T, 'U> =
    | Ok of 'T
    | Err of 'U

let consumeResult result =
    match result with
    | Ok result -> $"Do something with result: {result}"
    | Err error -> failwith error // calling code handles the error in an appropriate way (in this case throwing an error)


// 'Single case' DU can be used to help with safety and to enforce 'State pattern'-like behaviour

type Argument = Argument of string
let argument = Argument "I am an argument"

// can't compare raw values --> let comparison = argument = "hello"

//unwrapping argument with pattern matching

let (Argument unwrappedArgument) = argument
let comparison = unwrappedArgument = "compare me"

/// Wrapping values as single case DUs can enforce logic at the type level
type Email = Email of string
type Password = Password of string

type User = { email: string; password: string }

type ValidatedUser = ValidatedUser of User

let validateUser (user: User) =
    match user with
    | { User.email = email
        User.password = password } when email.Length > 0 && password.Length > 0 -> Some(ValidatedUser user)
    | _ -> None

let doSomethingWithValidatedUser (validUser: ValidatedUser) =
    let (ValidatedUser user) = validUser
    printf $"This function will only accept valid users: {user.email} {user.password}"

/// This code will only compile when passed a valid user. If Valid users can only be created by passing a user through
/// a validating method, then you have enforced this logic through the type system


[<EntryPoint>]
let main argv = 0
