// theme.js — dark/light mode. An explicit choice always wins; with no stored
// choice we follow the OS preference. wwwroot/index.html and the inline
// pre-paint script in App.razor use the same 'theme' key and the same order,
// so the marketing page and the app never disagree.
function resolveTheme() {
    var stored = null;
    try { stored = localStorage.getItem('theme'); } catch (e) { stored = null; }
    if (stored === 'dark' || stored === 'light') return stored;
    return (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) ? 'dark' : 'light';
}

window.themeInterop = {
    getTheme: function () {
        return resolveTheme();
    },
    setTheme: function (theme) {
        document.documentElement.setAttribute('data-theme', theme);
        try { localStorage.setItem('theme', theme); } catch (e) { /* private mode */ }
    }
};

function applyStoredTheme() {
    document.documentElement.setAttribute('data-theme', resolveTheme());
}

// Re-apply the theme after every Blazor "enhanced navigation" — these are
// client-side route changes that re-sync <html>'s attributes to match
// what the SERVER rendered, which has no data-theme attribute at all
// since the server doesn't know what's in localStorage. Without this,
// dark mode resets on every page-to-page click until manually toggled.
window.addEventListener('load', function () {
    if (window.Blazor) {
        Blazor.addEventListener('enhancedload', applyStoredTheme);
    }
});

