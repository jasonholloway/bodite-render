namespace Render.Engine

open System
open RazorEngine
open RazorEngine.Templating


type RenderSpec<'m> = {
    templatePath : string;
    model : 'm;
    url: string array
}


type RenderEngine() =
    member x.loadTemplate (path:string) =
        //returns as string
        0        
    
    member x.renderSpec (spec:RenderSpec<_>) =
        spec.templatePath
        |> Console.WriteLine


    member x.renderString (template:string) model =
        let modelType = model.GetType()
        let templateName = System.Guid.NewGuid().ToString()        
        RazorEngine.Engine.Razor.RunCompile (template, templateName, modelType, model :> obj)


