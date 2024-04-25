/*!
 * Dark Mode Switch v1.0.1 (https://github.com/coliff/dark-mode-switch)
 * Copyright 2021 C.Oliff
 * Licensed under MIT (https://github.com/coliff/dark-mode-switch/blob/main/LICENSE)
 */
let darkSwitch = document.getElementById("darkSwitch");
window.addEventListener("load", (function() {
        if (darkSwitch) {
            darkSwitch.addEventListener("change", (function() {
                    resetTheme()
                }
            ))
        }
    }
));

function resetTheme() {
    if (darkSwitch.checked) {
        document.body.setAttribute("data-theme", "dark");
        localStorage.setItem("darkSwitch", "dark")
    } else {
        document.body.removeAttribute("data-theme");
        localStorage.removeItem("darkSwitch")
    }
}
