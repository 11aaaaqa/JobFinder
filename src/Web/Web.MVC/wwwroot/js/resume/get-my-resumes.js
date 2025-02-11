const button = document.getElementById('status');
const modal = document.getElementById('update-status-modal');

button.onclick = function () {
    var rect = button.getBoundingClientRect();
    var containerRect = document.querySelector('.profile-section').getBoundingClientRect();

    modal.style.top = (rect.bottom - containerRect.top) + "px";
    modal.style.left = (rect.left - containerRect.left) + "px";
    modal.style.width = rect.width + "px";

    modal.style.display = modal.style.display === 'block' ? 'none' : 'block';
};

window.onclick = function(event) {
    if (event.target !== button && event.target !== modal) {
        modal.style.display = 'none';
    }
};