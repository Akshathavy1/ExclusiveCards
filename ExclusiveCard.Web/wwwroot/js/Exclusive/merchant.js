var Merchant = (function () {
    "use strict";
    var currentPage = $("#CurrentPageNumber").val();
    var formOriginaldata;
    var currentmerchantId;
    var socialMediaLinks = [];
    var currentbranchId;
    var inputsChanged = false;
    var sortfield = "MerchantName";
    var sortdir = "asc";
    var sorticon = "fa fa-sort-alpha-desc";

    function errorHighlight(e, message) {
        $(".validation-summary-valid").html(message);
    }

    function initialiseFormData() {

        formOriginaldata = $("#btnSaveMerchants").closest("form").serialize();
    }

    //Function for saving merchant and continue to add branch -winston
    function saveMerchant() {
        socialMediaLinks = [];
        //to get social Media Items
        var children = $("#SocialMediaItems").children();
        $.each(children,
            function (i, item) {
                var subChildren = $(item).children();
                var mediaItem = new Object();
                mediaItem.SocialMediaCompanyId = subChildren[0].value;
                var children = $(subChildren[2]).children();
                mediaItem.Name = "";
                mediaItem.URI = children[0].value;
                socialMediaLinks.push(mediaItem);
            });
        //

        //Id
        var id = $("#Id").val();
        var name = $("#Name").val();
        var shortDescription = $("#ShortDescription").val();
        var longDescription = $("#LongDescription").val();
        var webSite = $("#WebSite").val();
        var contactDetailId = $("#ContactDetailId").val();
        var landlinePhone = $("#LandlinePhone").val();
        var mobilePhone = $("#MobilePhone").val();
        var emailAddress = $("#EmailAddress").val();
        var color = $("#BrandColor").val();
        var featureChecked = $("#FeatureImageOfferText").is(':checked');
        var item = JSON.stringify(socialMediaLinks);
        var formData = new FormData();
        formData.append('Id', id);
        formData.append('Name', name);
        formData.append('ShortDescription', shortDescription);
        formData.append('LongDescription', longDescription);
        formData.append('WebSite', webSite);
        //formData.append('SocialMediaLinks', item);
        formData.append('ContactDetailId', contactDetailId);
        formData.append('LandlinePhone', landlinePhone);
        formData.append('MobilePhone', mobilePhone);
        formData.append('EmailAddress', emailAddress);
        formData.append('BrandColor', color);
        formData.append('FeatureImageOfferText', featureChecked);

        for (var index = 0; index < socialMediaLinks.length; index++) {
            var folder = socialMediaLinks[index];
            formData.append("SocialMediaLinks[" + index + "].SocialMediaCompanyId", folder.SocialMediaCompanyId);
            formData.append("SocialMediaLinks[" + index + "].Name", folder.Name);
            formData.append("SocialMediaLinks[" + index + "].URI", folder.URI);
        }

        if (name === "") {
            var message = "Name is required";
            errorHighlight(this, message);
            toastr.error(message);
            $(".spinner").hide();
        }
        else {
            //saveMerchant();
            $.ajax({
                url: "/Merchant/Save",
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
                    if (data.success) {
                        toastr.success("Saving Merchant Successful");
                        socialMediaLinks = [];
                        window.location.href = "/Merchant/AddBranch?merchantId=" + data.data;
                    }
                    else {
                        toastr.error(data.errorMessage);
                    }
                },

                error: function (xhr, status, error) {
                    toastr.error(xhr.responseText);
                }
            });
        }
    }

    //function of add or edit Merchant branch - prabhu
    function saveBranch(formData) {
        var data = "";
        if (formData !== null) {
            data = formData.serialize();
        }
        $.ajax({
            url: "/Merchant/SaveBranch",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: data,
            cache: false,
            complete: function () {
                $(".spinner").hide();
                $("#btnSaveBranch").prop('disabled', false);
            },
            success: function (data) {
                if (data.success) {
                    if (data.data !== "" || data.data !== undefined) {
                        toastr.success("Merchant branch saved successfully.");
                        setTimeout(function () {
                            window.location.href = "/Merchant/Edit?Id=" + data.data;
                        }, 1500);
                    }
                }
                else {
                    toastr.error(data.errorMessage);
                }
            },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    //function of search MerchantList with pagination - prabhu
    function loadMerchantList(pageNumber) {
        var searchText = $('input[name="SearchText_input"]').val(); //$("#SearchText option:selected").text();
        
        searchText = searchText.replace(/&/, '%26');
        
        var Url = "/Merchant/Search?";
        if (searchText !== "" && searchText !== "Select Merchant") {
            Url = Url + "searchText=" + searchText;
        }
        window.location.href = Url + "&pageNumber=" + pageNumber +
            "&sortField=" + sortfield + "&sortDirection=" + sortdir;
    }

    //common function of delete confirm - prabhu
    function deleteConfirmation(title, message) {
        var def = $.Deferred();
        $("#infoMessage").html(message);

        $("#dialog-confirm").dialog({
            title: title,
            draggable: false,
            autoResize: true,
            resizable: false,
            open: function () {
                var btn = $(this).closest(".ui-dialog")
                    .find(".ui-dialog-titlebar-close");
                btn.hide();
            },
            modal: true,
            buttons: {
                Yes: {
                    id: 'btnYesId',
                    text: 'Yes',
                    click: function () {
                        $("#infoMessage").html("");
                        $(this).dialog("close");
                        def.resolve("Yes");
                    },
                    "class": "btnYes"
                },
                No: {
                    id: 'btnNoId',
                    text: 'No',
                    click: function () {
                        $("#infoMessage").html("");
                        $(this).dialog("close");
                        def.resolve("No");
                    },
                    "class": "btnNo"
                }
            }
        });
        return def.promise();
    }

    function deleteMerchant(merchantId) {
        $.ajax({
            url: "/Merchant/Delete?id=" + merchantId,
            beforeSend: function () {
            },
            type: "Post",
            data: null,
            cache: false,
            complete: function () {
            },
            success: function (data) {
                if (data.success === true) {
                    toastr.success("Merchant Deleted Successfully");
                    setTimeout(function () {
                        window.location.reload();
                    },
                        1500);
                }
                else {
                    toastr.error("Error While Deleting Merchant");
                }
            },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });

    }

    function registerSummernoteNew(element, placeholder, max, callbackMax) {
        $(element).summernote({
            focus: false,
            disableDragAndDrop: true,
            toolbar: [
                ['style', ['bold', 'italic', 'underline', 'clear']],
                ['para', ['ul', 'ol', 'paragraph']]

            ],
            placeholder,
            callbacks: {
                onKeydown: function (e) {
                    var t = e.currentTarget.innerText;
                    if (t.trim().length >= 8000) {
                        //delete key
                        if (e.keyCode != 8)
                            e.preventDefault();
                    }
                },
                onKeyup: function (e) {
                    var t = e.currentTarget.innerText;
                    $('#summernote').text(8000 - t.trim().length);
                },
                onPaste: function (e) {
                    var maxLength = 8000;
                    var t = e.currentTarget.innerText;
                    var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');

                    if (t.length + bufferText.length >= maxLength) {
                        e.preventDefault();
                        var bufferTextAllowed = bufferText.trim().substring(0, maxLength - t.length);
                        setTimeout(function () {
                            document.execCommand('insertText', false, bufferTextAllowed);
                            $('#summernote').text(maxLength - t.length);
                        }, 10)
                    }
                }
            }
        });
    }

    function registerSummernote() {
        registerSummernoteNew('.summernotelg', '', 8000, function () {
        });
        registerSummernoteNew('.summernoteterms', '', 8000, function () {
        });

    }


    //function to delete MerchantBranch - prabhu
    function deleteBranch(branchId) {
        $.ajax({
            url: "/Merchant/DeleteBranch?id=" + branchId,
            beforeSend: function () {
            },
            type: "Post",
            data: null,
            cache: false,
            complete: function () {
            },
            success: function (data) {
                if (data.success === true) {
                    toastr.success("Merchant Branch Deleted Successfully");
                    window.location.reload();
                }
                else {
                    toastr.error("Error While Deleting Merchant");
                }
            },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    //function to load paged MerchantBranch - prabhu
    function loadMerchantBranchList(pageNumber) {
        var id = $("#Id").val();
        $.ajax({
            url: "/Merchant/SearchBranch/",
            beforeSend: function () { $('.spinner').show(); },
            type: "GET",
            data: { merchantId: id, pageNumber: pageNumber },
            cache: false,
            complete: function () {
                $(".spinner").hide();
            },
            success: function (data) {
                //Fill div with results
                $("#divBranchList").html(data);
                setUpEvents();
            },

            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function addMerchantImage(id, file) {
        var formData = new FormData();
        formData.append('id', id);
        formData.append('imageFile', file);

        $.ajax({
            url: "/Merchant/SaveMerchantImage",
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
                $('#Image-file').val(null);
            },
            success: function (data) {
                inputsChanged = true;
                $("#carouselImage").html(data);
                setUpEvents();
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function deleteMerchantImage(imageId, merchantId, displayOrder) {
        var formData = new FormData();
        formData.append('Id', imageId);
        formData.append('MerchantId', merchantId);
        formData.append('DisplayOrder', displayOrder);

        $.ajax({
            url: "/Merchant/DeleteMerchantImage",
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
                $('#Image-file').val(null);
                $('#btnDeleteImage').prop('disabled', false);
            },
            success: function (data) {
                if (data.error) {
                    toastr.error(data.ErrorMessage);
                }
                else {
                    inputsChanged = true;
                    $("#carouselImage").html(data);
                    setUpEvents();
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function deleteFeatureImage(imageId, merchantId, displayOrder) {
        var formData = new FormData();
        formData.append('Id', imageId);
        formData.append('MerchantId', merchantId);
        formData.append('DisplayOrder', displayOrder);

        $.ajax({
            url: "/Merchant/DeleteFeatureImage",
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
                $('#Feature-file').val(null);
                //$('#btnDeleteFeature').prop('disabled', false);
            },
            success: function (data) {
                if (data.error) {
                    toastr.error(data.ErrorMessage);
                }
                else {
                    inputsChanged = true;
                    $("#featureImage").html(data);
                    setUpEvents();
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function deleteDisabledLogo(imageId, merchantId, displayOrder) {
        var formData = new FormData();
        formData.append('Id', imageId);
        formData.append('MerchantId', merchantId);
        formData.append('DisplayOrder', displayOrder);

        $.ajax({
            url: "/Merchant/DeleteDisabledLogo",
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
                    $("#disabledImage").html(data);
                    setUpEvents();
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function addMerchantDisabledLogo(id, file) {
        var formData = new FormData();
        formData.append('id', id);
        formData.append('imageFile', file);

        $.ajax({
            url: "/Merchant/SaveDisabledLogo",
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
                $("#disabledImage").html(data);
                setUpEvents();
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function addMerchantFeatureImage(id, file) {
        var formData = new FormData();
        formData.append('id', id);
        formData.append('imageFile', file);

        $.ajax({
            url: "/Merchant/SaveFeatureImage",
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
                $("#featureImage").html(data);
                setUpEvents();
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function kendoUIBind() {
        // create ComboBox from select HTML element
        $("#SearchText").kendoComboBox({
            filter: "contains"
        });
    }

    function setUpEvents() {
        $("#btnAddMerchant").unbind().click(function (e) {
            $("#wait").show();
            window.location.href = "/Merchant/Add";
        });

        //row click to edit Merchant
        $('#divMerchantList div.card').unbind().click(function (e) {
            e.preventDefault();
            var id = $(this).attr('merchantId');
            window.location.href = "/Merchant/Edit?Id=" + id;

        });

        //row click to edit Merchant Branch
        $('#divBranchList div.card').unbind().click(function (e) {
            e.preventDefault();
            var id = $(this).attr('branchId');
            var merchantId = $(this).attr('merchantId');
            window.location.href = "/Merchant/EditBranch?merchantId=" + merchantId + "&branchId=" + id;
        });

        $('.btnDeleteMerchants').unbind().click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            currentmerchantId = $(this).attr('merchantId');
            $.when(deleteConfirmation("Delete Merchant?", "Deleting this merchant will remove all their offers from the app and website (customer cashback due will be unaffected). Are you sure you wish to continue?")).then(
                function (status) {
                    if (status === "Yes") {
                        deleteMerchant(currentmerchantId);
                    }
                }
            );
        });

        $("#btnSaveBranch").unbind().click(function (e) {
            e.preventDefault();
            var _form = $(this).closest("form");
            var isValid = _form.valid();
            if (isValid) {
                $(this).prop('disabled', true);
                $(".validation-summary-errors").empty();
                saveBranch(_form);
            }
            else {
                $(".spinner").hide();
                toastr.error("The Name field is required.");
                $("#Name").focus();
            }
        });

        $("#btnSaveMerchants").unbind().click(function (e) {
            e.preventDefault();
            $(this).prop('disabled', true);
            socialMediaLinks = [];
            //to get social Media Items
            var children = $("#SocialMediaItems").children();
            $.each(children,
                function (i, item) {
                    var subChildren = $(item).children();
                    var mediaItem = new Object();
                    mediaItem.SocialMediaCompanyId = subChildren[0].value;
                    var children = $(subChildren[2]).children();
                    mediaItem.Name = "";
                    mediaItem.URI = children[0].value;
                    socialMediaLinks.push(mediaItem);
                });
            //

            //Id
            var id = $("#Id").val();
            var name = $("#Name").val();
            var shortDescription = $("#ShortDescription").val();
            var longDescription = $("#LongDescription").val();
            var webSite = $("#WebSite").val();
            var contactDetailId = $("#ContactDetailId").val();
            var landlinePhone = $("#LandlinePhone").val();
            var mobilePhone = $("#MobilePhone").val();
            var emailAddress = $("#EmailAddress").val();
            var terms = $("#Terms").val();
            var color = $("#BrandColor").val();
            var featureChecked = $("#FeatureImageOfferText").is(':checked');
            var item = JSON.stringify(socialMediaLinks);
            var formData = new FormData();
            formData.append('Id', id);
            formData.append('Name', name);
            formData.append('ShortDescription', shortDescription);
            formData.append('LongDescription', longDescription);
            formData.append('WebSite', webSite);
            //formData.append('SocialMediaLinks', item);
            formData.append('ContactDetailId', contactDetailId);
            formData.append('LandlinePhone', landlinePhone);
            formData.append('MobilePhone', mobilePhone);
            formData.append('EmailAddress', emailAddress);
            formData.append('Terms', terms);
            formData.append('BrandColor', color);
            formData.append('FeatureImageOfferText', featureChecked);

            for (var index = 0; index < socialMediaLinks.length; index++) {
                var folder = socialMediaLinks[index];
                formData.append("SocialMediaLinks[" + index + "].SocialMediaCompanyId", folder.SocialMediaCompanyId);
                formData.append("SocialMediaLinks[" + index + "].Name", folder.Name);
                formData.append("SocialMediaLinks[" + index + "].URI", folder.URI);
            }

            if (name === "") {
                var message = "Name is required";
                errorHighlight(this, message);
                toastr.error(message);
                $(".spinner").hide();
                $(this).prop('disabled', false);
            }
            else {
                //saveMerchant();
                $.ajax({
                    url: "/Merchant/Save",
                    beforeSend: function () {
                        $(".spinner").show();
                    },
                    type: "Post",
                    data: formData,
                    contentType: false,
                    processData: false,
                    cache: false,
                    complete: function () {
                        $(".spinner").hide();
                        $("#btnSaveMerchants").prop('disabled', false);
                    },
                    success: function (data) {
                        if (data.success) {
                            toastr.success("Saving Merchant Successful");
                            socialMediaLinks = [];
                            setTimeout(function () {
                                window.location.href = "/Merchant/";
                            },
                                1500);
                        }
                        else {
                            toastr.error(data.errorMessage);
                        }
                    },

                    error: function (xhr, status, error) {
                        $(".spinner").hide();
                        toastr.error(xhr.responseText);
                    }
                });
            }
        });

        $("#btnAddBranch").unbind().click(function (e) {
            e.preventDefault();
            var _form = $(this).closest("form");
            var isValid = _form.valid();
            if (isValid) {
                if (_form.serialize() !== formOriginaldata) {
                    $(".validation-summary-errors").empty();
                    saveMerchant();
                } else {
                    var id = $("#Id").val();
                    if (id > 0) {
                        window.location.href = "/Merchant/AddBranch?merchantId=" + id;
                    } else {
                        $(".spinner").hide();
                        toastr.error('No details found to save merchant.');
                    }
                }
            }
        });

        $("#btnCancelBranch").unbind().click(function (e) {
            e.preventDefault();
            var id = $("#MerchantId").val();
            if (inputsChanged) {
                $.when(deleteConfirmation("Cancel",
                    "Do you really want to discard changes?"))
                    .then(
                        function (status) {
                            if (status === "Yes") {
                                window.location.href = "/Merchant/Edit?Id=" + id;
                            }
                        }
                    );
            }
            else {
                window.location.href = "/Merchant/Edit?Id=" + id;
            }

        });

        //click event of paging for Merchant list
        $("#divMerchantList #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                sortfield = $("#SortField").val();
                sortdir = $("#SortDirection").val();
                var pageNumber = $(this).attr('pagenumber');
                loadMerchantList(pageNumber);
            });
        });

        //click event of search button for Merchant
        $("#btnSearchMerchants").unbind().click(function (e) {
            e.preventDefault();
            var searchText = $("#SearchText").val();
            //if (searchText !== "" && searchText.trim() !== "") {
            loadMerchantList(1);
            //} else {
            //    toastr.error("Please provide search field.");
            //}
        });

        //click event of cancel in Add/Edit Merchant page
        $("#btnCancel").unbind().click(function (e) {
            e.preventDefault();
            if (inputsChanged) {
                $.when(deleteConfirmation("Cancel",
                    "Do you really want to discard changes?"))
                    .then(
                        function (status) {
                            if (status === "Yes") {
                                if ($("#Id").val() > 0) {
                                    window.location.href = "/Merchant/";
                                }
                                else {
                                    $.get("/Merchant/Cancel", function (data) {
                                        if (data.success) {
                                            window.location.href = "/Merchant/";
                                        }
                                        else {
                                            toastr.error(data.errorMessage);
                                        }
                                    });
                                }
                            }
                        }
                    );
            }
            else {
                window.location.href = "/Merchant/";
            }
        });

        $(".btnDeleteBranch").unbind().click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            currentbranchId = $(this).attr('branchId');
            $.when(deleteConfirmation("Delete Confirmation?", "Are you sure you wish to delete this record?")).then(
                function (status) {
                    if (status === "Yes") {
                        deleteBranch(currentbranchId);
                    }
                }
            );
        });

        $("#btnDeleteImage").unbind().click(function (e) {
            e.preventDefault();
            if ($(this).hasClass('navi') === false) {
                $(this).toggleClass('navi');
            }
            var imageId = $(".carousel-item.active img").attr('merchantimageid');
            var merchantId = $(".carousel-item.active img").attr('merchantid');
            var displayOrder = $(".carousel-item.active img").attr('displayorder');
            $(this).prop('disabled', true);
            deleteMerchantImage(imageId, merchantId, displayOrder);
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
            deleteFeatureImage(imageId, merchantId, displayOrder);
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
            deleteDisabledLogo(imageId, merchantId, displayOrder);
        });

        //Load spineer
        $('button').click(function () {
            if ($(this).hasClass('navi') === false) {
                $(this).toggleClass('navi');
            }
        });

        //click event of paging for Merchant Branch list
        $("#divBranchList #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pagenumber');
                loadMerchantBranchList(pageNumber);
            });
        });

        if (!$("#Id").val() > 0) {
            if ($("#btnDeleteImage").is(":disabled") === false) {
                inputsChanged = true;
            }
        }

        //merchant list shorting
        $(".merchantHeading").each(function () {
            $(this).click(function (e) {
                e.preventDefault();
                sortfield = this.childNodes[1].attributes["SelSortField"].value;

                if ($("#SortField").val() === sortfield) {
                    if ($("#SortDirection").val() === "asc") {
                        sortdir = "desc";
                        sorticon = "fa fa-sort-alpha-desc";
                    }
                    else {
                        sortdir = "asc";
                        sorticon = "fa fa-sort-alpha-asc";
                    }
                }
                loadMerchantList(1);
            });
        });

        //merchant list Edit case
        $(".merchantList .merchantEdit a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var id = $(this).attr('merchantId');
                if (id > 0) {
                    window.location.href = "/Merchant/Edit?Id=" + id;
                }
            });
        });

        //merchant list Delete Case
        $(".merchantList .merchantDelete a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                e.stopPropagation();
                currentmerchantId = $(this).attr('merchantId');
                $.when(deleteConfirmation("Delete Merchant?", "Deleting this merchant will remove all their offers from the app and website (customer cashback due will be unaffected). Are you sure you wish to continue?")).then(
                    function (status) {
                        if (status === "Yes") {
                            deleteMerchant(currentmerchantId);
                        }
                    }
                );
            });
        });
        //form input dirty operation handles if user click cancel without save
        $("form input").change(function () {
            inputsChanged = true;
        });

        //form textarea dirty operation handles if user click cancel without save
        $("form textarea").change(function () {
            inputsChanged = true;
        });

        //form select dirty operation handles if user click cancel without save
        $("form select").change(function () {
            inputsChanged = true;
        });

        //Clear KendoUI Text if defaultValue Select
        $(document).on("click", "input:text[name='SearchText_input']", function () {
            var merchantTxt = $("input:text[name='SearchText_input']").val();
            var defaultSel = "Select Merchant";
            if (merchantTxt === defaultSel) {
                $("input:text[name='SearchText_input']").val('');
            }
        });
    }

    return {
        setUpEvents: setUpEvents,
        initialiseFormData: initialiseFormData,
        registerSummernote: registerSummernote,
        addMerchantImage: addMerchantImage,
        kendoUIBind: kendoUIBind,
        addMerchantDisabledLogo: addMerchantDisabledLogo,
        addMerchantFeatureImage: addMerchantFeatureImage
    };
}());