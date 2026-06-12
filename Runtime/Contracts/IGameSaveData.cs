namespace MobileFramework.Core.Contracts
{
    /// <summary>
    /// Contratto per i dati di salvataggio di un gioco.
    /// Il SaveSystem usa <see cref="SaveKey"/> come nome file e <see cref="DataVersion"/>
    /// per la migrazione: se la versione su disco non corrisponde, chiama
    /// <see cref="MigrateFrom"/> automaticamente prima di restituire i dati.
    /// </summary>
    public interface IGameSaveData
    {
        /// <summary>Chiave univoca del salvataggio. Stabile tra le versioni del gioco.</summary>
        string SaveKey { get; }

        /// <summary>Versione corrente dello schema dati. Incrementare a ogni modifica dei campi.</summary>
        int DataVersion { get; }

        /// <summary>
        /// Aggiorna l'istanza a partire da una versione precedente dello schema.
        /// I campi compatibili sono già stati popolati; <paramref name="rawJson"/>
        /// è il payload originale per recuperare campi rinominati o rimossi.
        /// </summary>
        void MigrateFrom(int storedVersion, string rawJson);
    }
}
