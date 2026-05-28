# Konnektr.Npgsql.Age

[![Nuget](https://img.shields.io/nuget/v/Konnektr.Npgsql.Age?color=blue)](https://www.nuget.org/packages/Konnektr.Npgsql.Age/)

## What is Apache AGE?

Apache AGE is an open-source extension for PostgreSQL which provides it with the capabilities of a graph database. This package is a plugin for the Npgsql library which allows you to interact with Apache AGE from C#.

## Installation

```shell
dotnet add package Konnektr.Npgsql.Age
```

Then register the type plugin on your `NpgsqlDataSourceBuilder`:

```csharp
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.UseAge();
await using var dataSource = dataSourceBuilder.Build();
```

## Quickstart

```csharp
using Npgsql;
using Npgsql.Age;
using Npgsql.Age.Types;

var dataSourceBuilder = new NpgsqlDataSourceBuilder("Host=...");
dataSourceBuilder.UseAge();
await using var dataSource = dataSourceBuilder.Build();

await using var connection = await dataSource.OpenConnectionAsync();

// Create graph
await using (var cmd = connection.CreateGraphCommand("graph1"))
    await cmd.ExecuteNonQueryAsync();

// Add vertices
await using (var cmd = connection.CreateCypherCommand(
    "graph1", "CREATE (:Person {name: 'Alice', age: 30}), (:Person {name: 'Bob', age: 25})"))
    await cmd.ExecuteNonQueryAsync();

// Retrieve vertices
await using (var cmd = connection.CreateCypherCommand(
    "graph1", "MATCH (n:Person) RETURN n"))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
    {
        var agtype = await reader.GetFieldValueAsync<Agtype>(0);
        Vertex person = agtype.GetVertex();
        Console.WriteLine($"{person.Label}: id={person.Id}, age={person.Properties["age"]}");
    }
}

// Drop graph
await using (var cmd = connection.DropGraphCommand("graph1"))
    await cmd.ExecuteNonQueryAsync();
```

## Using Cypher Parameters

Parameters are referenced in Cypher queries using the `$` prefix (e.g., `$name`, `$age`).

### Using a Dictionary

```csharp
var parameters = new Dictionary<string, object?>
{
    ["name"] = "Alice",
    ["age"] = 30
};

await using (var cmd = connection.CreateCypherCommand(
    "graph1",
    "CREATE (p:Person {name: $name, age: $age}) RETURN p",
    parameters))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
    {
        var agtype = await reader.GetFieldValueAsync<Agtype>(0);
        Vertex person = agtype.GetVertex();
        Console.WriteLine($"Created: {person.Label}");
    }
}
```

### Using a JSON String

```csharp
string parametersJson = """{"name": "Bob", "age": 25}""";

await using (var cmd = connection.CreateCypherCommand(
    "graph1",
    "CREATE (p:Person {name: $name, age: $age}) RETURN p",
    parametersJson))
    await cmd.ExecuteNonQueryAsync();
```

### Complex Parameters

```csharp
var parameters = new Dictionary<string, object?>
{
    ["person"] = new Dictionary<string, object>
    {
        ["name"] = "Charlie",
        ["age"] = 35,
        ["hobbies"] = new[] { "reading", "cycling" }
    }
};

await using (var cmd = connection.CreateCypherCommand(
    "graph1",
    "CREATE (p:Person {name: $person.name, age: $person.age}) RETURN p",
    parameters))
    await cmd.ExecuteNonQueryAsync();
```

## Working with Agtype

The `Agtype` struct is the core type representing `ag_catalog.agtype` values from PostgreSQL.

### Type Detection

```csharp
Agtype value = await reader.GetFieldValueAsync<Agtype>(0);

if (value.IsVertex) { /* vertex */ }
else if (value.IsEdge) { /* edge */ }
else if (value.IsPath) { /* path */ }
else if (value.IsArray) { /* JSON array */ }
else if (value.IsMap) { /* JSON object */ }
else if (value.IsNull) { /* null */ }
```

### Explicit Cast Operators

```csharp
Vertex  vertex  = (Vertex)agtype;
Edge    edge    = (Edge)agtype;
string  text    = (string)agtype;
int     number  = (int)agtype;
double  dbl     = (double)agtype;
List<object?> list = (List<object?>)agtype;
Dictionary<string, object?> map = (Dictionary<string, object?>)agtype;
```

### Generic Deserialization

```csharp
// Deserialize to any compatible type
var person = agtype.Get<Vertex>();

// Typed properties
var personTyped = agtype.Get<Vertex<Dictionary<string, object?>>>();
```

### Working with Paths

```csharp
Agtype result = await reader.GetFieldValueAsync<Agtype>(0);
Path path = result.GetPath();

// Alternate access
foreach (var segment in path.Segments)
{
    if (segment is Vertex v)
        Console.WriteLine($"Vertex: {v.Label}");
    else if (segment is Edge e)
        Console.WriteLine($"Edge: {e.Label} ({e.StartId} -> {e.EndId})");
}

// Or use filtered helpers
foreach (var v in path.Vertices) { /* vertices only */ }
foreach (var e in path.Edges)    { /* edges only */ }
```

### Number Type Inference

When deserializing dynamic types via `Get<object?>()` or `Get<List<object?>>()`, numbers are inferred in this order:

| JSON value    | C# type      |
|---------------|--------------|
| Integer (fits int) | `int`    |
| Integer (fits long) | `long`  |
| Fractional    | `decimal`    |
| Scientific notation | `double` |

Special AGE literals (`Infinity`, `-Infinity`, `NaN`, `::numeric`) are handled transparently through custom JSON converters.

## Strongly-Typed Entities

For structured access to vertex/edge properties, use the generic variants:

```csharp
public record Person(string Name, int Age);

// Query
var result = agtype.Get<Vertex<Person>>();
Console.WriteLine($"Name: {result.Properties.Name}, Age: {result.Properties.Age}");
```

## Acknowledgements

* This project is a fork of [pg-age](https://github.com/Allison-E/pg-age) and incorporates improvements from [erdomke/npgsql-age](https://github.com/erdomke/npgsql-age).
* The project relies heavily on the work of the [Npgsql](https://github.com/npgsql/npgsql) team.
