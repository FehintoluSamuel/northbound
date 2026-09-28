// src/NorthboundSessions.Web/wwwroot/js/tradingview-widget.js
//
// Called from Lessons.razor via JS interop. This function must never throw:
// an unhandled exception in OnAfterRenderAsync tears down the Blazor Server
// circuit, after which no event can be processed and every button on the
// lesson page silently stops responding. Failure is reported by returning
// false instead.
window.initTradingViewWidget = (containerId, symbol) => {
    // The chart is decorative. A blocked CDN, an ad blocker, or an offline
    // client must not stop the student from advancing through the slides.
    if (typeof TradingView === "undefined" || !TradingView.widget) {
        console.warn("TradingView script not loaded; skipping chart.");
        return false;
    }

    // Symbols are stored with human spacing ("NGSE : DANGCEM") but the widget
    // API only accepts the compact form.
    const cleanSymbol = String(symbol ?? "").replace(/\s+/g, "");

    if (!cleanSymbol) {
        return false;
    }

    try {
        new TradingView.widget({
            width: "100%",
            height: 280,
            symbol: cleanSymbol,
            interval: "D",
            theme: "light",
            style: "1",
            locale: "en",
            container_id: containerId
        });
        return true;
    } catch (err) {
        console.warn("TradingView widget failed to render:", err);
        return false;
    }
};
