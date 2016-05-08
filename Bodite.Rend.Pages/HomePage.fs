namespace BoditeRender

                                                
type HomePage (locale: Locale) =
    inherit Page(locale, ["Index"])

    override Page.Path = ""
    
    member val FeaturedProducts : List<Product> = List.empty
    