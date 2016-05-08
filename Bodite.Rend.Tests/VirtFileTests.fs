namespace BoditeRender

open NUnit.Framework
open System.Text
open FsUnit

[<TestFixture>]
type VirtFileTests () =

    [<Test>]
    member x.``GetDataHash() returns same (semi)unique int for same data`` () =

        let sameData = ASCIIEncoding.UTF8.GetBytes("hello Jason!")

        let calcHash i =
            let vf = new VirtFile("a/path/" + i.ToString(), sameData)
            vf.GetDataHash()
            
        [0..20]
        |> Seq.map calcHash
        |> Seq.forall (fun h -> h.Equals(calcHash 313))
        |> should equal true

        
    [<Test>]
    member x.``GetDataHash() returns mostly unique number`` () =
        let vf1 = new VirtFile("lkjlj", "fipipokklkle")
        let vf2 = new VirtFile("afafjpojp", "ljpjpjpojdvds")
        let vf3 = new VirtFile("ads;lk;k;k;", "vsvsosoos")

        Assert.That(vf1.GetDataHash(), Is.Not.EqualTo(vf2.GetDataHash()))
        Assert.That(vf1.GetDataHash(), Is.Not.EqualTo(vf3.GetDataHash()))
        Assert.That(vf2.GetDataHash(), Is.Not.EqualTo(vf3.GetDataHash()))

