const paragraphs = document.querySelectorAll('p.format-number');

paragraphs.forEach(p => {
    const text = p.textContent;

    const formattedText = text.replace(/(\d+)/g, (match) => {
        const number = parseInt(match, 10);
        return number.toLocaleString('ru-RU');
    });

    p.textContent = formattedText;
});