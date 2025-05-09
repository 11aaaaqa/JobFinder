const elements = document.querySelectorAll('.format-number');

elements.forEach(el => {
    const text = el.textContent;

    const formattedText = text.replace(/(\d+)/g, (match) => {
        const number = parseInt(match, 10);
        return number.toLocaleString('ru-RU');
    });

    el.textContent = formattedText;
});