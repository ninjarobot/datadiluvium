// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Net.Http
open Newtonsoft.Json
open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful
open Suave.Writers

type Page = {
    Title : string
    Schema : string
}

let getPages path = 
    path 
    |> System.IO.File.ReadAllText
    |> JsonConvert.DeserializeObject<Page array>


// Single instance of HttpClient will be created on first use.
let httpClient = 
    let handler = new HttpClientHandler ()
    new HttpClient (handler)

// Get all the data at the specified URL and return as a sequence of lines.
let getLines url = 
    let readLines lines =
        seq {
            use sr = new System.IO.StreamReader(lines)
            while not sr.EndOfStream do
                yield sr.ReadLine ()
        }
    async {
        let! res = url |> Uri |> httpClient.GetAsync |> Async.AwaitTask
        let! s = res.Content.ReadAsStreamAsync () |> Async.AwaitTask
        return readLines s
    }


/// Random number generator - we just need one of these for our application.
let private rng = System.Random ()

let getCityData language = 
    async {
        return!
            sprintf "https://raw.githubusercontent.com/icrowley/fake/master/data/%s/cities" language 
            |> getLines
    }

/// Start the server, passing it a web part, basically an HTTP handler function.
let startServer getACityByLanguage =

    let app = 
        choose
            [ GET >=> choose
                [ pathScan "/random/city/%s" getACityByLanguage >=> setMimeType "text/plain" ]
            ]

    let port = "8080"
    let address = System.Net.IPAddress.Any
    startWebServer { defaultConfig with hideHeader=true; bindings = [HttpBinding.create HTTP address (Sockets.Port.Parse port)] } app

/// Function to get a random city.  Needs to cache, but works.
let getRandomCity lang = 
    async {
        let! cities = getCityData lang
        let arr = cities |> Seq.toArray
        let len = arr.Length
        return arr.[rng.Next len]
    }

/// The Suave web part - an HTTP handler.  Deals with input checks and the HTTP response codes.
let getACityWebPart lang = 
    fun (ctx:HttpContext) ->
        async {
            match lang with 
            | "en" | "ru" as lang ->
                let! city = getRandomCity lang
                return! OK city ctx
            | _ -> return! BAD_REQUEST "Only 'en' or 'ru' language are supported." ctx
        }


[<EntryPoint>]
let main argv = 
    { Title="hello"; Schema="world" } 
    |> JsonConvert.SerializeObject
    |> printfn "%s"
    getPages "../deluge.json" |> Array.iter (fun p -> printfn "%A" p)
    async {
        let! russianCities = getCityData "ru"
        russianCities |> Seq.head |> printfn "%s"
    } |> Async.RunSynchronously

    // Pass our web part to our server.
    startServer getACityWebPart

    0 // return an integer exit code
