namespace StockViewer

open WebSharper
open System.Net.Http
open System.Text.Json
open System.IO
open System.Globalization
open System.Collections.Generic

[<JavaScript false>]
module Server =

    type ApiUsage = {
        Date: string
        Count: int
    }

    let usageFile = "api_usage.log"
    let apiKeys = [ "CCLTI7YCJC74ONSR"; "LMQYMBQ8ZTCAIKXZ"; "VWZUV65XC0C2MXHB" ]
    let maxPerDay = 25

    let readUsage () : Dictionary<string, ApiUsage> =
        if File.Exists(usageFile) then
            JsonSerializer.Deserialize<Dictionary<string, ApiUsage>>(File.ReadAllText(usageFile))
        else
            Dictionary()

    let writeUsage (usage: Dictionary<string, ApiUsage>) =
        File.WriteAllText(usageFile, JsonSerializer.Serialize usage)

    let getAvailableKey () =
        let today = System.DateTime.UtcNow.ToString("yyyy-MM-dd")
        let usage = readUsage() // most már Dictionary

        let result =
            apiKeys
            |> List.tryPick (fun key ->
                match usage.TryGetValue(key) with
                | true, u when u.Date = today && u.Count < maxPerDay ->
                    usage.[key] <- { Date = today; Count = u.Count + 1 }
                    Some key
                | false, _ ->
                    usage.[key] <- { Date = today; Count = 1 }
                    Some key
                | _ -> None
            )
        match result with
        | Some key ->
            writeUsage usage
            Some key
        | None -> None

    
    [<Rpc>]
    let ExportToCsv (data: StockDataPoint list) : Async<string> =
        async {
            let path = "wwwroot/export.csv"
            let lines =
                seq {
                    yield "Date,Open,High,Low,Close"
                    for dp in data do
                        yield $"{dp.Date};{dp.Open};{dp.High},;{dp.Low};{dp.Close}"
                }
            System.IO.File.WriteAllLines(path, lines)
            return "/export.csv" // relatív útvonal a letöltéshez
        }


    [<Rpc>]
    let GetStockData (symbol: string) (interval: string)  : Async<StockDataPoint list> =
        async {

            let intervalSelection = 
                match interval with
                | "daily" -> "TIME_SERIES_DAILY"
                | "weekly" -> "TIME_SERIES_WEEKLY"
                | "monthly" -> "TIME_SERIES_MONTHLY"
                | _ -> "TIME_SERIES_DAILY"
            
            let seriesKeys = 
                match interval with
                | "daily" -> "Time Series (Daily)"
                | "weekly" -> "Weekly Time Series"
                | "monthly" -> "Monthly Time Series"
                | _ -> "Time Series (Daily)"


            let tryParse (name: string) (s: string) =
                let mutable result = 0.0
                if System.Double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, &result) then
                    Some result
                else
                    File.AppendAllText("parse_errors.log", $"Hiba: {name} = '{s}'\n")
                    None

            let getFromTwelvedata (symbol: string) (interval: string) : Async<StockDataPoint list> =
                async {
                    
                    let twelvedataInterval = 
                        match interval with
                        | "daily" -> "1day"
                        | "weekly" -> "1week"
                        | "monthly" -> "1month"
                        | _ -> "1day"

                    // Define Apikey for TwelveData
                    let tdApiKey = "c586877e482a4ca3a381f76175a8eb97"

                    // Return 30 data points
                    let url = $"https://api.twelvedata.com/time_series?symbol={symbol}&interval={twelvedataInterval}&apikey={tdApiKey}&outputsize=30"

                    use client = new HttpClient()
                    let! response = client.GetStringAsync(url) |> Async.AwaitTask

                    File.WriteAllText("log_response.json", response + "\n" + url)

                    File.AppendAllText("parse_errors.log", $"Twelvedata válasz:\n{response}\n")

                    try
                        let doc = JsonDocument.Parse(response)
                        let values = doc.RootElement.GetProperty("values")

                        let data = 
                            values.EnumerateArray()
                            |> Seq.choose (fun item ->
                                let date = item.GetProperty("datetime").GetString()
                                let openS = item.GetProperty("open").GetString()
                                let highS = item.GetProperty("high").GetString()
                                let lowS = item.GetProperty("low").GetString()
                                let closeS = item.GetProperty("close").GetString()

                                match
                                    tryParse "open" openS,
                                    tryParse "high" highS,
                                    tryParse "low"  lowS,
                                    tryParse "close" closeS
                                with
                                | Some openP, Some highP, Some lowP, Some closeP ->
                                    Some {
                                        Date = date
                                        Open = openP
                                        High = highP
                                        Low = lowP
                                        Close = closeP
                                    }
                                | _ -> None
                            )
                            |> Seq.sortBy (fun dp -> System.DateTime.Parse(dp.Date))
                            |> Seq.toList
                        
                        return data
                    with ex ->
                        File.AppendAllText("parse_errors.log", $"Twelvedata hiba: {ex.Message}\n")
                        return []
                }

            match getAvailableKey() with
            | None ->
                File.AppendAllText("parse_errors.log", "Nincs elérhető API kulcs (limit túllépve)\n")
                return! getFromTwelvedata symbol interval
                // return []
            | Some apiKey ->
                let url = $"https://www.alphavantage.co/query?function={intervalSelection}&symbol={symbol}&apikey={apiKey}"
                // let url = $"https://www.alphavantage.co/query?function={intervalSelection}&symbol={symbol}&apikey=demo"

                use client = new HttpClient()
                let! response = client.GetStringAsync(url) |> Async.AwaitTask

                File.WriteAllText("log_response.json", response + "\n" + url) 

                if response.Contains("Our standard API rate limit") then
                    File.AppendAllText("parse_errors.log", $"[{apiKey}] NAPI LIMIT ELÉRVE\n")
                    return []
                else
                    try
                        let doc = JsonDocument.Parse(response)
                        // let timeSeries = doc.RootElement.GetProperty("Time Series (Daily)")
                        let timeSeries = doc.RootElement.GetProperty(seriesKeys)

                        

                        let data =
                            timeSeries.EnumerateObject()
                            |> Seq.choose (fun entry ->
                                let date = entry.Name
                                let v = entry.Value

                                let openS = v.GetProperty("1. open").GetString()
                                let highS = v.GetProperty("2. high").GetString()
                                let lowS = v.GetProperty("3. low").GetString()
                                let closeS = v.GetProperty("4. close").GetString()

                                match
                                    tryParse "open" openS,
                                    tryParse "high" highS,
                                    tryParse "low"  lowS,
                                    tryParse "close" closeS
                                with
                                | Some openP, Some highP, Some lowP, Some closeP ->
                                    Some {
                                        Date = date
                                        Open = openP
                                        High = highP
                                        Low = lowP
                                        Close = closeP
                                    }
                                | _ -> None
                            )
                            |> Seq.sortByDescending (fun dp -> System.DateTime.Parse(dp.Date))
                            |> Seq.truncate 30
                            |> Seq.sortBy (fun dp -> System.DateTime.Parse(dp.Date))
                            |> Seq.toList

                        return data

                    with ex ->
                        File.AppendAllText("parse_errors.log", $"Általános hiba: {ex.Message}\n")
                        return []
        }
