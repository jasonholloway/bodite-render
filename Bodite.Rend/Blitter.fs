namespace BoditeRender

open System
open System.IO


module Blitter =

    let create buffer =
        let rec blit (sOut: Stream) (sIn: Stream) =
            match sIn.Read(buffer, 0, buffer.Length) with
            | 0 -> ()
            | c -> 
                sOut.Write(buffer, 0, c)
                blit sIn sOut
        blit
            
