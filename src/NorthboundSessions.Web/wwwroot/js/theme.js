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