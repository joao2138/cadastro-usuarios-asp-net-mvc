document.addEventListener("DOMContentLoaded", function () {

   const triedRenewSession = sessionStorage.getItem("triedRenewSession");

   if (triedRenewSession == null) {

      sessionStorage.setItem("triedRenewSession", "true");

      const isAuthenticated = getCookie("auth.isAuthenticated");

      if (isAuthenticated) {
         window.location.href = "/auth/refresh";
      }

   }
   
});




function getCookie(name) {
   const value = `; ${document.cookie}`;
   const parts = value.split(`; ${name}=`);
   if (parts.length === 2) return parts.pop().split(';').shift();
   return null;
}
