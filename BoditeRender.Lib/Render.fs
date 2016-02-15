namespace BoditeRender

open System
open System.IO
open RazorEngine.Configuration
open RazorEngine.Templating



type VirtFile (path, data: Stream) =
    new (path, data: string) =
        let str = new MemoryStream(Text.ASCIIEncoding.UTF8.GetBytes(data))
        new VirtFile(path, str)

    member val Path: string = path
    member val Data: Stream = data

    interface IDisposable with
        member x.Dispose () =
            x.Data.Dispose()



type LazyStream (fac : unit -> Stream) =
    inherit Stream()
    
    let lzStr = Lazy.Create fac
                                
    override x.get_CanRead () = true
    override x.get_CanSeek () = true
    override x.get_CanWrite () = false
    override x.get_Length () = lzStr.Value.Length
    override x.get_Position () = lzStr.Value.Position
    override x.set_Position (v) = lzStr.Value.Position <- v
    override x.Flush () = lzStr.Value.Flush()
    override x.Seek (offset, origin) = lzStr.Value.Seek(offset, origin)
    override x.SetLength (l) = raise (System.NotSupportedException())
    override x.Read (buffer, offset, count) = lzStr.Value.Read(buffer, offset, count)
    override x.Write (buffer, offset, count) = raise (System.NotSupportedException())



type Renderer (templateResolver: string -> string) =

    let templateMgr = DelegateTemplateManager(System.Func<_,_>(templateResolver)) :> ITemplateManager
    
    let renderService = 
                TemplateServiceConfiguration(TemplateManager=templateMgr) 
                |> RazorEngineService.Create
            
                
    member x.renderPage (p: Page) =    
        let data = new LazyStream (fun _ ->
                                        let str = new MemoryStream()
                                        let writer = new StreamWriter(str)
                                        renderService.RunCompile(p.GetType().Name, writer, p.GetType(), p)
                                        writer.Flush()
                                        str.Position <- 0L
                                        str :> Stream
                                        )

        [new VirtFile(path=p.Path, data=data)]



    
    member x.renderPages (pages: seq<Page>) =
        pages 
        |> Seq.collect (fun p -> p |> x.renderPage) 
        |> Seq.toList

