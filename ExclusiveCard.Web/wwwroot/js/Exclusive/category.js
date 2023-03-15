var Category = (function () {
    "use strict";
    var inputsChanged = false;

    function deleteSelectedImage(imageId, merchantId, displayOrder) {
        var formData = new FormData();
        formData.append('Id', imageId);
        formData.append('MerchantId', merchantId);
        formData.append('DisplayOrder', displayOrder);
        formData.append('countryCode', $("#CountryCode").val());

        $.ajax({
            url: "/Category/DeleteSelectedImage",
            type: "POST",
            beforeSend: function () {
                $(".spinner").show(); //$("#wait").show(); 
            },
            data: formData,//{ 'viewModel': formData, 'countryCode': country },
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide(); //$("#wait").hide();
                $('#Feature-file').val(null);
                //$('#btnDeleteFeature').prop('disabled', false);
            },
            success: function (data) {
                if (data.error) {
                    toastr.error(data.ErrorMessage);
                }
                else {
                    inputsChanged = true;
                    $("#selectedImage").html(data);
                    setupEvents();
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function deleteUnselectedImage(imageId, merchantId, displayOrder) {
        var formData = new FormData();
        formData.append('Id', imageId);
        formData.append('MerchantId', merchantId);
        formData.append('DisplayOrder', displayOrder);
        formData.append('countryCode', $("#CountryCode").val());

        $.ajax({
            url: "/Category/DeleteUnselectedImage",
            type: "POST",
            beforeSend: function () {
                $(".spinner").show(); //$("#wait").show(); 
            },
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide(); //$("#wait").hide();
                $('#Disabled-file').val(null);
                //$('#btnDeleteDisable').prop('disabled', false);
            },
            success: function (data) {
                if (data.error) {
                    toastr.error(data.ErrorMessage);
                }
                else {
                    inputsChanged = true;
                    $("#unselectedImage").html(data);
                    setupEvents();
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function addMerchantUnSelectedImage(id, file, countryCode) {
        var categoryId = $("#Id").val();
        var formData = new FormData();
        formData.append('id', categoryId);
        formData.append('merchantId', id);
        formData.append('imageFile', file);
        formData.append('countryCode', countryCode);

        $.ajax({
            url: "/Category/SaveUnselectedImage",
            type: "POST",
            beforeSend: function () {
                $(".spinner").show();
            },
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
                $('#Disabled-file').val(null);
            },
            success: function (data) {
                inputsChanged = true;
                $("#unselectedImage").html(data);
                setupEvents();
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function addMerchantSelectedImage(id, file, countryCode) {
        var categoryId = $("#Id").val();
        var formData = new FormData();
        formData.append('id', categoryId);
        formData.append('merchantId', id);
        formData.append('imageFile', file);
        formData.append('countryCode', countryCode);

        $.ajax({
            url: "/Category/SaveSelectedImage",
            type: "POST",
            beforeSend: function () {
                $(".spinner").show();
            },
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
                $('#Feature-file').val(null);
            },
            success: function (data) {
                inputsChanged = true;
                $("#selectedImage").html(data);
                setupEvents();
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function setupEvents() {
        $('button').unbind().click(function () {
            if ($(this).hasClass('navi') === false) {
                $(this).toggleClass('navi');
            }
        });

        $('#btnSaveCategory').click(function (e) {
            e.preventDefault();
            var id = $("#Id").val();
            var name = $('#Name').val();
            var parentId = $('#ParentId').val();
            var merchantId = $("#FeatureMerchantId").val();
            if (merchantId === undefined || merchantId === null || merchantId === '') {
                merchantId = null;
            }

            var formData = new FormData();
            formData.append('Id', id);
            formData.append('Name', name);
            formData.append('ParentId', parentId);
            formData.append('FeatureMerchantId', merchantId);

            $.ajax({
                url: "/Category/Save",
                type: "POST",
                beforeSend: function () {
                    $(".spinner").show();
                },
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                complete: function () {
                    $(".spinner").hide();
                    $('#Feature-file').val(null);
                },
                success: function (data) {
                    if (data.success) {
                        toastr.success("Saved Successfully");
                        setTimeout(function () {
                            window.location.href = "/Category/List";
                        }, 1500);
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                }
            });
        });

        $(document).on("click", "input:text[name='FeatureMerchantId_input']", function () {
            $("input:text[name='FeatureMerchantId_input']").val('');
        });

        $(document).on("click", "input:text[name='ParentId_input']", function () {
            $("input:text[name='ParentId_input']").val('');
        });

        $("#btnDeleteFeature").unbind().click(function (e) {
            e.preventDefault();
            if ($(this).hasClass('navi') === false) {
                $(this).toggleClass('navi');
            }
            var imageId = $(this).attr('merchantimageid');
            var merchantId = $(this).attr('merchantid');
            var displayOrder = 1;
            $(this).prop('disabled', true);
            deleteSelectedImage(imageId, merchantId, displayOrder);
        });

        $("#btnDeleteDisable").unbind().click(function (e) {
            e.preventDefault();
            if ($(this).hasClass('navi') === false) {
                $(this).toggleClass('navi');
            }
            var imageId = $(this).attr('merchantimageid');
            var merchantId = $(this).attr('merchantid');
            var displayOrder = 1;
            $(this).prop('disabled', true);
            deleteUnselectedImage(imageId, merchantId, displayOrder);
        });

        $("#FeatureMerchantId").change(function (e) {
            e.preventDefault();
            var id = $(this).val();
            if (id === undefined || id === null || id === '') {
                $('#btnAddFeature').prop('disabled', true);
                $('#btnAddDisabled').prop('disabled', true);
            } else if (parseInt(id) > 0) {
                var image = $("#selectedImage").find('img').attr('src');
                if (image === undefined || image === null) {
                    $('#btnAddFeature').prop('disabled', false);
                }
                var image1 = $("#unselectedImage").find('img').attr('src');
                if (image1 === undefined || image1 === null) {
                    $('#btnAddDisabled').prop('disabled', false);
                }
            }
        });

        $("#CountryCode").change(function(e) {
            e.preventDefault();
            var id = $("#Id").val();
            var country = $(this).val();

            $.ajax({
                url: "/Category/GetFeature?id=" + id + '&countryCode=' + country,
                type: "Get",
                beforeSend: function () {
                    $(".spinner").show(); //$("#wait").show(); 
                },
                data: null,
                contentType: false,
                processData: false,
                cache: false,
                complete: function () {
                    $(".spinner").hide(); //$("#wait").hide();
                    $('#Feature-file').val(null);
                    $('#Disabled-file').val(null);
                },
                success: function (data) {
                    if (data.error) {
                        toastr.error(data.ErrorMessage);
                    }
                    else {
                        inputsChanged = false;
                        $("#feature").html(data);
                        setupEvents();
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                }
            });
        });
    }

    function kendoUIBind() {
        // create ComboBox from select HTML element
        $("#ParentId").kendoComboBox({
            filter: "contains",
            select: onSelectParent
        });

        $("#FeatureMerchantId").kendoComboBox({
            filter: "contains"
        });
    }

    function onSelectParent(e) {
        if (e.dataItem) {
            var dataItem = e.dataItem;
            $("#ParentId").val(dataItem.value);
        } else {
            $("#ParentId").val('');
        }
    }

    function onSelectMerchant(e) {
        if (e.dataItem) {
            var dataItem = e.dataItem;
            $("#FeatureMerchantId").val(dataItem.value);
        } else {
            $("#FeatureMerchantId").val('');
        }
    }

    return {
        setupEvents: setupEvents,
        kendoUIBind: kendoUIBind,
        addMerchantSelectedImage: addMerchantSelectedImage,
        addMerchantUnSelectedImage: addMerchantUnSelectedImage
    };
}());