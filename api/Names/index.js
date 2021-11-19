const data = require('./names.json');

module.exports = async function (context, req) {
    var randomNames = []
    
    randomNames.push(getRandomName(), getRandomName(), getRandomName(), getRandomName());

    context.res.json(randomNames);
};

const getRandomName = function() {
    const randIndex = Math.floor(Math.random() * data.values.length);
    const randName = data.values[randIndex];
    return randName;
}