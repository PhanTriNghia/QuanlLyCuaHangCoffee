$(document).ready(function () {
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = 1;

        alert(id + " " + quantity);
        /* $.ajax({
             url: '/shoppingcart/AddToCart',
             type: 'POST',
             data: { id: id, quantity: quantity },
             success: function (rs) {
                 *//*if (rs.Success) {
            $('#checkout_items').html(rs.Count);
            alert(rs.msg);
        }*//*
    }
});*/
    });
});