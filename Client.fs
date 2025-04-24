namespace StockViewer

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Sitelets
open StockChart

[<JavaScript>]
module Client =
    
    open WebSharper.UI
    open WebSharper.UI.Notation

    type EndPoint =
        | [<EndPoint "/">] Home
        | [<EndPoint "/stock">] Stock

    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    module Pages = 
        open WebSharper.UI.Html
        open WebSharper.Charting


        // module Home =
        //     let People =
        //         ListModel.FromSeq [
        //             "John"
        //             "Paul"
        //         ]
            
        //     let NewName = Var.Create ""

        // let HomePage() =
        //     IndexTemplate.HomePage()
        //         .ListContainer(
        //             Home.People.View.DocSeqCached(fun (name: string) ->
        //                 IndexTemplate.ListItem().Name(name).Doc()
        //             )
        //         )
        //         .Name(Home.NewName)
        //         .Add(fun e ->
        //             Home.People.Add(Home.NewName.Value)
        //             Home.NewName.Value <- ""
        //         )
        //         .Doc()

        open Home
        let HomePage () = Home.MainContent()
            
        open StockChart
        let StockPage () = MainView()

    let router = Router.Infer<EndPoint>()
    let currentPage = Router.InstallHash Home router

    type Router<'T when 'T: equality> with
        member this.LinkHash (ep: 'T) = "#" + this.Link ep

    open WebSharper.UI.Html
    open WebSharper.UI.Client
    open WebSharper.UI.Notation
    open WebSharper.UI.Templating
    
    let navbar (currentPage: Var<EndPoint>) =
        let navItem (name: string) (ep: EndPoint) =
            let cls = currentPage.View.Map (fun cp ->
                if cp = ep then
                    "margin: 0 10px; padding: 6px 12px; color: white; text-decoration: none; background-color: #00cba9; border-radius: 5px; font-weight: bold;"
                else
                    "margin: 0 10px; padding: 6px 12px; color: white; text-decoration: none;"
            )
            a [
                attr.href (router.LinkHash ep)
                attr.styleDyn cls
            ] [text name]

        div [
            attr.style "background-color: #333; padding: 10px; text-align: center;"
        ] [
            navItem "Home" Home
            navItem "Shares" Stock
        ]

    [<SPAEntryPoint>]
    let Main () =
        
        let renderInnerPage (currentPage: Var<EndPoint>) =
            currentPage.View.Map (fun endpoint ->
                let content =
                    match endpoint with
                    | Home       -> Pages.HomePage()
                    | Stock      -> Pages.StockPage()
                div [] [ navbar currentPage; content ]
            )
            |> Doc.EmbedView

        IndexTemplate()
            .Content(renderInnerPage currentPage)
            // .SwitchToHome(fun _ -> currentPage := Home)
            .Bind()
