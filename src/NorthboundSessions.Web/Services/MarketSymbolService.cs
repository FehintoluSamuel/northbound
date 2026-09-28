using System.Linq;
using System.Text.RegularExpressions;

namespace NorthboundSessions.Web.Services
{
    /// <summary>
    /// Normalises and validates the market symbols an instructor types into the
    /// admin forms.
    ///
    /// TradingView only accepts {EXCHANGE}:{TICKER} with no whitespace —
    /// anything else silently fails to load a chart, and a lesson that looks
    /// blank is far easier to miss than a validation message. Symbols were
    /// previously stored exactly as typed, so "NGSE : DANGCEM" was saved
    /// verbatim and the widget never rendered.
    /// </summary>
    public static partial class MarketSymbolService
    {
        // Exchange prefixes: NASDAQ, NSENG, LSE, CME_MINI, FX_IDC, INDEX...
        // Tickers may also contain "." (BRK.B) and "!" (ES1!).
        [GeneratedRegex(@"^[A-Z0-9_]+:[A-Z0-9._!\-]+$")]
        private static partial Regex CanonicalFormat();

        /// <summary>
        /// Cleans up user input. Returns null for blank input, since a lesson
        /// without a chart is valid. Use <see cref="TryNormalize"/> when the
        /// caller wants to reject a malformed value rather than store it.
        /// </summary>
        public static string? Normalize(string? raw)
        {
            TryNormalize(raw, out var symbol, out _);
            return symbol;
        }

        /// <summary>
        /// Normalises <paramref name="raw"/> and reports whether it is a usable
        /// symbol. On failure <paramref name="symbol"/> is null and
        /// <paramref name="error"/> explains what to enter instead.
        /// </summary>
        public static bool TryNormalize(string? raw, out string? symbol, out string? error)
        {
            symbol = null;
            error = null;

            // Tolerate every flavour of spacing and case people type, including
            // the "NGSE : DANGCEM" and "nseng:dangcem" seen in real data.
            // Whitespace only: no real ticker contains a space, so dropping it
            // recovers the intended symbol. Other punctuation is left alone so
            // genuine mistakes are reported instead of silently "corrected"
            // into a different wrong symbol.
            var compact = new string((raw ?? string.Empty)
                .Where(c => !char.IsWhiteSpace(c))
                .Select(char.ToUpperInvariant)
                .ToArray());

            if (compact.Length == 0)
            {
                return true; // no symbol is a legitimate choice
            }

            var colon = compact.IndexOf(':');
            if (colon <= 0 || colon == compact.Length - 1)
            {
                error = "Enter the symbol as EXCHANGE:TICKER, for example NSENG:DANGCEM or NASDAQ:AAPL.";
                return false;
            }

            var exchange = compact[..colon];
            var ticker = compact[(colon + 1)..];
            symbol = $"{exchange}:{ticker}";

            if (!CanonicalFormat().IsMatch(symbol))
            {
                error = $"\"{symbol}\" is not a valid symbol. Use EXCHANGE:TICKER, for example NSENG:DANGCEM.";
                symbol = null;
                return false;
            }

            return true;
        }
    }
}
