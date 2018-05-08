window.addEventListener("load", function () {

    var data = new FormData();

    var image = null;

    $("#fileBasket").on("dragenter", function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
    });

    $("#fileBasket").on("dragover", function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
    });

    $("#fileBasket").on("drop", function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
        var files = evt.originalEvent.dataTransfer.files;
        var fileNames = "";
        if (files.length > 0) {
            fileNames += "Uploading <br/>";
            for (var i = 0; i < files.length; i++) {
                fileNames += files[i].name + "<br />";
            }
        }

        for (var j = 0; j < files.length; j++) {
            data.append(files[j].name, files[j]);
        }

        $("#fileBasket").html('<img src="#" id="auction-thumbnail-img" class="img-responsive"/>');

       var reader = new FileReader();

       reader.onloadend = function () {
           image = reader.result;
           alert(reader.result);
           $("#auction-thumbnail-img").attr("src", reader.result);
       }

       if (files[0]) {
           reader.readAsDataURL(files[0]);
       } else {
           $("#auction-thumbnail-img").attr("src", "");
       }
    });

    $('#add-auction-btn').click(function () {

        var title = $("#title").val();
        var duration = $("#duration").val();
        var description = $("#description").val();
        var category = $("#category").val();

        var validator = true;

        var alphNum = /^[a-zA-Z0-9]+$/;
        var numOnly = /^[0-9]+$/;

        validator = alphNum.test(title);
        validator = alphNum.test(description);
        validator = title.length > 3;
        validator = title.length < 50;
        validator = description.length > 6;
        validator = description.length < 5000;
        validator = duration.length < 2;
        validator = numOnly.test(duration);

        if (image !== null&&validator) {


            var formData = { "Title": title, "Duration": duration, "Description": description, "Category": category, "Data": image };

            $.ajax({
                type: 'POST',
                url: '/Auction/UploadFiles',
                dataType: 'JSON',
                contentType: 'application/json; charset=utf-8',
                //contentType: false,
                //processData: false,
                data: JSON.stringify(formData)
            });
        }

    });
},false);