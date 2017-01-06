package main

import (
    "encoding/json"
    "fmt"
    "io/ioutil"
    "os"
    "github.com/icrowley/fake"
    "time"
)

type Page struct {
    Title string `json:"title"`
    Schema   string `json:"schema"`
}

func (p Page) toString() string {
    return toJson(p)
}

func toJson(p interface{}) string {
    bytes, err := json.Marshal(p)
    if err != nil {
        fmt.Println(err.Error())
        os.Exit(1)
    }

    return string(bytes)
}

func main() {

    pages := getPages()
    for _, p := range pages {
        fmt.Println(p.toString())
        /* This should be parallelized for each of the schemas unless
        they're dependent and need to be executed in a specific order. */
    }

    fmt.Println(toJson(pages))

    fmt.Println("\nSome Russian Random Data")
    fake.SetLang("ru")
    fmt.Println(fake.FirstName())
    fmt.Println(fake.FullName())
    fmt.Println(fake.Product())
    fmt.Println(fake.Brand())
    fmt.Println(fake.Character())
    fmt.Println(fake.City())
    fmt.Println(fake.Color())
    fmt.Println(fake.Company())
    fmt.Println(fake.Continent())
    fmt.Println(fake.Country())
    fmt.Println(fake.Day())
    fmt.Println(fake.CurrencyCode())
    fmt.Println(fake.Currency())
    fmt.Println(fake.Currency())

    fmt.Println("\n\n\nSome English Random Data")
    fake.SetLang("en")
    fmt.Println(fake.FirstName())
    fmt.Println(fake.FullName())
    fmt.Println(fake.Product())
    fmt.Println(fake.Brand())
    fmt.Println(fake.Character())
    fmt.Println(fake.City())
    fmt.Println(fake.Color())
    fmt.Println(fake.Company())
    fmt.Println(fake.Continent())
    fmt.Println(fake.Country())
    fmt.Println(fake.Day())
    fmt.Println(fake.CurrencyCode())
    fmt.Println(fake.Currency())
    fmt.Println(fake.Currency())

    thisMany := 10
    counter := 1
    t := time.Now().Format(time.RFC850)

    fmt.Println("\n\n\nPrinting", thisMany)

  	for i := 0; i < thisMany; i++ {
      fmt.Println(counter, fake.FirstName())
      counter += 1
      fake.FirstName()
  	}

    // ~2 sec  Running 10 Million Name Generations
    // Start Time:  Friday, 06-Jan-17 14:16:39 PST
    // End Time:  Friday, 06-Jan-17 14:16:41 PST
    // ~13 sec Running with fmt.Println + counter
    // Start Time:  Friday, 06-Jan-17 14:14:46 PST
    // End Time:  Friday, 06-Jan-17 14:15:59 PST

    fmt.Println("Start Time: ", t)
    t = time.Now().Format(time.RFC850)
    fmt.Println("End Time: ", t)
}

func getPages() []Page {
    raw, err := ioutil.ReadFile("./deluge.json")
    if err != nil {
        fmt.Println(err.Error())
        os.Exit(1)
    }

    var c []Page
    json.Unmarshal(raw, &c)
    return c
}
