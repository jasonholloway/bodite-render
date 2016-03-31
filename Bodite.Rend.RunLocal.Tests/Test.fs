namespace BoditeRender.RunLocal.Tests

module RunLocal =

    open NUnit.Framework
    open FsUnit
    open BoditeRender
    open System.IO


    [<TestFixture>]
    type ``getFSLoader`` () =
    
        [<Test>]
        member x.``resolves file from immediate directory`` () =
            use file = new Helpers.TempFile("file.cshtml", "templatecontents")
        
            let loader = new FSLoader(file.DirPath)

            "file.cshtml"
            |> loader.Load 
            |> should equal "templatecontents"




    [<TestFixture>]
    type ``getFSCommitter`` () =

        [<Test>]
        member x.``commits to immediate directory`` () =
            use folder = new Helpers.TempFolder()

            let committer = new FSCommitter(folder.Path)
                    
            use vf = new VirtFile("file.html", "blarg")
                       
            vf |> committer.Commit

            File.ReadAllText(Path.Combine(folder.Path, vf.Path))
            |> should equal "blarg"

        
        [<Test>]
        member x.``creates intermediate folders if not existent, then commits`` () =
            use folder = new Helpers.TempFolder()
            use committer = new FSCommitter(folder.Path)                    
            use vf = new VirtFile("blarg/oof/file.html", "blarg")
                       
            vf |> committer.Commit
            
            Directory.Exists(Path.Combine(folder.Path, "blarg"))
            |> should equal true
            
            Directory.Exists(Path.Combine(folder.Path, "blarg", "oof"))
            |> should equal true
            
            File.ReadAllText(Path.Combine(folder.Path, vf.Path))
            |> should equal "blarg"

            
        [<Test>]
        member x.``commits into intermediate folders if existent`` () =
            use folder = new Helpers.TempFolder()
            use committer = new FSCommitter(folder.Path)
            
            Directory.CreateDirectory(Path.Combine(folder.Path, "krunk"))
            |> ignore
            
            use vf = new VirtFile("krunk/file.html", "blarg")
                       
            vf |> committer.Commit
            
            Directory.Exists(Path.Combine(folder.Path, "krunk"))
            |> should equal true
            
            File.ReadAllText(Path.Combine(folder.Path, vf.Path))
            |> should equal "blarg"


        [<Test>]
        member x.``concretizes hanging path to index doc in new named folder`` () =
            use committer = new FSCommitter("")
            
            "pomeranians/teddy-pom-pom"
            |> committer.ConcretizePath
            |> should equal "pomeranians/teddy-pom-pom/index.html"

        
        [<Test>]
        member x.``leaves fully specced paths alone`` () =
            use committer = new FSCommitter("")
            
            "pomeranians/teddy-pom-pom.html"
            |> committer.ConcretizePath
            |> should equal "pomeranians/teddy-pom-pom.html"

        [<Test>]
        member x.``concretizes empty path`` () =
            use committer = new FSCommitter("")

            ""
            |> committer.ConcretizePath
            |> should equal "index.html"
