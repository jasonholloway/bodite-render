namespace Bodite.Rend.Run.Tests

open NUnit.Framework
open System.Diagnostics
open System.Net
open Amazon.S3
open BoditeRender
open System
open System.Threading
open System.IO
open System.Text
open FSharp.Data
open HttpFs
open HttpFs.Client
open Flurl


module Settings =
    let Port = 787
    let BucketName = Guid.NewGuid().ToString()
    let BaseUri = UriBuilder("http", "localhost", Port).Uri 





[<SetUpFixture>]
type S3TestSetup() =

    let p = new Process()

    [<SetUp>]
    member x.SetUp () =
        let info = new ProcessStartInfo("s3rver")
        info.Arguments <- "-p " + Settings.Port.ToString() + " -d .s3rver -i index.html"
        info.UseShellExecute <- true
        
        p.StartInfo <- info
        p.Start()
        |> ignore

        Thread.Sleep(100) //to allow s3rver to open - could also poll here

    [<TearDown>]
    member x.TearDown () =
        p.CloseMainWindow() |> ignore
        p.Kill()
        p.Dispose()
        //remove root folder here!
        //...
      



[<TestFixture>]
type S3Tests() = 
        
    let getS3Client () =
        let s3Config = new AmazonS3Config()
        s3Config.ServiceURL <- Settings.BaseUri.ToString()
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
        
            
    let stream2String (s : Stream) =
        use reader = new StreamReader(s)
        reader.ReadToEnd()

    
    let ensureBucketExists (client : AmazonS3Client) (bucketName : string) =
        async {
            let bucketUri = Uri(Settings.BaseUri, bucketName)
            use! res = createRequest Get bucketUri |> getResponse

            if res.StatusCode.Equals(404) then                
                client.PutBucket(bucketName) |> ignore                        
        }
        |> Async.RunSynchronously



        

    [<Test>]
    member x.``s3rver is available to test`` () =               
        use client = getS3Client()

        let res = client.ListBuckets()
        
        Assert.That(
            res.HttpStatusCode,
            Is.EqualTo(HttpStatusCode.OK))
        ()



    [<Test>]
    member x.``each virtFile committed to correct location`` () =
        use client = getS3Client()

        ensureBucketExists client Settings.BucketName

        use committer = new S3Committer(client, Settings.BucketName)
        
        let files =
            [
                ("blah", "awdpojpojdwdwd");
                ("blah/blah", "asfafsfsafaff");
                ("blah/blah/blah.blah", "oiuyyewqebbbb");
                ("", "ihoihoh");
            ]
            
        files
        |> Seq.map (fun (path, data) -> new VirtFile(path, data))
        |> Seq.iter committer.Commit
            
        files
        |> Seq.map (fun (path, contents) ->
                        async {
                            let uri = Uri(Settings.BaseUri, Url.Combine(Settings.BucketName, path))
                            
                            use! resp = createRequest Get uri
                                        |> getResponse

                            Assert.That(resp.StatusCode, Is.EqualTo(200))
                            Assert.That(resp.Body |> stream2String, Is.EqualTo(contents))
                        })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore




    //what should be here is test of pre-existing file hash provider...