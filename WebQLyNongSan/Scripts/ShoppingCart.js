function toast({
    title = '',
    message = '',
    type = 'info',
    duration = 0
}) {
    const main = document.getElementById('toast');
    if (main) {
        const toast = document.createElement('div');


        //auto remove toast
        const autoRemoveId = setTimeout(function () {
            main.removeChild(toast);
        }, duration + 1000);
        //remove toast when clicked
        toast.onclick = function (e) {
            if (e.target.closest('.toast__close')) {
                main.removeChild(toast);
                clearTimeout(autoRemoveId);
            }
        }

        const icons = {
            success: 'fa-sharp fa-solid fa-check',
            info: 'fa-solid fa-info',
            warn: 'fa-sharp fa-solid fa-exclamation',
            error: 'fa-sharp fa-solid fa-exclamation',
        };

        const icon = icons[type];
        const delay = (duration / 1000).toFixed(2);

        toast.classList.add('toast', `toast--${type}`);
        toast.style.animation = `slideInLeft ease .6s, FadeOut linear 1s ${delay}s forwards`;
        toast.innerHTML = `
        <div class="toast__icon">
            <i class="${icon}"></i>
        </div>
        <div class="toast__body">
            <h3 class="toast__title">${title}</h3>
            <p class="toast__msg">${message}</p>
        </div>
        <div class="toast__close">
            <i class="fa-solid fa-xmark"></i>
        </div
        `;
        main.appendChild(toast);

    }
}


$(document).ready(function () {
    ShowCount();
    getPartialCart();
    $('body').on('click', '.btnAddCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = 1;
        var tQuantity = $('#quantity-value').val();
        if (tQuantity > 0) {
            quantity = tQuantity;
        }
        var model = {};
        model.id = id;
        model.quantity = quantity;
        $.ajax({
            url: "/ShoppingCart/AddToCart",
            type: 'POST',
            data: JSON.stringify(model),
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response.Success) {
                    $('.count__item').html(response.Count);
                    toast({
                        title: 'Thành công !!',
                        message: response.msg,
                        type: 'success',
                        duration: 2000
                    })
                    getPartialCart();
                }
            }
        });
    })


    $('body').on('click', '#btnUpdateCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = $('#Quantity_' + id).val();
        UpdateCart(id, quantity);
    })

    $('body').on('click', '#btnDeleteCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var model = {};
        model.id = id;

        swal({
            title: "Xác nhận xóa ?",
            text: `Bạn chắc chắn muốn xóa sản phẩm này không ?`,
            icon: "warning",
            buttons: {
                cancel: {
                    text: "Không",
                    value: null,
                    visible: true,
                    className: "",
                    closeModal: true,
                },
                confirm: {
                    text: "Có",
                    value: true,
                    visible: true,
                    className: "",
                    closeModal: true
                }
            }
        }).then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: "/ShoppingCart/Delete",
                    type: 'POST',
                    data: JSON.stringify(model),
                    datatype: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        if (response.Success) {
                            $('.count__item').html(response.Count);
                            $('#trow-' + id).remove();
                            getPartialCart();
                        }
                    }
                });
            }
        });
    })
});

function ShowCount() {
    $.ajax({
        url: "/ShoppingCart/ShowCount",
        type: 'GET',
        datatype: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('.count__item').html(response.Count);
        }
    });
}


function getPartialCart() {
    $.ajax({
        url: "/ShoppingCart/PartialCart",
        type: 'GET',
        datatype: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('.shopping-cart--display').empty().append(response);
        }
    });
}

function UpdateCart(id,quantity) {
    $.ajax({
        url: "/ShoppingCart/Update",
        type: 'POST',
        data: {id: id, quantity: quantity},
        success: function (response) {
            if (response.Success) {
                window.location.href = "/ShoppingCart/Index";                             
            }
        }
    });
}