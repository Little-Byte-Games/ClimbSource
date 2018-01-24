function createExhibition(challengerID, challengedID) {
    $.post("/Sets/Exhibition", {
        challengerID: challengerID,
        challengedID: challengedID
    }).done((data) => {
        location.href = "/Sets/Fight/" + data.id;
    }).fail(response => {
        alert("Could not create exhibition fight!\n" + response.responseText);
    });
}