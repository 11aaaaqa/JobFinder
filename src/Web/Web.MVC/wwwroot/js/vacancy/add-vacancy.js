function updateWorkerResponsibilitiesTextarea() {
    const content = document.getElementById('worker-responsibilities-input').innerHTML;
    document.getElementById('worker-responsibilities-hidden-textarea').value = content;
}

const inputDiv = document.getElementById('worker-responsibilities-input');

inputDiv.addEventListener('input', () => {
    inputDiv.style.height = 'auto';
    inputDiv.style.height = `${inputDiv.scrollHeight}px`;
});
