import { DotLottie } from "https://cdn.jsdelivr.net/npm/@lottiefiles/dotlottie-web@latest/dist/dotlottie-web.js";

window.mascotInterop = {

    init: function () {

        const mascot = document.getElementById("lesson-mascot");

        if (!mascot) {

            console.warn("Mascot element not found.");

            return;

        }

        const animation = mascot.querySelector("dotlottie-wc");

        if (!animation) {

            console.warn("Lottie animation element not found.");

            return;

        }

        animation.setAttribute("autoplay", "");

        animation.setAttribute("loop", "");

        console.log("Mascot initialized.");

    }

};