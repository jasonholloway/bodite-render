//module Model
namespace BoditeRender


type Model (?products, ?categories) = 
    member val Products = (defaultArg products Map.empty<string, Product>)
    member val Categories = (defaultArg categories Map.empty<string, Category>)
