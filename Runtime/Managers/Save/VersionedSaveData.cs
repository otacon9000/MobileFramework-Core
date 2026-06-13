using MobileFramework.Core.Contracts;
using Newtonsoft.Json;

namespace MobileFramework.Core.Managers.Save
{
    /// <summary>
    /// Base class consigliata per i dati di salvataggio. Le sottoclassi
    /// sovrascrivono MigrateFrom quando incrementano DataVersion:
    /// il SaveSystem la invoca automaticamente al primo Load dopo l'update.
    /// </summary>
    public abstract class VersionedSaveData : IGameSaveData
    {
        // Metadati: vivono nell'envelope del SaveSystem, non vanno nel payload serializzato.
        [JsonIgnore] public abstract string SaveKey { get; }
        [JsonIgnore] public abstract int DataVersion { get; }

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
