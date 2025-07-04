"use strict";

// Create a SignalR Hub connections
let connection = new signalR.HubConnectionBuilder().withUrl("/event-hub").build();

// Subscribe to the PublishEvent messages from the hub
// Just append them to the events list
connection.on("PublishEvent", function (aggregate_id, event, eventType) {
    let li = document.createElement("li");
    document.getElementById("eventList").appendChild(li);
    let eventContent = JSON.stringify(event);
    li.textContent = `${aggregate_id} - ${eventType}: ${eventContent}`;
});

// When the connection is opened, generate a random package ID
// and write it to the input field.
connection.start().then(function () {
    document.getElementById("package_id_input").value = crypto.randomUUID();
}).catch(function (err) {
    return console.error(err.toString());
});

// Add an event listener for the "Create Package" button
document.getElementById("createPackage").addEventListener("click", function (event) {
    // Get the data
    let aggregate_id = document.getElementById("package_id_input").value;
    let capacity = parseInt(document.getElementById("package_capacity_input").value);
    // Post a SignalR message on the hub to subscribe to the aggregate ID.
    connection.invoke("SubscribeToAggregate", aggregate_id).catch(function (err) {
        return console.error(err.toString());
    });
    // Create the command
    let command = {
        "boxId": aggregate_id,
        "desiredNumberOfSpots": capacity
    }
    // Post the command
    postCommand("/api/Command/Box/create", command);
    event.preventDefault();
});

document.getElementById("addLabel").addEventListener("click", function (event) {
    // Get the data
    let aggregate_id = document.getElementById("package_id_input").value;
    let carrier = parseInt(document.querySelector('input[name="label_carrier_input"]:checked').value);
    let code = document.getElementById("label_code_input").value;
    // Create the command
    let command = {
        "boxId": aggregate_id,
        "label": {
            "carrier": carrier,
            "trackingCode": code
        }
    }
    // Post the command
    postCommand("/api/Command/Box/add-label", command);
    event.preventDefault();
});

function postCommand(endpoint, command) {
    fetch(endpoint, {
        method: "POST",
        body: JSON.stringify(command),
        headers: {
            "Content-type": "application/json; charset=UTF-8"
        }
    }).then(_ => {});
}