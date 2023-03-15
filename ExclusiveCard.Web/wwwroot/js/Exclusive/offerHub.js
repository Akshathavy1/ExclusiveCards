var OfferHub = (function() {
    "use strict";

    function loadMerchantList(pageNumber) {
        window.location.href = "/OfferHub/Edit?page=" + pageNumber;
    }

    function setupEvents() {
        $("#RefMerchantId").val('');

        $(".btnSave").unbind().click(function (e) {
            e.preventDefault();
            var startDate = $(this).closest('tr').find('.startDate').val();
            var endDate = $(this).closest('tr').find('.endDate').val();
            var id = $(this).attr('id');
            if (startDate === undefined) {
                startDate = null;
            }

            var formData = new FormData();
            formData.append('Id', parseInt(id));
            formData.append('StartDate', startDate);
            formData.append('EndDate', endDate);

            $.ajax({
                url: "/OfferHub/Save",
                beforeSend: function () {
                    $(".spinner").show();
                },
                type: "Post",
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                complete: function () {
                },
                success: function (data) {
                    if (data.data === null && data.errorMessage !== null) {
                        toastr.error(data.errorMessage);
                    }
                    else {
                        $('#divMerchantList').html(data);
                        setupEvents();
                    }
                },

                error: function (xhr, status, error) {
                    toastr.error(xhr.responseText);
                }
            });
        });

        $("#btnAdd").unbind().click(function(e) {
            e.preventDefault();
            var merchant = $("#MerchantId").val();
            if (merchant !== null && merchant !== undefined && merchant !== '' && parseInt(merchant) > 0) {
                var startDate = $('#StartDate').val();
                var endDate = $('#EndDate').val();

                var formData = new FormData();
                formData.append('MerchantId', parseInt(merchant));
                formData.append('StartDate', startDate);
                formData.append('EndDate', endDate);

                $.ajax({
                    url: "/OfferHub/CreateHub",
                    beforeSend: function () {
                        $(".spinner").show();
                    },
                    type: "Post",
                    data: formData,
                    contentType: false,
                    processData: false,
                    cache: false,
                    complete: function () {
                    },
                    success: function (data) {
                        if (data.data === null && data.errorMessage !== null) {
                            toastr.error(data.errorMessage);
                        }
                        else {
                            $('#divMerchantList').html(data);
                            setupEvents();
                        }
                    },

                    error: function (xhr, status, error) {
                        toastr.error(xhr.responseText);
                    }
                });
            } else {
                toastr.error("Please select merchant to continue.");
            }
        });

        //click event of paging for offer hub list
        $("#divMerchantList #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pagenumber');
                loadMerchantList(pageNumber);
            });
        });
    }

    function kendoUIBind() {
        // create ComboBox from select HTML element
        $("#MerchantId").kendoComboBox({
            filter: "contains",
            select: onSelectMerchant
        });
    }

    function onSelectMerchant(e) {
        if (e.dataItem) {
            var dataItem = e.dataItem;
            $("#RefMerchantId").val(dataItem.value);
        } else {
            $("#RefMerchantId").val('');
        }
    }

    function setdefaultValue() {
        $("input:text[name = 'MerchantId_input']").val('');
    }

    return {
        setupEvents: setupEvents,
        kendoUIBind: kendoUIBind,
        setdefaultValue: setdefaultValue
    };
}());