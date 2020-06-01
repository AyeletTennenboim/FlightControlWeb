/* All scripts of the index.html, We added JQUERY to it
 so that the Java script will only run after the page loads.*/

// Setting the map.
let map = L.map('map').setView([0, 0], 1);
L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=LjrLBX4zwd2F8ebL9DTU', {
    attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>'
}).addTo(map);
var layerGroup = L.layerGroup().addTo(map);
var markers = new Object();

// Set new Icon of yellow airplane for click.
let clickedIcon = new L.Icon({
    iconUrl: 'images/airplane.png',
    iconAnchor: [12, 12],
    popupAnchor: [1, 1]
});

// Set new icon of black airplane when not clicked.
let blackIcon = new L.Icon({
    iconUrl: 'images/plane.png',
    iconAnchor: [12, 12],
    popupAnchor: [1, 1]
});

// Id of flight that was selected, -1 if no flight is selected.
let selectedId = -1;
// Array of all segments of flight.
var latlngs = Array();
var polyline;
var currentMarkId = -1;

// Java script will only run after the page loads.
$(document).ready(function () {   
    // Run loopFunc each second.
    setInterval(loopFunc, 3000);
});

function loopFunc() {
    // The date and time now.
    var date = new Date().toISOString().substr(0, 19) + "Z";
    console.log(date);//////////////////////////////////////////////////Delete/
    // Request to all flights in this date.
    var flightUrl = "../api/Flights?relative_to=" + date + "&sync_all";
    // Get requst to all flights in the current date.
    $.ajax({
        url: flightUrl,
        method: 'GET',
        success: function (data) {
            console.log(data);
            // Initialize the table before add the current flights.
            intializeTable();
            data.forEach(function (flight) {
                // If the flight isn't external appand to "my flights" table.
                if (flight.is_external === false) {
                    $("#Myflights").append(`<tr id=${flight.flight_id}><td onclick = getColumnValue(this,0)>` +
                        flight.flight_id + "</td>" + "<td onclick = getColumnValue(this,0)>" + flight.company_name +
                        "</td>" + "<td onclick = getColumnValue(this,0)>" + flight.is_external +
                        "</td>" + "<td><button style='font-size:10px' onclick = deleterow1(this)>delete</button></td>" + "</tr>");
                    // Mark this flight on Map.
                    markOnMap(flight.longitude, flight.latitude, flight.flight_id);
                 // If the flight is external appand to "External flights" table.
                } else {
                    $("#Externalflights").append(`<tr id=${flight.flight_id}><td onclick = getColumnValue(this,0)>` +
                        flight.flight_id + "</td>" + "<td onclick=getColumnValue(this,0)>" + flight.company_name + "</td>" +
                        "<td onclick=getColumnValue(this,0)>" + flight.is_external + "</td>" +
                        "<td onclick=getColumnValue(this,0)></td>" + "</tr>");
                    // Mark this flight on Map.
                    markOnMap(flight.longitude, flight.latitude, flight.flight_id);
                }
                // Mark this flight in the sutable table.
                markRow(selectedId);
            });
            // Remove from details table flights that doesn't exit anymore.
            removeFromDetails();
            // Remove markers that the flights doen't exist anymore.
            removeMarkers();
        },
        // Show an error message.
        error: function (jqXHR) {
            let message = "Unable to get flights from server";
            showErrorMessage(jqXHR.status, message);
        }
    }); 
}

// This function get the current td of delete button that was selected and remove this flight.
function deleterow1(el) {
    // Get the row of the td that was clicked.
    var row = $(el).closest('tr');
    
    // Get flight Id.
    var firstTd = row.find("td:first")[0].innerText;
    // Delete request.
    var urlDelete = "../api/Flights/" + firstTd;
    // Remove from the server.
    $.ajax({
        url: urlDelete,
        method: 'delete',
        success: function () {
            // Delete the previous error message.
            $('#errorsWindow').text("");
            // Remove row.
            row.remove();
            // Delete the flight from Details table if exist.
            deleteRowDetails(firstTd);
            // Remove mark of this flight from map.
            map.removeLayer(markers[firstTd]);
            delete markers[firstTd];
            // Delete polyline only if the deleted row is selected.
            if (firstTd === selectedId) {
                removePolyline();
            }
            // If the current marked flight is the flight which deleted - flag -1
            // There is no flight that is selected.
            if (currentMarkId === firstTd) {
                currentMarkId = -1;
                selectedId = -1;
            }
        },
         // Show an error message.
        error: function (jqXHR) {
            let message = "Unable to delete flight";
            showErrorMessage(jqXHR.status, message);
        }
    });
}

/* This function get td that was selected in flights tables or id and flag
   that signs if we clicked flight from tables or from icon on map.*/
function getColumnValue(e, flag) {
    // Flight id.
    let text;
    // If we clicked flight on map-we got the id flight.
    if (flag == 1) {
        text = e;      
    }
    // If we clicked flight on tables.
    else {
        // Get the row of the selected td.
        var row = $(e).closest('tr');
        // Get flight id from table.
        text = row.find("td:first")[0].innerText;
        // If there is flight that already selected-set it's icon to black ("not selected" anymore).
        /*if (currentMarkId != -1) {
            markers[currentMarkId].setIcon(blackIcon);
        }
        // Mark current flight that was selected with "clicked icon".
        markers[text].setIcon(clickedIcon);
        // Update current flight Id that is marked.
        currentMarkId = text;*/
    }
    // Update flight id which selected.
    //selectedId = text;
    // Get flight plan if this current flight that was selected.
    var flightplan = "../api/FlightPlan/" + text;
    $.ajax({
        url: flightplan,
        method: 'GET',
        success: function (flight) {
            if (currentMarkId != -1) {
                markers[currentMarkId].setIcon(blackIcon);
            }
            // Mark current flight that was selected with "clicked icon".
            markers[text].setIcon(clickedIcon);
            // Clean row that is marked before.
            cleanMarksRows();
            // Update current flight Id that is marked.
            currentMarkId = text;
            // Update flight id which selected.
            selectedId = text;
            // Mark row of this selected flight in table.
            markRow(selectedId);
            // Delete the previous error message.
            $('#errorsWindow').text("");
            var len = flight.segments.length;
            // Initial time of current flight in utc.
            let initialTime = new Date(flight.initial_location.date_time).toUTCString();
            // Remove GMT from string.
            let initialSubString = initialTime.substring(0, initialTime.indexOf("G"));
            // Calculate timespan from all segments.
            let addTime = flight.segments.map(segment => segment.timespan_seconds).reduce((a, b) => a + b, 0);
            let dateFlight = new Date(flight.initial_location.date_time);
            // Add timespan from all segments to initial time.
            dateFlight.setSeconds(dateFlight.getSeconds() + addTime);
            let arrival = dateFlight.toUTCString();
            // Remove GMT from string.
            let arrivalSubString = arrival.substring(0, arrival.indexOf("G"));
            var table = document.getElementById("flight-details");
            // If the row isn't already in details table add it.
            if (!table.rows[text]) {
                // Remove from details table the previous flight.
                $("#tbodyDetails").empty();
                // Delete selected row from table.
                /*for (let key in markers) {
                    deleteRowDetails(key);
                }*/
                deleteRowDetails(currentMarkId);
                // Append selected flight to details table.
                $("#flight-details").append(`<tr id=${text}><td>` + text + "</td><td> Longitude: " + flight.initial_location.longitude + "<br/>Latitude: " + flight.initial_location.latitude + "</td><td> Longitude: " + flight.segments[len - 1].longitude + "<br/>Latitude: " + flight.segments[len - 1].latitude + "</td><td>" + initialSubString + "</td><td>" + arrivalSubString + "</td><td>" + flight.company_name + "</td ><td>" + flight.passengers + "</td></tr > ");
                // Initial points segments
                latlngs = [];
                // Insert point segment of initial point.
                let pointSeg = L.marker([flight.initial_location.latitude, flight.initial_location.longitude]);
                latlngs.push(pointSeg.getLatLng());
                // For each segment insert it's location to lastlng array.
                for (let i in flight.segments) {
                    pointSeg = L.marker([flight.segments[i].latitude, flight.segments[i].longitude]);
                    latlngs.push(pointSeg.getLatLng());
                }
                // Remove previous polyline.
                removePolyline();
                // Add current polyline.
                polyline = L.polyline(latlngs, { color: 'red' }).addTo(map);
                // Mark row of this selected flight in table.
                //markRow(selectedId);

            } 
        },
         // Show an error message.
        error: function (jqXHR) {
            let message = "Unable to get flight plan details";
            showErrorMessage(jqXHR.status, message);
        }
    });  
}

// Remove all data in flights tables exept the first row of the head.
function intializeTable() {
    $('#Myflights tr:gt(0)').remove();
    $('#Externalflights tr:gt(0)').remove();
}

// Remove from details table by flight id.
function deleteRowDetails(flightId) {
    var table = document.getElementById("flight-details");
    // Delete flight id that we got from details table if exist.
    if (table.rows[flightId]) {
        var rowIndex = document.getElementById(flightId).rowIndex;
        table.deleteRow(rowIndex);
    }
}

// Remove from details table if flight isn't in tables anymore.
function removeFromDetails() {
    var table = document.getElementById("flight-details");
    var tableMyFlight = document.getElementById("Myflights");
    var tableExternalFlight = document.getElementById("Externalflights");
    // Iterate through rows in details table.
    for (var i = 2, row; row = table.rows[i]; i++) {
        // Get flight id of each row.
        let id = row.cells[0].innerText;
        // If flight id isn't on one of the tables delete row from details and remove polyline.
        if ((!tableMyFlight.rows[id]) && (!tableExternalFlight.rows[id])) {
            deleteRowDetails(id);
            removePolyline();
            if (selectedId === id) {
                selectedId = -1;
                currentMarkId = -1;
            }
        }
    }
}

// This function remove markers from map if flights doesn't exits in tables anymore.
function removeMarkers() {
    var tableMyFlight = document.getElementById("Myflights");
    var tableExternalFlight = document.getElementById("Externalflights");
    // Iterate all markers in map and remove if flight id isn't in tables anymore.
    for (let key in markers) {
        if ((!tableMyFlight.rows[key]) && (!tableExternalFlight.rows[key])) {
            map.removeLayer(markers[key]);
            delete markers[key];
            if (currentMarkId === key) {
                currentMarkId = -1;
                selectedId = -1;
            }
        }       
    }
    // Update current mark to -1 if there is no more markers on map.
    if (Object.keys(markers).length === 0) {
        currentMarkId = -1;
        selectedId = -1;
    }
}

// This function get location and flight id and add/update marker in map.
function markOnMap(longitude, latitude, id) {
    // If marker is already added-update it's location.
    if (markers.hasOwnProperty(id)) {
        markers[id].setLatLng([latitude, longitude]).update();
        // If marker isn't added- add it now with it's location and flight id as key.
    } else {
        let marker = L.marker([latitude, longitude]);
        // Set the new marker as "not selected" black icon.
        marker.setIcon(blackIcon);
        // Add function on click to marker.
        marker.on("click", function () {
            // If there is marker which mark now set it back to black.
            /*if (currentMarkId != -1) {
                markers[currentMarkId].setIcon(blackIcon);
            }
            // Set this marker we clicked on to yellow "clicked icon".
            marker.setIcon(clickedIcon);
            // Update current marker to current flight id.
            currentMarkId = id;*/
            // Send this flight id with flag 1(means that we mark marker and not a row)
            // to function which will mark also the row in table and add flight details to table.
            getColumnValue(id, 1);
        });
        // Add new marker to markers object with id flight as key.
        markers[id] = marker;       
        marker.addTo(layerGroup);
    }  
}

// Function click on when click on map.
map.on("click", function () {
    if (currentMarkId != -1) {
        // set current marker on map to black "not selected".
        markers[currentMarkId].setIcon(blackIcon);
        // delete row details.
        deleteRowDetails(currentMarkId);
        // clean mark rows in table.
        cleanMarksRows();
        //remove polyline.
        removePolyline();
        // Update that no id was selected.
        selectedId = -1;
        currentMarkId = -1;//////////////////////////////
    }
    
});

// This function get flight id and mark row in the suitable table.
function markRow(id) {
    // If there is id that was selected.
    if (selectedId != -1) {
        var tableMyFlight = document.getElementById("Myflights");
        var tableExternalFlight = document.getElementById("Externalflights");
        // Clean row that is marked before.
        //cleanMarksRows();
        // If the id that we want to mark is on my flights table- set the suitable row to red.
        if (tableMyFlight.rows[id]) {
            rows = tableMyFlight.getElementsByTagName('tr');
            rows[id].style.backgroundColor = "red";
        }
        // If the id that we want to mark is on external flights table- set the suitable row to red.
        if (tableExternalFlight.rows[id]) {
            rows = tableExternalFlight.getElementsByTagName('tr');
            rows[id].style.backgroundColor = "red";
        }
    }
}

// Clean red mark rows that was selected.
function cleanMarksRows() {
    var tableMyFlight = document.getElementById("Myflights");
    var tableExternalFlight = document.getElementById("Externalflights");
    var rows = tableMyFlight.getElementsByTagName('tr');
    // Iterate through rows and clean their background.
    if (tableMyFlight.rows[selectedId]) {
        rows = tableMyFlight.getElementsByTagName('tr');
        rows[selectedId].style.backgroundColor = "";
    }
    if (tableExternalFlight.rows[selectedId]) {
        rows = tableExternalFlight.getElementsByTagName('tr');
        rows[selectedId].style.backgroundColor = "";
    }
    /*for (var i = 1, row; row = tableMyFlight.rows[i]; i++) {
        row.style.backgroundColor = "";
    }
    var rows = tableExternalFlight.getElementsByTagName('tr');
    for (var i = 1, row; row = tableExternalFlight.rows[i]; i++) {
        row.style.backgroundColor = "";
    }*/
}

// Remove polyline if it was add to map and initialize it.
function removePolyline() {
    if (map.hasLayer(polyline)) {
        map.removeLayer(polyline);
        polyline = {};
    }
}

// Post flight plan to the server.
$('#add-flight-plan-button').on('click', function (e) {
    let addFlightUrl = "../api/FlightPlan";
    let flightPlan = document.getElementById('flight-plan').files[0];
    $.ajax({
        type: 'POST',
        url: addFlightUrl,
        processData: false,
        contentType: false,
        data: flightPlan,
        dataType: "json",
        contentType: "application/json",
        // Delete the previous error message.
        success: function () { $('#errorsWindow').text("") },
        // Show an error message.
        error: function (jqXHR) {
            let message = "Invalid flight plan file selected. Please check your file and try again";
            showErrorMessage(jqXHR.status, message);
        }        
    });
});

// Show an error message when an action fails.
function showErrorMessage(errorStatusCode, message) {
    if (errorStatusCode != 0) {
        $('#errorsWindow').text("Error: " + message + "\n" + "(Status Code: " + errorStatusCode + ")");
    } else {
        $('#errorsWindow').text("Error: Server connection failure.\n Try reconnecting.");
    }
}
