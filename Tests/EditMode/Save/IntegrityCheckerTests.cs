using MobileFramework.Core.Managers.Save;
using NUnit.Framework;

namespace MobileFramework.Tests.EditMode.Save
{
    public class IntegrityCheckerTests
    {
        [Test]
        public void ComputeChecksum_IsDeterministic()
        {
            var a = SaveIntegrityChecker.ComputeChecksum("{\"score\":1}");
            var b = SaveIntegrityChecker.ComputeChecksum("{\"score\":1}");

            Assert.AreEqual(a, b);
        }

        [Test]
        public void ComputeChecksum_ChangesWithContent()
        {
            var a = SaveIntegrityChecker.ComputeChecksum("{\"score\":1}");
            var b = SaveIntegrityChecker.ComputeChecksum("{\"score\":2}");

            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void ComputeChecksum_ReturnsLowercaseHexSha256()
        {
            var checksum = SaveIntegrityChecker.ComputeChecksum("content");

            Assert.AreEqual(64, checksum.Length);
            StringAssert.IsMatch("^[0-9a-f]+$", checksum);
        }

        [Test]
        public void Verify_MatchingContent_ReturnsTrue()
        {
            var content = "{\"lives\":3}";
            var checksum = SaveIntegrityChecker.ComputeChecksum(content);

            Assert.IsTrue(SaveIntegrityChecker.Verify(content, checksum));
        }

        [Test]
        public void Verify_ModifiedContent_ReturnsFalse()
        {
            var checksum = SaveIntegrityChecker.ComputeChecksum("{\"lives\":3}");

            Assert.IsFalse(SaveIntegrityChecker.Verify("{\"lives\":99}", checksum));
        }

        [Test]
        public void Verify_NullOrEmptyChecksum_ReturnsFalse()
        {
            Assert.IsFalse(SaveIntegrityChecker.Verify("content", null));
            Assert.IsFalse(SaveIntegrityChecker.Verify("content", ""));
        }

        [Test]
        public void Verify_IsCaseInsensitiveOnChecksum()
        {
            var content = "abc";
            var checksum = SaveIntegrityChecker.ComputeChecksum(content).ToUpperInvariant();

            Assert.IsTrue(SaveIntegrityChecker.Verify(content, checksum));
        }
    }
}
