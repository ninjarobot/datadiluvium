namespace DataDiluvium

module PostgresqlGenerator =

    /// Defines the name and source for a column.
    type Column = {
        Name : string
        Source : Fake
    }

    /// Alias of a TableName to a string for code clarity
    type TableName = string

    /// Defines a single table schema, with source columns.
    type Table = {
        TableName : TableName
        Columns : Column list
    }

    /// Number of rows of data to generate
    type RowsToGenerate = int

    /// A request to generate several tables of data.
    type GenerationRequest = {
        RequestedData : (Table * RowsToGenerate) list
    }

    /// Type of columns supported.
    type ColType = 
        | Int
        | Text
        | Bool
        static member ParamType x =
            match x with
            | Int -> "int"
            | Text -> "text"
            | Bool -> "bool"

    /// Holds a SQL parameter and its value.
    type ParameterValue = 
        | IntParam of int
        | TextParam of string
        | BoolParam of bool

    /// A list of parameters for an INSERT statement.
    type InsertParameters = ParameterValue list

    /// Data to prepare in a PREPARE statement
    type Prepare = Prepare of TableName * (string * ColType) list
    /// Parameterized data to pass when executing a prepared statement
    type PreparedStatement = PreparedStatement of Prepare * InsertParameters list

    /// Possible characters to be used in randomly generated alpha strings.
    [<Literal>]
    let private ALPHA_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

    /// Generates a random string of the specified lenght.
    let private randomString len =
        let rng = new System.Random ()
        seq {
            while true do
                yield ALPHA_CHARS.[rng.Next(ALPHA_CHARS.Length)]
        } |> Seq.take len |> Seq.toArray |> System.String

    /// Generates a dollar quoting symbol for postgres with a random 
    /// 6 character string so fake string data can't inject SQL
    let private randomDollarQuote s = 
        let dollarQuote = randomString 6 |> sprintf "$%s$"
        [|dollarQuote; s; dollarQuote|] |> System.String.Concat

    /// Accepts a prepared statement and builds a string of SQL to insert random data.
    let buildStatement (prepared:PreparedStatement) =
        let rng = System.Random ()
        let planName = rng.Next 1000 |> sprintf "plan%i"
        let (PreparedStatement(prepare, inserts)) = prepared
        let prepared =
            match prepare with
            | Prepare (tableName, cols) -> 
                let paramTypes = 
                    cols
                    |> List.map (snd >> ColType.ParamType)
                    |> String.concat ", "
                let placeHolders = 
                    [1..cols.Length]
                    |> Seq.map (sprintf "$%i")
                    |> String.concat ", "
                let colNames = cols |> List.map fst |> String.concat ", " |> sprintf "(%s)"
                sprintf "PREPARE %s (%s) AS INSERT INTO %s %s VALUES (%s);" planName paramTypes tableName colNames placeHolders
        let buildParameter (pv:ParameterValue) =
            match pv with
            | IntParam i -> i.ToString()
            | BoolParam b -> if b then "TRUE" else "FALSE"
            | TextParam t -> randomDollarQuote t
        let buildExecuteStatement (pvs:ParameterValue list) =
            pvs
            |> List.map buildParameter
            |> String.concat ", "
            |> sprintf "EXECUTE %s (%s);" planName
        let executes =
            inserts |> List.map buildExecuteStatement
        prepared :: executes |> String.concat System.Environment.NewLine

    /// Maps Fake cases to column types
    let fakeToColType fake =
        match fake with
        | Country _ | City _ | Continent _ | Currency _ | Gender _ | Street _ -> Text
        | Number -> Int

    /// Builds a prepared statement given a table definition and number of rows.
    let generateTableData table rows =
        let prepare = Prepare(table.TableName, table.Columns |> List.map (fun col -> col.Name, (col.Source |> fakeToColType)))
        let generateInserts (fakes:Fake list) =
            fakes |> List.map (fun fake ->
                match fake with
                | Country _ | City _ | Continent _ | Currency _ | Gender _ | Street _ -> 
                    fake |> (FakeProvider.getRandomSingleItem >> Seq.head >> TextParam)
                | Number -> 
                    fake |> (FakeProvider.getRandomSingleItem >> Seq.head >> System.Int32.Parse >> IntParam)
                )
        let fakesForTable = table.Columns |> List.map (fun col -> col.Source) 
        let insertParams =
            seq {
                while true do
                    yield fakesForTable |> generateInserts
            } |> Seq.take rows |> List.ofSeq
        PreparedStatement(prepare, insertParams)

    /// Processes a generation request, returning the SQL to execute
    let generateData (req:GenerationRequest) = 
        let preparedStatements =
            req.RequestedData 
            |> List.map (fun (table, rows) -> generateTableData table rows)
        preparedStatements |> List.map buildStatement
        |> String.concat "\n"
