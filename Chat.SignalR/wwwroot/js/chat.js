"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Function to display a message
connection.on("ReceiveMessage", function (user, message) {
    displayMessage(user, message);
});

// Function to retrieve and display chat messages from cache
function retrieveAndDisplayMessages() {
    fetch("/Home/GetChatMessages")
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(messages => {
            // Display each message
            messages.forEach(message => {
                displayMessage(message.userName, message.message);
            });
        })
        .catch(error => {
            console.error('Error retrieving chat messages:', error);
        });
}

// Function to display a message in the UI
function displayMessage(user, message) {
    var senderName = document.createElement("h6");
    senderName.textContent = user;

    var currentTime = new Date().toLocaleTimeString(); // Get the current time
    var timeElement = document.createElement("span");
    timeElement.textContent = currentTime;
    timeElement.className = " ms-3";
    timeElement.style = "font-size: smaller;";

    var messageBox = document.createElement("div");
    messageBox.className = "row p-1 ms-3 justify-content-start";
    messageBox.style = "border-radius: 15px; background-color: rgba(57, 192, 237,.2);";

    var msgContent = document.createElement("p");
    msgContent.textContent = message;
    messageBox.appendChild(senderName);
    messageBox.appendChild(msgContent);
    messageBox.appendChild(timeElement);

    var msg_with_date = document.createElement("div");
    msg_with_date.className = "row";
    msg_with_date.appendChild(messageBox);

    var MessageCard = document.createElement("div");
    MessageCard.className = "d-flex flex-row mb-4";
    MessageCard.appendChild(msg_with_date);

    document.getElementById("MessageCardContainer").appendChild(MessageCard);
}

// Start the connection and retrieve existing messages
connection.start()
    .then(function () {
        document.getElementById("sendButton").disabled = false;
        // Retrieve and display existing chat messages from cache
        retrieveAndDisplayMessages();
    })
    .catch(function (err) {
        console.error('Error starting SignalR connection:', err.toString());
    });

// Send message when send button is clicked
document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    if (message.trim() !== "") { // Ensure message is not empty
        // Send message to the server
        connection.invoke("SendMessage", message)
            .then(function () {
                // Clear the input field after successful message sending
                document.getElementById("messageInput").value = "";
            })
            .catch(function (err) {
                console.error('Error sending message:', err.toString());
            });
    }
    event.preventDefault();
});

// Retrieve and display existing chat messages from cache when the page initially loads
retrieveAndDisplayMessages();
