module WebApiTests

open System
open Xunit
open DataDiluvium.WebApi

[<Fact>]
let ``Deserializes basic reqeust`` () =
    """
    [
        {
            "schema": "relational",
            "database": "postgresql",
            "structure": [
            {
                "table": "Users",
                "columns": [
                {"name": "id", "type": "uuid"},
                {"name": "firstname", "type": "firstname"},
                {"name": "lastname", "type": "lastname"},
                {"name": "email_address", "type": "email"}
                ]
            },
            {
                "table": "Addresses",
                "columns": [
                {"name": "id", "type": "uuid"},
                {"name": "street", "type": "address"},
                {"name": "city", "type": "city"},
                {"name": "state", "type": "state"},
                {"name": "postalcode", "type": "zip"}
                ]
            },
            {
                "table": "Transactions",
                "columns": [
                { "name": "id", "type": "uuid" },
                { "name": "transaction", "type": "money" },
                { "name": "stamp", "type": "date" }
                ]
            }
            ]
        }
    ]
    """
    |> Newtonsoft.Json.JsonConvert.DeserializeObject<RelationalDbRequest []>
    |> (fun req -> // Make sure we get what we expect.
        Assert.Equal(1, req.Length)
        Assert.Equal("postgresql", req.[0].Database)
        Assert.Equal(3, req.[0].Structure.Length)
        Assert.Equal("Users", req.[0].Structure.[0].Table)
        Assert.Equal("Addresses", req.[0].Structure.[1].Table)
        Assert.Equal("Transactions", req.[0].Structure.[2].Table)
        Assert.Equal(4, req.[0].Structure.[0].Columns.Length)
        Assert.Equal(5, req.[0].Structure.[1].Columns.Length)
        Assert.Equal(3, req.[0].Structure.[2].Columns.Length)
        Assert.Equal("id", req.[0].Structure.[0].Columns.[0].Name)
        Assert.Equal("uuid", req.[0].Structure.[0].Columns.[0].Type)
        Assert.Equal("firstname", req.[0].Structure.[0].Columns.[1].Name)
        Assert.Equal("firstname", req.[0].Structure.[0].Columns.[1].Type)
        Assert.Equal("email_address", req.[0].Structure.[0].Columns.[3].Name)
        Assert.Equal("email", req.[0].Structure.[0].Columns.[3].Type)
    )
