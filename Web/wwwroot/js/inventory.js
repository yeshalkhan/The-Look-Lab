"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/inventoryHub").build();
connection.on("ReceiveInventoryUpdate", function (productId, newStock) {
    // Update the product stock level on the page
    document.getElementById(`product-stock-${productId}`).innerText = `Hurry! only ${newStock} items left`;
});
connection.start().catch(function (err) {
    return console.error(err.toString());
});
