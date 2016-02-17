namespace BoditeRender

open System


type PageKey (v: obj) =
    member val Value = v
    member val Hash = v.GetHashCode()

    override x.Equals(o) =
        x.Value.Equals(o)

    override x.GetHashCode() =
        x.Hash

    interface IComparable with
        member x.CompareTo(o) =
            let other = (o :?> PageKey)
            x.Hash - other.Hash





type PageContext = {
    Locale: Locale
    Model: Model
    //FindPage: obj seq -> Page
}



[<AbstractClass>]
type Page (ctx: PageContext, keys : Set<PageKey>) =

    new(ctx, keys : obj seq) =
        Page(ctx, keys 
                  |> Seq.map (fun k -> PageKey(k))
                  |> Set.ofSeq)

    new(ctx, key : obj) =
        Page(ctx, [key])
        
    member val Ctx = ctx
    member val Keys = keys

    abstract member Path : string
    abstract member Title : string



                                                    
type HomePage (ctx: PageContext) =
    inherit Page(ctx, ["Index", ctx.Locale])

    override Page.Path = ""
    override Page.Title = "Brigitas Bodite"

    member val FeaturedProducts : List<Product> = List.empty
    



type ProductPage (ctx: PageContext, prod: Product, cat: Category) = 
    inherit Page(ctx, [cat, prod, ctx.Locale])
    
    override Page.Path = "product/" + prod.Key
    override Page.Title = defaultArg (prod.Name.get ctx.Locale) ""
    
    member val Product = prod
    member val Category = cat
        



type CategoryPage (ctx: PageContext, cat: Category) =
    inherit Page(ctx, [cat, ctx.Locale])
    
    override Page.Path = "category/" + cat.Key
    override Page.Title = defaultArg (cat.Name.get ctx.Locale) ""
    
    member val Category = cat
    

    

module Pages =
   
    let buildHomePage (m: Model) =        
        [for l in Locales.All ->
                    let ctx = {
                        Locale = l
                        Model = m
                    }
         
                    HomePage(ctx) :> Page]


    let buildCategoryPages (m: Model) =
        Locales.All
        |> List.collect (fun l ->  
                            let ctx = {
                                Locale = l
                                Model = m
                            }

                            [for (_, c) in (Map.toSeq m.Categories) ->
                                CategoryPage(ctx, c) :> Page
                                ]
                            )



    let buildProductPages (m: Model) =
        Locales.All
        |> List.collect (fun l ->
                            let ctx = {
                                Locale = l
                                Model = m
                            }

                            m.Categories
                            |> Map.toSeq
                            |> Seq.collect (fun (_, c) ->
                                                c.Products
                                                |> Seq.map (fun p -> ProductPage(ctx, p, c) :> Page) )
                            |> Seq.toList )


    let buildPages (m: Model) =        
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


        