﻿function toggleMembership(membership) {
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
        data: { leagueID: leagueID },
        success: function (response) {
            var season = JSON.parse(response);
            location.href = "/Seasons/Home/" + season.ID;
        },
        error: function (response) {
            alert("Can't create season\n" + response.responseText);
        }
    });
}

function createLeague() {
    var form = $("#new-league-form");
    $.post({
        url: "/Leagues/Create",
        data: form.serialize(),
        success: response => {
            location.href = "/Leagues/Home/" + response.id;
        },
        error: function (response) {
            alert("Can't create league\n" + response.responseText);
        }
    });
}