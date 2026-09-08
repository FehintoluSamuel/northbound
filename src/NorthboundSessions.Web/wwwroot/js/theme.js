// theme.js — dark/light mode, defaults to light unless the user toggled before.
window.themeInterop = {
    getTheme: function () {
        return localStorage.getItem('theme') || 'light';
    },
    setTheme: function (theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    }
};

function applyStoredTheme() {
    const theme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', theme);
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

