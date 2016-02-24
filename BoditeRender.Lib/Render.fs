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




type RenderContext (model: Model, getPage: obj seq -> Page option) = 
    member x.Model = model
    member x.GetPage = getPage

   



type IBoditeTemplate =
    abstract member Context : RenderContext with get, set
    
   
type BoditeTemplate<'P when 'P :> Page> () as x =
    inherit HtmlTemplateBase<'P>()

    let mutable context = None

    member x.Context with get() = context.Value
    
    interface IBoditeTemplate with
        member x.Context with get() = context.Value
                          and set v = context <- Some v




type Renderer (templateResolver: string -> string, ctx: RenderContext) =

    let templateMgr = DelegateTemplateManager(System.Func<_,_>(templateResolver)) :> ITemplateManager
    

    let renderService = 
                FluentTemplateServiceConfiguration(fun x -> 
                                                        x.ManageUsing(templateMgr)
                                                         .ActivateUsing(fun c -> 
                                                                            match c.Loader.CreateInstance c.TemplateType with
                                                                            | :? IBoditeTemplate as t -> 
                                                                                    t.Context <- ctx
                                                                                    t :?> ITemplate
                                                                            | t -> 
                                                                                    t
                                                                            )                                                      
                                                        |> ignore)
                |> RazorEngineService.Create
            
                
    member x.renderPage (p: Page) =    
        let data = new LazyStream (fun _ ->
                                        let str = new MemoryStream()
                                        let writer = new StreamWriter(str)
                                        renderService.RunCompile(p.GetType().Name + ".cshtml", writer, p.GetType(), p)
                                        writer.Flush()
                                        str.Position <- 0L
                                        str :> Stream
                                        )
                                        
        [new VirtFile(path=p.Path, data=data)]



    
    member x.renderPages (pages: seq<Page>) =
        pages 
        |> Seq.collect (fun p -> p |> x.renderPage) 
        |> Seq.toList
