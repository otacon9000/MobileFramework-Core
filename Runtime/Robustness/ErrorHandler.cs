using System;
using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;
using UnityEngine;

namespace MobileFramework.Core.Robustness
{
    /// <summary>
    /// Cattura le eccezioni non gestite a runtime. Se l'eccezione arriva durante
    /// il gameplay tenta il recovery: chiude la sessione via UnloadingState e
    /// riporta il giocatore al menu invece di lasciare il gioco congelato.
    /// </summary>
    public sealed class ErrorHandler : IDisposable
    {
        private readonly GameManager _gameManager;
        private bool _initialized;
        private bool _recovering;

        /// <summary>Notifica (condition, stackTrace) per analytics/crash reporting del gioco.</summary>
        public event Action<string, string> ExceptionCaught;

        public int ExceptionCount { get; private set; }

        public ErrorHandler(GameManager gameManager = null)
        {
            _gameManager = gameManager;
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            Application.logMessageReceived += OnLogMessage;
        }

        public void Dispose()
        {
            if (_initialized)
            {
                _initialized = false;
                Application.logMessageReceived -= OnLogMessage;
            }
        }

        private void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception)
            {
                return;
            }

            ExceptionCount++;
            ExceptionCaught?.Invoke(condition, stackTrace);
            TryRecover();
        }

        private void TryRecover()
        {
            if (_gameManager == null || _recovering)
            {
                return;
            }

            if (_gameManager.IsIn<PlayingState>() || _gameManager.IsIn<PausedState>())
            {
                _recovering = true;
                try
                {
                    Debug.LogWarning("[ErrorHandler] Eccezione durante il gameplay: recovery verso il menu.");
                    _gameManager.ChangeState<UnloadingState>();
                }
                finally
                {
                    _recovering = false;
                }
            }
        }
    }
}
