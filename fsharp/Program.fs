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
    async {
        use! res = url |> Uri |> httpClient.GetAsync |> Async.AwaitTask
        use! s = res.Content.ReadAsStreamAsync () |> Async.AwaitTask
        return
            seq {
                use sr = new System.IO.StreamReader (s)
                while not sr.EndOfStream do
                    yield sr.ReadLine ()
            }
            |> Seq.toArray
    }

/// Random number generator - we just need one of these for our application.
let private rng = System.Random ()

/// Retrieves the data from the url and returns a generator that picks on of the lines.
let getRandomLines url =
    let lines = getLines url |> Async.RunSynchronously
    let len = lines.Length
    seq {
        yield lines.[rng.Next(len)]
    }

/// Cache for the randomized sequence generated from data at a URI.
let cache = System.Collections.Concurrent.ConcurrentDictionary<string, string seq>()

let baseUri = "https://raw.githubusercontent.com/icrowley/fake/master/data/"

/// Language code for the fake data.
type Language = string

/// The type of fake data to return.
type Fake =
    | City of Language
    | Country of Language
    | Continent of Language
    | Street of Language
    | Gender of Language
    | Currency of Language

/// Returns a single item randomly the source for each type of fake data.
let getRandomSingleItem fake =
    let uri = 
        match fake with 
        | City language -> sprintf "%s%s/cities" baseUri language
        | Country language -> sprintf "%s%s/countries" baseUri language
        | Continent language -> sprintf "%s%s/continents" baseUri language
        | Street language -> sprintf "%s%s/streets" baseUri language
        | Gender language -> sprintf "%s%s/genders" baseUri language
        | Currency language -> sprintf "%s%s/currencies" baseUri language
    cache.GetOrAdd (uri, Func<string, string seq> (getRandomLines))

/// Start the server, passing it a web part, basically an HTTP handler function.
let startServer getSingleByLanguage =

    let app = 
        choose
            [ GET >=> choose
                [ 
                    pathScan "/random/city/%s" (getSingleByLanguage City) >=> setMimeType "text/plain"
                    pathScan "/random/country/%s" (getSingleByLanguage Country) >=> setMimeType "text/plain"
                    pathScan "/random/continent/%s" (getSingleByLanguage Continent)  >=> setMimeType "text/plain"
                    pathScan "/random/street/%s" (getSingleByLanguage Street) >=> setMimeType "text/plain"
                    pathScan "/random/gender/%s" (getSingleByLanguage Gender) >=> setMimeType "text/plain"
                    pathScan "/random/currency/%s" (getSingleByLanguage Currency) >=> setMimeType "text/plain"
                ]
            ]

    let port = System.Environment.GetEnvironmentVariable "LISTEN_PORT" |> function
               | null | "" -> "8080"
               | s -> s
    let address = System.Net.IPAddress.Any
    startWebServer { defaultConfig with hideHeader=true; bindings = [HttpBinding.create HTTP address (Sockets.Port.Parse port)] } app

/// The Suave web part - an HTTP handler.  Deals with input checks and the HTTP response codes.
let getSingleItemWebPart (l2f:Language->Fake) (lang:string) = 
    fun (ctx:HttpContext) ->
        async {
            match lang with 
            | "en" | "ru" as lang ->
                let item =
                    lang |> l2f |> getRandomSingleItem |> Seq.head
                return! OK item ctx
            | _ -> return! BAD_REQUEST "Only 'en' or 'ru' language are supported." ctx
        }


[<EntryPoint>]
let main argv = 
    { Title="hello"; Schema="world" } 
    |> JsonConvert.SerializeObject
    |> printfn "%s"
    if File.Exists "../deluge.json" then
        getPages "../deluge.json" |> Array.iter (fun p -> printfn "%A" p)
    "ru" |> City |> getRandomSingleItem
    |> Seq.head
    |> printfn "%s"

    // Pass our web part to our server.
    startServer getSingleItemWebPart

    0 // return an integer exit code
