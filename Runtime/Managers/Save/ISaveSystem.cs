using MobileFramework.Core.Contracts;

namespace MobileFramework.Core.Managers.Save
{
    /// <summary>
    /// Persistenza dei dati di gioco. Ogni IGameSaveData è un file separato
    /// identificato da SaveKey, con checksum anti-manomissione e migrazione
    /// automatica quando DataVersion cambia.
    /// </summary>
    public interface ISaveSystem
    {
        void Save<T>(T data) where T : class, IGameSaveData;

        /// <summary>
        /// Carica i dati. Se il file non esiste o è manomesso restituisce un'istanza
        /// nuova con i valori di default. Se la versione su disco è precedente,
        /// chiama MigrateFrom e ripersiste i dati migrati.
        /// </summary>
        T Load<T>() where T : class, IGameSaveData, new();

        bool HasSave<T>() where T : class, IGameSaveData, new();
        void Delete<T>() where T : class, IGameSaveData, new();
        void DeleteAll();
    }
}
