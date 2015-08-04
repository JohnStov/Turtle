module Program

#if INTERACTIVE
#r @"System.Windows.Presentation.dll"
#r @"PresentationCore.dll"
#r @"PresentationFramework.dll"
#r @"..\packages\FParsec.1.0.1\lib\net40-client\FParsecCS.dll"
#r @"..\packages\FParsec.1.0.1\lib\net40-client\FParsec.dll"
#endif

open System
open System.Reflection
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media
open FParsec

let test p str =
    match run p str with
    | Success(result, _, _)   -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

type Arg = float
type Command = 
    | Forward of Arg
    | Turn of Arg
    | Repeat of int * Command list

let pforward = pstring "forward" >>. spaces1 >>. pfloat |>> Forward
let pright = pstring "right" >>. spaces1 >>. pfloat |>> Turn
let pleft = pstring "left" >>. spaces1 >>. pfloat |>> fun n -> Turn (-n)
let prepeat, prepeatimpl = createParserForwardedToRef ()
let pcommand = pforward <|> pright <|> pleft <|> prepeat
let pcommands = many (pcommand .>> spaces)
let block = between (pstring "[") (pstring "]") pcommands
prepeatimpl := pstring "repeat" >>. spaces1 >>. pfloat .>> spaces .>>. block |>> fun (n, commands) -> Repeat(int n, commands)

type Turtle = { x : float; y : float; angle : float}

let rec decode command turtle = 
    match command with 
    | Forward(n) -> [{turtle with x = turtle.x + (n * cos(turtle.angle / 360.0 * 2.0 * Math.PI)); y = turtle.y + (n * sin(turtle.angle / 360.0 * 2.0 * Math.PI));}]
    | Turn(n) -> [{turtle with angle = (turtle.angle + n) % 360.0}]
    | Repeat(n, cmds) ->
        let mutable result = [turtle]
        let mutable i = 0
        while i < n do
            result <- List.append result (treeWalk cmds (List.last result))
            i <- i + 1
        result

and treeWalk commands turtle =
    match commands with
    | [] -> [turtle]
    | h :: t -> 
        let turtles = decode h turtle
        List.append (turtle :: turtles) (treeWalk t (List.last(turtles)))

let getXamlResource xaml = 
  sprintf "/%s;component/%s" (Assembly.GetExecutingAssembly().GetName().Name) xaml
  |> fun nme -> Uri(nme, UriKind.Relative)
  |> Application.LoadComponent
  :?> _

let mainWindow : Window = getXamlResource "MainWindow.xaml" 
let canvas = mainWindow.FindName("canvas") :?> Canvas
let startx = mainWindow.Width/2.0
let starty = mainWindow.Height/2.0

let drawLine x1 y1 x2 y2 =
    let line = new Line()
    line.Stroke <- Brushes.Red
    line.X1 <- x1
    line.X2 <- x2
    line.Y1 <- y1
    line.Y2 <- y2
    canvas.Children.Add(line) |> ignore

let rec drawTurtles turtles =
    match turtles with
    | [] -> ignore
    | h :: [] -> ignore
    | h :: t -> 
        let endTurtle = List.head(t)
        drawLine h.x h.y endTurtle.x endTurtle.y
        drawTurtles t

let result = run pcommands "repeat 10 [right 36 repeat 5 [forward 54 right 72]]"

match result with
    | Success(commands, _, _) -> 
        let turtles = treeWalk commands {x = startx; y = starty; angle = 0.0;}
        drawTurtles turtles |> ignore
        printfn "Success"
    | Failure(error, _, _) -> printfn "Failed"

[<System.STAThread>]
do System.Windows.Application().Run(mainWindow) |> ignore
