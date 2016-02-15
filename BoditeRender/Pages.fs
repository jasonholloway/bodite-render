namespace BoditeRender

open System

[<AbstractClass>]
type Page (keys : Set<IComparable>) =

    new(keys : IComparable seq) =
        Page(Set(keys))

    new(key) =
        Page(Set([key]))
        
    member val Keys = keys

    abstract member Locale : Locale
    abstract member Path : string
    abstract member Title : string



                                                    
type HomePage (locale) =
    inherit Page(["Index", locale])

    override Page.Locale = locale
    override Page.Path = ""
    override Page.Title = "Brigitas Bodite"

    member val FeaturedProducts : List<Product> = List.empty
    



type ProductPage (prod: Product, cat: Category, locale) = 
    inherit Page([cat, prod, locale])
    
    override Page.Locale = locale
    override Page.Path = "blah"
    override Page.Title = defaultArg (prod.Name.get locale) ""
    
    member val Product = prod
    member val Category = cat
        



type CategoryPage (cat: Category, locale) =
    inherit Page([cat, locale])
    
    override Page.Locale = locale
    override Page.Path = "category/" + cat.Key
    override Page.Title = defaultArg (cat.Name.get locale) ""
    
    member val Category = cat
    

    

module Pages =
   
    let buildHomePage (m: Model) =        
        [for l in Locales.All -> (HomePage l) :> Page]


    let buildCategoryPages (m: Model) =
        Locales.All
        |> List.collect (fun l ->  
                            [for (_, c) in (Map.toSeq m.Categories) ->
                                CategoryPage(c, l) :> Page
                                ]
                            )



    let buildProductPages (m: Model) =
        Locales.All
        |> List.collect (fun l ->
                            m.Categories
                            |> Map.toSeq
                            |> Seq.collect (fun (_, c) ->
                                                c.Products
                                                |> Seq.map (fun p -> ProductPage(p, c, l) :> Page) )
                            |> Seq.toList )


    let buildPages (m: Model) =        
        [
            buildHomePage m
            buildCategoryPages m
            buildProductPages m
        ]
        |> List.concat

        //now build pather from each page's associated model sack
        //...


        