//module Model
namespace BoditeRender


type Model (?products, ?categories) = 
    member val Products = (defaultArg products Seq.empty<Product>) |> Seq.toList
    member val Categories = (defaultArg categories Seq.empty<Category>) |> Seq.toList
