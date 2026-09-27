// AdminLayout off-canvas drawer: Escape-to-close, with a single shared
// document listener so N open layouts (e.g. nested admin pages) don't each
// attach their own handler.
window.adminNav = (() => {
    let handler = null;

    return {
        registerEscape(dotNetRef) {
            if (handler) return;
            handler = (e) => {
                if (e.key === "Escape" || e.key === "Esc") {
                    dotNetRef.invokeMethodAsync("OnEscapeKey");
                }
            };
            document.addEventListener("keydown", handler);
        },
        unregisterEscape() {
            if (!handler) return;
            document.removeEventListener("keydown", handler);
            handler = null;
        }
    };
})();
