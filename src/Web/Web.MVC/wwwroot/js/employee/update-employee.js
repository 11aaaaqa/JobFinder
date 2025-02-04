const toggleButton = document.getElementById('toggleButton');
const radioGroup = document.getElementById('radioGroup');

toggleButton.addEventListener('mousedown', (event) => {
    event.preventDefault();
});

toggleButton.onclick = function () {
    radioGroup.style.display = radioGroup.style.display === 'none' ? 'block' : 'none';
    if (radioGroup.style.display === 'none') {
        toggleButton.classList.remove("highlighted-gender-input");
    } else {
        toggleButton.classList.add("highlighted-gender-input");
    }
};

const radios = document.querySelectorAll('#radioGroup input[type="radio"]');
radios.forEach(radio => {
    radio.addEventListener('change', function () {
        const selectedGender = this.value;
        toggleButton.value = selectedGender === 'Мужской' ? 'Мужской' : 'Женский';
        radioGroup.style.display = 'none';
        toggleButton.classList.remove("highlighted-gender-input");
    });
});
        
window.onclick = function(event) {
    if (event.target !== toggleButton) {
        radioGroup.style.display = 'none';
        toggleButton.classList.remove("highlighted-gender-input");
    }
}