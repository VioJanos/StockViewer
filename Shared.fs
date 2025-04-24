namespace StockViewer

open WebSharper

[<JavaScript>]
type StockDataPoint = {
    Date: string
    Open: float
    High: float
    Low: float
    Close: float
}

// type EndPoint =
//     | [<WebSharper.EndPoint "/">] Home
//     | [<WebSharper.EndPoint "/charting">] Charting
//     | [<WebSharper.EndPoint "/forms">] Forms
//     | [<WebSharper.EndPoint "/counter">] Counter
//     | [<WebSharper.EndPoint "/calculator">] Calculator
//     | [<WebSharper.EndPoint "/stock">] Stock
