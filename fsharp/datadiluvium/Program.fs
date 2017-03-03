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


    open PostgresqlGenerator
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

        // As an example, here is some generated SQL:
        let sql = PostgresqlGenerator.generateData {
            RequestedData=
                [
                {
                    TableName="places"
                    Columns=
                        [
                        { Name="street_num"; Source=Number }
                        { Name="street"; Source=Street("en") }
                        { Name="city"; Source=City("en") }
                        { Name="country"; Source=Country("en") }
                        ]
                }, 10
                {
                    TableName="sofware_releases"
                    Columns=
                        [
                        { Name="version"; Source=Number }
                        { Name="code_name"; Source=City("en") }
                        ]
                }, 25
                ]
            }
        printfn "Generated SQL: \n\n%s" sql
        
        // Pass our web part to our server.
        WebApi.startServer WebApi.getSingleItemWebPart

        0 // return an integer exit code
