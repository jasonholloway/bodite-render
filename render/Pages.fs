namespace BoditeRender

open System


type Page (keys : Set<IComparable>) =

    new(keys : IComparable seq) =
        Page(Set(keys))

    new(key) =
        Page(Set([key]))
        
    member val Keys = keys

    abstract member Locale : string
    abstract member Path : string
    abstract member Title : string



                                                    
type HomePage (locale) =
    inherit Page(["Index", locale])

    member val Locale = locale
    member val Path = ""
    member val Title = "Brigitas Bodite"

    member val FeaturedProducts : List<Product> = List.empty
    



type ProductPage (prod: Product, cat: Category, locale) = 
    inherit Page([cat, prod, locale])
    
    member val Locale = locale
    member val Path = "blah"
    member val Title = prod.Name.LV
    
    member val Product = prod
    member val Category = cat
        



type CategoryPage (cat: Category, locale) =
    inherit Page([cat, locale])
    
    member val Locale = locale
    member val Path = "category/" + cat.Key
    member val Title = cat.Name.LV
    
    member val Category = cat
    


//
//question: is categorypage to link to productpages? ideally not.
//tho if there were nice way of doing it, then yes. It would be ideal! all the references nicely provided.
//
//


module Pages =
   

    let buildHomePage (m: Model) (pages: Page list) : Page list =
        //will build in both languages
        //and register both with pather,
        //which has to be returned also
        
        //and each returned page has its magic sack
        //or maybe the magic sack can be created from the list as a separate step...

        HomePage("LV") :: pages


    let buildCategoryPages (m: Model) (pages: Page list) =
        pages



    let buildPages2 (m: Model) =
        []
        |> buildHomePage m
        |> buildCategoryPages m

        //now build pather from each page's associated model sack
        //...



    let buildPages (m: Model) =    
        seq {
            yield new HomePage() :> Page

            yield! m.Categories
                    |> Map.toSeq
                    |> Seq.collect (fun kc -> seq { 
                                                let _, c = kc 

                                                yield new CategoryPage(c) :> Page

                                                yield! c.Products 
                                                       |> Seq.map (fun p -> new ProductPage(p, c) :> Page) 

                                                })

            //...
        }
        |> Seq.toList








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