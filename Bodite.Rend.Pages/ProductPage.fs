namespace BoditeRender


type ProductPage (locale: Locale, prod: Product, cat: Category) = 
    inherit Page(locale, [cat; prod])
    
    override Page.Path = "product/" + prod.Key

    member val Product = prod
    member val Category = cat
    