var Withdraw = (function() {
    "use strict";
    var sortField = "CustomerName";
    var sortDir = "asc";
    var sortIcon = "fa fa-sort-alpha-asc";

    function searchWithdrawals(pageNumber) {
        var status = $("#StatusId").find(":selected").val();
        var startDate = $("#StartDate").val();
        var endDate = $("#EndDate").val();

        window.location.href = "/Payments/CustomerWithdrawal?statusId=" +
            status +
            "&page=" +
            pageNumber +
            "&sortField=" +
            sortField +
            "&sortDirection=" +
            sortDir +
            "&startDate=" +
            startDate +
            "&endDate=" +
            endDate;

  }

    //
    function financialReport(pageNumber) {
      var status = $("#StatusId").find(":selected").val();
      var startDate = $("#StartDate").val();
      var endDate = $("#EndDate").val();

      window.location.href = "/Payments/FinancialReport?page="+
        pageNumber +
        "&startDate=" +
        startDate +
        "&endDate=" +
        endDate;
    }
    //

    function updateWithdrawals(withdrawalId) {
        $.ajax({
            url: "/Payments/UpdateWithdrawal",
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
            
            var status = $("#StatusId").find(":selected").val();
            if (status === "" || status === undefined || status === null) {
                toastr.error("Please select valid payment status");
                return;
            }
            if (parseInt(status) > 0) {
                searchWithdrawals(1);
            }
        });

        $("#btnFinancialRptSearch").click(function (e) {
          e.preventDefault();

          financialReport(1);
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

        $("#divFinancialReport #pager a").each(function () {
          $(this).unbind().click(function (e) {
            e.preventDefault();
            sortField = $("#SortField").val();
            sortDir = $("#SortDirection").val();
            var pageNumber = $(this).attr('pagenumber');
            financialReport(pageNumber);
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