﻿namespace BoditeRender

open System
open System.IO
open RazorEngine.Configuration
open RazorEngine.Templating

//
//
//type LazyStream (fac : unit -> Stream) =
//    inherit Stream()
//    
//    let lzStr = Lazy.Create fac
//                                
//    override x.get_CanRead () = true
//    override x.get_CanSeek () = true
//    override x.get_CanWrite () = false
//    override x.get_Length () = lzStr.Value.Length
//    override x.get_Position () = lzStr.Value.Position
//    override x.set_Position (v) = lzStr.Value.Position <- v
//    override x.Flush () = lzStr.Value.Flush()
//    override x.Seek (offset, origin) = lzStr.Value.Seek(offset, origin)
//    override x.SetLength (l) = raise (System.NotSupportedException())
//    override x.Read (buffer, offset, count) = lzStr.Value.Read(buffer, offset, count)
//    override x.Write (buffer, offset, count) = raise (System.NotSupportedException())

   
        
type RenderContext<'M when 'M :> Model> (model: 'M, findPage: obj seq -> Page option) = 
    member x.Model = model
    member x.GetPage(keys) = findPage keys

   

type IBoditeTemplate<'M when 'M :> Model> =
    abstract member Context : RenderContext<'M> with get, set
    
   
type BoditeTemplate<'M,'P when 'M :> Model and 'P :> Page> () as x =
    inherit HtmlTemplateBase<'P>()

    let mutable context : RenderContext<'M> option = None

    member x.Context with get() = context.Value
    member x.Page = x.Model
    member x.Site = x.Context.Model
    member x.FindPage([<ParamArray>]keys: obj[]) = x.Context.GetPage(keys).Value
    member x.FindLocalizedPage([<ParamArray>]keys: obj[]) = x.FindPage(seq { yield x.Page.Locale :> obj; yield! keys } |> Seq.toArray)

    member x.T(ls : LocaleString) = 
        match ls.Map.TryFind(x.Page.Locale) with
        | Some v -> v
        | None ->
            match ls.Map.TryFind(Locales.Default) with
            | Some v -> v
            | None -> ""

    member x.T([<ParamArray>]rs : string[]) =
        Locales.All
        |> Seq.zip rs
        |> Seq.find (fun (_, l) -> l = x.Page.Locale)
        |> (fun (s, _) -> s)



    interface IBoditeTemplate<'M> with
        member x.Context with get() = context.Value
                          and set v = context <- Some v



type Activator<'M when 'M :> Model> (renderContext) =
    interface IActivator with
        member x.CreateInstance (c) =                                      
            match c.Loader.CreateInstance c.TemplateType with
            | :? IBoditeTemplate<'M> as t -> 
                    t.Context <- renderContext
                    t :?> ITemplate
            | t -> 
                    t
        
        

type Renderer<'M when 'M :> Model> (loader: TemplateLoader, renderContext) =

//    let templateMgr = DelegateTemplateManager(System.Func<_,_>(loader.Load)) :> ITemplateManager
    
    let renderService =
        let c = TemplateServiceConfiguration()
        c.TemplateManager <- DelegateTemplateManager(System.Func<_,_>(loader.Load))
        c.Activator <- Activator<'M>(renderContext)
        c.Language <- RazorEngine.Language.CSharp
        c.Debug <- true

        c |> RazorEngineService.Create
                    
                
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
