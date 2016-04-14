namespace BoditeRender

open NUnit.Framework
open System.Text
open FsUnit

[<TestFixture>]
type VirtFileTests () =

    [<Test>]
    [<Ignore>]
    member x.``GetDataHash() returns same (semi)unique int for same data`` () =

        let sameData = ASCIIEncoding.UTF8.GetBytes("hello Jason!")

        let lastResult = null

        [0..20]
        |> Seq.map (fun i ->
                        use vf = new VirtFile("a/path/" + i.ToString(), sameData)                        
                        vf.GetDataHash()
                    )
        |> should all equal 0
        

        ()

