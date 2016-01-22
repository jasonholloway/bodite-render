module Engine

    open System
    open RazorEngine
    open RazorEngine.Templating


    type RenderSpec<'m> = {
        templatePath : string;
        model : 'm;
        url: string array
    }
    


    let loadTemplate (path:string) =
        //returns as string
        0
        
    
    let renderSpec (spec:RenderSpec<_>) =
        spec.templatePath
        |> Console.WriteLine


    let renderString (template:string) model =
        let modelType = model.GetType()
        let templateName = System.Guid.NewGuid().ToString()        
        RazorEngine.Engine.Razor.RunCompile (template, templateName, modelType, model :> obj)
