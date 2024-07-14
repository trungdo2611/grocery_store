// add hovered class to selected list item
let list = document.querySelectorAll(".navigation li");

function activeLink() {
  list.forEach((item) => {
    item.classList.remove("hovered");
  });
  this.classList.add("hovered");
}

list.forEach((item) => item.addEventListener("mouseover", activeLink));

// Menu Toggle
let toggle = document.querySelector(".toggle");
let navigation = document.querySelector(".navigation");
let main = document.querySelector(".main");

toggle.onclick = function () {
  navigation.classList.toggle("active");
  main.classList.toggle("active");
};

      const btnAdd = document.querySelector('.btnAdd')
      const modal = document.querySelector('.js-modal')
      const modalClose = document.querySelector('.js-modal-close')
      const modalContainer = document.querySelector('.js-form')

       //hàm hiển thị modal tạo mới(thêm class open vào modal)
        function showCreate() {
            modal.classList.add('open');
        }
        //hàm ẩn modal mua vé (gỡ bỏ class open vào modal)
        function hideCreate() {
            modal.classList.remove('open')
        }
         //hàm  nghe hành vi click
        
         btnAdd.addEventListener('click', showCreate)
        

        // nghe hành vi click vào nút button close
        modalClose.addEventListener('click', hideCreate)
        modal.addEventListener('click', hideCreate)
        modalContainer.addEventListener('click', function (event) {
            event.stopPropagation()
        })

//Hàm hiển thị hình trên form nhập
function showImagePreview(imageUploader,previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}





