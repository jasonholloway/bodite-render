namespace Bodite.Rend.Run.Tests

open NUnit.Framework
open System.Diagnostics
open System.Net
open Amazon.S3
open BoditeRender
open System.Threading
open System.IO

[<TestFixture>]
type S3Tests() = 

    let port = 9583
    let p = new Process()


    
    let getS3Client () =
        let s3Config = new AmazonS3Config()
        s3Config.ServiceURL <- "http://localhost:" + port.ToString()
        s3Config.UseHttp <- true
        s3Config.ReadEntireResponse <- true
        s3Config.ForcePathStyle <- true
        
        let creds = Amazon.Runtime.BasicAWSCredentials("", "")
        
        new AmazonS3Client(creds, s3Config)



    let rec compareStreams (s1 : Stream, s2 : Stream) =
        if not(s1.CanRead.Equals(s2.CanRead)) then false
        else if not(s1.CanRead) then true
        else if not(s1.ReadByte().Equals(s2.ReadByte())) then false
        else compareStreams(s1, s2) 
        
                

    [<SetUp>]
    member x.SetUp () =    
        let info = new ProcessStartInfo("fakes3")        
        info.Arguments <- "-p " + port.ToString() + " -r .fakes3-root"
        info.UseShellExecute <- true
        
        p.StartInfo <- info
        p.Start()
        |> ignore

        Thread.Sleep(500) //to allow fakes3 to open - could also poll here



    [<TearDown>]
    member x.TearDown () =
        p.CloseMainWindow() |> ignore
        p.Kill()
        p.Dispose()
        //remove root folder here!
        //...

        

    [<Test>]
    member x.``fakes3 is available to test`` () =               
        use client = getS3Client()

        let res = client.ListBuckets()
        
        Assert.That(
            res.HttpStatusCode,
            Is.EqualTo(HttpStatusCode.OK))
        ()


    [<Test>]
    member x.``each virtFile committed to correct location`` () =
        use client = getS3Client()
        use committer = new S3Committer(client, "bb")

        let virtFiles =
            [1..10]
            |> List.map (fun i -> new VirtFile("blah" + i.ToString(), "abcdefghijklmnopqrstuvwxyz"))       
            
        virtFiles
        |> Seq.map (fun vf -> vf |> committer.Commit)
        |> ignore
        
        virtFiles
        |> Seq.map (fun vf ->
                        let res = client.GetObject("bb", vf.Path)
                        use str = res.ResponseStream
                        
                        Assert.That(res.HttpStatusCode, Is.EqualTo(HttpStatusCode.OK))
                        Assert.That(compareStreams(vf.Data, str))
                        ()
                        )
        |> ignore

        

