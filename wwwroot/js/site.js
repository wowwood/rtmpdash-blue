// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function copyToClipboard(str) {
    navigator.clipboard.writeText(str);
}

$(document).ready(function() {
    $("body").tooltip({ selector: '[data-toggle=tooltip]' });
});
