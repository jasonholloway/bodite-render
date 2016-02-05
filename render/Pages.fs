module Pages


type ProductPage<'P, 'C> = {
    path : string;
    title: string;
    product : 'P;
    category: 'C;
}

//should be one page per product*category combination
//plus category field should be filled in here...


let getProductPages =
    Data.getAllProducts
    |> Seq.filter (fun p -> p.MachineName.IsSome)
    |> Seq.map (fun p -> {
                            path = if p.MachineName.IsSome then ("produkti/" + p.MachineName.Value) else "";
                            title = "Brigitas Bodite - Produkti - " + p.Name.Lv;  
                            product = p;
                            category = 13;
                        })