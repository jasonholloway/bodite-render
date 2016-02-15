namespace BoditeRender


module Program =

    [<EntryPoint>]
    let main argv =
    
        //data layer should return datamodel
                
        let model = Hydrator.hydrateModel()

        let pages = Pages.buildPages model


        

        //if debug, should use a different resolver and committer
        //just switch by preprocessor?
        //or switch to library with different runners?
        //switching to library would allow better testing.










        //get products

        //get categories

        //buildpages

        //render to files

            
        0






    

