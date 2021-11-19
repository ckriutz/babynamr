// First, lets grab some of the items on the page to use...
var meaning = document.getElementById("meaning");
var orgin = document.getElementById("orgin");
var name1 = document.getElementById("name1");
var name2 = document.getElementById("name2");
var name3 = document.getElementById("name3");

// Three test names to use until we attach the API
var names = [
    {
        "id": "2350",
        "name": "Misty",
        "orgin": "English",
        "gender": "Female",
        "meaning": "Thin fog"
    },
    {
        "id": "2403",
        "name": "Natan",
        "orgin": "Hebrew",
        "gender": "Male",
        "meaning": "God has given"
    },
    {
        "id": "2806",
        "name": "Stacy",
        "orgin": "Greek",
        "gender": "Male",
        "meaning": "Fruitful; good harvest"
    }
]

// Here is where we will get teh names from the API to display.
name1.innerHTML = names[0].name;
name2.innerHTML = names[1].name;
name3.innerHTML = names[2].name;

// The orgin and meaning also...
var nameMeaning = names[2].meaning;
var nameOrgin = names[2].orgin;
var nameGender = names[2].gender;

const again = function() {

    // Clear Everything.
    meaning.innerHTML= "";
    orgin.innerHTML= "";
    document.getElementById("btnAgain").style.visibility = "hidden";

    console.log("no u");
    spin();
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

var ml4 = {};
ml4.opacityIn = [0,1];
ml4.scaleIn = [0.2, 1];
ml4.scaleOut = 3;
ml4.durationIn = 200;
ml4.durationOut = 200;
ml4.delay = 10;

anime.timeline({loop: false})
  .add({
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
        // Fill in the data.
        meaning.innerHTML = nameMeaning;
        orgin.innerHTML= nameOrgin;
        document.getElementById("btnAgain").style.visibility = "visible";

        changeNameColor(nameGender);
    }
  });