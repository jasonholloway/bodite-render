module Locale

open BoditeRender
open NUnit.Framework
open FsUnit
open System



[<TestFixture>]
type ``Locale`` () =
    
    [<Test>]
    member x.``implements IComparable`` () =
        typeof<IComparable>.IsAssignableFrom(typeof<BoditeRender.Locale>)
        |> should equal true


    [<Test>]
    member x.``code determines equality`` () =
        (BoditeRender.Locale "A").Equals(BoditeRender.Locale "A")
        |> should equal true




[<TestFixture>]
type ``LocaleString`` () =

    [<Test>]
    member x.``returns locale value if provided`` () =    
        let s = BoditeRender.LocaleString [
                                            (Locales.LV, "Nope!")
                                            (Locales.RU, "Hello")
                                         ]
    
        s.getString Locales.RU
        |> should equal "Hello"



    [<Test>]
    member x.``returns default locale value if requested not provided`` () =    
        let s = BoditeRender.LocaleString [
                                            (Locales.LV, "Hello")
                                         ]
    
        s.getString Locales.RU
        |> should equal "Hello"

        
        
    [<Test>]
    member x.``returns empty string if nothing provided`` () =    
        let s = BoditeRender.LocaleString []

        s.getString Locales.LV 
        |> should equal ""





//        
//    [<Test>]
//    member x.``throws error if nothing provided`` () =    
//        let s = BoditeRender.LocaleString []
//
//        (fun () -> s.get Locale.LV |> ignore) |> should throw typeof<System.Exception>








