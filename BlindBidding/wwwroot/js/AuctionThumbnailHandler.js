window.addEventListener("load", function () {

    var data = new FormData();

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

            $.ajax({
            type: "POST",
            url: "/Auction/UploadFiles",
            contentType: false,
            processData: false,
            data: data
        });
    });
},false);