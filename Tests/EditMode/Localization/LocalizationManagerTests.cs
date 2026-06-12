using MobileFramework.Core.Events;
using MobileFramework.Core.Localization;
using NUnit.Framework;

namespace MobileFramework.Tests.EditMode.Localization
{
    public class LocalizationManagerTests
    {
        private const string ItalianJson = @"{
            ""_meta"": { ""language"": ""it"", ""version"": 1 },
            ""btn_play"": ""Gioca"",
            ""lives_remaining"": ""Hai {0} {0:vita|vite} rimanenti""
        }";

        private const string EnglishJson = @"{
            ""_meta"": { ""language"": ""en"", ""version"": 1 },
            ""btn_play"": ""Play"",
            ""btn_back"": ""Back""
        }";

        private LocalizationManager _manager;

        [SetUp]
        public void SetUp()
        {
            _manager = new LocalizationManager();
            _manager.SetFallbackFromJson(EnglishJson);
            _manager.LoadFromJson("it", ItalianJson);
        }

        [Test]
        public void Get_KnownKey_ReturnsTranslation()
        {
            Assert.AreEqual("Gioca", _manager.Get("btn_play"));
            Assert.AreEqual("it", _manager.CurrentLanguage);
        }

        [Test]
        public void Get_KeyMissingInLanguage_FallsBackToEnglish()
        {
            Assert.AreEqual("Back", _manager.Get("btn_back"));
        }

        [Test]
        public void Get_KeyMissingEverywhere_ReturnsKeyItself()
        {
            Assert.AreEqual("missing_key", _manager.Get("missing_key"));
        }

        [Test]
        public void Get_NullOrEmptyKey_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, _manager.Get(null));
            Assert.AreEqual(string.Empty, _manager.Get(string.Empty));
        }

        [Test]
        public void Get_WithArgs_ResolvesPlurals()
        {
            Assert.AreEqual("Hai 1 vita rimanenti", _manager.Get("lives_remaining", 1));
            Assert.AreEqual("Hai 3 vite rimanenti", _manager.Get("lives_remaining", 3));
        }

        [Test]
        public void MetaKey_IsNotExposedAsString()
        {
            Assert.IsFalse(_manager.Has("_meta"));
            Assert.AreEqual("_meta", _manager.Get("_meta"));
        }

        [Test]
        public void MergeFromJson_AddsAndOverridesKeys_WithoutClearingOthers()
        {
            _manager.MergeFromJson(@"{
                ""_meta"": { ""game"": ""flappy_clone"" },
                ""flappy_tap_hint"": ""Tocca per volare"",
                ""btn_play"": ""VOLA!""
            }");

            Assert.AreEqual("Tocca per volare", _manager.Get("flappy_tap_hint"));
            Assert.AreEqual("VOLA!", _manager.Get("btn_play"), "l'overlay del gioco vince sulla chiave Core");
            Assert.AreEqual("Hai 2 vite rimanenti", _manager.Get("lives_remaining", 2), "le altre chiavi restano");
        }

        [Test]
        public void LoadFromJson_EmitsLanguageChangedEvent()
        {
            var bus = new EventBus();
            var manager = new LocalizationManager(bus);
            string received = null;
            bus.Subscribe<LanguageChangedEvent>(e => received = e.LanguageCode);

            manager.LoadFromJson("es", @"{ ""btn_play"": ""Jugar"" }");

            Assert.AreEqual("es", received);
            Assert.AreEqual("es", manager.CurrentLanguage);
        }

        [Test]
        public void LoadFromJson_ReplacesPreviousLanguageCompletely()
        {
            _manager.LoadFromJson("en", EnglishJson);

            Assert.AreEqual("Play", _manager.Get("btn_play"));
            Assert.AreEqual("lives_remaining", _manager.Get("lives_remaining"), "le chiavi della lingua precedente non devono sopravvivere");
        }

        [Test]
        public void InvalidJson_IsIgnored_AndKeepsCurrentStrings()
        {
            _manager.MergeFromJson("{broken json");

            Assert.AreEqual("Gioca", _manager.Get("btn_play"));
        }
    }
}
