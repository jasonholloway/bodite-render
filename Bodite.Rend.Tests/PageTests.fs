module Pages

open System
open NUnit.Framework
open FsUnit

open BoditeRender



let createCatOfProds key prods =    
    {
        Key = key
        Name = LocaleString []
        Description = LocaleString []
        Children = []
        Products = prods
    }

let createProduct catKeys =
    {
        Product.Key = Guid.NewGuid().ToString()
        Name = LocaleString []
        Description = LocaleString []
//        MachineName = ""
        CategoryKeys = catKeys
    }
    

let ofType<'T> subject =
    box subject :? 'T


let catKeys = [1..5] |> List.map (fun _ -> Guid.NewGuid().ToString())

let locales = Locales.All |> Set.ofSeq

let prods = [1..30] 
            |> Seq.map (fun _ -> createProduct (catKeys |> Helpers.getRandomSelection 2)) 
            |> Set.ofSeq 

let cats =  catKeys 
            |> Seq.map 
                (fun key -> createCatOfProds key (prods 
                                                    |> Seq.filter (fun p -> p.CategoryKeys |> Seq.exists (fun ck -> ck.Equals(key)))
                                                    |> Seq.toList
                                                    ))
            |> Set.ofSeq

let model = new TestModel(locales, prods, cats)




[<TestFixture>]
type ``buildPages`` () =    

    [<Test>]
    member x.``returns seq of pages`` () =
        let res = Pages.buildPages model

        Assert.That(
                res,
                Is.InstanceOf<seq<Page>>())


    [<Test>]
    member x.``builds one ProductPage per Product * Category * Locale`` () =    
        model
        |> Pages.buildPages
        |> Seq.filter ofType<ProductPage>
        |> Seq.length |> should equal ((model.Products 
                                            |> Seq.collect (fun p -> p.CategoryKeys) 
                                            |> Seq.length) 
                                        * Locales.All.Length)


    [<Test>]
    member x.``builds CategoryPage per Category * Locale`` () = 
        model
        |> Pages.buildPages
        |> Seq.filter ofType<CategoryPage>
        |> Seq.length |> should equal (model.Categories.Count * Locales.All.Length)



    [<Test>]
    member x.``builds one HomePage per Locale`` () =
        TestModel(Locales.All |> Set.ofSeq, Set.empty, Set.empty)
        |> Pages.buildPages
        |> Seq.filter ofType<HomePage>
        |> Seq.length |> should equal Locales.All.Length




        

type TestPage (keys: obj seq) =
    inherit Page(keys)

    override Page.Path = ""
    override Page.Title = "Hello"



[<TestFixture>]
type ``PageRegistry`` () =
        
    [<Test>]
    member x.``builds from page list and returns map of keys * pages`` () =
        [ for i in [0..10] -> TestPage([i]) :> Page]
        |> PageReg.build
        |> should be ofExactType<Map<Set<WrappedKey>, Page>>


    [<Test>]
    member x.``finds page given correct key seq`` () =
        let createKeys () : obj list = 
            [
                Locale(Guid.NewGuid().ToString())
                Guid.NewGuid().ToString()
                obj()
            ]

        let pages = [| for i in [0..100] -> TestPage(createKeys()) :> Page |]

        let reg = pages
                  |> PageReg.build

        pages
        |> Seq.iter (fun page -> 
                        page.Keys |> Set.toSeq |> Seq.map (fun k -> k.Value) |> Seq.rev
                        |> PageReg.findPage reg                        
                        |> should equal (Some page)
                    )
