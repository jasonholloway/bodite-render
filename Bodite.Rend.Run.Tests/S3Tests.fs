﻿namespace Bodite.Rend.Run.Tests

open NUnit.Framework
open System.Diagnostics
open System.Net
open Amazon.S3

[<TestFixture>]
type S3Tests() = 

    let port = 9583
    let p = new Process()

    [<SetUp>]
    member x.SetUp () =    
        let info = new ProcessStartInfo("fakes3")        
        info.Arguments <- "-p " + port.ToString() + " -r .fakes3-root"
        info.UseShellExecute <- true
        
        p.StartInfo <- info
        p.Start()
        |> ignore


    [<TearDown>]
    member x.TearDown () =
        p.CloseMainWindow() |> ignore
        p.Kill()
        p.Dispose()
        //remove root folder here!
        //...



    [<Test>]
    member x.``fakes3 is available to test`` () =
               
        use client =     
            let s3Config = new AmazonS3Config()
            s3Config.ServiceURL <- "http://localhost:" + port.ToString()
            s3Config.UseHttp <- true
            s3Config.ReadEntireResponse <- true
            s3Config.ForcePathStyle <- true
        
            let creds = Amazon.Runtime.BasicAWSCredentials("", "")
        
            new AmazonS3Client(creds, s3Config)

        let res = client.ListBuckets()
        
        Assert.That(
            res.HttpStatusCode,
            Is.EqualTo(HttpStatusCode.OK))
        ()


