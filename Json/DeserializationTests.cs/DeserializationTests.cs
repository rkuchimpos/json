using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Json.JsonParser;
using System.Collections.Generic;
using System.Linq;

namespace DeserializationTests.cs
{
    // note: use http://json2csharp.com/ to generate C# POCOs

    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void DeserializeString()
        {
            string json = "\"hello, world\"";

            var deserialized = JsonParser.Deserialize<string>(json);

            Assert.IsInstanceOfType(deserialized, typeof(string));
            Assert.AreEqual("hello, world", deserialized);
        }

        [TestMethod]
        public void DeserializeStringWithUnicode()
        {
            string json = "\"The temperature is 75.4\u2109 today.\"";

            var deserialized = JsonParser.Deserialize<string>(json);

            Assert.AreEqual("The temperature is 75.4℉ today.", deserialized);
        }

        [TestMethod]
        public void DeserializeStringWithEscapedChars()
        {
            string json = "\"hello,\t\tworld\"";

            var deserialized = JsonParser.Deserialize<string>(json);

            Assert.IsInstanceOfType(deserialized, typeof(string));
            Assert.AreEqual("hello,\t\tworld", deserialized);
        }

        [TestMethod]
        public void DeserializeBasicJsonPair()
        {
            string json = "{\"name\": \"john\", \"age\": 42}";

            var person = JsonParser.Deserialize<Person>(json);

            Assert.AreEqual("john", person.name);
            Assert.AreEqual(42, person.age);
        }

        [TestMethod]
        public void DeserializeJsonWithHierarchy()
        {
            string json = @"
            {
                'name' : 'Lenovo Thinkcentre',
                'specs' : {
                    'cpu' : 'Intel Core 2 Duo E8400',
                    'ram_gb' : 4
                },
                'has_os' : true
            }";

            var computer = JsonParser.Deserialize<Computer>(json);

            Assert.AreEqual("Intel Core 2 Duo E8400", computer.specs.cpu);
        }

        [TestMethod]
        public void DeserializeBasicArray()
        {
            string json = @"[ 'Los Angeles', 'New York', 'Seattle']";

            var array = JsonParser.Deserialize<List<object>>(json);

            var expected = new List<object> { "Los Angeles", "New York", "Seattle" };

            Assert.IsTrue(array.Except(expected).Count() == 0);
        }
        [TestMethod]
        public void DeserializeArray()
        {
            string json = @"[ 'Los Angeles', {'name':'dingleberry'}, 'Seattle']";

            var array = JsonParser.Deserialize<List<object>>(json);

            Assert.AreEqual(array[0], "Los Angeles");
            Assert.AreEqual("dingleberry", ((JsonObject)array[1])["name"]);
            Assert.AreEqual(array[2], "Seattle");
        }
    }

    class Person
    {
        public string name { get; set; }
        public long age { get; set; }
    }

    class Computer
    {
        public string name { get; set; }
        public specs specs { get; set; }
        public bool has_os { get; set; }
    }

    class specs
    {
        public string cpu { get; set; }
        public long ram_gb { get; set; }
    }
}
