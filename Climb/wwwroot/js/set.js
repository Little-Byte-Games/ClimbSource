function addMatch() {
    var matches = $("#matches");
    for (var i = 0; i < matches.children().length; i++) {
        if (matches.children().eq(i).hidden) {
            matches.children().eq(i).hidden = false;
            continue;
        }
    }
}