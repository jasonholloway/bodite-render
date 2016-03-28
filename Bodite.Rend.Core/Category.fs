namespace BoditeRender


type Category = {
    Key : string
    Name : LocaleString
    Description : LocaleString
    Children : Category list
    Products : Product list   
}


