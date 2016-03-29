namespace BoditeRender

[<AbstractClass>]
type BoditeModel () =
    inherit Model ()

    abstract member Products : Map<string, Product>
    abstract member Categories : Map<string, Category>

