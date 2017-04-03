module PostgresqlGeneratorTests

open System
open Xunit
open DataDiluvium

[<Fact>]
let ``Generates enough records for two tables`` () = 
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
    let prepareStatements = sql.Split([|'\n'|]) |> Array.fold(fun acc line -> if line.StartsWith("PREPARE ") then acc+1 else acc) 0
    let executeStatements = sql.Split([|'\n'|]) |> Array.fold(fun acc line -> if line.StartsWith("EXECUTE ") then acc+1 else acc) 0
    Assert.Equal (2, prepareStatements)
    Assert.Equal (35, executeStatements)
    Assert.True (sql.Contains("(int, text, text, text) AS INSERT INTO places (street_num, street, city, country) VALUES ($1, $2, $3, $4);"))
    Assert.True (sql.Contains("(int, text) AS INSERT INTO sofware_releases (version, code_name) VALUES ($1, $2);"))
