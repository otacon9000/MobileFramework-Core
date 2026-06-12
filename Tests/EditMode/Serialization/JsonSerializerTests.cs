using System.Collections.Generic;
using NUnit.Framework;
using JsonSerializer = MobileFramework.Core.Serialization.JsonSerializer;

namespace MobileFramework.Tests.EditMode.Serialization
{
    public class JsonSerializerTests
    {
        private sealed class Payload
        {
            public int Number;
            public string Text;
            public List<string> Items = new List<string>();
        }

        [Test]
        public void Roundtrip_PreservesValues()
        {
            var source = new Payload { Number = 42, Text = "ciao", Items = { "a", "b" } };

            var json = JsonSerializer.Serialize(source);
            var loaded = JsonSerializer.Deserialize<Payload>(json);

            Assert.AreEqual(42, loaded.Number);
            Assert.AreEqual("ciao", loaded.Text);
            CollectionAssert.AreEqual(new[] { "a", "b" }, loaded.Items);
        }

        [Test]
        public void Serialize_PrettyPrint_ContainsNewlines()
        {
            var compact = JsonSerializer.Serialize(new Payload());
            var pretty = JsonSerializer.Serialize(new Payload(), prettyPrint: true);

            Assert.IsFalse(compact.Contains("\n"));
            Assert.IsTrue(pretty.Contains("\n"));
        }

        [Test]
        public void Deserialize_IgnoresUnknownMembers()
        {
            var loaded = JsonSerializer.Deserialize<Payload>("{\"Number\":7,\"Ghost\":true}");

            Assert.AreEqual(7, loaded.Number);
        }

        [Test]
        public void Populate_KeepsFieldsMissingFromJson()
        {
            var target = new Payload { Number = 5, Text = "originale" };

            JsonSerializer.Populate("{\"Number\":9}", target);

            Assert.AreEqual(9, target.Number);
            Assert.AreEqual("originale", target.Text);
        }

        [Test]
        public void TryDeserialize_InvalidJson_ReturnsFalse()
        {
            var ok = JsonSerializer.TryDeserialize<Payload>("{not valid json", out var result);

            Assert.IsFalse(ok);
            Assert.IsNull(result);
        }

        [Test]
        public void TryDeserialize_ValidJson_ReturnsTrue()
        {
            var ok = JsonSerializer.TryDeserialize<Payload>("{\"Number\":1}", out var result);

            Assert.IsTrue(ok);
            Assert.AreEqual(1, result.Number);
        }
    }
}
