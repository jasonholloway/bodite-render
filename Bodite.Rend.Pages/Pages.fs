namespace BoditeRender

open System

    
module Pages =
   
    let buildHomePage (m: BoditeModel) =        
        [for l in Locales.All ->
                    HomePage(l) :> Page]


    let buildCategoryPages (m: BoditeModel) =
        Locales.All
        |> List.collect (fun l ->  
                            [for (_, c) in (Map.toSeq m.Categories) ->
                                CategoryPage(l, c) :> Page
                                ]
                            )



    let buildProductPages (m: BoditeModel) =
        Locales.All
        |> List.collect (fun l ->
                            m.Categories
                            |> Map.toSeq
                            |> Seq.collect (fun (_, c) ->
                                                c.Products
                                                |> Seq.map (fun p -> ProductPage(l, p, c) :> Page) )
                            |> Seq.toList )


    let buildPages (m: BoditeModel) =        
        [
            buildHomePage m
            buildCategoryPages m
            buildProductPages m
        ]
        |> List.concat


    let buildPageRegistry (pages: Page seq) =
        pages
        |> Seq.map (fun p -> (p.Keys, p))
        |> Map.ofSeq


        