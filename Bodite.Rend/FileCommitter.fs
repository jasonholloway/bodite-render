namespace BoditeRender

open System
open System.IO



type VirtFile (path, data: byte[]) =

    let r = System.Random()  //to be removed, blatantly

    new (p, s : string) =
        let d = Text.ASCIIEncoding.UTF8.GetBytes(s)
        new VirtFile(p, d)

        

    member val Path: string = path
    member val Data: byte[] = data



    
    member x.GetDataHash () =
        x.Data
        |> DataHasher.MD5Hash

        
    interface IDisposable with
        member x.Dispose () =
//            x.Data.Dispose()
            ()



[<AbstractClass>]
type FileCommitter () as o =

    abstract member Commit : VirtFile -> unit
    abstract member Dispose : unit -> unit
    
    interface IDisposable with
        member x.Dispose() = o.Dispose()


[<AbstractClass>]
type CommitMap () =
    abstract member GetHashFor : string -> string
        


     
type CommitTransaction<'M when 'M :> CommitMap> (committer : FileCommitter, commitMapProvider: unit -> 'M, files : VirtFile list) =    
    new (committer, mapProv) =
        CommitTransaction (committer, mapProv, [])

    member x.Add vf =
        CommitTransaction(committer, commitMapProvider, vf :: files)

    member x.Complete () =
        let commitMap = commitMapProvider()
        
        files
        |> Seq.filter (fun vf -> not ( (commitMap.GetHashFor(vf.Path)).Equals(vf.GetDataHash()) ))
        |> Seq.iter (fun vf -> vf |> committer.Commit)



