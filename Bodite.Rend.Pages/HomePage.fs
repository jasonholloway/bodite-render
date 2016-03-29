namespace BoditeRender

                                                
type HomePage (locale: Locale) =
    inherit Page(["Index", locale])

    override Page.Path = ""
    override Page.Title = "Brigitas Bodite"
    
    member val Locale = locale
    member val FeaturedProducts : List<Product> = List.empty
    