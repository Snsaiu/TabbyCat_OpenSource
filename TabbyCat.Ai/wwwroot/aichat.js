export function reWidth() {
    setWidth();
    window.addEventListener('resize', function () {
        setWidth();
    });
}

function setWidth() {
    const header = document.getElementById("chat-header");

    if (header && header.parentElement) {
        const width = header.parentElement.clientWidth;
        header.style.width = `${width}px`;
    }
}

