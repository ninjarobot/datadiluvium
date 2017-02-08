namespace DataDiluvium

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful
open Suave.Writers

module WebApi = 
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
                        lang |> l2f |> FakeProvider.getRandomSingleItem |> Seq.head
                    return! OK item ctx
                | _ -> return! BAD_REQUEST "Only 'en' or 'ru' language are supported." ctx
            }


