const button = document.getElementById('status');
const modal = document.getElementById('update-status-modal');

button.onclick = function() {
    modal.style.display = modal.style.display === 'block' ? 'none' : 'block';
};

window.onclick = function(event) {
    if (event.target !== button) {
        modal.style.display = 'none';
    }
};