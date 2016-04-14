namespace BoditeRender

open System
open System.IO
open RazorEngine.Configuration
open RazorEngine.Templating



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

   
        
type RenderContext<'M when 'M :> Model> (model: 'M, getPage: obj seq -> Page option) = 
    member x.Model = model
    member x.GetPage = getPage

   

type IBoditeTemplate<'M when 'M :> Model> =
    abstract member Context : RenderContext<'M> with get, set
    
   
type BoditeTemplate<'M,'P when 'M :> Model and 'P :> Page> () as x =
    inherit HtmlTemplateBase<'P>()

    let mutable context = None

    member x.Context with get() = context.Value
    
    interface IBoditeTemplate<'M> with
        member x.Context with get() = context.Value
                          and set v = context <- Some v




type Renderer<'M when 'M :> Model> (loader: TemplateLoader, ctx: RenderContext<'M>) =

    let templateMgr = DelegateTemplateManager(System.Func<_,_>(loader.Load)) :> ITemplateManager
    

    let renderService = 
                FluentTemplateServiceConfiguration(fun x -> 
                                                        x.ManageUsing(templateMgr)
                                                         .ActivateUsing(fun c -> 
                                                                            match c.Loader.CreateInstance c.TemplateType with
                                                                            | :? IBoditeTemplate<'M> as t -> 
                                                                                    t.Context <- ctx
                                                                                    t :?> ITemplate
                                                                            | t -> 
                                                                                    t
                                                                            )                                                      
                                                        |> ignore)
                |> RazorEngineService.Create
            
                
    member x.renderPage (p: Page) =    
        use str = new MemoryStream()
        use writer = new StreamWriter(str)

        renderService.RunCompile(
                        p.GetType().Name + ".cshtml",
                        writer,
                        p.GetType(),
                        p)

        writer.Flush()

        let data = str.ToArray()
                                
        [new VirtFile(path=p.Path, data=data)]



    
    member x.renderPages (pages: seq<Page>) =
        pages 
        |> Seq.collect (fun p -> p |> x.renderPage) 
        |> Seq.toList
