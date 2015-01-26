using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Json;
using System.Collections.Generic;

namespace DeserializationTests.cs
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializeBasicClass()
        {
            Item item = new Item { name = "A" };
            string serialized = JsonParser.Serialize(item);
            string expected = "{\"name\":\"A\"}";

            Assert.AreEqual(expected, serialized);
        }

        [TestMethod]
        public void SerializeClassWithInnerClass()
        {
            Dot p = new Dot() { color = "orange", description = null, coords = new Point { x = 4, y = 9 } };
            string serialized = JsonParser.Serialize(p);
            string expected = "{\"color\":\"orange\",\"description\":null,\"coords\":{\"x\":4,\"y\":9}}";

            Assert.AreEqual(expected, serialized);
        }

        [TestMethod]
        public void SerializeBasicArray()
        {
            string[] array = new[] { "A", "B", "C" };
            string serialized = JsonParser.Serialize(array);
            string expected = "[\"A\",\"B\",\"C\"]";

            Assert.AreEqual(expected, serialized);
        }

        [TestMethod]
        public void SerializeBasicList()
        {
            List<string> list = new List<string>() { "A", "B", "C" };
            string serialized = JsonParser.Serialize(list);
            string expected = "[\"A\",\"B\",\"C\"]";

            Assert.AreEqual(expected, serialized);
        }

        [TestMethod]
        public void SerializeEmptyArray()
        {
            string[] array = new string[] { };
            string serialized = JsonParser.Serialize(array);
            string expected = "[]";

            Assert.AreEqual(expected, serialized);
        }

        [TestMethod]
        public void SerializeArrayWithDifferentTypes()
        {
            object[] array = new object[] { "X", "Y", "Z", 1.1, 1.2, 1.3 };
            string serialized = JsonParser.Serialize(array);
            string expected = "[\"X\",\"Y\",\"Z\",1.1,1.2,1.3]";

            Assert.AreEqual(expected, serialized);
        }

        [TestMethod]
        public void SerializeObjectWithArray()
        {
            Foo foo = new Foo() { A = "Hello", B = new string[] {"X", "Y", "Z"}, C = 42 };
            string serialized = JsonParser.Serialize(foo);
            string expected = "{\"A\":\"Hello\",\"B\":[\"X\",\"Y\",\"Z\"],\"C\":42}";

            Assert.AreEqual(expected, serialized);
        }
    }

    class Item
    {
        public string name { get; set; }
    }

    class Dot
    {
        public string color { get; set; }
        public string description { get; set; }
        public Point coords { get; set; }
    }

    class Point
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Foo
    {
        public string A { get; set; }
        public string[] B { get; set; }
        public int C { get; set; }
    }
}
