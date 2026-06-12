using MobileFramework.Core.Bootstrap;

namespace MobileFramework.Core.StateMachine
{
    /// <summary>
    /// Classe base per ogni stato del GameManager. Gli stati del Core coprono il
    /// flusso standard; i giochi possono aggiungere stati propri (es. TutorialState)
    /// registrandoli nel GameManager e aggiungendo le regole al validator.
    /// </summary>
    public abstract class GameState
    {
        public GameManager Manager { get; private set; }

        protected GameContext Context => Manager.Context;

        internal void Attach(GameManager manager) => Manager = manager;

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Tick(float deltaTime)
        {
        }
    }
}
