using MobileFramework.Core.Localization;
using NUnit.Framework;

namespace MobileFramework.Tests.EditMode.Localization
{
    public class PluralResolverTests
    {
        [Test]
        public void Resolve_One_UsesSingular()
        {
            var result = PluralResolver.Resolve("Hai {0} {0:vita|vite} rimanenti", 1);

            Assert.AreEqual("Hai 1 vita rimanenti", result);
        }

        [TestCase(0, "Hai 0 vite rimanenti")]
        [TestCase(2, "Hai 2 vite rimanenti")]
        [TestCase(15, "Hai 15 vite rimanenti")]
        public void Resolve_NotOne_UsesPlural(int count, string expected)
        {
            var result = PluralResolver.Resolve("Hai {0} {0:vita|vite} rimanenti", count);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Resolve_MultipleTokens_ResolvedIndependently()
        {
            var result = PluralResolver.Resolve("{0} {0:tubo|tubi}, {1} {1:moneta|monete}", 1, 5);

            Assert.AreEqual("1 tubo, 5 monete", result);
        }

        [Test]
        public void Resolve_TemplateWithoutTokens_BehavesLikeFormat()
        {
            var result = PluralResolver.Resolve("Punteggio: {0}", 1200);

            Assert.AreEqual("Punteggio: 1200", result);
        }

        [Test]
        public void Resolve_NoArgs_ReturnsTemplateUntouched()
        {
            var template = "Hai {0} {0:vita|vite}";

            Assert.AreEqual(template, PluralResolver.Resolve(template));
        }

        [Test]
        public void Resolve_MissingArgumentIndex_KeepsTokenVisible()
        {
            var result = PluralResolver.Resolve("{0} e {1:uno|tanti}", 3);

            StringAssert.Contains("{1:uno|tanti}", result);
        }

        [Test]
        public void Resolve_FloatArgument_OneIsSingular()
        {
            Assert.AreEqual("1 vita", PluralResolver.Resolve("{0} {0:vita|vite}", 1.0f));
            Assert.AreEqual("1.5 vite", PluralResolver.Resolve("{0} {0:vita|vite}", 1.5f));
        }

        [Test]
        public void Resolve_NonNumericArgument_UsesPlural()
        {
            var result = PluralResolver.Resolve("{0:elemento|elementi}: {0}", "molti");

            Assert.AreEqual("elementi: molti", result);
        }

        [Test]
        public void Resolve_EmptyTemplate_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, PluralResolver.Resolve(null, 1));
            Assert.AreEqual(string.Empty, PluralResolver.Resolve(string.Empty, 1));
        }
    }
}
