namespace MobileFramework.Core.Localization
{
    /// <summary>
    /// Shorthand statico per la localizzazione: Loc.Get("btn_play").
    /// Il CoreBootstrapper esegue il Bind all'avvio; senza bind le chiavi
    /// vengono restituite com'è, utile nei test e nelle preview in Editor.
    /// </summary>
    public static class Loc
    {
        private static LocalizationManager _manager;

        public static void Bind(LocalizationManager manager) => _manager = manager;

        public static void Unbind() => _manager = null;

        public static bool IsBound => _manager != null;

        public static string Get(string key, params object[] args)
        {
            return _manager != null ? _manager.Get(key, args) : key;
        }
    }
}
