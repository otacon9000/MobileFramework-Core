using MobileFramework.Core.Contracts;

namespace MobileFramework.Core.Managers.UI
{
    /// <summary>Id dei pannelli base del Core. I giochi li usano per override via IUISlot.</summary>
    public static class CorePanelIds
    {
        public const string MainMenu = "main_menu";
        public const string HUD = "hud";
        public const string Pause = "pause";
        public const string GameOver = "game_over";
    }

    /// <summary>
    /// Gestione UI a stack: Push mostra un pannello sopra quello corrente,
    /// Pop torna al precedente. I pannelli vanno registrati prima dell'uso.
    /// </summary>
    public interface IUIManager
    {
        /// <summary>Id del pannello in cima allo stack, null se lo stack è vuoto.</summary>
        string CurrentPanelId { get; }

        int StackDepth { get; }

        /// <summary>Registra un pannello. Una seconda registrazione con lo stesso id lo sostituisce (override del gioco).</summary>
        void RegisterPanel(UIPanel panel);

        /// <summary>Aggancia un pannello del gioco che sostituisce un pannello base del Core.</summary>
        void BindSlot(IUISlot slot);

        void Push(string panelId);
        void Pop();

        /// <summary>Svuota lo stack nascondendo tutti i pannelli.</summary>
        void PopToRoot();

        bool IsOpen(string panelId);
    }
}
