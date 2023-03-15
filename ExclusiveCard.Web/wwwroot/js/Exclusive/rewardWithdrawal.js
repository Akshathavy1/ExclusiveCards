var RewardWithdrawal = (function () {
    "use strict";
    var sortField = "CustomerName";
    var sortDir = "asc";
    var sortIcon = "fa fa-sort-alpha-asc";

    function searchWithdrawals(pageNumber) {
        var partner = $("#PartnerId").find(":selected").val();
        var status = $("#StatusId").find(":selected").val();

        window.location.href = "/Payments/RewardWithdrawal?statusId=" +
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

    function updateWithdrawals(withdrawalId) {
        $.ajax({
            url: "/Payments/UpdateRewardWithdrawal",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: { withdrawalId: withdrawalId },
            cache: false,
            dataType: "json",
            complete: function () {
                $(".spinner").hide();
            },
            success: function (data) {
                if (data.success) {
                    toastr.success("Withdrawal updated to paid.");
                    setTimeout(
                        function () {
                            searchWithdrawals(1);
                        }, 3000);
                } else {
                    toastr.error("Could not update the withdrawal. Please try again.");
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

        $("#PartnerId").change(function (e) {
            e.preventDefault();
            var selected = $(this).find(":selected").val();
            if (selected === "" || selected === undefined || selected === null) {
                toastr.error("Please select valid partner");
            }
        });

        $("#StatusId").change(function (e) {
            e.preventDefault();
            var selected = $(this).find(":selected").val();
            if (selected === "" || selected === undefined || selected === null) {
                toastr.error("Please select valid payment status");
            }
        });

        $("#btnSearch").click(function (e) {
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
                searchWithdrawals(1);
            }
        });

        $(".withdrawPaid").click(function (e) {
            e.preventDefault();
            var withdrawalId = $(this).attr("rewardId");
            if (withdrawalId === null || withdrawalId === undefined || withdrawalId === "") {
                toastr.error("Partner reward withdrawal request not found");
            } else {
                updateWithdrawals(withdrawalId);
            }
        });

        $("#divWithdrawals #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                sortField = $("#SortField").val();
                sortDir = $("#SortDirection").val();
                var pageNumber = $(this).attr('pagenumber');
                searchWithdrawals(pageNumber);
            });
        });

        $(".transactionHeading").each(function () {
            $(this).click(function (e) {
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

                searchWithdrawals(1);
            });
        });
    }

    return {
        setUpEvents: setUpEvents
    };
}());