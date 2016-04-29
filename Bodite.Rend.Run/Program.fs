module Program

open BoditeRender
open System.IO
open System.Security.AccessControl
open Amazon.S3


//
//type EmptyCommitMap () =
//    inherit CommitMap ()
//    override x.GetHashFor key = ""


type CommitTransaction (repo : FileRepo) =

    member this.Yield x : list<VirtFile> = []

//    member this.Bind(m, f) =
//                List.collect f m
//
//    member this.Zero m = 
//                //commit all here
//                m
                    
    member __.Quote() = ()
    member __.Run(q) = q

    [<CustomOperation("commit")>]
    member this.Commit (files, vf) = List.Cons(vf, files)




[<EntryPoint>]
let main argv =     
    let templatePath = argv.[0]
    //let outputPath = argv.[1]
        
    use s3Client =     
        let s3Config = new AmazonS3Config()
        s3Config.ServiceURL <- "http://localhost:9988"
        s3Config.UseHttp <- true
        s3Config.ReadEntireResponse <- true
        s3Config.ForcePathStyle <- true
        
        let creds = Amazon.Runtime.BasicAWSCredentials("", "")
        
        new AmazonS3Client(creds, s3Config)
        

    use templateLoader = new FSLoader(templatePath)
    use repo = new S3Repo(s3Client, "bodite")

//    use committer = new S3Committer(s3Client, "bodite")
                    
    CouchDbLoader.loadDbModel "http://localhost:5984/bb"
    |> Hydrate.hydrateModel
    |> (fun m ->                        
            let pages = m |> Pages.buildPages

            let pageReg = pages |> Pages.buildPageRegistry

            let ctx = RenderContext(m, (fun objs -> objs 
                                                     |> Seq.map (fun o -> PageKey(o))
                                                     |> Set.ofSeq
                                                     |> pageReg.TryFind)
                                                     )   
            CommitTransaction(repo) {                                               
                        printfn "213"

                        commit (new VirtFile("", ""))
                
                        commit (new VirtFile("", ""))
                
                        commit (new VirtFile("", ""))
                
                        commit (new VirtFile("", ""))                
                }
                |> (fun q -> ())
                

    //                do pages
    //                    |> Renderer(templateLoader, ctx).renderPages     
    //                    |> iter (fun p -> ())
                
                


//            let commit = CommitTransaction(null)
//
//            commit {
//                let! x = [13]
//                
//            }
                         

            //committer should check commitMap
            //encapsulation away from here - keeps this clear



//                                          
//            let transaction =
//                pages
//                |> Renderer(templateLoader, ctx).renderPages
//                |> Seq.fold 
//                    (fun (t : CommitTransaction<EmptyCommitMap>) vf -> t.Add vf) 
//                    (CommitTransaction(committer, fun () -> EmptyCommitMap()))
//            
//            transaction.Complete()
            ) 
        
    printfn "Rendered to S3"

    0
