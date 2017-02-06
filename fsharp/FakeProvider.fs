namespace DataDiluvium

open System
open System.Net.Http

module FakeProvider =

    // Single instance of HttpClient will be created on first use.
    let private httpClient = 
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

