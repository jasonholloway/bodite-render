namespace Render.Engine.Test

open NUnit.Framework
open FsUnit
open Render.Engine

module EngineTests =

    type Hamster = {
        name:string;
        colour:string;
    }


    [<Test>]
    let ``renderString renders with model`` () =        
        let engine = new RenderEngine()
        
        let template = [|
                            "@using RazorEngine.Templating"
                            "@using Render.Engine.Test";
                            "@inherits TemplateBase<EngineTests.Hamster>";
                            "Hello @Model.name, you are @Model.colour!";
                        |]
                        |> String.concat System.Environment.NewLine
    
        { name = "Jeremy"; colour = "mauve" }
        |> engine.renderString template
        |> should equal "Hello Jeremy, you are mauve!"


