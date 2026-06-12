using System.Collections.Generic;
using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Contracts;
using UnityEngine;

namespace MobileFramework.Core.Managers.UI
{
    /// <summary>
    /// Stack di pannelli: Push nasconde il pannello corrente e mostra il nuovo,
    /// Pop ripristina il precedente. I pannelli figli vengono registrati
    /// automaticamente; il gioco può sovrascriverli via RegisterPanel/BindSlot.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UIManager : MonoBehaviour, IUIManager
    {
        private readonly Dictionary<string, UIPanel> _panels = new Dictionary<string, UIPanel>();
        private readonly List<UIPanel> _stack = new List<UIPanel>();

        private GameContext _context;
        private bool _childrenRegistered;

        public string CurrentPanelId => _stack.Count > 0 ? _stack[_stack.Count - 1].PanelId : null;

        public int StackDepth => _stack.Count;

        private void Awake() => AutoRegisterChildren();

        /// <summary>Chiamato dal CoreBootstrapper: rende il contesto disponibile agli IUISlot.</summary>
        public void Initialize(GameContext context)
        {
            _context = context;
            AutoRegisterChildren();

            foreach (var panel in _panels.Values)
            {
                if (panel is IUISlot slot)
                {
                    slot.OnSlotAttached(_context);
                }
            }
        }

        public void RegisterPanel(UIPanel panel)
        {
            if (panel == null)
            {
                return;
            }

            _panels[panel.PanelId] = panel; // stessa chiave ⇒ override del gioco
            panel.ForceHidden();

            if (_context != null && panel is IUISlot slot)
            {
                slot.OnSlotAttached(_context);
            }
        }

        public void BindSlot(IUISlot slot)
        {
            if (slot is UIPanel panel)
            {
                RegisterPanel(panel);
            }
            else if (_context != null)
            {
                slot?.OnSlotAttached(_context);
            }
        }

        public void Push(string panelId)
        {
            if (!_panels.TryGetValue(panelId, out var panel))
            {
                Debug.LogWarning($"[UIManager] Pannello '{panelId}' non registrato.");
                return;
            }

            if (_stack.Count > 0)
            {
                _stack[_stack.Count - 1].Hide();
            }

            _stack.Add(panel);
            panel.Show();
        }

        public void Pop()
        {
            if (_stack.Count == 0)
            {
                return;
            }

            _stack[_stack.Count - 1].Hide();
            _stack.RemoveAt(_stack.Count - 1);

            if (_stack.Count > 0)
            {
                _stack[_stack.Count - 1].Show();
            }
        }

        public void PopToRoot()
        {
            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                _stack[i].Hide();
            }

            _stack.Clear();
        }

        public bool IsOpen(string panelId)
        {
            for (var i = 0; i < _stack.Count; i++)
            {
                if (_stack[i].PanelId == panelId)
                {
                    return true;
                }
            }

            return false;
        }

        private void AutoRegisterChildren()
        {
            if (_childrenRegistered)
            {
                return;
            }

            _childrenRegistered = true;
            foreach (var panel in GetComponentsInChildren<UIPanel>(true))
            {
                RegisterPanel(panel);
            }
        }

        private void OnDestroy()
        {
            foreach (var panel in _panels.Values)
            {
                if (panel is IUISlot slot)
                {
                    slot.OnSlotDetached();
                }
            }
        }
    }
}
