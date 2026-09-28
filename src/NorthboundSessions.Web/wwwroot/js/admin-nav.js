// AdminSidebar off-canvas drawer: Escape-to-close.
//
// This dispatches a real click on the drawer's own close control rather than
// calling back into .NET over a DotNetObjectReference. The reference round-trip
// silently did nothing (the promise never settled, with no rejection to catch),
// so Escape appeared to do nothing at all. A synthetic click goes through
// Blazor's normal DOM event pipeline, so it reaches the same
// @onclick="CloseSidebar" handler as a finger tap and cannot get stuck.
window.adminNav = (() => {
    let handler = null;

    return {
        registerEscape() {
            if (handler) return;

            handler = (e) => {
                if (e.key !== "Escape" && e.key !== "Esc") return;

                // The backdrop only exists while the drawer is open, so it is
                // the better target; the X is the fallback.
                const close = document.querySelector("button.backdrop")
                    || document.querySelector("button.close-btn");

                if (close) close.click();
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
