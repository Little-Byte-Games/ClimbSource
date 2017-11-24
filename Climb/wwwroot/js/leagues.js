function toggleMembership(membership) {
    var memberships = document.getElementById("memberships").children;
    var i;
    for (i = 0; i < memberships.length; i++) {
        memberships[i].style.display = "none";
    }

    var membershipButtons = document.getElementById("membershipButtons").children;
    for (i = 0; i < memberships.length; i++) {
        membershipButtons[i].classList.remove("active");
    }

    document.getElementById(membership).style.display = "block";


    document.getElementById("membershipButtons." + membership).classList.add("active");
}

function createSeason(leagueID) {
    $.post({
        url: "/Seasons/CreateAndStart",
        data: { leagueID: leagueID }
    }).done((data, statusText, xhr) => {
        if (xhr.status === 200) {
            location.href = "/Seasons/Home/" + data.id;
        }
    });
}