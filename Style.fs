namespace StockViewer

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html

[<AutoOpen>]
[<JavaScript>]
module Style =

    // ==================== Styles for the Stock page ====================
    let containerStyle =
        attr.style "max-width: 1000px; margin: auto; padding: 20px; font-family: Arial, sans-serif;"

    let headerStyle =
        attr.style "font-size: 28px; color: #333; margin-bottom: 20px; text-align: center;"

    let selectStyle =
        attr.style "margin: 10px 10px 10px 0; padding: 5px; font-size: 16px;"

    let chartContainerStyle =
        attr.style "margin-top: 20px; border: 1px solid #ccc; padding: 10px; border-radius: 8px; background-color: #f9f9f9;"

    // ==================== Styles for the navbar ====================
    let navbarContainer =
        attr.style "background-color: #333; padding: 10px 0; text-align: center; margin-bottom: 20px;"

    let navbarLink =
        attr.style "color: white; margin: 0 15px; text-decoration: none; font-weight: bold; font-size: 18px;"

    let navbarLinkActive =
        attr.style "color: #00d0ff; border-bottom: 2px solid #00d0ff;"


    // ==================== Styles for a home page ====================
    let centeredPageContainer =
        attr.style "display: flex; justify-content: center; align-items: flex-start; min-height: 100vh; background-color: #f4f4f4; padding-top: 20px;"

    let cardContainer =
        attr.style """
            background-color: white;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
            max-width: 800px;
            width: 100%;
            font-family: 'Segoe UI', sans-serif;
        """

    let mainTitle =
        attr.style "font-size: 2.5em; color: #00cba9; margin-bottom: 10px; text-align: center;"

    let introText =
        attr.style "font-size: 1.2em; margin-bottom: 10px; text-align: center;"

    let sectionTitle =
        attr.style "color: #333; margin-top: 15px; font-size: 1.4em;"

    let fadeInStyle =
        attr.style "animation: fadeIn 1s ease-in;"

    let fadeInKeyframes =
        Doc.Verbatim """
            <style>
            @keyframes fadeIn {
                from { opacity: 0; transform: translateY(10px); }
                to { opacity: 1; transform: translateY(0); }
            }
            </style>
        """
    
    