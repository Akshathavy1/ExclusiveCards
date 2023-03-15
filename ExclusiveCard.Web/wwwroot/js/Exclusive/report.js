var Report = (function() {
    "use strict";
    var sortField = "Date";
    var sortDir = "desc";
    var sortIcon = "fa fa-sort-alpha-desc";

    function searchTransactions(pageNumber) {
        var startDate = $("#StartDate").val();
        var endDate = $("#EndDate").val();
        var partner = $("#PartnerId").find(":selected").val();
        //var type = $("#ReportType").find(":selected").val();

        window.location.href = "/Report/?sortField=" +
            sortField +
            "&sortDirection=" +
            sortDir +
            "&partnerId=" +
            partner +
            "&startDate=" +
            startDate +
            "&endDate=" +
            endDate +
            "&page=" +
            pageNumber;
    }

    function searchCustomerPartnerWithdrawal(pageNumber) {
        var startDate = $("#StartDate").val();
        var endDate = $("#EndDate").val();
        //var type = $("#ReportType").find(":selected").val();

        window.location.href = "/Report/PartnerWithdrawal?startDate=" +
            startDate +
            "&endDate=" +
            endDate +
            "&page=" +
            pageNumber;
    }

    function setUpEvents() {
        //Spinner on button click
        $('a, button').click(function () {
            $(this).toggleClass('active');
        });

        //Search for data based on the criteria for TAM investment
        $("#btnSearch").click(function (e) {
            e.preventDefault();
            searchTransactions(1);
            $(this).toggleClass('active');
        });

        //Pagination for Payments
        $("#divTransactions #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                sortField = $("#SortField").val();
                sortDir = $("#SortDirection").val();
                var pageNumber = $(this).attr('pagenumber');
                searchTransactions(pageNumber);
            });
        });

        //Sorting in Payments
        $(".transactionHeading").each(function () {
            $(this).click(function (e) {
                e.preventDefault();
                sortField = this.childNodes[1].attributes["SelSortField"].value;

                if ($("#SortField").val() === sortField) {
                    if ($("#SortDirection").val() === "asc") {
                        sortDir = "desc";
                    } else {
                        sortDir = "asc";
                    }
                } else {
                    sortDir = "desc";
                }

                if (sortDir === "asc") {
                    if (sortField === "CreatedDate" || sortField === "PaidDate")
                        sortIcon = "fa fa-sort-numeric-asc";
                    else
                        sortIcon = "fa fa-sort-alpha-asc";
                }
                else {
                    if (sortField === "CreatedDate" || sortField === "PaidDate")
                        sortIcon = "fa fa-sort-numeric-desc";
                    else
                        sortIcon = "fa fa-sort-alpha-desc";
                }
                searchTransactions(1);
            });
        });

        //Search for data based on the criteria for Customer TAM withdrawals
        $("#btnSearchWithdraw").click(function (e) {
            e.preventDefault();
            searchCustomerPartnerWithdrawal(1);
            $(this).toggleClass('active');
        });

        //Pagination for Customer TAM withdrawals
        $("#divPartnerWithdrawals #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                sortField = $("#SortField").val();
                sortDir = $("#SortDirection").val();
                var pageNumber = $(this).attr('pagenumber');
                searchCustomerPartnerWithdrawal(pageNumber);
            });
        });
    }

    return {
        setUpEvents: setUpEvents
    };
}());