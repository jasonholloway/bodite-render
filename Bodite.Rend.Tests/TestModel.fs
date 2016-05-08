namespace BoditeRender


type TestModel (locales: Set<Locale>, products: Set<Product>, categories: Set<Category>) =
    inherit BoditeModel ()

    override x.Locales = locales
    override x.Products = products
    override x.Categories = categories


type EmptyTestModel () =
    inherit TestModel (Set.empty, Set.empty, Set.empty)