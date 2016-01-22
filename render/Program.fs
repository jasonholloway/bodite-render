open Render.Engine


[<EntryPoint>]
let main argv =     
    let prod = new render.model.ProductVM()
    prod.Name <- "cardboard"
    
    
    let template = [|
                        "@using render.model";
                        "@using RazorEngine";
                        "@using RazorEngine.Templating"
                        "@inherits TemplateBase<ProductVM>";
                        "Hello @Model.Name!";
                    |]
                    |> String.concat System.Environment.NewLine
    
    
    Engine.renderString template prod
    |> System.Console.WriteLine

    0

