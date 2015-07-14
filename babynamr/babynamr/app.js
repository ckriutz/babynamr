// If showStats is turned on, then the Status Controlled will be called. The status controller checks for an index, and if it does not find one will create one.
// If there are no names in the index, it will be populated with some. This is great for getting started, but if you're up and running it will cause extra unneeded cycles.
// Might be handy to see in any case though.
var showStats = true;

var statusUri = 'api/status/Get';
var searchUri = 'api/babyname/Get';

$(document).ready(function () {
    if (showStats) {
        $.getJSON(statusUri)
        .done(function (data) {
            $('#statusBox').html("<h5>Status: " + data.StatusCode + " | # of items: " + data.DocumentCount + " | Storage Size: " + data.StorageSize + "</h5>");
            $('#statusBox').addClass("alert alert-success");
        })
        .error(function (jqXHR, textStatus, errorThrown) {
            $('#statusBox').html(errorThrown);
            $('#statusBox').addClass("alert alert-danger");
        });
    }

    // This is if the user presses enter on the search box.
    $('#txtSearch').keydown(function (event) {
        if (event.keyCode == 13) {
            search($('#txtSearch').val());
            return false;
        }
    });
});

// This is the big search WebApi call. At this point all we do is pass in the search terms to the search API, and then sort them by name.
function search(name) {
    $.getJSON(searchUri + '?q=' + name)
        .done(function (data) {
            $('#dataName').empty();
            $('#resultsFound').html("Results Found: " + data.length);
            for (var i = 0; i < data.length; i++) {
                if (data[i].Document.gender == 'Male') {
                    $('#dataName').append("<div class='row rowboy'><div class='col-md-12'><h3>" + data[i].Document.name + "<br /><small>" + data[i].Document.orgin + "<br>" + data[i].Document.gender + "</br></small></h3>" + data[i].Document.meaning + "</div></div>");
                }
                if (data[i].Document.gender == 'Female') {
                    $('#dataName').append("<div class='row rowgirl'><div class='col-md-12'><h3>" + data[i].Document.name + "<br /><small>" + data[i].Document.orgin + "<br>" + data[i].Document.gender + "</br></small></h3>" + data[i].Document.meaning + "</div></div>");
                }
                if (data[i].Document.gender == 'Both') {
                    $('#dataName').append("<div class='row rowboth'><div class='col-md-12'><h3>" + data[i].Document.name + "<br /><small>" + data[i].Document.orgin + "<br>" + data[i].Document.gender + "</br></small></h3>" + data[i].Document.meaning + "</div></div>");
                }
            }
        })
        .error(function (jqXHR, textStatus, errorThrown) {
            $('#resultsFound').html("Results Found: 0");
            // This is great to see what the search error was, but not a great visual for an end user. Uncomment at your own risk.
            //$('#statusBox').html(errorThrown);
            //$('#statusBox').addClass("alert alert-danger");
        });
}