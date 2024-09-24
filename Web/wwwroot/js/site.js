
window.onscroll = function () {
    var arrow = document.querySelector('.arrow');
    if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
        arrow.classList.add('show');
    } else {
        arrow.classList.remove('show');
    }
};

function showMessage(msg) {
    var message = document.getElementById("message");
    message.innerText = msg;
    message.className = "message show";
    setTimeout(function () {
        message.className = "message";
    }, 2000); // Hide after 2 seconds
};

function addToCart(button) {
    var productId = $(button).data('product-id');
    var quantity = document.getElementById("quantity").value;
    $.ajax({
        type: 'POST',
        url: '/Cart/AddToCart',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: { productId: productId, quantity: quantity },
        success: function (result) {
            showMessage(result);
        },
        error: function (jqXHR) {
            console.error('Error details:', jqXHR);
            showMessage('Could not add product to cart');
        }
    })
};

function updateQuantity(index, input) {
    const newQuantity = parseInt(input.value) || 0;
    const originalQuantity = parseInt(input.getAttribute('data-original-quantity')) || 0;
    const pricePerUnit = parseInt(input.getAttribute('data-price')) || 0;

    // Calculate the change in quantity
    const quantityDifference = newQuantity - originalQuantity;

    // Update the total price
    const totalPriceElement = document.getElementById('total-price');
    let currentTotalPrice = parseInt(totalPriceElement.textContent) || 0;
    currentTotalPrice += quantityDifference * pricePerUnit;
    totalPriceElement.textContent = currentTotalPrice;

    // Update the hidden total price field
    document.getElementById('total-hidden').value = currentTotalPrice;

    // Update the hidden quantitiesString field
    const quantities = Array.from(document.querySelectorAll('.quantity-input')).map(input => input.value);
    document.querySelector('input[name="quantitiesString"]').value = JSON.stringify(quantities);

    // Update the data attributes for the next change
    input.setAttribute('data-original-quantity', newQuantity);
    input.setAttribute('data-original-price', newQuantity * pricePerUnit);
}

function deleteFromCart(button) {
    var productId = $(button).data('product-id');
    $.ajax({
        type: 'POST',
        url: '/Cart/DeleteFromCart',
        data: { productId: productId },
        success: function (result) {
            $("#cart").load(location.href + " #cart");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error details:', jqXHR);
            alert('Error: ' + jqXHR.responseText);
        }
    });
};

function addProduct() {
    var form = document.getElementById('add-product-form');
    var formData = new FormData(form);

    $.ajax({
        type: 'POST',
        url: '/Products/AddProduct',
        data: formData,
        processData: false,  // Prevent jQuery from automatically transforming the data into a query string
        contentType: false,  // Let the browser set the content type
        success: function (response) {
            showMessage('Product added successfully');
            form.reset();  // Reset the form fields
            $('#image-validation').html(''); // Clear image validation error message if any
            $('span.text-danger').html(''); // Clear any other validation error messages
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('Error details:', jqXHR.responseText);

            // Update the form with server-side validation errors
            if (jqXHR.status === 400) {
                var response = JSON.parse(jqXHR.responseText);
                var validationSummary = $('div.asp-validation-summary');
                validationSummary.html('');
                $.each(response, function (key, value) {
                    var errorContainer = $('[name="' + key + '"]').next('span');
                    errorContainer.html(value[0]);
                });
                // Handle image validation errors
                if (response.image) {
                    $('#image-validation').html(response.image[0]);
                }
            } else {
                showMessage('Could not add product');
            }
        }
    });
};

// Search button
$(document).ready(function () {
    $('#search-symbol').click(function (e) {
        e.preventDefault(); // Prevent default form submission

        var data = $('#search-field').val();

        // Perform AJAX request
        $.ajax({
            url: '/Products/Search',
            method: 'GET',
            data: { searchterm: data },
            success: function (result) {
                // Replace main content with search results
                $('#main-content').html(result);
            },
            error: function (error) {
                console.error('Error:', error); // Log any errors to the console
            }
        });
    });

    $('#admin-search-symbol').click(function (e) {
        e.preventDefault(); // Prevent default form submission

        var data = $('#search-field').val();

        // Perform AJAX request
        $.ajax({
            url: '/Products/AdminSearch',
            method: 'GET',
            data: { searchterm: data },
            success: function (result) {
                // Replace body with search results
                $('body').html(result);
            },
            error: function (error) {
                console.error('Error:', error); // Log any errors to the console
            }
        });
    });

});

