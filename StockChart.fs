namespace StockViewer

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.Charting
open WebSharper.Charting.Renderers
open WebSharper.Charting.Pervasives
open StockViewer.Style
open WebSharper.JavaScript


[<JavaScript>]
module StockChart =

    let symbols = [ "IBM"; "AAPL"; "MSFT"; "TSLA"; "GOOGL"; "AMZN" ]
    let intervals = [ "daily"; "weekly"; "monthly" ]
    

    let MainView () =

        let selected = Var.Create "IBM"
        let selectedInterval = Var.Create "daily"
        let chartVar = Var.Create (text "Loading...")
        let loadClicked = Var.Create false


        let downloadButton =
            Doc.Button "Letöltés CSV" [ attr.style "margin-top: 10px;" ] (fun _ ->
                async {
                    let symbol = selected.Value
                    let interval = selectedInterval.Value

                    let! points = Server.GetStockData symbol interval

                    if not points.IsEmpty then
                        let! url = Server.ExportToCsv points
                        JS.Window.Location.Href <- url
                    else
                        JS.Alert("Nincs adat a letöltéshez!")
                }
                |> Async.Start
            )
        

        let updateChart () =
            async {
                let symbol = selected.Value
                let interval = selectedInterval.Value
                let! (points: StockDataPoint list) = Server.GetStockData symbol selectedInterval.Value

                let downloadButton points =
                    Doc.Button "CSV export" [] (fun _ ->
                        async {
                            let! url = Server.ExportToCsv points
                            WebSharper.JavaScript.JS.Window.Location.Href <- url
                        }
                        |> Async.Start
                    )

                if points.IsEmpty then
                    chartVar.Value <- text "No data available."
                else
                    let labelsArr = points |> List.map (fun p -> p.Date) |> Array.ofList
                    let openArr   = points |> List.map (fun p -> p.Open) |> Array.ofList
                    let highArr   = points |> List.map (fun p -> p.High) |> Array.ofList
                    let lowArr    = points |> List.map (fun p -> p.Low)  |> Array.ofList
                    let closeArr  = points |> List.map (fun p -> p.Close)|> Array.ofList

                    let chartLines = [
                        Array.zip labelsArr openArr
                        |> Chart.Line
                        |> fun c -> c.WithTitle("Open").WithStrokeColor(Color.Name "green")

                        Array.zip labelsArr highArr
                        |> Chart.Line
                        |> fun c -> c.WithTitle("High").WithStrokeColor(Color.Name "red")

                        Array.zip labelsArr lowArr
                        |> Chart.Line
                        |> fun c -> c.WithTitle("Low").WithStrokeColor(Color.Name "purple")

                        Array.zip labelsArr closeArr
                        |> Chart.Line
                        |> fun c -> c.WithTitle("Close").WithStrokeColor(Color.Name "blue")
                    ]

                    let chart = Chart.Combine chartLines

                    chartVar.Value <- ChartJs.Render(chart, Size = Size(1200, 600))
                    
            }
            

        let onClickLoad () = 
            loadClicked.Value <- true
            updateChart()
            |> Async.Start

        


        div [ attr.style "max-width: 800px; margin: auto;" ] [
            h2 [headerStyle] [ text "Részvényárfolyam (választható intervallum)" ]
            Doc.InputType.Select [selectStyle] id symbols selected
            span [ attr.style "margin-left: 10px;" ] [
                Doc.InputType.Select [selectStyle] id intervals selectedInterval
            ]
            span [ attr.style "margin-left: 10px;" ] [
                button [ on.click (fun _ _ -> onClickLoad()) ] [ text "Lekérés" ]
            ]
            span [ attr.style "margin-left: 10px;" ] [
                downloadButton 
            ]
            br [] []
            div [] [ chartVar.View |> Doc.EmbedView ]
        ]

