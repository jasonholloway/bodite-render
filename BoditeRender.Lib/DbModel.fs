namespace BoditeRender


[<CLIMutable>]
type DbProduct = {
    Key: string
    Name: Map<string, string>
    Description: Map<string, string>
    CategoryKeys: string list
}


[<CLIMutable>]
type DbCategory = {
    Key: string
    Name: Map<string, string>
//    Description: Map<string, string>
    ChildKeys: string list
}



type DbModel (?categories : DbCategory list, ?products : DbProduct list) =
    member x.Categories = defaultArg categories []
    member x.Products = defaultArg products []
