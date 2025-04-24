namespace StockViewer

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html

[<JavaScript>]
module Home =

    let MainContent () =
        div [ centeredPageContainer ] [
            fadeInKeyframes
            div [ cardContainer ] [
                h1 [ mainTitle ] [ text "📈 StockViewer" ]

                p [ introText ] [
                    text "Welcome to "
                    strong [] [ text "StockViewer" ]
                    text " – a web app to explore and visualize historical stock prices."
                ]

                h2 [ sectionTitle ] [ text "🎯 Purpose" ]
                p [] [
                    text "This project was created as part of university coursework to demonstrate how F# and WebSharper can be used to build interactive, data-driven web applications."
                ]

                h2 [ sectionTitle ] [ text "🛠️ Technologies Used" ]
                ul [ attr.style "line-height: 1.8;" ] [
                    li [] [ strong [] [ text "F#" ]; text " – functional programming" ]
                    li [] [ strong [] [ text "WebSharper" ]; text " – F#-based SPA and RPC" ]
                    li [] [ strong [] [ text "Chart.js" ]; text " – responsive charting" ]
                    li [] [ strong [] [ text "Render.com" ]; text " – free hosting with CI/CD" ]
                ]

                h2 [ sectionTitle ] [ text "🚧 Work in Progress" ]
                p [] [
                    text "This application is still under development. Some features may be incomplete or unavailable."
                ]

                h2 [ sectionTitle ] [ text "🧩 Planned Features" ]
                ul [ attr.style "line-height: 1.8;" ] [
                    li [] [ strong [] [ text "Login system" ]; text " – secure authentication for personalized features." ]
                    li [] [ strong [] [ text "Compare multiple stocks" ]; text " – visualize several companies on the same chart." ]
                    li [] [ strong [] [ text "Saved queries" ]; text " – registered users can save and quickly access past searches." ]
                    li [] [ strong [] [ text "Dark mode" ]; text " – switch between light and dark themes." ]
                    li [] [ strong [] [ text "Mobile optimization" ]; text " – better user experience on phones and tablets." ]
                ]
                h2 [ sectionTitle ] [ text "📬 Contact Information" ]
                p [] [
                    text "For questions, feedback, or collaboration inquiries, feel free to reach out:"
                ]
                ul [ attr.style "line-height: 1.8;" ] [
                    li [] [ strong [] [ text "Email:" ]; text " janos.violadev@gmail.com" ]
                    li [] [ strong [] [ text "GitHub:" ]; text " github.com/VioJanos" ]
                ]
            ]
        ]
