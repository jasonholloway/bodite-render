//module Model
namespace BoditeRender



type Product = {
    Key: string
    Name: LocaleString
    Description: LocaleString
    CategoryKeys: List<string>
}


type Category = {
    Key : string
    Name : LocaleString
    Description : LocaleString
    Children : Category list
    Products : Product list   
}




// Finally: a map of sets





type Model (?products, ?categories) = 
    member val Products = (defaultArg products Map.empty<string, Product>)
    member val Categories = (defaultArg categories Map.empty<string, Category>)
