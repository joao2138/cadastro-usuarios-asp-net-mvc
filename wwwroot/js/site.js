// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function showMessage(messagesJson) {
   const messages = JSON.parse(messagesJson || "[]");
   if (!messages.length) return;

   const toastBar = document.getElementById("toastBar");
   const toastMessages = document.getElementById("toastMessages");

   toastMessages.innerHTML = messages
      .map(msg => `<li>${msg}</li>`)
      .join("");

   toastBar.style.display = "block";

   setTimeout(() => {
      toastBar.style.display = "none";
   }, 6000);
}


document.addEventListener("DOMContentLoaded", () => {
   const toastBar = document.getElementById("toastBar");
   const errorsJson = toastBar.dataset.errors;
   if (errorsJson && errorsJson !== "[]") {
      showMessage(errorsJson);
   }
});

