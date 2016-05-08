namespace BoditeRender

open System
open System.IO

type VirtFile (path, data: byte[]) =
    new (p, s : string) =
        let d = Text.ASCIIEncoding.UTF8.GetBytes(s)
        new VirtFile(p, d)
    
    member val Path: string = path
    member val Data: byte[] = data
        
    member x.GetDataHash () =
        x.Data
        |> DataHasher.MD5Hash
                


[<AbstractClass>]
type FileRepo () as o =
    abstract member Read : string -> VirtFile
    abstract member Write : VirtFile -> unit
    abstract member Remove : string -> unit

    abstract member Dispose : unit -> unit
    
    interface IDisposable with
        member x.Dispose() = o.Dispose()

               

[<AbstractClass>]
type CommitMap () =
    abstract member TryRegister : VirtFile -> bool
    abstract member Paths : string seq
    abstract member Remove : string -> unit



type FileRepoCommitMap (repo : FileRepo) =
    inherit CommitMap()

    override x.TryRegister vf =
        true

    override x.Paths =
        [""] |> Seq.ofList

    override x.Remove path =
        ()
    



    // Problem of garbage collection - how to remove old files from storage? 
    // Every time we course through the entire model (that is, every time we render from scratch)
    // we should cross-compare against the existing commit map.
    //
    // So, the commit map can't just be hidden behind a boolean test: we need to be able to extract 
    // entries from it. If, after a render, orphanes commits are found, we should delete them via the repo.
    //
    


type FilteringFileRepo (innerRepo : FileRepo, commitMap : CommitMap) =
    inherit FileRepo()

    override x.Read path =
        innerRepo.Read path

    override x.Write vf =
        if commitMap.TryRegister vf then innerRepo.Write vf
        
    override x.Remove path =
        ()

    override x.Dispose () =
        ()

