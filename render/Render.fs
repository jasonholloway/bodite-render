namespace BoditeRender

open System.IO

type VirtFile = {
    Path: string
    Data: Stream
}


module Render =
    
    let renderPage (p: Page) =
        [{ Path= ""; Data= new MemoryStream() }]



    
    let renderPages (pages: seq<Page>) =
        pages 
        |> Seq.collect (fun p -> p |> renderPage) 
        |> Seq.toList



