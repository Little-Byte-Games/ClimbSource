var actionTime = 0;
var isRunning = false;

setInterval(function () {
    if (!isRunning) return;
    actionTime++;
    document.getElementById("modal-time").innerHTML = actionTime + "s";
}, 1000);

function openModal(message) {
    actionTime = 0;
    isRunning = true;

    document.getElementById("closeModal").style.display = "none";
    document.getElementById("modalAction").innerHTML = message;
    document.getElementById("modal").style.display = "block";
}

function closeModal() {
    document.getElementById("modal").style.display = "none";
}

function displayResult(message) {
    isRunning = false;

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

function sendSetReminders() {
    openModal("Sending Set Reminders");
    $.post({
        url: "/Admin/SendSetReminders",
        data: {
            key: "steve"
        },
        success(response) {
            displayResult(response);
        },
        error(response) {
            displayResult(JSON.stringify(response));
        }
    });
}

function updateFeatureToggles() {
    openModal("Updating Feature Toggles");
    var form = $("#feature-toggle-form");
    $.post({
        url: form.attr("action"),
        data: form.serialize(),
        success(response) {
            displayResult(response);
        },
        error(response) {
            displayResult(JSON.stringify(response));
        }
    });
}