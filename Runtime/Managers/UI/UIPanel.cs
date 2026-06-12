using UnityEngine;

namespace MobileFramework.Core.Managers.UI
{
    /// <summary>
    /// Classe base per ogni pannello UI. L'identità è il PanelId: i pannelli Core
    /// usano gli id di CorePanelIds, così i giochi possono sostituirli registrando
    /// un pannello con lo stesso id.
    /// </summary>
    [DisallowMultipleComponent]
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private string panelId;

        public string PanelId => string.IsNullOrEmpty(panelId) ? DefaultPanelId : panelId;

        /// <summary>Id usato quando il campo serializzato è vuoto. I pannelli Core lo sovrascrivono.</summary>
        protected virtual string DefaultPanelId => GetType().Name;

        public bool IsVisible { get; private set; }

        /// <summary>Imposta l'id da codice (setup runtime e test). Da chiamare prima della registrazione.</summary>
        public void SetPanelId(string id) => panelId = id;

        public void Show()
        {
            if (IsVisible)
            {
                return;
            }

            gameObject.SetActive(true);
            IsVisible = true;
            OnShow();
        }

        public void Hide()
        {
            if (!IsVisible)
            {
                return;
            }

            OnHide();
            gameObject.SetActive(false);
            IsVisible = false;
        }

        /// <summary>Nasconde senza passare per i hook: usato alla registrazione iniziale.</summary>
        internal void ForceHidden()
        {
            gameObject.SetActive(false);
            IsVisible = false;
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }
    }
}
