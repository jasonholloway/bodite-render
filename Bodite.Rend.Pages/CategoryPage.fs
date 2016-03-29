namespace BoditeRender

    
type CategoryPage (locale: Locale, cat: Category) =
    inherit Page([cat, locale])
    
    override Page.Path = "category/" + cat.Key
    override Page.Title = defaultArg (cat.Name.get locale) ""
    
    member val Locale = locale
    member val Category = cat
    