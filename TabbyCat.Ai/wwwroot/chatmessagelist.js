export function reHeight(marginBottom) {

    setHeight(marginBottom);
    window.addEventListener('resize', function () {
        setHeight(marginBottom);

    });
}

function setHeight(marginBottom) {
    const chatList = document.getElementById('messagesContainer');
    const windowHeight = window.innerHeight;

    if (chatList) {
        chatList.style.height = `${windowHeight - marginBottom}px`;
        console.log('Window height:', windowHeight);
        console.log('Chat list height:', chatList.style.height);
    }
}


export function scrollToBottom() {
    const chatList = document.getElementById('messagesContainer');
    if (chatList) {
        chatList.scrollTop = chatList.scrollHeight;
    }
}