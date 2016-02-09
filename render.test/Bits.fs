module Bits

open System
open System.IO
open NUnit.Framework
open FsUnit


let rnd = Random()

let getRandomSelection count (values: 'V seq) =        
    let rec getPicks c (s: Set<'V>) (r : 'V list) =
        match c with
        | 0 -> 
            r
        | _ ->
            if s.IsEmpty then failwith "Too few values to pick from!"

            let p = (s |> Set.toSeq) |> Seq.nth (rnd.Next(0, s.Count))

            getPicks (c-1) (s.Remove p) (List.Cons(p, r))

    getPicks count (values |> Set.ofSeq) []
    
[<Test>]
let ``getRandomSelection picks unique vals`` () =
    getRandomSelection 100 [0..99]
    |> Seq.distinct
    |> Seq.length |> should equal 100





let stream2String (str: Stream) =
    if str.CanSeek then str.Position <- 0L

    use r = new StreamReader(str)
    r.ReadToEnd()




type TempFile (relPath: string, data: string) =    

    let dirPath = Path.GetTempPath()
    let fullPath = Path.Combine(dirPath, relPath)

    do
        use file = File.CreateText(fullPath)
        file.Write(data)
        file.Flush()
             
    member val DirPath = dirPath
    member val FullPath = fullPath    
                      
    interface System.IDisposable with
        member x.Dispose() =
            File.Delete fullPath