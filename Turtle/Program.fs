module Program

open System
open System.Reflection
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media


type Turtle = { x : float; y : float; }

let getXamlResource xaml = 
  sprintf "/%s;component/%s" (Assembly.GetExecutingAssembly().GetName().Name) xaml
  |> fun nme -> Uri(nme, UriKind.Relative)
  |> Application.LoadComponent
  :?> _

let mainWindow : Window = getXamlResource "MainWindow.xaml" 
let canvas = mainWindow.FindName("canvas") :?> Canvas

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
        let startPoint = h
        let endPoint = List.head(t)
        drawLine startPoint.x startPoint.y endPoint.x endPoint.y
        drawTurtles t

let myTurtles = [{ x = 100.0; y = 100.0; }; {x = 200.0; y = 200.0; }; { x = 300.0; y = 100.0; }]
drawTurtles myTurtles |> ignore

[<System.STAThread>]
do System.Windows.Application().Run(mainWindow) |> ignore
