//module Model
namespace BoditeRender


[<AbstractClass>]
type BoditeModel () =
    inherit Model ()

    abstract member Products : Map<string, Product>
    abstract member Categories : Map<string, Category>




//
//type BoditeModel (?products, ?categories) = 
//    inherit Model ()
//    
//    member val Products = (defaultArg products Map.empty<string, Product>)
//    member val Categories = (defaultArg categories Map.empty<string, Category>)
