using System;
using System.Collections.Generic;
using System.IO;
using MobileFramework.Core.Managers.Save;
using NUnit.Framework;
using JsonSerializer = MobileFramework.Core.Serialization.JsonSerializer;

namespace MobileFramework.Tests.EditMode.Save
{
    public class SaveSystemTests
    {
        private sealed class TestSaveData : VersionedSaveData
        {
            public override string SaveKey => "test_save";
            public override int DataVersion => 1;

            public int score;
            public string playerName = "default";
        }

        // Vecchio schema: il campo si chiamava "coins".
        private sealed class WalletV1 : VersionedSaveData
        {
            public override string SaveKey => "wallet";
            public override int DataVersion => 1;

            public int coins;
        }

        // Schema nuovo: "coins" è stato rinominato in "currency".
        private sealed class WalletV2 : VersionedSaveData
        {
            public override string SaveKey => "wallet";
            public override int DataVersion => 2;

            public int currency;

            [NonSerialized] public int MigratedFromVersion = -1;

            public override void MigrateFrom(int storedVersion, string rawJson)
            {
                MigratedFromVersion = storedVersion;
                if (storedVersion == 1)
                {
                    var legacy = JsonSerializer.Deserialize<Dictionary<string, int>>(rawJson);
                    if (legacy.TryGetValue("coins", out var coins))
                    {
                        currency = coins;
                    }
                }
            }
        }

        private string _root;
        private SaveSystem _saveSystem;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), "mfw_save_tests_" + Guid.NewGuid().ToString("N"));
            _saveSystem = new SaveSystem(_root);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, recursive: true);
            }
        }

        [Test]
        public void Load_WithoutFile_ReturnsDefaults()
        {
            var data = _saveSystem.Load<TestSaveData>();

            Assert.AreEqual(0, data.score);
            Assert.AreEqual("default", data.playerName);
        }

        [Test]
        public void SaveAndLoad_RoundtripsValues()
        {
            _saveSystem.Save(new TestSaveData { score = 1500, playerName = "Andrea" });

            var loaded = _saveSystem.Load<TestSaveData>();

            Assert.AreEqual(1500, loaded.score);
            Assert.AreEqual("Andrea", loaded.playerName);
        }

        [Test]
        public void HasSave_ReflectsFilePresence()
        {
            Assert.IsFalse(_saveSystem.HasSave<TestSaveData>());

            _saveSystem.Save(new TestSaveData());

            Assert.IsTrue(_saveSystem.HasSave<TestSaveData>());
        }

        [Test]
        public void Delete_RemovesSave()
        {
            _saveSystem.Save(new TestSaveData { score = 10 });

            _saveSystem.Delete<TestSaveData>();

            Assert.IsFalse(_saveSystem.HasSave<TestSaveData>());
            Assert.AreEqual(0, _saveSystem.Load<TestSaveData>().score);
        }

        [Test]
        public void DeleteAll_RemovesEverySave()
        {
            _saveSystem.Save(new TestSaveData());
            _saveSystem.Save(new WalletV1 { coins = 5 });

            _saveSystem.DeleteAll();

            Assert.IsFalse(_saveSystem.HasSave<TestSaveData>());
            Assert.IsFalse(_saveSystem.HasSave<WalletV1>());
        }

        [Test]
        public void Load_TamperedPayload_ResetsToDefaults()
        {
            _saveSystem.Save(new TestSaveData { score = 777 });

            var path = Path.Combine(_root, "test_save.save");
            File.WriteAllText(path, File.ReadAllText(path).Replace(":777", ":999"));

            var loaded = _saveSystem.Load<TestSaveData>();

            Assert.AreEqual(0, loaded.score, "un file manomesso deve azzerare i dati");
        }

        [Test]
        public void Load_CorruptedFile_ResetsToDefaults()
        {
            _saveSystem.Save(new TestSaveData { score = 10 });
            File.WriteAllText(Path.Combine(_root, "test_save.save"), "{garbage///");

            var loaded = _saveSystem.Load<TestSaveData>();

            Assert.AreEqual(0, loaded.score);
        }

        [Test]
        public void Load_VersionMismatch_InvokesMigrateFromAutomatically()
        {
            _saveSystem.Save(new WalletV1 { coins = 320 });

            var migrated = _saveSystem.Load<WalletV2>();

            Assert.AreEqual(1, migrated.MigratedFromVersion, "MigrateFrom deve ricevere la versione su disco");
            Assert.AreEqual(320, migrated.currency, "il campo rinominato va recuperato dal payload legacy");
        }

        [Test]
        public void Load_AfterMigration_PersistsNewVersion_AndDoesNotMigrateAgain()
        {
            _saveSystem.Save(new WalletV1 { coins = 50 });

            _saveSystem.Load<WalletV2>();          // prima load: migra e ri-salva
            var second = _saveSystem.Load<WalletV2>(); // seconda load: schema già aggiornato

            Assert.AreEqual(-1, second.MigratedFromVersion, "la migrazione deve girare una sola volta");
            Assert.AreEqual(50, second.currency);
        }

        [Test]
        public void Load_SameVersion_DoesNotInvokeMigrateFrom()
        {
            _saveSystem.Save(new WalletV2 { currency = 12 });

            var loaded = _saveSystem.Load<WalletV2>();

            Assert.AreEqual(-1, loaded.MigratedFromVersion);
            Assert.AreEqual(12, loaded.currency);
        }
    }
}
