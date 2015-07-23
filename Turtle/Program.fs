module Program

open System
open System.Reflection
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media

let getXamlResource xaml = 
  sprintf "/%s;component/%s" (Assembly.GetExecutingAssembly().GetName().Name) xaml
  |> fun nme -> Uri(nme, UriKind.Relative)
  |> Application.LoadComponent
  :?> _

let mainWindow : Window = getXamlResource "MainWindow.xaml" 
let canvas = mainWindow.FindName("canvas") :?> Canvas

let drawLine X1 Y1 X2 Y2 =
    let line = new Line()
    line.Stroke <- Brushes.Red
    line.X1 <- X1
    line.X2 <- X2
    line.Y1 <- Y1
    line.Y2 <- Y2
    canvas.Children.Add(line) |> ignore

drawLine 100.0 100.0 200.0 200.0

[<System.STAThread>]
do System.Windows.Application().Run(mainWindow) |> ignore
