var Payment = (function() {
    "use strict";
    var sortField = "Date";
    var sortDir = "desc";
    var sortIcon = "fa fa-sort-alpha-desc";

    function searchTransactions(pageNumber) {
        var partner = $("#PartnerId").find(":selected").val();
        var status = $("#StatusId").find(":selected").val();

        window.location.href = "/Payments/PartnerTransactions?statusId=" +
            status +
            "&partnerId=" +
            partner +
            "&page=" +
            pageNumber +
            "&sortField=" +
            sortField +
            "&sortDirection=" +
            sortDir;
    }

    function updateTransaction(fileId) {
        $.ajax({
            url: "/Payments/UpdateTransaction",
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
                    toastr.success("Transaction updated to paid.");
                    setTimeout(
                        function () {
                            searchTransactions(1);
                        }, 3000);
                } else {
                    toastr.error("Could not update the transaction. Please try again.");
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

        $("#PartnerId").change(function(e) {
            e.preventDefault();
            var selected = $(this).find(":selected").val();
            if (selected === "" || selected === undefined || selected === null) {
                toastr.error("Please select valid partner");
            }
        });

        $("#StatusId").change(function(e) {
            e.preventDefault();
            var selected = $(this).find(":selected").val();
            if (selected === "" || selected === undefined || selected === null) {
                toastr.error("Please select valid payment status");
            }
        });

        $("#btnSearch").click(function(e) {
            e.preventDefault();
            var partner = $("#PartnerId").find(":selected").val();
            var status = $("#StatusId").find(":selected").val();
            if ((partner === "" || partner === undefined || partner === null) &&
                (status === "" || status === undefined || status === null)) {
                toastr.error("Please select valid partner and payment status.");
                return;
            } else if (partner === "" || partner === undefined || partner === null) {
                toastr.error("Please select valid partner");
                return;
            } else if (status === "" || status === undefined || status === null) {
                toastr.error("Please select valid payment status");
                return;
            }
            if (parseInt(partner) > 0 && parseInt(status) > 0) {
                searchTransactions(1);
            }
        });

        $(".transactionPaid").click(function(e) {
            e.preventDefault();
            var fileId = $(this).attr("fileId");
            if (fileId === null || fileId === undefined || fileId === "") {
                toastr.error("File not found");
            } else {
                updateTransaction(fileId);
            }
        });

        $("#divTransactions #pager a").each(function() {
            $(this).unbind().click(function(e) {
                e.preventDefault();
                sortField = $("#SortField").val();
                sortDir = $("#SortDirection").val();
                var pageNumber = $(this).attr('pagenumber');
                searchTransactions(pageNumber);
            });
        });

        $(".transactionHeading").each(function() {
            $(this).click(function(e) {
                e.preventDefault();
                sortField = this.childNodes[1].attributes["SelSortField"].value;

                if ($("#SortField").val() === sortField) {
                    if ($("#SortDirection").val() === "asc") {
                        sortDir = "desc";
                        sortIcon = "fa fa-sort-alpha-desc";
                    } else {
                        sortDir = "asc";
                        sortIcon = "fa fa-sort-alpha-asc";
                    }
                } else {
                    sortDir = "desc";
                }

                if (sortDir === "asc") {
                    if (sortField === "Date")
                        sortIcon = "fa fa-sort-numeric-asc";
                    else
                        sortIcon = "fa fa-sort-alpha-asc";
                }
                else {
                    if (sortField === "Date")
                        sortIcon = "fa fa-sort-numeric-desc";
                    else
                        sortIcon = "fa fa-sort-alpha-desc";
                }
                searchTransactions(1);
            });
        });
    }

    return {
        setUpEvents: setUpEvents
    };
}());