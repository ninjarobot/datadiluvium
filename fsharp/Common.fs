namespace DataDiluvium

[<AutoOpen>]
module Common =

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
