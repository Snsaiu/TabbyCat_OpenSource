export function reHeight() {

    setHeight();
    window.addEventListener('resize', function () {
        setHeight();

    });
}

function setHeight() {
    const chatList = document.getElementById('chatcontainer');
    const windowHeight = window.innerHeight;

    if (chatList) {
        chatList.style.height = `${windowHeight - 50}px`;
        console.log('Window height:', windowHeight);
    }
}

