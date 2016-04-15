namespace BoditeRender

open System.IO


type Debouncer (committer : FileCommitter) =
    inherit FileCommitter ()
    
    //NOTE PLEASE! DEBOUNCINGCOMMITTER GOES STALE QUICKLY
    //Recreate on every transactional batch of commits

    
    //before every commit, check against table   


    override x.Commit (vf : VirtFile) =
        vf |> committer.Commit



    override x.Dispose () =
        committer.Dispose()
        

