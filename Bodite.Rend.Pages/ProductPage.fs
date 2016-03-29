namespace BoditeRender


type ProductPage (locale: Locale, prod: Product, cat: Category) = 
    inherit Page([cat, prod, locale])
    
    override Page.Path = "product/" + prod.Key
    override Page.Title = defaultArg (prod.Name.get locale) ""
    
    member val Locale = locale
    member val Product = prod
    member val Category = cat
    