var File = (function() {
    "use strict";

    function updateStatus(fileId) {
        $.ajax({
            url: "/File/Update",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: { fileId: fileId },
            cache: false,
            dataType: "json",
            complete: function () {
                $(".spinner").hide();
            },
            success: function (data) {
                if (data.success) {
                    toastr.success("File data updated to sent.");
                    setTimeout(
                        function () {
                            $("#btnSearch").click();
                        }, 3000);
                } else {
                    toastr.error("Could not update the file data. Please try again.");
                }
            },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function setUpEvents() {
        $('a, button').click(function () {
            $(this).toggleClass('active');
        });

      $("#btnSearch").click(function(e) {
            e.preventDefault();
            var page =1;
            var dateFrom = $("#CreatedFrom").val();
            var dateTo = $("#CreatedTo").val();
            var state = $("#State").val();
            var type = $("#Type").val();
            if (state === "" || state === undefined || state === null) {
                state = 0;
            }
            if (type === "" || type === undefined || type === null) {
                type = null;
            }
            if (dateFrom === "" || dateFrom === undefined || dateFrom === null) {
              dateFrom = null;
            }
            if (dateTo === "" || dateTo === undefined || dateTo === null) {
              dateTo = null;
            }
            var url = "/File/Index/?page=" + page + '&state=' + state;
            if (dateFrom != null) {
              url = url + '&dateFrom=' + dateFrom;
            }
            if (dateTo != null) {
               url = url + '&dateTo=' + dateTo;
            }
            if (type !== null) {
                url = url + '&fileType=' + type;
            }
            window.location.href = url;
        });

      $("#divTransactions #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pagenumber');
                var dateFrom = $("#CreatedFrom").val();
                var dateTo = $("#CreatedTo").val();
                var state = $("#State").val();
                var type = $("#Type").val();
                if (state === "" || state === undefined || state === null) {
                    state = 0;
                }
                if (type === "" || type === undefined || type === null) {
                    type = null;
                }
                if (dateFrom === "" || dateFrom === undefined || dateFrom === null) {
                  dateFrom = null;
                }
                if (dateTo === "" || dateTo === undefined || dateTo === null) {
                  dateTo = null;
                }
                var url = "/File/Index/?page=" + pageNumber + '&state=' + state;
                if (dateFrom != null) {
                  url = url + '&dateFrom=' + dateFrom;
                }
                if (dateTo != null) {
                  url = url + '&dateTo=' + dateTo;
                }
                if (type !== null) {
                    url = url + '&fileType=' + type;
                }
                window.location.href = url;
            });
        });

        $(".fileSent").click(function (e) {
            e.preventDefault();
            var fileId = $(this).attr("fileId");
            if (fileId === null || fileId === undefined || fileId === "") {
                toastr.error("File not found");
            } else {
                updateStatus(fileId);
            }
        });
    }

    return {
        setUpEvents: setUpEvents
    }
}());