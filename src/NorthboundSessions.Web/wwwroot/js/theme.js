// Applies theme before Blazor paints anything, to avoid a flash of the
// wrong theme. Persisted in localStorage; falls back to OS preference.
window.themeInterop = {
    getTheme: function () {
        return localStorage.getItem('theme') || 'light';
    },
    setTheme: function (theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    }
};