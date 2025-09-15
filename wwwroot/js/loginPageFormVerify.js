document.addEventListener("DOMContentLoaded", function () {
   registerFormVerification();
   loginFormVerification();


});


function registerFormVerification() {
   const passfieldvalue = document.getElementById("registerSenha")?.value ?? "";
   const confirmField = document.getElementById("confirmSenha");
   if (confirmField) confirmField.value = passfieldvalue;


   const form = document.getElementById("formRegistro");
   const senhaInput = document.getElementById("registerSenha");
   const confirmInput = document.getElementById("confirmSenha");
   const erroMsg = document.getElementById("senhaErro");


   if (form) {
      form.addEventListener("submit", function (e) {
         const senha = senhaInput.value;
         const confirmacao = confirmInput.value;

         if (senha !== confirmacao) {
            e.preventDefault();
            erroMsg.style.display = "block";
            confirmInput.classList.add("is-invalid");

         } else {
            erroMsg.style.display = "none";
            confirmInput.classList.remove("is-invalid");

            preventDoubleClickForm(form);
         }

      });
   }


}


function loginFormVerification() {
   const form = document.getElementById("formLogin");

   if (form) {
      form.addEventListener("submit", function () {
         preventDoubleClickForm(form);
      })
   }
}


function preventDoubleClickForm(form) {
   const submitBtn = form.querySelector('button[type="submit"]');

   if (submitBtn) {
      const previousText = submitBtn.textContent;

      submitBtn.innerHTML = "Enviando Formulário...";
      submitBtn.disabled = true;


      setTimeout(() => {
         if (submitBtn) {
            submitBtn.innerHTML = previousText;
            submitBtn.disabled = false;
         }

      }, 3000);


     
   }
}






