function execCmd(command) {
    document.execCommand(command, false, null);
    updateButtonStates();
}

function updateButtonStates() {
    document.getElementById('boldBtn').classList.toggle('active', document.queryCommandState('bold'));
    document.getElementById('italicBtn').classList.toggle('active', document.queryCommandState('italic'));
}

function updateWorkerResponsibilitiesTextarea() {
    const content = document.getElementById('worker-responsibilities-input').innerHTML;
    document.getElementById('worker-responsibilities-hidden-textarea').value = content;
    updateButtonStates();
}

const inputDiv = document.getElementById('worker-responsibilities-input');

inputDiv.addEventListener('input', () => {
    inputDiv.style.height = 'auto';
    inputDiv.style.height = `${inputDiv.scrollHeight}px`;
});

window.onload = function () {
    updateButtonStates();
};