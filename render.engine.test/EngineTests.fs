namespace render.engine.test

open NUnit.Framework
open FsUnit
open render.engine

module EngineTests =

    type Hamster = {
        name:string;
        colour:string;
    }


    [<Test>]
    let ``renderString renders with model`` () =        
        let template = [|
                            "@using RazorEngine.Templating"
                            "@using render.engine.test";
                            "@inherits TemplateBase<EngineTests.Hamster>";
                            "Hello @Model.name, you are @Model.colour!";
                        |]
                        |> String.concat System.Environment.NewLine
    
        { name = "Jeremy"; colour = "mauve" }
        |> Engine.renderString template
        |> should equal "Hello Jeremy, you are mauve!"


