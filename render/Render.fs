namespace BoditeRender

open System.IO
open RazorEngine.Configuration
open RazorEngine.Templating



type VirtFile = {
    Path: string
    Data: Stream
}


type LazyStream (create : unit -> Stream) =
    inherit Stream()
    
    let lzStr = Lazy.Create create
                                
    override Stream.get_CanRead () = true
    override Stream.get_CanSeek () = true
    override Stream.get_CanWrite () = false
    override Stream.get_Length () = lzStr.Value.Length
    override Stream.get_Position () = lzStr.Value.Position
    override Stream.set_Position (v) = lzStr.Value.Position <- v
    override Stream.Flush () = lzStr.Value.Flush()
    override Stream.Seek (offset, origin) = lzStr.Value.Seek(offset, origin)
    override Stream.SetLength (l) = raise (System.NotSupportedException())
    override Stream.Read (buffer, offset, count) = lzStr.Value.Read(buffer, offset, count)
    override Stream.Write (buffer, offset, count) = raise (System.NotSupportedException())



type Renderer (templateResolver: string -> string) =

    let templateMgr = DelegateTemplateManager(System.Func<_,_>(templateResolver)) :> ITemplateManager
    
    let renderService = 
                TemplateServiceConfiguration(TemplateManager=templateMgr) 
                |> RazorEngineService.Create
            
                
    member x.renderPage (p: Page) =    
        let data = new LazyStream 
                        (fun _ ->
                            let str = new MemoryStream()
                            let writer = new StreamWriter(str)
                            renderService.RunCompile(p.GetType().Name, writer, p.GetType(), p)
                            writer.Flush()
                            str.Position <- 0L
                            str :> Stream
                            )

        [{ Path=p.Path; Data=data }]



    
    member x.renderPages (pages: seq<Page>) =
        pages 
        |> Seq.collect (fun p -> p |> x.renderPage) 
        |> Seq.toList



