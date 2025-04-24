namespace StockViewer

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html

[<AutoOpen>]
[<JavaScript>]
module Style =

    let containerStyle =
        attr.style "max-width: 1000px; margin: auto; padding: 20px; font-family: Arial, sans-serif;"

    let headerStyle =
        attr.style "font-size: 28px; color: #333; margin-bottom: 20px; text-align: center;"

    let selectStyle =
        attr.style "margin: 10px 10px 10px 0; padding: 5px; font-size: 16px;"

    let chartContainerStyle =
        attr.style "margin-top: 20px; border: 1px solid #ccc; padding: 10px; border-radius: 8px; background-color: #f9f9f9;"

    // Új stílusok a navigációhoz
    let navbarContainer =
        attr.style "background-color: #333; padding: 10px 0; text-align: center; margin-bottom: 20px;"

    let navbarLink =
        attr.style "color: white; margin: 0 15px; text-decoration: none; font-weight: bold; font-size: 18px;"

    let navbarLinkActive =
        attr.style "color: #00d0ff; border-bottom: 2px solid #00d0ff;"
