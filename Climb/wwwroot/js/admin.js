function resetDB() {
    openModal("Resetting DB");
    $.post({
        url: "/Admin/ResetDB"
    }).done(result => {
        displayResult(result);
    });
}

function openModal(message) {
    document.getElementById('closeModal').style.display = "none";
    document.getElementById('modalAction').innerHTML = message;
    document.getElementById('modal').style.display = "block";
}

function closeModal() {
    document.getElementById('modal').style.display = "none";
}

function displayResult(message) {
    document.getElementById('modalResult').innerHTML = message;
    document.getElementById('closeModal').style.display = "block";
}

$(document).ready(() => {
    document.getElementById('closeModal').onclick = closeModal;
});