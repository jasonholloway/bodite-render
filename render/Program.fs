open render.model
open RazorEngine.Configuration
open RazorEngine.Templating
//open Data


[<EntryPoint>]
let main argv =
    let templateMgr = ResolvePathTemplateManager([| @"../../../render.templates/" |])
    
    let renderService = 
            TemplateServiceConfiguration(TemplateManager=templateMgr) 
            |> RazorEngineService.Create
            
    let prods = [|
                    new ProductPage(Url = "", Title = "Hello!") //one page per product * category
                    new ProductPage(Url = "", Title = "Hello again!")
                |];
                                                                                                                          
    prods
    |> Seq.map (fun p -> renderService.RunCompile("product.cshtml", p.GetType(), p))
    |> Seq.iter (fun t -> t |> System.Console.WriteLine)
    
    0






    


    //the problem with prerendering:
    // > dynamic filtering of products still requires angular (not just filtering, but sorting...)
    //      and, because of this, crawling will still fail.
    //      though sharing would be possible on product pages
    //
    // > additional problem of keeping products in sync with pages, though this isn't
    //   too great a thing.
    //
    // > the point of CouchDB diminishes disappointingly if consumed from .NET
    // 
    
    //
    // Can sorting and filtering be well done with couch?
    //
    // What alternatives? Could render json documents of products per category, to be processed client-side.
    // Kinda cool thought. In fact, I like it very much.
    //
    // Problem is, crawling again.
    //
    //
    //
    //




    //
    // So... now need to fill flat buffer of pages to be created
    //
    // These pages will also be in an articulated structure - network, even (just no cyclicity please!)
    //
    //