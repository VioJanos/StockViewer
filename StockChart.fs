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
        let chartVar = Var.Create (text "Please select a stock and interval.")
        let loadClicked = Var.Create false
        let errorVar = Var.Create Doc.Empty
        let chartLoaded = Var.Create false

        // let downloadButton =
        //         Doc.Button "Download CSV" [ attr.style "margin-top: 10px;" ] (fun _ ->
        //             async {
        //                 let symbol = selected.Value
        //                 let interval = selectedInterval.Value

        //                 let! points = Server.GetStockData symbol interval

        //                 if not points.IsEmpty then
        //                     let! url = Server.ExportToCsv points
        //                     JS.Window.Location.Href <- url
        //                 else
        //                     JS.Alert("No data available to download!")
        //             }
        //             |> Async.Start
        //         )

        let downloadButton =
            chartLoaded.View
            |> View.Map (fun enabled ->
                let baseAttrs = 
                    [
                                               
                        if enabled then buttonPrimary else buttonDisabled
                        if not enabled then attr.disabled "disabled"
                        if not enabled then attr.title "Please load the chart first."
                    ]

                Doc.Button "Download CSV" baseAttrs (fun _ ->
                    async {
                        let symbol = selected.Value
                        let interval = selectedInterval.Value
                        let! points = Server.GetStockData symbol interval
                        if not points.IsEmpty then
                            let! url = Server.ExportToCsv points
                            JS.Window.Location.Href <- url
                        else
                            JS.Alert("No data available to download!")
                    }
                    |> Async.Start
                )
            )
            |> Doc.EmbedView

        let updateChart () =
            async {
                div [ spinnerContainer ] [
                    spinnerKeyframes
                    div [ spinnerStyle ] []
                    div [ loadingTextStyle ] [ text "Loading..." ]
                ]
                errorVar.Value <- Doc.Empty
                try
                    let symbol = selected.Value
                    let interval = selectedInterval.Value
                    let! (points: StockDataPoint list) = Server.GetStockData symbol selectedInterval.Value

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
                            |> fun c -> c.WithTitle("Open (USD)").WithStrokeColor(Color.Name "green").WithPointStrokeColor(Color.Name "black")

                            Array.zip labelsArr highArr
                            |> Chart.Line
                            |> fun c -> c.WithTitle("High (USD)").WithStrokeColor(Color.Name "red").WithPointStrokeColor(Color.Name "black")

                            Array.zip labelsArr lowArr
                            |> Chart.Line
                            |> fun c -> c.WithTitle("Low (USD)").WithStrokeColor(Color.Name "purple").WithPointStrokeColor(Color.Name "black")

                            Array.zip labelsArr closeArr
                            |> Chart.Line
                            |> fun c -> c.WithTitle("Close (USD)").WithStrokeColor(Color.Name "blue").WithPointStrokeColor(Color.Name "black")
                        ]

                        let chart = Chart.Combine chartLines 
                        chartVar.Value <- 
                            div [
                                attr.style "display: flex; justify-content: center; margin-top: 30px;"
                                fadeInStyle
                            ] [
                                ChartJs.Render(chart, Size = Size(1200, 600))
                            ]
                        chartLoaded.Value <- true
                with ex ->
                    chartVar.Value <- text "Failed to load data."
                    chartLoaded.Value <- false
                    errorVar.Value <- 
                        p [ attr.style "color: red;" ] [
                            text (
                                if ex.Message.Contains("500") then
                                    "🚫 API limit reached! Please wait a minute and try again."
                                else
                                    $"API error: {ex.Message}"
                            )
                        ]              
            }

            

        let onClickLoad () = 
            loadClicked.Value <- true
            updateChart()
            |> Async.Start

        div [ attr.style "max-width: 800px; margin: auto;" ] [
            h2 [headerStyle] [ text "Stock Price Viewer (select interval)" ]
            Doc.InputType.Select [selectStyle] id symbols selected
            span [ attr.style "margin-left: 10px;" ] [
                Doc.InputType.Select [selectStyle] id intervals selectedInterval
            ]
            span [ attr.style "margin-left: 10px;" ] [
                button 
                    [ 
                        on.click (fun _ _ -> onClickLoad())
                        buttonBase
                        // buttonPrimary
                    ] 
                    [ text "Load Data" ]
            ]
            span [ attr.style "margin-left: 10px;" ] [
                downloadButton 
            ]
            br [] []
            div [chartWrapper] [
                chartVar.View |> Doc.EmbedView 
                errorVar.View |> Doc.EmbedView
            ]
        ]

