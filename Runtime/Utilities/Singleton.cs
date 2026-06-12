namespace MobileFramework.Core.Utilities
{
    /// <summary>
    /// Base class per singleton lazy. Per regola del framework è usata SOLO dal
    /// ServiceLocator: ogni altro servizio si ottiene tramite il locator stesso,
    /// mai con un proprio Instance statico.
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _instance;

        public static T Instance => _instance ??= new T();

        /// <summary>Azzera l'istanza. Usato dai test e dal teardown del bootstrapper.</summary>
        public static void ResetInstance() => _instance = null;
    }
}
