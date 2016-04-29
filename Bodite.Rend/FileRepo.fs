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



//but if committer is to filter by commit map, what if commitmap itself requires committing (which it will, probably)
//almost as if committer has to consist of an inner committer, as if the filtered committer were a wrapper...
//
[<AbstractClass>]
type FileRepo () as o =
    abstract member Read : string -> VirtFile
    abstract member Write : VirtFile -> unit
    abstract member Dispose : unit -> unit
    
    interface IDisposable with
        member x.Dispose() = o.Dispose()

               

[<AbstractClass>]
type CommitMap () =
    abstract member GetHashFor : string -> string
        

        //a new committer should be used each time
        //wasteful: why would you have to reestablish the client etc
        //what's needed is - a CommitTransaction

        //the transaction will gather state, then dispatch it
        //this is all a bit monadic computation expression, isn't it?

        //and there's my answer... and an excuse to learn how to do that.




type FilteredFileRepo (innerRepo : FileRepo, commitMap : CommitMap) =
    inherit FileRepo()

    override x.Read path =
        innerRepo.Read path

    override x.Write vf =
        //filter here
        innerRepo.Write vf

    override x.Dispose () =
        ()




//
//
//     
//type CommitTransaction<'M when 'M :> CommitMap> (committer : FileCommitter, commitMapProvider: unit -> 'M, files : VirtFile list) =    
//    new (committer, mapProv) =
//        CommitTransaction (committer, mapProv, [])
//
//    member x.Add vf =
//        CommitTransaction(committer, commitMapProvider, vf :: files)
//
//    member x.Complete () =
//        let commitMap = commitMapProvider()
//        
//        files
//        |> Seq.filter (fun vf -> not ( (commitMap.GetHashFor(vf.Path)).Equals(vf.GetDataHash()) ))
//        |> Seq.iter (fun vf -> vf |> committer.Commit)
//
//
//[<AbstractClass>]
//type CommitFilter () =
//    abstract member Filter : VirtFile seq -> VirtFile seq

