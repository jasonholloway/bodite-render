namespace BoditeRender

open System.IO


type FSLoader (dirPath : string) =
    inherit TemplateLoader()

    override x.Load (path : string) =
        use file = File.OpenRead (Path.Combine(dirPath, path))
        use reader = new StreamReader(file)
        reader.ReadToEnd()
        

    override x.Dispose () =
        ()