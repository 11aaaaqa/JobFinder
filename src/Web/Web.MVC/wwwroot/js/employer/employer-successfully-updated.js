const successInput = document.getElementById("isUpdated");

const isUpdated = successInput.value;
const modal = document.getElementById('modal');

if (isUpdated === 'true') {
    modal.classList.add('show');
    setTimeout(() => {
        modal.classList.remove('show');
    }, 3000);
}

const span = document.getElementById('close-modal');
span.onclick = function () {
    modal.classList.remove('show');
};
