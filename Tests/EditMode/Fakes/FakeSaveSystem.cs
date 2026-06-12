using System.Collections.Generic;
using MobileFramework.Core.Contracts;
using MobileFramework.Core.Managers.Save;

namespace MobileFramework.Tests.EditMode.Fakes
{
    /// <summary>ISaveSystem finto: persistenza in memoria, nessun I/O su disco.</summary>
    public sealed class FakeSaveSystem : ISaveSystem
    {
        public readonly Dictionary<string, IGameSaveData> Stored = new Dictionary<string, IGameSaveData>();
        public int SaveCalls;
        public int LoadCalls;

        public void Save<T>(T data) where T : class, IGameSaveData
        {
            SaveCalls++;
            Stored[data.SaveKey] = data;
        }

        public T Load<T>() where T : class, IGameSaveData, new()
        {
            LoadCalls++;
            var key = new T().SaveKey;
            return Stored.TryGetValue(key, out var data) && data is T typed ? typed : new T();
        }

        public bool HasSave<T>() where T : class, IGameSaveData, new()
        {
            return Stored.ContainsKey(new T().SaveKey);
        }

        public void Delete<T>() where T : class, IGameSaveData, new()
        {
            Stored.Remove(new T().SaveKey);
        }

        public void DeleteAll() => Stored.Clear();
    }
}
