using MobileFramework.Core.Contracts;

namespace MobileFramework.Core.Managers.Save
{
    /// <summary>
    /// Base class consigliata per i dati di salvataggio. Le sottoclassi
    /// sovrascrivono MigrateFrom quando incrementano DataVersion:
    /// il SaveSystem la invoca automaticamente al primo Load dopo l'update.
    /// </summary>
    public abstract class VersionedSaveData : IGameSaveData
    {
        public abstract string SaveKey { get; }
        public abstract int DataVersion { get; }

        /// <summary>
        /// Default: nessuna migrazione. I campi con lo stesso nome sono già stati
        /// popolati dal payload precedente; qui si recuperano campi rinominati
        /// o si calcolano i valori dei campi nuovi.
        /// </summary>
        public virtual void MigrateFrom(int storedVersion, string rawJson)
        {
        }
    }
}
