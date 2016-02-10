//module Model
namespace BoditeRender


type LocaleString = {
    LV : Option<string>;
    RU : Option<string>;
}


type Product = {
    Key: string
    Name: LocaleString
    Description: LocaleString
    MachineName: string
    CategoryKeys: List<string>
}


type Category = {
    Key : string
    Name : LocaleString
    Description : LocaleString
    Children : List<Category> 
    Products : List<Product>   
}




type Model (?products, ?categories) = 
    member val Products = (defaultArg products Map.empty<string, Product>)
    member val Categories = (defaultArg categories Map.empty<string, Category>)


