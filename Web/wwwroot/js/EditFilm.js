let checkboxForVideo = document.getElementById("checkboxForVideo");
let checkboxForImage = document.getElementById("checkboxForImage");


let VideoFileFieldset = document.getElementById("VideoFileFieldset");
let ImageFileFieldset = document.getElementById("ImageFileFieldset");




function setOrUnsetInputForVideo() {
    if (checkboxForVideo.checked) {
        VideoFileFieldset.disabled = false;
    }
    else {
        VideoFileFieldset.disabled = true;
    }
}

function setOrUnsetInputForImage() {
    if (checkboxForImage.checked) {
        ImageFileFieldset.disabled = false;
    }
    else {
        ImageFileFieldset.disabled = true;
    }
}
checkboxForImage.addEventListener("click", setOrUnsetInputForImage);
checkboxForVideo.addEventListener("click", setOrUnsetInputForVideo);