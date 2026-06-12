using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MobileFramework.Core.Localization
{
    /// <summary>
    /// Parser dei token plurali nel formato {indice:singolare|plurale},
    /// es. "Hai {0} {0:vita|vite}". I token vengono risolti in base al valore
    /// dell'argomento, poi la stringa passa per string.Format standard.
    /// </summary>
    public static class PluralResolver
    {
        private static readonly Regex PluralToken = new Regex(
            @"\{(\d+):([^|{}]*)\|([^{}]*)\}",
            RegexOptions.Compiled);

        public static string Resolve(string template, params object[] args)
        {
            if (string.IsNullOrEmpty(template))
            {
                return string.Empty;
            }

            var resolved = PluralToken.Replace(template, match =>
            {
                var index = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                if (args == null || index >= args.Length)
                {
                    return match.Value; // argomento mancante: lascia il token visibile per il debug
                }

                return IsSingular(args[index]) ? match.Groups[2].Value : match.Groups[3].Value;
            });

            if (args == null || args.Length == 0)
            {
                return resolved;
            }

            try
            {
                return string.Format(CultureInfo.InvariantCulture, resolved, args);
            }
            catch (FormatException)
            {
                // indice fuori range o template malformato: meglio il testo grezzo di un'eccezione in UI
                return resolved;
            }
        }

        private static bool IsSingular(object value)
        {
            try
            {
                return Math.Abs(Convert.ToDouble(value, CultureInfo.InvariantCulture) - 1d) < double.Epsilon;
            }
            catch (Exception)
            {
                return false; // non numerico: usa il plurale
            }
        }
    }
}
