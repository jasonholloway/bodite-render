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

        
        [<Test>]
        member x.``creates intermediate folders if not existent, then commits`` () =
            use folder = new Helpers.TempFolder()

            let committer = FSCommitter.create folder.Path
                    
            use vf = new VirtFile("blarg/oof/file.html", "blarg")
                       
            committer vf 
            |> ignore


            Directory.Exists(Path.Combine(folder.Path, "blarg"))
            |> should equal true
            
            Directory.Exists(Path.Combine(folder.Path, "blarg", "oof"))
            |> should equal true
            
            File.ReadAllText(Path.Combine(folder.Path, vf.Path))
            |> should equal "blarg"

            
        [<Test>]
        member x.``commits into intermediate folders if existent`` () =
            use folder = new Helpers.TempFolder()

            let committer = FSCommitter.create folder.Path
            
            Directory.CreateDirectory(Path.Combine(folder.Path, "krunk"))
            |> ignore
            
            use vf = new VirtFile("krunk/file.html", "blarg")
                       
            committer vf 
            |> ignore
            
            Directory.Exists(Path.Combine(folder.Path, "krunk"))
            |> should equal true
            
            File.ReadAllText(Path.Combine(folder.Path, vf.Path))
            |> should equal "blarg"


        [<Test>]
        member x.``concretizes hanging path to index doc in new named folder`` () =
            FSCommitter.concretizePath "pomeranians/teddy-pom-pom"
            |> should equal "pomeranians/teddy-pom-pom/index.html"

        
        [<Test>]
        member x.``leaves fully specced paths alone`` () =
            FSCommitter.concretizePath "pomeranians/teddy-pom-pom.html"
            |> should equal "pomeranians/teddy-pom-pom.html"

        [<Test>]
        member x.``concretizes empty path`` () =
            FSCommitter.concretizePath ""
            |> should equal "index.html"
