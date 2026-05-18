const phoneInput = document.getElementById("phone");
if (phoneInput) { 
    const iti = window.intlTelInput(phoneInput, {
        initialCountry: "eg",
        separateDialCode: true,
        loadUtils: () => import("https://cdn.jsdelivr.net/npm/intl-tel-input@23.0.0/build/js/utils.js")
    });
    document.querySelector("form").addEventListener("submit", function () {
        phoneInput.value = iti.getNumber();
    });
}