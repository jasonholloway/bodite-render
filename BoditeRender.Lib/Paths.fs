namespace BoditeRender

module Paths =


    let buildPather pages =
        0



    let resolvePath (m: 'M) =
        match box m with

        | :? Product ->
            let p = box m :?> Product
            "product/" + "" //p.MachineName 

        | :? Category ->
            let c = box m :?> Category
            "category/" + c.Key

        | _ -> 
            failwith "Can't resolve path for mystery object"




//
// products are to have multiple approaches
// one per category: a ProductView - or, indeed, ProductPage
// but the category page isn't to know product pages, or is it? Well, yes. Each ProductPage is a view.
// but we want each 
//
//