using System;
using System.IO;
using MobileFramework.Core.Contracts;
using UnityEngine;
using JsonSerializer = MobileFramework.Core.Serialization.JsonSerializer;

namespace MobileFramework.Core.Managers.Save
{
    /// <summary>
    /// Persistenza su file: un file per SaveKey, formato envelope
    /// { version, checksum, payload }. Checksum non valido ⇒ dati azzerati;
    /// version diversa da DataVersion ⇒ MigrateFrom automatico e ri-salvataggio.
    /// </summary>
    public sealed class SaveSystem : ISaveSystem
    {
        private sealed class SaveEnvelope
        {
            public int version;
            public string checksum;
            public string payload;
        }

        private const string FileExtension = ".save";

        private readonly string _root;

        /// <param name="rootDirectory">Directory dei salvataggi; default Application.persistentDataPath/Saves. I test passano una directory temporanea.</param>
        public SaveSystem(string rootDirectory = null)
        {
            _root = string.IsNullOrEmpty(rootDirectory)
                ? Path.Combine(Application.persistentDataPath, "Saves")
                : rootDirectory;

            Directory.CreateDirectory(_root);
        }

        private string PathFor(string saveKey) => Path.Combine(_root, saveKey + FileExtension);

        public void Save<T>(T data) where T : class, IGameSaveData
        {
            if (data == null)
            {
                return;
            }

            var payload = JsonSerializer.Serialize(data);
            var envelope = new SaveEnvelope
            {
                version = data.DataVersion,
                checksum = SaveIntegrityChecker.ComputeChecksum(payload),
                payload = payload
            };

            File.WriteAllText(PathFor(data.SaveKey), JsonSerializer.Serialize(envelope));
        }

        public T Load<T>() where T : class, IGameSaveData, new()
        {
            var data = new T();
            var path = PathFor(data.SaveKey);

            if (!File.Exists(path))
            {
                return data;
            }

            if (!JsonSerializer.TryDeserialize<SaveEnvelope>(ReadFileSafe(path), out var envelope)
                || string.IsNullOrEmpty(envelope.payload))
            {
                Debug.LogWarning($"[SaveSystem] Salvataggio '{data.SaveKey}' illeggibile: reset ai default.");
                return data;
            }

            if (!SaveIntegrityChecker.Verify(envelope.payload, envelope.checksum))
            {
                Debug.LogWarning($"[SaveSystem] Checksum non valido per '{data.SaveKey}': reset ai default.");
                return data;
            }

            try
            {
                JsonSerializer.Populate(envelope.payload, data);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveSystem] Payload di '{data.SaveKey}' non compatibile ({e.GetType().Name}): reset ai default.");
                return new T();
            }

            if (envelope.version != data.DataVersion)
            {
                data.MigrateFrom(envelope.version, envelope.payload);
                Save(data); // persiste subito lo schema nuovo: la migrazione gira una sola volta
            }

            return data;
        }

        public bool HasSave<T>() where T : class, IGameSaveData, new()
        {
            return File.Exists(PathFor(new T().SaveKey));
        }

        public void Delete<T>() where T : class, IGameSaveData, new()
        {
            var path = PathFor(new T().SaveKey);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteAll()
        {
            if (!Directory.Exists(_root))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(_root, "*" + FileExtension))
            {
                File.Delete(file);
            }
        }

        private static string ReadFileSafe(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (IOException)
            {
                return string.Empty;
            }
        }
    }
}
