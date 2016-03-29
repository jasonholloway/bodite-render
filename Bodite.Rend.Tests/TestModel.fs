namespace BoditeRender


type TestModel (products : Map<string, Product>, categories : Map<string, Category>) =
    inherit BoditeModel ()

    override x.Products = products
    override x.Categories = categories


type EmptyTestModel () =
    inherit TestModel (Map.empty<string, Product>, Map.empty<string, Category>)