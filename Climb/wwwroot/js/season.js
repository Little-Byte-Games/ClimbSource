function endSeason(seasonID) {
    $.post({
        url: "/Seasons/EndSeason",
        data: { id: seasonID }
    }).done((data, statusText, xhr) => {
        if (xhr.status === 200) {
            location.reload();
        }
    });
}

function goToSeason(seasonID) {
    window.location.href = "/Seasons/Home/" + seasonID;
}

function leaveSeason(leagueID) {
    var form = $("#leave-form");
    $.post({
        url: form.attr("action"),
        data: form.serialize(),
        success: () => {
            location.href = "/Leagues/Home/" + leagueID;
        },
        error: () => {
            alert("Could not leave season.");
        }
    });
}

function joinSeason() {
    var form = $("#join-form");
    $.post({
        url: form.attr("action"),
        data: form.serialize(),
        success: () => {
            location.reload();
        },
        error: () => {
            alert("Could not join season.");
        }
    });
}