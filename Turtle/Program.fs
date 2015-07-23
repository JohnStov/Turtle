module Program

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media

let assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name

let getXamlResource xaml = 
  sprintf "/%s;component/%s" assemblyName xaml
  |> fun nme -> System.Uri(nme, System.UriKind.Relative)
  |> System.Windows.Application.LoadComponent
  :?> _

let mainWindow : Window = getXamlResource "MainWindow.xaml" 
let canvas = mainWindow.FindName("canvas") :?> Canvas

let drawLine (canvas : Canvas) X1 Y1 X2 Y2 =
    let line = new Line()
    line.Stroke <- Brushes.Red
    line.X1 <- X1
    line.X2 <- X2
    line.Y1 <- Y1
    line.Y2 <- Y2
    canvas.Children.Add(line) |> ignore

drawLine canvas 100.0 100.0 200.0 200.0

[<System.STAThread>]
do System.Windows.Application().Run(mainWindow) |> ignore
