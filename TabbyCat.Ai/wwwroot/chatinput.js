export function reWidth() {

    setWidth();
    window.addEventListener('resize', function () {
        setWidth();

    });
}
function setWidth() {
    const bottomDiv = document.getElementById('bottom-div');
    const area = document.getElementById('textarea');
    const sendBtn = document.getElementById('sendBtn');
    if (bottomDiv && bottomDiv.parentElement) {
        const parentWidth = bottomDiv.parentElement.clientWidth;
        bottomDiv.style.width = `${parentWidth}px`;
        if (area) {
            area.style.width = `${parentWidth - 50}px`;
        }
        if (sendBtn) {
            sendBtn.style.width = '50px';
            sendBtn.style.height = `${area.clientHeight}px`;
        }

        console.log('Parent width:', parentWidth);
    }
}

export function clearTextArea() {
    const textArea = document.getElementById("textarea");
    if (textArea) {
        textArea.value = "";
    }
}



export function setInputHeight() {
    const textarea = document.getElementById("textarea");
    const bottomDiv = document.getElementById('bottom-div');
    if (textarea) {
        textarea.addEventListener('focus', function () {
            bottomDiv.style.bottom = "300px";
            console.log("focus");
        });

        textarea.addEventListener('blur', function () {
            bottomDiv.style.bottom = "45px";
            consle.log("blur");
        });
    }


}


export function textAreaListener (dotnetobj){
    const textarea = document.getElementById("textarea");
    function handleEnter(event) {
        if(event.key==="Enter")
        {
            event.preventDefault();
            dotnetobj.invokeMethodAsync("TextSend",textarea.value)
        }
    }
    
    if(textarea)
    {
        textarea.addEventListener("keydown",handleEnter)
    }
}