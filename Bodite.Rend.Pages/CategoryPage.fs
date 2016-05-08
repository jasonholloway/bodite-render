namespace BoditeRender

    
type CategoryPage (locale: Locale, cat: Category) =
    inherit Page([locale; cat])
    
    override Page.Path = "category/" + cat.Key
    override Page.Title = defaultArg (cat.Name.get locale) ""
    
    member val Locale = locale
    member val Category = cat
    