const toggleButton = document.getElementById('toggleButton');
const radioGroup = document.getElementById('radioGroup');

toggleButton.onclick = function () {
    radioGroup.style.display = radioGroup.style.display === 'none' ? 'block' : 'none';
    if (radioGroup.style.display === 'none') {
        toggleButton.classList.remove("highlighted-employees-input");
    } else {
        toggleButton.classList.add("highlighted-employees-input");
    }
};

const radios = document.querySelectorAll('#radioGroup input[type="radio"]');
radios.forEach(radio => {
    radio.addEventListener('change', function () {
        const selectedColleaguesCount = this.value;
        switch (selectedColleaguesCount) {
            case "До 50":
                toggleButton.value = "До 50 сотрудников";
                break;
            case "50 - 100":
                toggleButton.value = "50 - 100 сотрудников";
                break;
            case "100 - 500":
                toggleButton.value = "100 - 500 сотрудников";
                break;
            case "500 - 1000":
                toggleButton.value = "500 - 1000 сотрудников";
                break;
            case "1000 - 5000":
                toggleButton.value = "1000 - 5000 сотрудников";
                break;
            case "Более 5000":
                toggleButton.value = "Более 5000 сотрудников";
                break;
        }
        radioGroup.style.display = 'none';
        toggleButton.classList.remove("highlighted-employees-input");
    });
});

window.onclick = function (event) {
    if (event.target !== toggleButton) {
        radioGroup.style.display = 'none';
        toggleButton.classList.remove("highlighted-employees-input");
    }
}