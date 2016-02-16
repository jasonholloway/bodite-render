namespace BoditeRender.RunLocal.Tests

module RunLocal =

    open NUnit.Framework
    open FsUnit
    open BoditeRender
    open System.IO


    [<TestFixture>]
    type ``getFSResolver`` () =
    
        [<Test>]
        member x.``resolves file from immediate directory`` () =
            use file = new Helpers.TempFile("file.cshtml", "templatecontents")
        
            let resolver = FSResolver.create file.DirPath

            resolver "file.cshtml"
            |> should equal "templatecontents"




    [<TestFixture>]
    type ``getFSCommitter`` () =

        [<Test>]
        member x.``commits to immediate directory`` () =
            use folder = new Helpers.TempFolder()

            let committer = FSCommitter.create folder.Path
                    
            use vf = new VirtFile("file.html", "blarg")
                       
            committer vf 
            |> ignore

            File.ReadAllText(Path.Combine(folder.Path, vf.Path))
            |> should equal "blarg"
