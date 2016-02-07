module PageTests

open NUnit.Framework
open FsUnit

open Products
open Categories
open Pages




[<Test>]
let ``Pages.buildPages returns list of pages`` () =
    let res = Pages.buildPages (new Model())
    res.GetType() |> should equal (List.empty<Page>.GetType())
    

[<Test>]
let ``Pages.buildPages builds one ProductPage per Product * Category`` () =
    new Model(
        products = List.empty
        )
    |> Pages.buildPages
    |> Seq.iter (fun p -> ())


[<Test>]
let ``Pages.buildPages builds one page per Category`` () = 
    failwith "Unimpl"


[<Test>]
let ``Pages.buildPages builds one IndexPage`` () =
    failwith "Unimpl"










