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


            

[<AbstractClass>]
type Page (keys: Set<PageKey>) =

    new(keys : obj seq) =
        Page(keys 
             |> Seq.map (fun k -> PageKey(k))
             |> Set.ofSeq)

    new(key : obj) =
        Page([key])
        
    member val Keys = keys

    abstract member Path : string
    abstract member Title : string


        
                                                    
type HomePage (locale: Locale) =
    inherit Page(["Index", locale])

    override Page.Path = ""
    override Page.Title = "Brigitas Bodite"
    
    member val Locale = locale
    member val FeaturedProducts : List<Product> = List.empty
    



type ProductPage (locale: Locale, prod: Product, cat: Category) = 
    inherit Page([cat, prod, locale])
    
    override Page.Path = "product/" + prod.Key
    override Page.Title = defaultArg (prod.Name.get locale) ""
    
    member val Locale = locale
    member val Product = prod
    member val Category = cat
        



type CategoryPage (locale: Locale, cat: Category) =
    inherit Page([cat, locale])
    
    override Page.Path = "category/" + cat.Key
    override Page.Title = defaultArg (cat.Name.get locale) ""
    
    member val Locale = locale
    member val Category = cat
    

    

module Pages =
   
    let buildHomePage (m: Model) =        
        [for l in Locales.All ->
                    HomePage(l) :> Page]


    let buildCategoryPages (m: Model) =
        Locales.All
        |> List.collect (fun l ->  
                            [for (_, c) in (Map.toSeq m.Categories) ->
                                CategoryPage(l, c) :> Page
                                ]
                            )



    let buildProductPages (m: Model) =
        Locales.All
        |> List.collect (fun l ->
                            m.Categories
                            |> Map.toSeq
                            |> Seq.collect (fun (_, c) ->
                                                c.Products
                                                |> Seq.map (fun p -> ProductPage(l, p, c) :> Page) )
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


        