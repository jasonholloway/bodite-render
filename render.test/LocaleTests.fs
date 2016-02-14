module Locale

open BoditeRender
open NUnit.Framework
open FsUnit


[<TestFixture>]
type ``LocaleString`` () =


    [<Test>]
    member x.``returns locale value if provided`` () =    
        let s = BoditeRender.LocaleString [
                                            (Locale.LV, "Nope!")
                                            (Locale.RU, "Hello")
                                         ]
    
        s.get Locale.RU
        |> should equal (Some "Hello")



    [<Test>]
    member x.``returns default locale value if requested not provided`` () =    
        let s = BoditeRender.LocaleString [
                                            (Locale.LV, "Hello")
                                         ]
    
        s.get Locale.RU
        |> should equal (Some "Hello")

        
        
    [<Test>]
    member x.``returns None if nothing provided`` () =    
        let s = BoditeRender.LocaleString []

        s.get Locale.LV 
        |> should equal None





//        
//    [<Test>]
//    member x.``throws error if nothing provided`` () =    
//        let s = BoditeRender.LocaleString []
//
//        (fun () -> s.get Locale.LV |> ignore) |> should throw typeof<System.Exception>








