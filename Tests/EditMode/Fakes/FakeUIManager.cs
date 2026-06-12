using System.Collections.Generic;
using MobileFramework.Core.Contracts;
using MobileFramework.Core.Managers.UI;

namespace MobileFramework.Tests.EditMode.Fakes
{
    /// <summary>IUIManager finto: replica la semantica a stack su semplici liste di id.</summary>
    public sealed class FakeUIManager : IUIManager
    {
        private readonly List<string> _stack = new List<string>();

        public readonly List<string> PushHistory = new List<string>();
        public readonly List<UIPanel> RegisteredPanels = new List<UIPanel>();
        public readonly List<IUISlot> BoundSlots = new List<IUISlot>();

        public string CurrentPanelId => _stack.Count > 0 ? _stack[_stack.Count - 1] : null;
        public int StackDepth => _stack.Count;

        public void RegisterPanel(UIPanel panel) => RegisteredPanels.Add(panel);

        public void BindSlot(IUISlot slot) => BoundSlots.Add(slot);

        public void Push(string panelId)
        {
            _stack.Add(panelId);
            PushHistory.Add(panelId);
        }

        public void Pop()
        {
            if (_stack.Count > 0)
            {
                _stack.RemoveAt(_stack.Count - 1);
            }
        }

        public void PopToRoot() => _stack.Clear();

        public bool IsOpen(string panelId) => _stack.Contains(panelId);
    }
}
