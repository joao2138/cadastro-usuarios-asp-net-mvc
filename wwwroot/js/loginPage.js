document.addEventListener("DOMContentLoaded", function () {

   btnToggleHiddenState();

});




function showPassWord() {
   const passwordsFields = document.querySelectorAll(".campo-senha");
   const icons = document.querySelectorAll(".btn-toggle-senha");

   passwordsFields.forEach(field => {
      field.type = field.type === "password" ? "text" : "password";
   });

   icons.forEach(icon => {
      icon.classList.toggle("bi-eye-fill");
      icon.classList.toggle("bi-eye-slash-fill");
   });

   localStorage.setItem("passwordState",
      passwordsFields[0].type === "password" ? "hidden" : "text");
}



function btnToggleHiddenState() {

   const savedState = localStorage.getItem("passwordState")

   //console.log(savedState);
   const currentElementType = document.getElementById("registerSenha").type;

   if (savedState === "text" && currentElementType === "password") {
      showPassWord();
   }

   else if (savedState === "hidden" && currentElementType === "text") {
      showPassWord();
   }

}


//function showMessageError(errosJson = null) {
//   const errorMessages = Array.from(JSON.parse(errosJson ?? "[]"));


//   if (errorMessages.length > 0) {
//      try {
//         const toastBar = document.getElementById("toastBar");
//         const toastMessages = document.getElementById("toastMessages");

//         toastMessages.innerHTML = "";
//         errorMessages.forEach(msg => {
//            const li = document.createElement("li");
//            li.textContent = msg;
//            toastMessages.appendChild(li);
//         });

//         toastBar.style.display = "block";

//         setTimeout(() => {
//            toastBar.style.display = "none";
//         }, 6000);
//      } catch (e) {
//         console.error("Erro ao exibir mensagens:", e);
//      }
//   }

//}



