function addMatch() {
    var matches = document.getElementById("matches");
    for (var i = 0; i < matches.children.length; i++) {
        var match = matches.children[i];
        if (match.style.display === "none") {
            match.style.display = "block";
            break;
        }
    }
}

function removeMatch(index) {
    var matches = document.getElementById("matches");
    var match = matches.children[index];
    match.style.display = "none";
}

function scoreUpdated() {
    var player1Score = 0;
    var player2Score = 0;

    var player1Scores = document.getElementsByClassName("player-score-1");
    var player2Scores = document.getElementsByClassName("player-score-2");

    for (var i = 0; i < player1Scores.length; i++) {
        if (player1Scores[i].style.display === "none") {
            continue;
        }

        if (player1Scores[i].value > player2Scores[i].value) {
            player1Score++;
        } else if (player1Scores[i].value < player2Scores[i].value) {
            player2Score++;
        }
    }

    document.getElementById("player-1-score").innerHTML = player1Score;
    document.getElementById("player-2-score").innerHTML = player2Score;

    document.getElementById("submit-button").disabled = player1Score === player2Score;
}
