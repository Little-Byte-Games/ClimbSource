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