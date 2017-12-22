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
        data: { leagueID: leagueID },
        success: function (response) {
            var season = JSON.parse(response);
            location.href = "/Seasons/Home/" + season.ID;
        },
        error: function (response) {
            alert(JSON.stringify(response));
        }
    });
}

function onUpdateUserProfilePicClick() {
    var form = $("#user-profile-pic-form");
    $.post({
        url: "/Users/UploadProfilePic",
        data: new FormData(form[0]),
        //data: form.serialize(),
        processData: false,
        contentType: false,
        success: function () {
            location.reload();
        },
        error: function (response) {
            alert(response.statusText + "\n" + response.responseText);
        }
    });
}