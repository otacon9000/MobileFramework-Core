using MobileFramework.Core.Bootstrap;

namespace MobileFramework.Core.Contracts
{
    /// <summary>
    /// Contratto per i pannelli UI del gioco che sostituiscono un pannello base del Core
    /// (es. FlappyHUDPanel al posto di HUDPanel). Lo SlotId deve corrispondere
    /// all'id del pannello Core da rimpiazzare.
    /// </summary>
    public interface IUISlot
    {
        /// <summary>Id del pannello Core che questo slot sostituisce (vedi CorePanelIds).</summary>
        string SlotId { get; }

        /// <summary>Chiamato quando lo slot viene agganciato all'UIManager.</summary>
        void OnSlotAttached(GameContext context);

        /// <summary>Chiamato quando lo slot viene rimosso o l'UIManager si spegne.</summary>
        void OnSlotDetached();
    }
}
