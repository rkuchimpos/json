# C# JSON Parser (in development)

## Usage

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
  'name': 'John Doe',
  'age': 42,
  'married': true,
  'car' : {
    'name': 'Honda',
    'num_doors': 4
  }
}";

var person = JsonParser.Deserialize<person>(json);
Console.WriteLine(person.age);      // 42
Console.WriteLine(person.car.name); // Honda
```
