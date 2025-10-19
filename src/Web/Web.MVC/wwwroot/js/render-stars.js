function renderStars(rating, blockToFill) {
    blockToFill.innerHTML = '';

    for (let i = 1; i <= 5; i++) {
        const star = document.createElement('div');
        star.className = 'rating-star';

        const emptyStar = document.createElement('div');
        emptyStar.className = 'empty';
        emptyStar.innerHTML = '★';

        const fullStar = document.createElement('div');
        fullStar.className = 'full';
        fullStar.innerHTML = '★';

        if (i <= Math.floor(rating)) {
            fullStar.style.width = '100%';
        } else if (i === Math.ceil(rating) && rating % 1 !== 0) {
            fullStar.style.width = (rating - Math.floor(rating)) * 100 + '%';
        } else {
            fullStar.style.width = '0%';
        }

        star.appendChild(emptyStar);
        star.appendChild(fullStar);
        blockToFill.appendChild(star);
    }
}

const blocksToRender = document.querySelectorAll('.rating-stars');
blocksToRender.forEach(block => {
    const ratingValue = parseFloat(block.getAttribute('data-rating-value'));
    renderStars(ratingValue, block);
});