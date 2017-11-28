function openModal(message) {
    document.getElementById("closeModal").style.display = "none";
    document.getElementById("modalAction").innerHTML = message;
    document.getElementById("modal").style.display = "block";
}

function closeModal() {
    document.getElementById("modal").style.display = "none";
}

function displayResult(message) {
    document.getElementById("modalResult").innerHTML = message;
    document.getElementById("closeModal").style.display = "block";
}

$(document).ready(() => {
    document.getElementById("closeModal").onclick = closeModal;
});

function resetDB() {
    openModal("Resetting DB");
    $.post({
        url: "/Admin/ResetDB",
        success(response) {
            displayResult(response);
        },
        error(response) {
            displayResult(JSON.stringify(response));
        }
    });
}

function takeSnapshots() {
    openModal("Taking Snapshots");
    $.post({
        url: "/Admin/TakeRankSnapshots",
        success(response) {
            displayResult(response);
        },
        error(response) {
            displayResult(JSON.stringify(response));
        }
    });
}