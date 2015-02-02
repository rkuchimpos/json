# C# JSON Parser (in development)

## Usage

##### Deserialization
```c#
public class person
{
  public string name { get; set; }
  public long age { get; set; }
  public bool married { get; set; }
  public car car { get; set; }
}

public class car
{
  public string name { get; set; }
  public long num_doors { get; set; }
}

string json = @"
{
  ""name"": ""John Doe"",
  ""age"": 42,
  ""married"": true,
  ""car"" : {
    ""name"": ""Honda"",
    ""num_doors"": 4
  }
}";

var person = JsonParser.Deserialize<person>(json);
Console.WriteLine(person.age);      // 42
Console.WriteLine(person.car.name); // Honda
```

##### Dynamic Deserialization

```c#
string json = "{\"name\": \"Bill\"}";

dynamic person = JsonParser.Deserialize(json);
Console.WriteLine(person.name); // Bill
```

##### Serialization

```c#
public class Point
{
  public int X { get; set; }
  public int Y { get; set; }
}

var point = new Point() { X = 3, Y = 4 };

string serialized = JsonParser.Serialize(point); // {"X":3,"Y":4}
```
