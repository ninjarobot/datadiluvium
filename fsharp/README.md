How this was implemented
------------------------

With .NET Core installed using [current GA (1.1 SDK)](https://www.microsoft.com/net/download/core#/current), VSCode current GA, and IonIDE extention to VS Code.

mkdir fsharp
dotnet new --lang=fsharp
code .

"Required assets to build and debut are missing from 'fsharp' add them?" 
Click "Yes"

There are unresolved dependencies from 'project.json'.  Please execute the restore command...
Click "Restore"


Switch to "Debug" view, click little green arrow, ".NET Core Launch".  It builds and runs the generated "Hello World."

We're not a microservice yet, so let's add Suave for HTTP.  We also should use Newtonsoft.Json as a quick JSON serializer.

Edit project.json and add
```
  "dependencies": { 
    "Suave": "2.0.2",
    "Newtonsoft.Json": "9.0.1"
  }
```

Warning about the FSharp.Core version build number, so changing that to accept any:
```
"Microsoft.FSharp.Core.netcore": "1.0.0-alpha-*"
```

After saving, prompted again to "restore", and click "Restore."  Run again, and the generated "Hello world!" still works.

To make sure all is good with JSON serialization, let's hello world with a record:

First define the record type, just like the Page in the Go implementation.
```
type Page = {
    Title : string
    Schema : string
}
```

Now replace the simple "Hello World!" with some JSON serialization.

```
{ Title="hello"; Schema="world" } 
|> JsonConvert.SerializeObject
|> printfn "%s"
```

Looking good:
```
{"Title":"hello","Schema":"world"}
```

Ok, now for the real data, from `deluge.json`.  We need a function that can load the JSON from a file and deserialize it into `Page` records:

```
let getPages path = 
    path 
    |> System.IO.File.ReadAllText
    |> JsonConvert.DeserializeObject<Page array>
```

and call it with

```
getPages "../deluge.json" |> Array.iter (fun p -> printfn "%A" p)
```

Now we want to display lots of fake data, and there is a lot of it here:
https://github.com/icrowley/fake/tree/master/data/ru

Just need to pull it down and make some sequences for these.  F# type providers might be the way to go, but don't exist yet on CoreCLR land, so:

Going to pull them down on request using lazy loading with the System.Net.Http client, so adding that dependency in `project.json`:
```
    "System.Net.Http": "4.3.0"
```

And a few functions to load the data:

```
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


let getCityData language = 
    async {
        return!
            sprintf "https://raw.githubusercontent.com/icrowley/fake/master/data/%s/cities" language 
            |> getLines
    }
```
I'll have to make some more wrappers to really account for that whole `fake` library, but definitely getting close:

```
  async {
      let! russianCities = getCityData "ru"
      russianCities |> Seq.head |> printfn "%s"
  } |> Async.RunSynchronously
```

Adding the above to `Program.main` will print the first city in the list.  I'll have to make more similar functions, but the framework is there now.

Looking good so far, we just need our actual HTTP service.  Next up, Suave.

First, let's define what our API should look like, meaning the API routes:

```
[ GET >=> choose
    [ pathScan "/random/city/%s" getACityByLanguage >=> setMimeType "text/plain" ]
]
```

Even if you've never seen F# before, there's a decent chance this snippet makes sense.

We need a function to start up the server and write up the routes.  For the time being, I'll let that function accept a handler for it's one route as well.

```
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
```

Now for some code to retrieve a city.  Not dealing with caching the data yet.  This is async because 
HTTP requests are async, but also Suave needs async handlers.
```
let getRandomCity lang = 
    async {
        let! cities = getCityData lang
        let arr = cities |> Seq.toArray
        let len = arr.Length
        return arr.[rng.Next len]
    }
```

We need to wrap that in a web part that deals with HTTP parameters and response codes.
```
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
```
This is probably a tad confusing to the uninitiated.  It's a function that returns a function which accepts an HttpContext and returns a new HttpContext asynchronously.

Wire it all up at startup, passing the web part to the `startServer` function.

```
startServer getACityWebPart
```

Now you can get random cities from http://localhost:8080/random/city/ru.  Again, this isn't every bit of data available from `fake`, but it's a pattern that can be used to add more random data.

Publishing
----------

With .NET Core, the build toolchain can publish releases that include a self-contained runtime, which must be specified in the project.json so that it will download them on `dotnet restore`.

```
"runtimes": {
  "win10-x64": {},
  "osx.10.11-x64": {},
  "ubuntu.14.04-x64": {},
  "debian.8-x64": {}
}
```

To build for a certain runtime, use the following commands, which create a directory under `bin/release` with the executable and its dependencies.

```
dotnet build -r debian.8-x64
dotnet publish -c release -r debian.8-x64
```

After running the above commands, a build for Debian 8 (64-bit) will be available at `bin/release/netcoreapp1.1/debian.8-x64/publish`, ready to compress and distribute or copy to a docker image.

To execute it on the target platform, use the name used for `targetName` in project.json:

```
"outputName": "datadiluvium"
```

In this case, we can run with `./datadiluvium`.

