/*
All scripts of the index.html, We added JQUERY to it
 so that the Java script will only run after the page loads.*/

//Setting the map.
let map = L.map('map').setView([0, 0], 1);
L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=LjrLBX4zwd2F8ebL9DTU', {
    attribution: '<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>'
}).addTo(map);
var layerGroup = L.layerGroup().addTo(map);
var markers = new Object();

let redIcon = new L.Icon({
    iconUrl: 'images/airplane.png',
    iconAnchor: [12, 12],
    popupAnchor: [1, 1]
});

let blueIcon = new L.Icon({
    iconUrl: 'images/plane.png',
    iconAnchor: [12, 12],
    popupAnchor: [1, 1]
});


let selectedId = -1;
var latlngs = Array();
var polyline;
var currentMarkId = -1;



$(document).ready(function () {
   
    //Run loopFunc every half second.
    setInterval(loopFunc, 1000);  
    
});

function loopFunc() {
    var date = new Date().toISOString().substr(0, 19) + "Z";
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
                    $("#Myflights").append(`<tr id=${flight.flight_id}><td onclick = getColumnValue(this,0)>` + flight.flight_id + "</td>" + "<td onclick = getColumnValue(this)>" + flight.company_name + "</td>" +
                        "<td onclick = getColumnValue(this)>" + flight.is_external + "</td>" + "<td><button style='font-size:10px' onclick = deleterow1(this)>delete</button></td>" + "</tr>");
                    markOnMap(flight.longitude, flight.latitude, flight.flight_id);

                } else {
                    $("#Externalflights").append(`<tr id=${flight.flight_id}><td onclick = getColumnValue(this,0)>` + flight.flight_id + "</td>" + "<td onclick=getColumnValue(this)>" + flight.company_name + "</td>" +
                        "<td onclick=getColumnValue(this)>" + flight.is_external + "</td>" + "<td onclick=getColumnValue(this)></td>" + "</tr>");
                    markOnMap(flight.longitude, flight.latitude, flight.flight_id);
                }
                markRow(selectedId);
            });
            //remove from details table flights that doesn't exit anymore.
            removeFromDetails();
            //remove markers that the flights doen't exist anymore.
            removeMarkers();
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
    map.removeLayer(markers[firstTd]);
    delete markers[firstTd];
    removePolyline();
    if (currentMarkId === firstTd) {
        currentMarkId = -1;
    }

}
function getColumnValue(e, flag) {
    
    let text;
    if (flag == 1) {
        text = e;
        
    }
    else {
        var row = $(e).closest('tr');
        text = row.find("td:first")[0].innerText;
        /*for (let key in markers) {
            markers[key].setIcon(blueIcon);
            //remove icon from flights list///////////////////////////////////////////////////////////
        }*/
        if (currentMarkId != -1) {
            markers[currentMarkId].setIcon(blueIcon);
        }
        markers[text].setIcon(redIcon);
        currentMarkId = text;
    }  
    selectedId = text;
    
    console.log(text);
    var flightplan = "../api/FlightPlan/" + text;
    $.ajax({
        url: flightplan,
        method: 'GET',
        success: function (flight) {
            var len = flight.segments.length;
            let initialTime = new Date(flight.initial_location.date_time).toUTCString();
            /*var mySubString = initialTime.substring(
                initialTime.lastIndexOf("T") + 1,
                initialTime.lastIndexOf("Z")
            );*/
            let addTime = flight.segments.map(segment => segment.timespan_seconds).reduce((a, b) => a + b, 0);
            let dateFlight = new Date(flight.initial_location.date_time);
            dateFlight.setSeconds(dateFlight.getSeconds() + addTime);
            let arrival = dateFlight.toUTCString();
            var table = document.getElementById("flight-details");
            //If the row isn' already in table add it.
            if (!table.rows[text]) {
                $("#tbodyDetails").empty();
                for (let key in markers) {
                    deleteRowDetails(key);
                    //remove icon from flights list///////////////////////////////////////////////////////////
                }
                $("#flight-details").append(`<tr id=${text}><td>` + text + "</td><td> Longitude: " + flight.initial_location.longitude + "<br/>Latitude: " + flight.initial_location.latitude + "</td><td> Longitude: " + flight.segments[len - 1].longitude + "<br/>Latitude: " + flight.segments[len - 1].latitude + "</td><td>" + initialTime + "</td><td>" + arrival + "</td><td>" + flight.company_name + "</td ><td>" + flight.passengers + "</td></tr > ");
                //add Path Segments
                latlngs = [];
                /////////////////////////////initial location to path
                let pointSeg = L.marker([flight.initial_location.latitude, flight.initial_location.longitude]);
                latlngs.push(pointSeg.getLatLng());
                for (let i in flight.segments) {
                    pointSeg = L.marker([flight.segments[i].latitude, flight.segments[i].longitude]);
                    latlngs.push(pointSeg.getLatLng());
                }
                console.log(latlngs);
                removePolyline();
                //polyline[text] = L.polyline(latlngs, { color: 'red' }).addTo(map);
                polyline = L.polyline(latlngs, { color: 'red' }).addTo(map);
                markRow(selectedId);
               
                

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
            removePolyline();
        }
    }
    
}

function removeMarkers() {
    var tableMyFlight = document.getElementById("Myflights");
    var tableExternalFlight = document.getElementById("Externalflights");
    for (let key in markers) {
        if ((!tableMyFlight.rows[key]) && (!tableExternalFlight.rows[key])) {
            map.removeLayer(markers[key]);
            delete markers[key];
        }       
    }
    if (Object.keys(markers).length === 0) {
        currentMarkId = -1;
    }
}

function markOnMap(longitude, latitude, id) {
    if (markers.hasOwnProperty(id)) {
        markers[id].setLatLng([latitude, longitude]).update();
    } else {
        let marker = L.marker([latitude, longitude]);
        marker.setIcon(blueIcon);
        marker.on("click", function () {
            /*for (let key in markers) {
                markers[key].setIcon(blueIcon);
                //remove icon from flights list///////////////////////////////////////////////////////////
            }*/
            if (currentMarkId != -1) {
                markers[currentMarkId].setIcon(blueIcon);
            }
            marker.setIcon(redIcon);
            currentMarkId = id;
            getColumnValue(id, 1);
        });
        markers[id] = marker;
        
        marker.addTo(layerGroup);
        //layerGroup.clearLayers();
    }  
}

map.on("click", function () {
    for (let key in markers) {
        markers[key].setIcon(blueIcon);
        deleteRowDetails(key);
        //remove icon from flights list///////////////////////////////////////////////////////////
        cleanMarksRows();
        removePolyline();
        selectedId = -1;
    }
});

function markRow(id) {
    if (selectedId != -1) {
        var tableMyFlight = document.getElementById("Myflights");
        var tableExternalFlight = document.getElementById("Externalflights");
        cleanMarksRows();
        if (tableMyFlight.rows[id]) {
            rows = tableMyFlight.getElementsByTagName('tr');
            rows[id].style.backgroundColor = "red";
        }
        if (tableExternalFlight.rows[id]) {
            rows = tableExternalFlight.getElementsByTagName('tr');
            rows[id].style.backgroundColor = "red";
        }
    }
   

}

function cleanMarksRows(){
    var tableMyFlight = document.getElementById("Myflights");
    var tableExternalFlight = document.getElementById("Externalflights");
    var rows = tableMyFlight.getElementsByTagName('tr');
    //iterate through rows.
    for (var i = 1, row; row = tableMyFlight.rows[i]; i++) {
        row.style.backgroundColor = "";
    }
    var rows = tableExternalFlight.getElementsByTagName('tr');
    for (var i = 1, row; row = tableExternalFlight.rows[i]; i++) {
        row.style.backgroundColor = "";
    }
}

function removePolyline(){
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
        //////////////////////////////////////////////////////////////////// Change
        success: function () { alert("success"); },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ": " + jqXHR.status + " " + errorThrown);
        }
    });
});
