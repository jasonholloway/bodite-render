namespace BoditeRender
//
//open NUnit.Framework
//
//
//type SpyCommitter () =
//    inherit FileCommitter()
//
//    let m = Map([])
//    
//
//    override x.Commit vf =
//        ()
//
//    override x.Dispose () =
//        ()
//
////definitely needs to be a CommitTransaction, which will mediate between the committer and the VirtFiles
////we assume that only one transaction will be occuring at a time
//
////how will this be hosted? a heroku worker, written in node, will invoke it as a single-shot process
////on every refresh request a gulp script will be run
//
////so: don't worry about concurrency: only one will be run at once by the runner
//
//
//
//
//
//[<TestFixture>]
//type DebouncerTests () =
//
//    [<Test>]
//    [<Ignore("To do...")>]
//    member x.``wraps calls to commit, eagerly loading each vf to calc its hash`` () =
//        //...
//        ()
//
//
//    [<Test>]
//    member x.``only commits when file hash is new`` () =        
//        use committer = new SpyCommitter()
//        use debouncer = new Debouncer(committer)
//
//        use vf1 = new VirtFile("lkjldiuu", "fjpojpfiuua")
//        use vf2 = new VirtFile("oipoanlk", "p[pankwon")
//        
//        [vf1; vf2]
//        |> Seq.iter debouncer.Commit 
//
//        //both should have been committed
//
//        //debouncer needs to 
//
//
//
//
//
//        //...
//        ()
