namespace MobileFramework.Core.Events
{
    /// <summary>Avanzamento di un caricamento (scena o asset). Progress in [0,1].</summary>
    public readonly struct SceneLoadProgressEvent
    {
        public readonly string SceneName;
        public readonly float Progress;

        public SceneLoadProgressEvent(string sceneName, float progress)
        {
            SceneName = sceneName;
            Progress = progress;
        }
    }
}
