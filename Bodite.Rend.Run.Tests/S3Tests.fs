namespace Bodite.Rend.Run.Tests

open NUnit.Framework
open System.Diagnostics
open System.Net
open Amazon.S3
open BoditeRender
open System
open System.Threading
open System.IO
open FSharp.Data
open HttpFs
open HttpFs.Client


[<TestFixture>]
type S3Tests() = 

    let port = 787
    let bucketName = Guid.NewGuid().ToString()
        
    let bucketUri =    
        let uriBuilder = UriBuilder("http", "localhost", port)
        uriBuilder.Path <- bucketName
        uriBuilder.Uri
        

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
        
            
    
    let ensureBucketExists (client : AmazonS3Client) (bucketName : string) =
        async {
            use! res = createRequest Get bucketUri |> getResponse
            if res.StatusCode.Equals(404) then                
                client.PutBucket(bucketName) |> ignore                        
        }
        |> Async.RunSynchronously



    [<SetUp>]
    member x.SetUp () =    ()
//        let info = new ProcessStartInfo("fakes3")        
//        info.Arguments <- "-p " + port.ToString() + " -r .fakes3-root"
//        info.UseShellExecute <- true
//        
//        p.StartInfo <- info
//        p.Start()
//        |> ignore
//
//        Thread.Sleep(500) //to allow fakes3 to open - could also poll here


    [<TearDown>]
    member x.TearDown () = ()
//        p.CloseMainWindow() |> ignore
//        p.Kill()
//        p.Dispose()
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

        ensureBucketExists client bucketName

        use committer = new S3Committer(client, bucketName)
        
        let virtFiles =
            [
                ("blah", "awdpojpojdwdwd");
                ("blah/blah", "asfafsfsafaff");
                ("blah/blah/blah.blah", "oiuyyewqebbbb");
                ("", "ihoihoh");
            ]
            |> List.map (fun (p, d) -> new VirtFile(p, d))
            
        virtFiles
        |> Seq.iter committer.Commit
                

        virtFiles
        |> Seq.map (fun vf ->
                        async {
                            let uri = Uri(bucketUri, vf.Path)
                            
                            use! resp = createRequest Get uri
                                        |> getResponse

                            Assert.That(resp.StatusCode, Is.EqualTo(200))
                            Assert.That(compareStreams(vf.Data, resp.Body))
                        })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore

        

