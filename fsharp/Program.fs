namespace DataDiluvium

open System
open System.IO
open Newtonsoft.Json


module Program = 

    type Page = {
        Title : string
        Schema : string
    }

    let getPages path = 
        path 
        |> System.IO.File.ReadAllText
        |> JsonConvert.DeserializeObject<Page array>


    /// Start the server, passing it a web part, basically an HTTP handler function.
    [<EntryPoint>]
    let main argv = 
        { Title="hello"; Schema="world" } 
        |> JsonConvert.SerializeObject
        |> printfn "%s"
        if File.Exists "../deluge.json" then
            getPages "../deluge.json" |> Array.iter (fun p -> printfn "%A" p)
        "ru" |> City |> FakeProvider.getRandomSingleItem
        |> Seq.head
        |> printfn "%s"

        // Pass our web part to our server.
        WebApi.startServer WebApi.getSingleItemWebPart

        0 // return an integer exit code
