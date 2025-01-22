export function reHeight() {

    setHeight();
    window.addEventListener('resize', function () {
        setHeight();

    });
}

function setHeight() {
    const container = document.getElementById('container');
    const windowHeight = window.innerHeight;

    if (container) {
        container.style.height = `${windowHeight}px`;
    }
}

