// src/NorthboundSessions.Web/wwwroot/js/tradingview-widget.js
window.initTradingViewWidget = (containerId, symbol) => {
    new TradingView.widget({
        width: "100%",
        height: 280,
        symbol: symbol,
        interval: "D",
        theme: "light",
        style: "1",
        locale: "en",
        container_id: containerId
    });
};