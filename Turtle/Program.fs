module Program

open System
open System.Reflection
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media


type MyLine = { x1 : float; x2 : float; y1 : float; y2 : float
}

let getXamlResource xaml = 
  sprintf "/%s;component/%s" (Assembly.GetExecutingAssembly().GetName().Name) xaml
  |> fun nme -> Uri(nme, UriKind.Relative)
  |> Application.LoadComponent
  :?> _

let mainWindow : Window = getXamlResource "MainWindow.xaml" 
let canvas = mainWindow.FindName("canvas") :?> Canvas

let drawLine myLine =
    let line = new Line()
    line.Stroke <- Brushes.Red
    line.X1 <- myLine.x1
    line.X2 <- myLine.x2
    line.Y1 <- myLine.y1
    line.Y2 <- myLine.y2
    canvas.Children.Add(line) |> ignore

let rec drawLines lines =
    match lines with
    | [] -> ignore
    | h :: t -> 
        drawLine h
        drawLines t

let myLines = [{ x1 = 100.0; y1 = 100.0; x2 = 200.0; y2 = 200.0; }; { x1 = 200.0; y1 = 200.0; x2 = 300.0; y2 = 100.0; }]
drawLines myLines |> ignore

[<System.STAThread>]
do System.Windows.Application().Run(mainWindow) |> ignore
