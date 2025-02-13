const button = document.getElementById('status');
const modal = document.getElementById('update-status-modal');
const label = document.getElementById('label');
const dynamicStatus = document.getElementById('dynamic-status');
const img = document.getElementById('img');

button.addEventListener('click', function() {
    var rect = button.getBoundingClientRect();
    var containerRect = document.querySelector('.profile-section').getBoundingClientRect();

    modal.style.top = (rect.bottom - containerRect.top) + "px";
    modal.style.left = (rect.left - containerRect.left) + "px";
    modal.style.width = rect.width + "px";

    modal.style.display = modal.style.display === 'block' ? 'none' : 'block';
});

window.addEventListener('click', function (event) {
    if (event.target !== button && event.target !== modal && event.target !== label && event.target !== dynamicStatus && event.target !== img) {
        modal.style.display = 'none';
    }
});