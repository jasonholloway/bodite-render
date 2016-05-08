namespace BoditeRender

    
type CategoryPage (locale: Locale, cat: Category) =
    inherit Page(locale, [cat])
    
    override Page.Path = "category/" + cat.Key

    member val Category = cat
    