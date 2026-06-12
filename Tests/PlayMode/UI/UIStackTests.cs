using System.Collections;
using MobileFramework.Core.Managers.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MobileFramework.Tests.PlayMode.UI
{
    public class UIStackTests
    {
        private GameObject _root;
        private UIManager _ui;
        private UIPanel _panelA;
        private UIPanel _panelB;
        private UIPanel _panelC;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("[UIManager under test]");
            _ui = _root.AddComponent<UIManager>();

            _panelA = CreatePanel("a");
            _panelB = CreatePanel("b");
            _panelC = CreatePanel("c");
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_root);
        }

        private UIPanel CreatePanel(string id)
        {
            var go = new GameObject("panel_" + id);
            go.transform.SetParent(_root.transform);
            var panel = go.AddComponent<UIPanel>();
            panel.SetPanelId(id);
            _ui.RegisterPanel(panel);
            return panel;
        }

        [UnityTest]
        public IEnumerator RegisteredPanels_StartHidden()
        {
            Assert.IsFalse(_panelA.gameObject.activeSelf);
            Assert.IsFalse(_panelA.IsVisible);
            Assert.AreEqual(0, _ui.StackDepth);
            Assert.IsNull(_ui.CurrentPanelId);
            yield break;
        }

        [UnityTest]
        public IEnumerator Push_ShowsPanel_AndHidesPrevious()
        {
            _ui.Push("a");
            Assert.IsTrue(_panelA.gameObject.activeSelf);
            Assert.AreEqual("a", _ui.CurrentPanelId);

            _ui.Push("b");
            Assert.IsTrue(_panelB.gameObject.activeSelf);
            Assert.IsFalse(_panelA.gameObject.activeSelf, "il pannello sotto va nascosto");
            Assert.AreEqual("b", _ui.CurrentPanelId);
            Assert.AreEqual(2, _ui.StackDepth);
            Assert.IsTrue(_ui.IsOpen("a"), "'a' è ancora nello stack anche se nascosto");
            yield break;
        }

        [UnityTest]
        public IEnumerator Pop_RestoresPreviousPanel()
        {
            _ui.Push("a");
            _ui.Push("b");

            _ui.Pop();

            Assert.IsFalse(_panelB.gameObject.activeSelf);
            Assert.IsTrue(_panelA.gameObject.activeSelf);
            Assert.AreEqual("a", _ui.CurrentPanelId);
            Assert.AreEqual(1, _ui.StackDepth);
            yield break;
        }

        [UnityTest]
        public IEnumerator Pop_OnEmptyStack_IsHarmless()
        {
            _ui.Pop();

            Assert.AreEqual(0, _ui.StackDepth);
            yield break;
        }

        [UnityTest]
        public IEnumerator PopToRoot_HidesEverything()
        {
            _ui.Push("a");
            _ui.Push("b");
            _ui.Push("c");

            _ui.PopToRoot();

            Assert.AreEqual(0, _ui.StackDepth);
            Assert.IsFalse(_panelA.gameObject.activeSelf);
            Assert.IsFalse(_panelB.gameObject.activeSelf);
            Assert.IsFalse(_panelC.gameObject.activeSelf);
            yield break;
        }

        [UnityTest]
        public IEnumerator Push_UnknownPanel_LogsWarning_AndKeepsStack()
        {
            LogAssert.Expect(LogType.Warning, "[UIManager] Pannello 'ghost' non registrato.");

            _ui.Push("ghost");

            Assert.AreEqual(0, _ui.StackDepth);
            yield break;
        }

        [UnityTest]
        public IEnumerator RegisterPanel_SameId_OverridesPrevious()
        {
            var replacement = CreatePanel("a"); // stesso id: override (caso IUISlot del gioco)

            _ui.Push("a");

            Assert.IsTrue(replacement.gameObject.activeSelf);
            Assert.IsFalse(_panelA.gameObject.activeSelf, "deve mostrare il pannello sostitutivo, non l'originale");
            yield break;
        }
    }
}
