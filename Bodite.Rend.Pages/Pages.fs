namespace BoditeRender

open System

    
module Pages =
   
    let buildHomePage (m: BoditeModel) =        
        m.Locales
        |> Seq.map (fun l -> HomePage(l) :> Page)


    let buildCategoryPages (m: BoditeModel) =
        m.Locales 
        |> Seq.collect (fun l ->  
                            [for c in m.Categories ->
                                CategoryPage(l, c) :> Page
                                ]
                            )



    let buildProductPages (m: BoditeModel) =
        m.Locales
        |> Seq.collect (fun l ->
                            m.Categories
                            |> Seq.collect (fun c ->
                                                c.Products
                                                |> Seq.map (fun p -> ProductPage(l, p, c) :> Page) )
                            |> Seq.toList )


    let buildPages (m: BoditeModel) =        
        [
            buildHomePage m
            buildCategoryPages m
            buildProductPages m
        ]
        |> Seq.concat

