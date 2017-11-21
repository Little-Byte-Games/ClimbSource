function getSeasonStatus(seasonID) {
    $("#season-" + seasonID).addClass("active").siblings().removeClass("active");

    $.get({
        url: "/Seasons/GetStatus",
        data: { id: seasonID }
    }).done(data => {
        var status = JSON.parse(data);
        document.getElementById("seasonStatus.totalSetCount").innerText = status.totalSetCount;
        document.getElementById("seasonStatus.completedCount").innerText = status.completedCount;
        document.getElementById("seasonStatus.availableCount").innerText = status.availableCount;
        document.getElementById("seasonStatus.overdueCount").innerText = status.overdueCount;
    });
}

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