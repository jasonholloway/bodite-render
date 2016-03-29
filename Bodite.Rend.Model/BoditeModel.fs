//module Model
namespace BoditeRender


type BoditeModel (?products, ?categories) = 
    inherit Model ()
    
    member val Products = (defaultArg products Map.empty<string, Product>)
    member val Categories = (defaultArg categories Map.empty<string, Category>)
