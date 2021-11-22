// First, lets grab some of the items on the page to use...
var meaning = document.getElementById("meaning");
var orgin = document.getElementById("orgin");
var name1 = document.getElementById("name1");
var name2 = document.getElementById("name2");
var name3 = document.getElementById("name3");

var nameMeaning = "";
var nameOrgin = "";
var nameGender = "";

async function getNames() {
    await fetch('/api/names')
    .then(response => response.json())
    .then(
        anime.timeline({loop: false})
        .add({
            targets: '.ml4 .letters-3',
            opacity: 0,
            scale: 3,
            duration: 200,
            easing: "easeInExpo",
            delay: 10
        })
    )
    .then(data => { 
        console.log(data)
        name1.innerHTML = data[0].name;
        name2.innerHTML = data[1].name;
        name3.innerHTML = data[2].name;

        nameMeaning = data[2].meaning;
        nameOrgin = data[2].orgin;
        nameGender = data[2].gender;
    })
    .then(data => {
        var ml4 = {};
        ml4.opacityIn = [0,1];
        ml4.scaleIn = [0.2, 1];
        ml4.scaleOut = 3;
        ml4.durationIn = 200;
        ml4.durationOut = 200;
        ml4.delay = 10;

        anime.timeline({loop: false})
        .add({
            targets: '.ml4 .letters-3',
            opacity: 0,
            scale: ml4.scaleOut,
            duration: ml4.durationOut,
            easing: "easeInExpo",
            delay: ml4.delay
        }).add({
            targets: '.ml4 .letters-1',
            opacity: ml4.opacityIn,
            scale: ml4.scaleIn,
            duration: ml4.durationIn
        }).add({
            targets: '.ml4 .letters-1',
            opacity: 0,
            scale: ml4.scaleOut,
            duration: ml4.durationOut,
            easing: "easeInExpo",
            delay: ml4.delay
        }).add({
            targets: '.ml4 .letters-2',
            opacity: ml4.opacityIn,
            scale: ml4.scaleIn,
            duration: ml4.durationIn
        }).add({
            targets: '.ml4 .letters-2',
            opacity: 0,
            scale: ml4.scaleOut,
            duration: ml4.durationOut,
            easing: "easeInExpo",
            delay: ml4.delay
        }).add({
            targets: '.ml4 .letters-3',
            opacity: ml4.opacityIn,
            scale: ml4.scaleIn,
            duration: ml4.durationIn,
            complete: function() {
                meaning.innerHTML= nameMeaning
                orgin.innerHTML= nameOrgin
                document.getElementById("btnAgain").style.visibility = "visible";

                changeNameColor(nameGender);
            }
        });
    });
}

const again = function() {

    // Clear Everything.
    meaning.innerHTML= "";
    orgin.innerHTML= "";
    document.getElementById("btnAgain").style.visibility = "hidden";

    // Get New Names
    getNames();

}

const changeNameColor = function(gender) {
    // Change the color of the name depending on gender.
    if (gender == "Female") {
        name3.style.color = "#EA4C89";
    }
    if (gender == "Male") {
        name3.style.color = "#2980B9";
    }
    if (gender == "Both")
    {
        name3.style.color = "#F1C40F";
    }
}

const spin = function() {
    var ml4 = {};
    ml4.opacityIn = [0,1];
    ml4.scaleIn = [0.2, 1];
    ml4.scaleOut = 3;
    ml4.durationIn = 200;
    ml4.durationOut = 200;
    ml4.delay = 10;

    anime.timeline({loop: false})
    .add({
        targets: '.ml4 .letters-3',
        opacity: 0,
        scale: ml4.scaleOut,
        duration: ml4.durationOut,
        easing: "easeInExpo",
        delay: ml4.delay
    }).add({
        targets: '.ml4 .letters-1',
        opacity: ml4.opacityIn,
        scale: ml4.scaleIn,
        duration: ml4.durationIn
    }).add({
        targets: '.ml4 .letters-1',
        opacity: 0,
        scale: ml4.scaleOut,
        duration: ml4.durationOut,
        easing: "easeInExpo",
        delay: ml4.delay
    }).add({
        targets: '.ml4 .letters-2',
        opacity: ml4.opacityIn,
        scale: ml4.scaleIn,
        duration: ml4.durationIn
    }).add({
        targets: '.ml4 .letters-2',
        opacity: 0,
        scale: ml4.scaleOut,
        duration: ml4.durationOut,
        easing: "easeInExpo",
        delay: ml4.delay
    }).add({
        targets: '.ml4 .letters-3',
        opacity: ml4.opacityIn,
        scale: ml4.scaleIn,
        duration: ml4.durationIn,
        complete: function() {
            meaning.innerHTML= nameMeaning
            orgin.innerHTML= nameOrgin
            document.getElementById("btnAgain").style.visibility = "visible";

            changeNameColor(nameGender);
        }
    });
}

getNames();