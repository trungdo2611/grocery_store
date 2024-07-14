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
    initFavorite();
});

function initFavorite() {
    if ($('.favorite').length) {
        var favs = $('.favorite');
        favs.each(function () {
            var fav = $(this);
            var active = false;
            if (fav.hasClass('activeHeart')) {
                active = true;
            }
            fav.on('click', function () {
                var id = $(this).data('id');
                if (active) {
                    fav.removeClass('activeHeart');
                    active = false;
                    DeleteWishList(id);
                } else {
                    fav.addClass('activeHeart');
                    active = true;                  
                    AddWishList(id);
                }
            })
        })
    }
}

function AddWishList(id) {
    $.ajax({
        url: '/Wishlist/PostWishlist',
        type: 'POST',
        data: { ProductID: id },
        success: function (response) {
            if (response.Success) {
                toast({
                    title: 'Thành công !!',
                    message: "Thêm thành công sản phẩm vào mục ưa thích",
                    type: 'success',
                    duration: 2000
                })
            } else {
                alert(response.msg);
            }         
        }
        })
}

function DeleteWishList(id) {
    $.ajax({
        url: '/Wishlist/DeleteWishlist',
        type: 'POST',
        data: { ProductID: id },
        success: function (response) {
            if (response.Success) {
                toast({
                    title: 'Thành công !!',
                    message: "Xóa thành công ra khỏi mục ưa thích",
                    type: 'success',
                    duration: 2000
                })
            } else {
                alert(response.msg);
            }
        }
    })
}