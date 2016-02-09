module Render

open NUnit.Framework
open FsUnit
open FsUnit.TopLevelOperators
open System
open System.IO
open BoditeRender


let createCategory key prods =    
    {
        Key = key
        Name = { LV=None; RU=None }
        Description = { LV=None; RU=None }
        Children = []
        Products = prods
    }

let createProduct catKeys =
    {
        Key = Guid.NewGuid().ToString()
        Name = { LV=None; RU=None }
        Description = { LV=None; RU=None }
        MachineName = ""
        CategoryKeys = catKeys
    }
    
let createCatAndProd () =
    let catKey = Guid.NewGuid().ToString()
    let prod = createProduct [catKey]
    let cat = createCategory catKey [prod]
    (cat, prod)
        



[<TestFixture>]
type ``renderPage`` () =

    [<Test>]
    member x.``returns VirtFile list`` () =
        let renderer = Renderer(fun s -> "hello!")

        renderer.renderPage (new Page(path="", title=None))
        |> should be ofExactType<VirtFile list>
         
    
    [<Test>]
    member x.``resolves template with typename of passed Page`` () =
        let renderer = Renderer(fun k -> 
                                    match k with
                                    | "Page"    -> "hello!" 
                                    | _         -> failwith "Bad template key!")
                                    
        let result = renderer.renderPage (new Page(path="", title=None))
        
        result |> should be ofExactType<VirtFile list>
        result |> should haveLength 1

        result.Head.Data
        |> Helpers.stream2String
        |> should equal "hello!"



[<TestFixture>]
type ``getFSResolver`` () =
    
    [<Test>]
    member x.``resolves file from immediate directory`` () =
        use file = new Helpers.TempFile("file.cshtml", "templatecontents")
        
        let resolver = Render.getFSResolver file.DirPath

        resolver "file.cshtml"
        |> should equal "templatecontents"




[<TestFixture>]
type ``getFSCommitter`` () =

    [<Test>]
    member x.``commits to immediate directory`` () =
        use folder = new Helpers.TempFolder()

        let committer = Render.getFSCommitter folder.Path
                    
        use vf = new VirtFile("file.html", "blarg")
                       
        committer vf 
        |> ignore

        File.ReadAllText(Path.Combine(folder.Path, vf.Path))
        |> should equal "blarg"
