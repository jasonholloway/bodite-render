open Render.Engine
open render.model
open RazorEngine.Configuration
open RazorEngine.Templating

[<EntryPoint>]
let main argv =     
           
    let templateMgr = ResolvePathTemplateManager([| @"../../../render.templates/" |])
    
    let renderService = 
            TemplateServiceConfiguration(TemplateManager=templateMgr) 
            |> RazorEngineService.Create
            
    let prods = [|
                    new ProductPage(Url = "", Title = "Hello!");
                    new ProductPage(Url = "", Title = "Hello again!");
                |];
                                                                                                                          
    prods
    |> Seq.map (fun p -> renderService.RunCompile("product.cshtml", p.GetType(), p))
    |> Seq.iter (fun t -> t |> System.Console.WriteLine)
    
    0



    //***** FIRST THINGS FIRST ******
    //
    // Need nice slug names for each product (and category, unfortunately)
    //
    


    //the problem with prerendering:
    // > dynamic filtering of products still requires angular
    //      and, because of this, crawling will still fail.
    //      though sharing would be possible on product pages
    //
    // > additional problem of keeping products in sync with pages, though this isn't
    //   too great a thing.
    //
    // > the point of CouchDB diminishes disappointingly if consumed from .NET
    // 





    //
    // So... now need to fill flat buffer of pages to be created
    //
    // These pages will also be in an articulated structure - network, even (just no cyclicity please!)
    //
    //