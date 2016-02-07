namespace BoditeRender


type Product = {
    Key: string
    Name: LocaleString
    Description: LocaleString
    MachineName: string
    CategoryKeys: List<string>
}


module Products =

    open FSharp.Data
    open System
    open System.Text.RegularExpressions

    [<Literal>]
    let allProductsUrl = "https://jasonholloway.cloudant.com/bb/_design/bb/_view/all-products   "


    type ProductDbView = JsonProvider<allProductsUrl>

    

    let getAllProducts = 
        ProductDbView.Load(allProductsUrl).Rows        
        |> Seq.map (fun r -> 
                        {
                            Key = Regex.Match(r.Value.Id, "^product[\/-](.+)").Value
                            Name = {
                                    LV = Some r.Value.Name.Lv
                                    RU = r.Value.Name.Ru
                                   }
                            Description = {
                                            LV = None
                                            RU = None
                                          }
                            MachineName = r.Value.MachineName
                            CategoryKeys = r.Value.CategoryKeys |> Seq.map (fun g -> g.ToString()) |> Seq.toList
                        }
                   )
        |> List.ofSeq


