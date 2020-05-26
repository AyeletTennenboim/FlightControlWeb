/*
All scripts of the index.html, We added JQUERY to it
 so that the Java script will only run after the page loads.*/

//Setting the map.
let map = L.map('map').setView([0, 0], 1);
L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=LjrLBX4zwd2F8ebL9DTU', {
    attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>'
}).addTo(map);
var layerGroup = L.layerGroup().addTo(map);



$(document).ready(function () {
   
    //Run loopFunc every half second.
    setInterval(loopFunc, 1000);  
    
});

function loopFunc() {
    var date = new Date().toISOString().substr(0, 19);
    console.log(date);
    var flightUrl = "../api/Flights?relative_to=" + date + "&sync_all";
    console.log(flightUrl);
    $.ajax({
        url: flightUrl,
        method: 'GET',
        success: function (data) {
            
            console.log(data);
            intializeTable();
            data.forEach(function (flight) {
                if (flight.is_external === false) {
                    $("#Myflights").append(`<tr id=${flight.flight_id}><td onclick = getColumnValue(this)>` + flight.flight_id + "</td>" + "<td onclick = getColumnValue(this)>" + flight.company_name + "</td>" +
                        "<td onclick = getColumnValue(this)>" + flight.is_external + "</td>" + "<td><button style='font-size:10px' onclick = deleterow1(this)>delete</button></td>" + "</tr>");
                    markOnMap(flight.longitude, flight.latitude);
                } else {
                    $("#Externalflights").append(`<tr id=${flight.flight_id}><td onclick = getColumnValue(this)>` + flight.flight_id + "</td>" + "<td onclick=getColumnValue(this)>" + flight.company_name + "</td>" +
                        "<td onclick=getColumnValue(this)>" + flight.is_external + "</td>" + "<td><button style='font-size:10px' onclick = deleterow1(this)>delete</button></td>" + "</tr>");
                    markOnMap(flight.longitude, flight.latitude);
                }
            });
            removeFromDetails();
        }
    });
   
}



function deleterow1(el) {
    var row = $(el).closest('tr');
    row.remove();
    //get flight Id
    var firstTd = row.find("td:first")[0].innerText;
    var urlDelete = "../api/Flights/" + firstTd;
    console.log(urlDelete);
    //remove from the server
    $.ajax({
        url: urlDelete,
        method: 'delete'
    });
    deleteRowDetails(firstTd);
}
function getColumnValue(e) {
    var row = $(e).closest('tr');
    let text = row.find("td:first")[0].innerText;
    console.log(text);
    var flightplan = "../api/FlightPlan/" + text;
    $.ajax({
        url: flightplan,
        method: 'GET',
        success: function (flight) {
            var len = flight.segments.length;
            let initialTime = flight.initial_location.date_time;
            var mySubString = initialTime.substring(
                initialTime.lastIndexOf("T") + 1,
                initialTime.lastIndexOf("Z")
            );
            var table = document.getElementById("flight-details");
            if (!table.rows[text]) {
                $("#flight-details").append(`<tr id=${text}><td>` + text + "</td><td> Longitude: " + flight.initial_location.longitude + "<br/>Latitude: " + flight.initial_location.latitude + "</td><td> Longitude: " + flight.segments[len - 1].longitude + "<br/>Latitude: " + flight.segments[len - 1].latitude + "</td><td>" + mySubString + "</td><td>" + flight.company_name + "</td ><td>" + flight.passengers + "</td></tr > ");
            }
            
            

            
        }
    });


    //for verification purposes
   
}

function intializeTable() {
    $('#Myflights tr:gt(0)').remove();
    //$("#tbodyMyFlights").empty();
    $('#Externalflights tr:gt(0)').remove();
}

//remove from details
function deleteRowDetails(flightId) {
    var table = document.getElementById("flight-details");
    if (table.rows[flightId]) {
        var rowIndex = document.getElementById(flightId).rowIndex;
        table.deleteRow(rowIndex);
    }
}

//Remove from details table if flight isn't in tables anymore.
function removeFromDetails() {
    var table = document.getElementById("flight-details");
    var tableMyFlight = document.getElementById("Myflights");
    var tableExternalFlight = document.getElementById("Externalflights");
    //iterate through rows.
    for (var i = 2, row; row = table.rows[i]; i++) {
        // get flight id of each row.
        let id = row.cells[0].innerText;
        // if flight id isn't on one of the tables delete row from details.
        if ((!tableMyFlight.rows[id]) && (!tableExternalFlight.rows[id])) {
            deleteRowDetails(id);
        }
    }
}

function markOnMap(longitude, latitude) {
    layerGroup.clearLayers();
    let marker = L.marker([longitude, latitude]).addTo(layerGroup);
}


