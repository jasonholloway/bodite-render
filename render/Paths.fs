namespace BoditeRender

module Paths =

    let resolvePath (m: 'M) =
        match box m with
        | :? Product ->
            let p = box m :?> Product
            "product/" + p.MachineName 

        | :? Category ->
            let c = box m :?> Category
            "category/" + c.Key

        | _ -> 
            failwith "Can't resolve path for mystery object"