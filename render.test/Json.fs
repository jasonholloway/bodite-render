﻿#if INTERACTIVE
#r @"Newtonsoft.Json.dll"
#endif

module Json

open Newtonsoft.Json.Linq


type Json =
    | JObj of Json seq
    | JProp of string * Json
    | JArr of Json seq
    | JVal of obj

let (!!) (o: obj) = JVal o

let rec toJson = function
    | JVal v -> new JValue(v) :> JToken
    | JProp(name, (JProp(_) as v)) -> new JProperty(name, new JObject(toJson v)) :> JToken
    | JProp(name, v) -> new JProperty(name, toJson v) :> JToken
    | JArr items -> new JArray(items |> Seq.map toJson) :> JToken
    | JObj props -> new JObject(props |> Seq.map toJson) :> JToken

// Suppose we want to create the following Json object:
// {
//     "id": 123,
//     "props": {
//         "prop": [
//             { "id": 1, v: 123 },
//             { "id": 2, v: 456 }
//         ]
//     }
// }
//
// Then the simplified lightweight Json creation will look like this:
let j =
    JObj [
        JProp("id", !! "123");
        JProp(
            "props", 
            JProp(
                "prop", 
                JArr [
                    JObj [JProp("id", !! 1); JProp("v", !! 123)];
                    JObj [JProp("id", !! 2); JProp("v", !! 456)]
                ]))
    ]



let k = [ ("id", 123), ("ewrrw", 7676) ]






