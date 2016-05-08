namespace BoditeRender

[<AbstractClass>]
type BoditeModel () =
    inherit Model ()

    abstract member Locales : Set<Locale>
    abstract member Products : Set<Product>
    abstract member Categories : Set<Category>

