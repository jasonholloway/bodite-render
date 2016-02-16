module FSResolver

open System.IO

let create dirPath =
    fun relPath -> 
        use file = File.OpenRead (Path.Combine(dirPath, relPath + ".cshtml"))
        use reader = new StreamReader(file)
        reader.ReadToEnd()