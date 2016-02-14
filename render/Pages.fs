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
   
    let buildHomePage (m: Model) (pages: Page list) =        
        (HomePage(Locales.LV) :> Page) :: pages


    let buildCategoryPages (m: Model) (pages: Page list) =
        pages


    let buildProductPages (m: Model) (pages: Page list) =
        pages


    let buildPages (m: Model) =
        []
        |> buildHomePage m
        |> buildCategoryPages m
        |> buildProductPages m

        //now build pather from each page's associated model sack
        //...



//    let buildPages (m: Model) =    
//        seq {
//            yield new HomePage() :> Page
//
//            yield! m.Categories
//                    |> Map.toSeq
//                    |> Seq.collect (fun kc -> seq { 
//                                                let _, c = kc 
//
//                                                yield new CategoryPage(c) :> Page
//
//                                                yield! c.Products 
//                                                       |> Seq.map (fun p -> new ProductPage(p, c) :> Page) 
//
//                                                })
//
//            //...
//        }
//        |> Seq.toList








    //should be one page per product*category combination
    //plus category field should be filled in here...

    //
    //let getProductPages =
    //    Data.getAllProducts
    //    |> Seq.filter (fun p -> p.MachineName.IsSome)
    //    |> Seq.map (fun p -> {
    //                            path = if p.MachineName.IsSome then ("produkti/" + p.MachineName.Value) else "";
    //                            title = "Brigitas Bodite - Produkti - " + p.Name.Lv;  
    //                            product = p;
    //                            category = 13;
    //                        })