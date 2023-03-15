var Offer = (function () {
    "use strict";
    var formOriginaldata;
    var categories;
    var tags;
    var tagNameList = []; //autocomplete tagName
    var inputsChanged = false;
    var sortfield = "MerchantName";
    var sortdir = "asc";
    var sorticon = "fa fa-sort-alpha-desc";
    var merchantList = [];
    var thirdPartyList = [];
    var fileData;
    var affiliateData;
    var offerPosition;
    var offerList = [];
    var rightselected = offerList.length;
    var pagesize = 20;
    var enablePaginaton = false;

    function initialiseFormData() {
        formOriginaldata = $("#btnSaveMerchants").closest("form").serialize();
    }

    function errorHighlight(e, message) {
        $(".validation-summary-valid").html(message);
    }

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

    function navConfirmation(title, message) {
        var nav = $.Deferred();
        $("#navInfoMessage").html(message);

        //$("#dialog-confirmNav").dialog({
        //    title: title,
        //    draggable: false,
        //    autoResize: true,
        //    resizable: false,
        //    open: function () {
        //        var btn = $(this).closest(".ui-dialog")
        //            .find(".ui-dialog-titlebar-close");
        //        btn.hide();
        //    },
        //    modal: true,
        //    buttons: {
        //        Confirm: {
        //            id: 'btnYesId',
        //            text: 'Confirm',
        //            click: function () {
        //                $("#navInfoMessage").html("");
        //                $(this).dialog("close");
        //                nav.resolve("Yes");
        //            },
        //            "class": "btnYes"
        //        },
        //        Cancel: {
        //            id: 'btnNoId',
        //            text: 'Cancel',
        //            click: function () {
        //                $("#navInfoMessage").html("");
        //                $(this).dialog("close");
        //                nav.resolve("No");
        //            },
        //            "class": "btnNo"
        //        }
        //    }
        //});
        return nav.promise();
    }

    function popupoffer(title, message, editlink) {
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
                $('#btnYesId')
                    .wrap(editlink);
            },
            modal: true,
            buttons: {
                Yes: {
                    id: 'btnYesId',
                    text: 'Edit',
                    click: function () {
                        $("#infoMessage").html();
                        $(this).dialog("close");
                        def.resolve("Yes");
                    },
                    "class": "btnYes"
                },
                No: {
                    id: 'btnNoId',
                    text: 'Close',
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

    function addTags(tagName, tagId) {
        var tagHtml = "<span class='tagCard col-md-9'><label class='form-group row control-label col-md-6 tagName' tagId = '" + tagId + "'>" + tagName + "</label><button class='col-md-1'><i class='fa fa-times'></i></button></span>";
        $("#tagsList").append(tagHtml);
    }

    function setCategories(data) {
        categories = data;
        categoryTree();
    }

    function categoryTree() {
        simTree({
            linkParent: false,

            // custom key
            response: {
                name: 'Name',
                id: 'Id',
                pid: 'ParentId',
                checked: 'IsChecked',
                expand: 'expand',
                disabled: 'disabled'
            },

            el: '#categoryTree',
            data: categories,
            check: true,
            childNodeAsy: false,
            onChange: function () {
                inputsChanged = true;
            }
        });
    }

    function setTags(data) {
        tags = data;

        $.each(tags, function (i) {
            tagNameList.push(tags[i].Tags);
        });
    }

    function setMerchants(data) {
        $.each(data, function (i) {
            merchantList.push(data[i].Text);
        });
    }

    function setThirdPartySites(data) {
        $.each(data, function (i) {
            thirdPartyList.push(data[i].Text);
        });
    }
    //function to remove arrows icon in list2
    function removerdownarrow() {
        $('#lstBox2 > li:not(:first-child) > #upArrow').css('visibility', 'visible');
        $('#lstBox2 > li:first-child > #upArrow').css('visibility', 'hidden');
        $('#lstBox2 > li:not(:last-child) > #downArrow').css('visibility', 'visible');
        $('#lstBox2 > li:last-child > #downArrow').css('visibility', 'hidden');
    }

    function saveOffer(userData) {
        var id = $("#Id").val();
        var dateAdded = $("#DateAdded").val();
        var merchantId = $("#MerchantId").val();
        var thirdPartySiteId = $("#SSOThirdpartySiteId").val();
        var productCode = $("#ProductCode").val();
        var affiliateId = $("#AffiliateId option:selected").eq(0).val();
        var offerTypeId = $("#OfferTypeId").val();
        var statusId = $("#StatusId").val();
        var searchRanking = $("#SearchRanking").val();
        var reoccuring = $("#Reoccuring").is(':checked');
        var validIndefintely = $("#ValidIndefintely").is(':checked');
        var validFrom = $("#ValidFrom").val();
        var validTo = $("#ValidTo").val();
        var headline = $("#Headline").val();
        var shortDescription = $("#ShortDescription").val();
        var longDescription = $("#LongDescription").val();
        var instructions = $("#Instructions").val();
        var terms = $("#Terms").val();
        var exclusions = $("#Exclusions").val();
        var linkUrl = $("#LinkURL").val();
        var offerCode = $("#OfferCode").val();
        var affiliateReference = $("#AffiliateReference").val();
        var redemptionAccountNumber = $("#RedemptionAccountNumber").val();
        var redemptionProductCode = $("#RedemptionProductCode").val();
        var selected = $("#MerchantBranchId").val();

        var formData = new FormData();
        formData.append('Id', id);
        formData.append('DateAdded', dateAdded);
        formData.append('MerchantId', merchantId);
        formData.append('SSOThirdpartySiteId', thirdPartySiteId);
        formData.append('productCode', productCode);
        formData.append('AffiliateId', affiliateId);
        formData.append('OfferTypeId', offerTypeId);
        formData.append('StatusId', statusId);
        formData.append('SearchRanking', searchRanking);
        formData.append('Reoccuring', reoccuring);
        formData.append('ValidIndefintely', validIndefintely);
        formData.append('ValidFrom', validFrom);
        formData.append('ValidTo', validTo);
        formData.append('Headline', headline);
        formData.append('ShortDescription', shortDescription);
        formData.append('LongDescription', longDescription);
        formData.append('Instructions', instructions);
        formData.append('Terms', terms);
        formData.append('Exclusions', exclusions);
        formData.append('LinkURL', linkUrl);
        formData.append('OfferCode', offerCode);
        formData.append('AffiliateReference', affiliateReference);
        formData.append('RedemptionAccountNumber', redemptionAccountNumber);
        formData.append('RedemptionProductCode', redemptionProductCode);

        $("#tagsList .tagCard").each(function (index, value) {
            formData.append("ListofTag[" + index + "].Id", $(this).find(".tagName").attr("tagId"));
            formData.append("ListofTag[" + index + "].Tags", $(this).find(".tagName").text());
        });

        $.each(selected, function (index, value) {
            // Get value in alert
            formData.append("ListofBranches[" + index + "].Text", value);
            formData.append("ListofBranches[" + index + "].Value", value);
        });

        $("input[name='countryCode']:checked").each(function (i) {
            if (this.checked) {
                formData.append("ListofCountries[" + i + "].Name", $(this).val());
                formData.append("ListofCountries[" + i + "].Accepted", true);
            }
        });

        $(".sim-tree-checkbox.checked").each(function (i) {
            formData.append("ListofCategory[" + i + "].Id", $(this).closest("li").attr("data-id"));
            formData.append("ListofCategory[" + i + "].IsChecked", true);
        });

        $.ajax({
            url: "/Offer/Save",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
                $(this).removeAttr("disabled");
            },
            success: function (data) {
                if (data.success) {
                    toastr.success("Offer saved successfully.");
                    setTimeout(function () {
                        window.history.back();
                        // window.location.href = "/Offer/Index?setfromSession=" + true;
                    },
                        1500);
                }
                else {
                    toastr.error(data.errorMessage);
                    $(this).removeAttr("disabled");
                }
            },

            error: function (xhr, status, error) {
                $(this).removeAttr("disabled");
                toastr.error(xhr.responseText);
            }
        });
    }

    function searchOffersMaintain() {
        var offerListId = $("#OfferListItemId").val();
        var countryCode = $("#CountryCode").val();

        var Url = "/Offer/OfferlistSearch?";
        if (offerListId !== "") {
            Url = Url +
                "offerlistId=" +
                offerListId;
        }
        if (countryCode !== "") {
            Url = Url +
                "&countryCode=" +
                countryCode;
        }
        window.location.href = Url;
    }
    $("#MerchantId").change(function () {
        var merchantId = $(this).val();
        if (merchantId !== "" && parseInt(merchantId) > 0) {
            getMerchantBranches(merchantId);
        } else {
            toastr.error("Please select valid merchant");
        }
    });
    //getMerchant Branches
    function getMerchantBranches(merchantid) {
        var merchantId = merchantid;
        $.ajax({
            url: '/Offer/GetMerchantBranches',
            type: "GET",
            dataType: "JSON",
            data: { merchantId: merchantId },
            success: function (branches) {
                var select = $("#MerchantBranchId");
                select.empty();
                if (branches.data.length != 0) {
                    console.log(branches.data.length);
                    $('.multiselect').removeClass("disabled").removeAttr('disabled', false);
                } else {
                    $('.multiselect').addClass("disabled").attr('disabled', true);
                };
                $.each(branches.data, function (i, branch) {
                    select.append($('<option/>', {
                        value: branch.id,
                        text: branch.name,
                        selected: "selected"
                    }));
                    //$("#MerchantBranchId").prop("disabled", branches.data.length != 0);
                });
                $('#MerchantBranchId').multiselect('rebuild');
            }
        });
    }

    function updatedisplayOrder() {
        var OfferListsort = new FormData();
        var offerListPageNumber = $("#OfferListItem #currentPage").val();
        $('#lstBox2 > li').each(function (index, value) {
            var offerId = $(this).attr('offerId');
            var listId = $("#OfferListItemId").val();
            var countryCode = $("#CountryCode").val();
            var NewOrderId = index + 1;

            OfferListsort.append("requestModel[" + index + "].OfferId", offerId);
            OfferListsort.append("requestModel[" + index + "].OfferListId", listId);
            OfferListsort.append("requestModel[" + index + "].CountryCode", countryCode);
            OfferListsort.append("requestModel[" + index + "].DisplayOrder", NewOrderId);
            OfferListsort.append("requestModel[" + index + "].PageNumber", offerListPageNumber);
        });
        $.ajax({
            url: "/Offer/UpdateOfferListOrder",
            beforeSend: function () { $('#wait').show(); },
            type: "POST",
            data: OfferListsort,
            processData: false,
            contentType: false,
            dataType: 'json',
            cache: false,
            complete: function () {
                $("#wait").hide();
            },
            success: function (data) {
                if (data) {
                    //$("#OfferListItem").html(data);
                }
                else {
                    //toastr.error(data.ErrorMessage);
                }
            },
            error: function (xhr, status, error) {
                $("#OfferListItem").html(xhr.responseText);
                removerdownarrow();
                setUpEvents();
            }
        });
    }

    function searchOffers(pageNumber) {
        var merchantId = $("#MerchantId").val();
        var affiliateId = $("#AffiliateId").val();
        var keyword = $("#Keyword").val();
        var validFrom = $("#ValidFrom").val();
        var validTo = $("#ValidTo").val();
        var offerType = $("#OfferType").val();
        var offerStatus = $("#OfferStatus").val();

        var Url = "/Offer/Search?";
        if (merchantId !== "" && merchantId !== null) {
            Url = Url +
                "MerchantId=" +
                merchantId;
        }
        if (affiliateId !== "") {
            Url = Url +
                "&AffiliateId=" +
                affiliateId;
        }
        if (keyword !== "") {
            Url = Url +
                "&Keyword=" +
                keyword;
        }
        if (validFrom !== "") {
            Url = Url +
                "&ValidFrom=" +
                validFrom;
        }
        if (validTo !== "") {
            Url = Url +
                "&ValidTo=" +
                validTo;
        }
        if (offerType !== "") {
            Url = Url +
                "&typeId=" +
                offerType;
        }
        if (offerStatus !== "") {
            Url = Url +
                "&statusId=" +
                offerStatus;
        }
        window.location.href = Url + "&page=" + pageNumber +
            "&sortField=" +
            sortfield +
            "&sortDirection=" +
            sortdir;
    }

    function addDeleteOfferListItem(offerIds, addToList, pos, items) {
        $("#Processing").val(true);
        var listId = $("#OfferListItemId").val();
        var countryCode = $("#CountryCode").val();
        var merchantId = $("#MerchantId").val();
        var affiliateId = $("#AffiliateId").val();
        var keyword = $("#Keyword").val();
        var validFrom = $("#ValidFrom").val();
        var validTo = $("#ValidTo").val();
        var offerType = $("#OfferType").val();
        var offerStatus = $("#OfferStatus").val();
        var offerPageNumber = $("#OfferList #currentPage").val();
        var formData = new FormData();
        formData.append('OfferListId', listId);
        //formData.append('OfferId', offerId);
        formData.append('OfferIds', offerIds);
        formData.append('Countrycode', countryCode);
        formData.append('MerchantId', merchantId);
        formData.append('AffiliateId', affiliateId);
        formData.append('Keyword', keyword);
        formData.append('ValidFrom', validFrom);
        formData.append('ValidTo', validTo);
        formData.append('OfferType', offerType);
        formData.append('OfferStatus', offerStatus);
        formData.append('offerPage', offerPageNumber);
        formData.append('AddTolist', addToList);
        formData.append('SelectedOrder', pos);
        formData.append('ItemsSelected', items);

        updateToList(formData, offerIds);
        //$.each(offerIds, function (i, item) {
        //    formData.set('OfferId', item);
        //    updateToList(formData);
        //});
    }

    function updateToList(formData, offerIds) {
        $.ajax({
            url: "/Offer/AddDeleteOfferlistItem",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
                $(this).removeAttr("disabled");
            },
            success: function (data) {
                if (data.success) {
                    if (data.data === 0) {
                        $("#Processing").val(false);
                    }
                    // $("#OfferList").html(data);
                    //toastr.success("OfferList Updated");
                    // removerdownarrow();
                    // setUpEvents();
                }
                else {
                    //toastr.error(data.errorMessage);
                    //$(this).removeAttr("disabled");
                    //setUpEvents();
                }
            },

            error: function (xhr, status, error) {
                $(this).removeAttr("disabled");
                toastr.error(xhr.responseText);
                setUpEvents();
            }
        });
    }

    function tagAutoComplete() {
        // create autocomplete UI
        $("#Tags").autocomplete({
            source: tagNameList
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
    /*Summernote Editor Starts here*/
    //$(document).ready(function (element) {
    //    $(element).summernote({
    //        name: 'myenter',
    //        events: {
    //            'insertParagraph': function (event, editor, layoutInfo) {
    //                var newLine = '<br />';
    //                pasteHtmlAtCaret(newLine);
    //                event.preventDefault();
    //            }
    //        }
    //    });
    //});

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
                    if (t.trim().length >= 512) {
                        //delete key
                        if (e.keyCode != 8)
                            e.preventDefault();
                    }
                },
                onKeyup: function (e) {
                    var t = e.currentTarget.innerText;
                    $('#summernote').text(512 - t.trim().length);
                },
                onPaste: function (e) {
                    var maxLength = 512;
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
        registerSummernoteNew('.summernotelg', '', 512, function () {
        });
        registerSummernoteNew('.summernoteins', '', 512, function () {
        });
        registerSummernoteNew('.summernoteterms', '', 512, function () {
        });
        registerSummernoteNew('.summernoteexcl', '', 512, function () {
        });
    }

    /*Summernote Editor Ends here*/

    function highlightEditors(id) {
        $(id).parent().find(".note-editor").addClass("input-validation-error");
    }

    function removehighlightEditors(id) {
        $(id).parent().find(".note-editor").removeClass("input-validation-error");
    }

    function getEditorValue(htmlValue) {
        var dom_nodes = $($.parseHTML(htmlValue));

        return dom_nodes[0].textContent;
    }

    function getFileImport() {
        var id = parseInt($("#AffiliateId").find(":selected").val());
        var fileId = parseInt($("#FileTypeId").find(":selected").val());
        var countryCode = $("#ImportCountryCode").find(":selected").val();
        if (id > 0 && fileId > 0 && countryCode !== "") {
            window.location.href = "/Offer/GetImport?affiliateId=" + id + "&fileId=" + fileId + "&countryCode=" + countryCode;
        } else {
            $(".affiliateSpinner").hide();
            toastr.error("Please select affiliate and file type");
        }
    }

    function importFile() {
        var formData = new FormData();
        formData.append("affiliateId", parseInt($("#AffiliateId").find(":selected").val()));
        formData.append("fileId", parseInt($("#FileTypeId").find(":selected").val()));
        formData.append("countryCode", $("#ImportCountryCode").find(":selected").val());
        formData.append("file", fileData);

        $.ajax({
            url: "/Offer/SaveFile",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: formData,
            datatype: "JSON",
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                getProcessingStatus();
                $(".spinner").hide();
                $(this).removeAttr("disabled");
            },
            success: function (data) {
                if (data.success) {
                    toastr.success("File imported successfully.");
                    $("#progress").hide();
                    $(".importbtn").attr("disabled", true);
                    $(".importspinner").show();
                    $('#messageExists .errmsg').text('Import in progress');
                    $("#messageExists").removeClass("hidden");
                }
                else {
                    toastr.error(data.errorMessage);
                }
            },

            error: function (xhr, status, error) {
                $(this).removeAttr("disabled");
                toastr.error(xhr.responseText);
            }
        });
    }

    function getProcessingStatus() {
        var id = parseInt($("#AffiliateId").find(":selected").val());
        var fileId = parseInt($("#FileTypeId").find(":selected").val());
        var countryCode = $("#ImportCountryCode").find(":selected").val();
        if (id > 0 && fileId > 0 && countryCode !== "") {
            var checkImportStatus = setInterval(function () {
                $.ajax({
                    url: "/Offer/GetProcessingFile",
                    type: "GET",
                    data: {
                        affiliateId: id,
                        fileId: fileId,
                        countryCode: countryCode
                    },
                    datatype: "JSON",
                    cache: false,
                    success: function (data) {
                        if (data.success) {
                            if (data.data.currentStatus === "Processed") {
                                $(".importspinner").hide();
                                clearInterval(checkImportStatus);
                                window.location.reload();
                            }
                            if (data.data.currentStatus === "Processing") {
                                $('#messageExists .errmsg').text('Import in progress ' + (data.data.success + data.data.updates + data.data.duplicates + data.data.failed) + ' of ' + data.data.totalRecords);
                            }
                        }
                    },
                    error: function (xhr, status, error) {
                    }
                });
            }, 5000);
        }
    }

    function updateCompleteImport() {
        var formData = new FormData();
        formData.append("affiliateId", parseInt($("#AffiliateId").find(":selected").val()));
        formData.append("fileTypeId", parseInt($("#FileTypeId").find(":selected").val()));
        $.ajax({
            url: "/Offer/CompleteImport",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: formData,
            datatype: "JSON",
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
                $(this).removeAttr("disabled");
            },
            success: function (data) {
                if (data.success) {
                    window.location.reload();
                }
                else {
                    toastr.error(data.errorMessage);
                }
            },

            error: function (xhr, status, error) {
                $(this).removeAttr("disabled");
                toastr.error(xhr.responseText);
            }
        });
    }

    function loadAffiliate(data) {
        affiliateData = data;
    }

    function assignToFileData(data) {
        fileData = data;
    }

    function displayToastr() {
        toastr.success("File exists for importing data.");
    }

    function pagingControl() {
        var pageList = new List('select-list', {
            page: 20,
            pagination: true
        });
    }

    function offerListRearrange() {
        $("#lstBox2 li").each(function (i, val) {
            $(this).attr("pos", i);
            $(this).attr("id", 'offerId_' + i);
        });
    }

    function getOffersSelectItems(pageNumber) {
        var merchantId = $("#MerchantId").val();
        var affiliateId = $("#AffiliateId").val();
        var keyword = $("#Keyword").val();
        var validFrom = $("#ValidFrom").val();
        var validTo = $("#ValidTo").val();
        var offerType = $("#OfferType").val();
        var offerStatus = $("#OfferStatus").val();
        var offerListId = $("#OfferListItemId").val();
        var countryCode = $("#CountryCode").val();
        var formData = {};// empty JSON object
        if (offerListId !== "") {
            formData["offerlistId"] = offerListId;
        }
        if (countryCode !== "") {
            formData["countryCode"] = countryCode;
        }
        if (merchantId !== "" && merchantId !== null) {
            formData["MerchantId"] = merchantId;
        }
        if (affiliateId !== "") {
            formData["AffiliateId"] = affiliateId;
        }
        if (keyword !== "") {
            formData["Keyword"] = keyword;
        }
        if (validFrom !== "") {
            formData["ValidFrom"] = validFrom;
        }
        if (validTo !== "") {
            formData["ValidTo"] = validTo;
        }
        if (offerType !== "") {
            formData["typeId"] = offerType;
        }
        if (offerStatus !== "") {
            formData["statusId"] = offerStatus;
        }
        formData["OfferPage"] = pageNumber;
        formData["isPartialView"] = true;
        $.ajax({
            url: "/Offer/OfferlistSearch",
            type: "GET",
            data: formData,
            datatype: "json",
            cache: false,
            success: function (data) {
                $("#offersSelect").html(data);
                setUpEvents();
            },
            error: function (xhr, status, error) {
            }
        });
        $(".spinner").hide();
        $("#btnSearchOfferMaintain").prop('disabled', false);
    }

    function scrollfunction() {
        $('.offrcontent').animate({
            scrollTop: $("#mainDiv").offset().top
        },
            500);
    }

    function setUpEvents() {
        function getParameterByName(name) {
            var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
            return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
        }
        var letterid = getParameterByName("letterid");

        $("#btnAddNewsLetter").hide();

        if (letterid > 0 && letterid != null) {
            $("#OfferListItemId").attr("disabled", true);
            $("#btnAddNewsLetter").show();
        }

        $("#btnAddNewsLetter").click(function (e) {
            debugger
            location.href = "/Newsletter/GetEmailTemplate?letterId=" + letterid;
        });

        if ($('#lstBox1 li').length == 0) {
            $('.pagination').hide();
        }
        else {
            $('.pagination').show();
        }

        removerdownarrow();

        if ($("#ValidIndefintely").is(':checked')) {
            $("#ValidFrom").attr("disabled", true);
            $("#ValidTo").attr("disabled", true);
        }

        $(window).on("beforeunload",
            function () {
                var inProcess = $("#Processing").val();
                if (inProcess === true || inProcess === "true") {
                    return navConfirmation("Confirmation", "You have unsaved changed being saved. Would you like to continue and not to see your changes?");
                }
            });

        $("#ValidIndefintely").change(function (e) {
            e.preventDefault();
            if ($("#ValidIndefintely").is(':checked')) {
                $("#ValidFrom").attr("disabled", true);
                $("#ValidTo").attr("disabled", true);
            }
            else {
                $("#ValidFrom").removeAttr("disabled");
                $("#ValidTo").removeAttr("disabled");
            }
        });

        /*popup for List Box starts*/
        $(".detailpop").unbind().click(function (e) {
            e.preventDefault();
            var message = "<span class='popupsd'>Short Description:</span><br>";
            if ($(this).parent('li').attr("shortDescription") !== undefined) {
                message = message + $(this).parent('li').attr("shortDescription");
            }
            message = message + "<br/><hr>Long Description: <br>"
            if ($(this).parent('li').attr("longDescription") !== undefined) {
                message = message + $(this).parent('li').attr("longDescription");
            }
            $.when(popupoffer($(this).parent('li').attr("merchantname"), message, '<a class="editOfferLink" href=' + '/Offer/Edit?Id=' + $(this).parent('li').attr("offerid") + ' target="_blank"></a>'));
        });
        /*popup for List Box Ends*/

        /*********List Box**********************/

        /***Single Click Starts Here***/
        $('#btnRight').on('click', function (e) {
            e.preventDefault();
            var offerPos = 0;
            var offerIds = '';
            var selectedOpts = $('#lstBox1 li.selected');
            if (selectedOpts.length > 0) {
                if (rightselected === 0) {
                    if ($('#lstBox2 li').length === 0) {
                        offerPosition = -1;
                    }
                    else {
                        offerPosition = $('#lstBox2 li').length - 1;
                    }
                    offerPos = offerPosition;
                    selectedOpts.map(selected => {
                        rearrangeposition(offerPos, selected, false);
                        offerPos++;

                        var sel = $('#lstBox1 li.selected')[selected];
                        if (offerIds === '') {
                            offerIds = $(sel).attr("offerId");
                        } else {
                            offerIds = offerIds + ',';
                            offerIds = offerIds + $(sel).attr("offerId");
                        }
                    });
                    //Add to DB
                    addDeleteOfferListItem(offerIds, true, parseInt(offerPosition) + 1, selectedOpts.length);

                    $(selectedOpts).remove();
                    if (offerPosition === 0) {
                        offerListRearrange();
                    }
                    setUpEvents();
                }
                else if (rightselected > 1) {
                    toastr.warning("Please select one offer in list");
                }
                else {
                    offerPos = offerPosition;
                    selectedOpts.map(selected => {
                        rearrangeposition(offerPos, selected, false);
                        offerPos++;
                        var sel = $('#lstBox1 li.selected')[selected];
                        if (offerIds === '') {
                            offerIds = $(sel).attr("offerId");
                        } else {
                            offerIds = offerIds + ',';
                            offerIds = offerIds + $(sel).attr("offerId");
                        }
                    });

                    //Add to DB
                    addDeleteOfferListItem(offerIds, true, parseInt(offerPosition) + 1, selectedOpts.length);

                    $(selectedOpts).remove();
                    if (offerPosition === 0) {
                        offerListRearrange();
                    }
                    setUpEvents();
                }
            }
        });

        $('#btnLeft').on('click', function (e) {
            offerList = [];
            offerPosition = 0;
            rightselected = 0;
            var offerIds = '';
            var selectedOpts = $('#lstBox2 li.selected');
            if (selectedOpts.length === 0) {
                e.preventDefault();
            }
            else {
                selectedOpts.map(selected => {
                    var selectedlist = $('#lstBox2 li.selected')[selected];
                    var merchantName = $(selectedlist).attr("MerchantName");
                    var offerId = $(selectedlist).attr("offerid");
                    var longdesp = $(selectedlist).attr("longdesp");
                    var shortdesp = "";
                    if ($(selectedlist).attr("shortDescription") !== undefined) {
                        shortdesp = $(selectedlist).attr("shortDescription");
                    }
                    var selectedOpts = '<li  class="col-xs-12 navundline" offerid=' + offerId + '  merchantname="' + $.trim(merchantName) + '" shortDescription = "' + shortdesp + '" longdescription="' + longdesp + '" > <a class="col-xs-4">' +
                        merchantName +
                        '</a> <a class="col-xs-2 pull-right detailpop">Details</a></li>';
                    if ($('#lstBox1 li').length === 0) {
                        $('#lstBox1').empty();
                    }
                    $('#lstBox1').append(selectedOpts);

                    if (offerIds === '') {
                        offerIds = $(selectedlist).attr("offerid");
                    } else {
                        offerIds = offerIds + ',';
                        offerIds = offerIds + $(selectedlist).attr("offerid");
                    }
                });

                //Remove from DB
                addDeleteOfferListItem(offerIds, false, offerPosition + 1, selectedOpts.length);

                $(selectedOpts).remove();
                offerListRearrange();
                setUpEvents();
                e.preventDefault();
            }
        });
        /***Single Click Ends Here***/

        /***Double Click Starts Here***/
        $('#lstBox1 > li').unbind().dblclick(function (e) {
            e.preventDefault();
            var offerIds = '';
            var selected = $(this).index();
            if (rightselected === 0) {
                if ($('#lstBox2 li').length === 0) {
                    offerPosition = 0;
                }
                else {
                    offerPosition = $('#lstBox2 li').length - 1;
                }

                var sel = $('#lstBox1 li')[selected];
                offerIds = $(sel).attr("offerId");
                addDeleteOfferListItem(offerIds, true, parseInt(offerPosition) + 1, selected.length);

                rearrangeposition(offerPosition, selected, true);
                $('#lstBox1 li')[selected].remove();
                setUpEvents();
            }
            else if (rightselected > 1) {
                toastr.warning("Please select one offer in list");
            }
            else {
                rearrangeposition(offerPosition, selected, true);

                var sel = $('#lstBox1 li')[selected];
                offerIds = $(sel).attr("offerId");
                addDeleteOfferListItem(offerIds, true, parseInt(offerPosition) + 1, selected.length);

                $('#lstBox1 li')[selected].remove();
                setUpEvents();
            }
        });

        $('#lstBox2 > li').unbind().dblclick(function (e) {
            e.preventDefault();
            var selected = $(this).index();
            var offerIds = '';
            var select = $('#lstBox2 li')[selected];
            var merchantName = $(select).attr("MerchantName");
            var offerId = $(select).attr("offerid");
            var longdesp = $(select).attr("longdesp");
            var shortdesp = "";
            if ($(select).attr("shortDescription") !== undefined) {
                shortdesp = $(select).attr("shortDescription");
            }
            var selectedOpts = '<li  class="col-xs-12 navundline" offerid=' + offerId + ' merchantname="' + $.trim(merchantName) + '" shortDescription = "' + shortdesp + '" longdescription="' + longdesp + '"> <a class="col-xs-4">' +
                merchantName +
                '</a> <a class="col-xs-2 pull-right detailpop">Details</a></li>';
            if ($('#lstBox1 li').length === 0) {
                $('#lstBox1').empty();
            }
            $('#lstBox1').append(selectedOpts);

            //Remove from List
            var sel = $('#lstBox2 li')[selected];
            offerIds = $(sel).attr("offerid");
            addDeleteOfferListItem(offerIds, false, offerPosition + 1, selected.length);

            $(select).remove();
            offerListRearrange();
            setUpEvents();
        });

        /***Double Click Ends Here***/

        /***Selection Click Starts Here***/
        $('#lstBox1 > li').on('click', function () {
            if ($(this).hasClass("selected")) {
                $(this).removeClass('selected');
            }
            else {
                $(this).addClass('selected');
            }
        });

        $('#lstBox2 > li').on('click', function () {
            var last_element;
            if ($(this).hasClass("selected")) {
                var index = offerList.indexOf($(this)[0].attributes);
                if (index > -1) {
                    offerList.splice(index, 1);
                }
                last_element = offerList[offerList.length - 1];
                if (last_element) {
                    offerPosition = last_element.pos.value;
                }
                rightselected = offerList.length;
                $(this).removeClass('selected');
            }
            else {
                offerList.push($(this)[0].attributes);
                rightselected = offerList.length;
                //offerPosition = rightselected;
                last_element = offerList[offerList.length - 1];
                if (last_element) {
                    offerPosition = last_element.pos.value;
                }
                $(this).addClass('selected');
            }
        });
        /***Selection  Click Ends Here***/

        /***Offer List Sorting and up down click Starts Here***/
        $('[data-z]').click(function (e) {
            var jTarget = $(e.target),
                dir = jTarget.data('dir'),
                jItem = $(e.currentTarget),
                jItems = $('li'),
                index = jItems.index(jItem);

            switch (dir) {
                case 'up':
                    if (index !== 0) {
                        var item = $(this).detach().insertBefore(jItems[index - 1]);
                        removerdownarrow();
                        offerListRearrange();
                        updatedisplayOrder();
                    }
                    break;
                case 'down':
                    if (index !== jItems.length - 1) {
                        var item2 = $(this).detach().insertAfter(jItems[index + 1]);
                        removerdownarrow();
                        offerListRearrange();
                        updatedisplayOrder();
                    }
                    break;
            }
        });

        /***Offer List Sorting and up down click  Ends Here***/

        var Lielemnts = $('ul#lstBox2 li').length;
        if (Lielemnts > 1) {
            $("#NorecordsLi").addClass("hidden");
        }
        else {
            $("#NorecordsLi").removeClass("hidden");
        }
        var elementul = $('ul');
        //var $list = $('ul').sortable({
        //    disabled: false,
        //    update: function () {
        //        console.log('line');
        //        var sortOrder = $('ul').sortable('toArray', { attribute: 'data-z' });
        //        $('.info').text(sortOrder);
        //    }
        //});

        /*Progress bar  starts here*/
        $("#show").on("click", function () {
            $("#progress").show();
            $(function () {
                var current_progress = 0;
                var interval = setInterval(function () {
                    current_progress += 10;
                    $("#progressbar")
                        .css("width", current_progress + "%")
                        .attr("aria-valuenow", current_progress)
                        .text(current_progress + "% Completed");
                    if (current_progress >= 100)
                        clearInterval(interval);
                }, 1000);
            });
        });
        /*Progress bar  Ends here*/

        $("#btnAddTag").unbind().click(function (e) {
            e.preventDefault();
            var tagIsValid = true;
            var tagstr = $("#Tags").val();
            if (tagstr !== "" && tagstr !== undefined && tagstr !== null) {
                $("#tagsList .tagCard").each(function (index, value) {
                    if ($(this).find(".tagName").text().toLowerCase() === $.trim(tagstr).toLowerCase()) {
                        tagIsValid = false;
                    }
                });
                if (tagIsValid) {
                    if (tags.length > 0) {
                        var searchText = $.trim(tagstr).toLowerCase();
                        var filterResults = $.grep(tags, function (elem, i) {
                            return elem.Tags.toLowerCase().indexOf(searchText) > -1;
                        });
                        if (filterResults.length > 0) {
                            addTags($.trim(tagstr), filterResults[0].Id);
                        } else {
                            addTags($.trim(tagstr), 0);
                        }
                    } else {
                        addTags($.trim(tagstr), 0);
                    }
                    $("#Tags").val("");
                    setUpEvents();
                    $("#Tags").focus();
                } else {
                    toastr.error("Tag name already exists.");
                }
            }
            else {
                $(".spinner").hide();
                toastr.error("Please provide tag name and continue.");
            }
        });

        $("#tagsList .tagCard button").unbind().click(function (e) {
            e.preventDefault();
            inputsChanged = true;
            $(this).parent('.tagCard').remove();
        });

        $("#btnAddOffer").unbind().click(function () {
            $("#wait").show();
            window.location.href = "/Offer/Add";
        });

        $("#btnSearchOffer").unbind().click(function (e) {
            var form = $(this).closest("form");

            var merchantId = $("#MerchantId").val();
            var affiliateId = $("#AffiliateId").val();
            var keyword = $("#Keyword").val();
            var validFrom = $("#ValidFrom").val();
            var validTo = $("#ValidTo").val();
            var offerType = $("#OfferType").val();
            var offerStatus = $("#OfferStatus").val();
            toastr.success("Searching for offers");
            $(this).prop('disabled', true);
            searchOffers(1);
        });

        $("#Sortablehead").click(function (e) {
            sortTable(0);
        });

        $("#btnSearchOfferMaintain").unbind().click(function (e) {
            var _form = $(this).closest("form");
            var isValid = _form.valid();
            if (isValid) {
                var offerListId = $("#OfferListItemId").val();
                //$("#ListofOfferListItems option:selected").text();
                var countryCode = $("#CountryCode").val();
                if (!offerListId.trim() || !countryCode.trim()) {
                    toastr.error("Please select both List Name and Country");
                    $(".spinner").hide();
                } else {
                    toastr.success("Searching for offers");
                    $(this).prop('disabled', true);
                    getOffersSelectItems(1);
                    //searchOffersFeatured(_form);
                }
            } else {
                toastr.error("Please select both List Name and Country");
                $(".spinner").hide();
            }
        });

        $("#secondDiv #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                sortfield = $("#SortField").val();
                sortdir = $("#SortDirection").val();
                var pageNumber = $(this).attr('pagenumber');
                var form = $("#btnSearchOffer").closest("form");
                searchOffers(pageNumber);
            });
        });

        $("#btnSaveOffer").unbind().click(function (e) {
            e.preventDefault();
            $(this).prop('disabled', true);
            //$("#AffiliateId").val("0");
            var errorMsg = "";
            //var longDescription = getEditorValue($("#LongDescription").val());
            //var instructions = getEditorValue($("#Instructions").val());
            //var longDescription = $("#LongDescription").val();
            var instructions = $("#Instructions").val();

            var refMerchantId = $("#RefMerchantId").val();

            if (refMerchantId !== null || refMerchantId !== 0) {
                if (refMerchantId !== $("#MerchantId").val()) {
                    errorMsg = "Please select valid merchant";
                    $(this).prop('disabled', false);
                }
            }
            else {
                errorMsg = "Please select valid merchant";
                $(this).prop('disabled', false);
            }
            //if (merchantList.length > 0) {
            //    var searchText = $('#MerchantId :selected').text();
            //    var filterResults = $.grep(merchantList, function (elem, i) {
            //        return elem === searchText;
            //    });
            //    if (filterResults.length === 0) {
            //        errorMsg = "Please select valid merchant";
            //    }
            //}
            if ($("#MerchantId").val() === 0 || $("#MerchantId").val() === null || $("#MerchantId").val() === "") {
                errorMsg = $("#MerchantId").attr("data-val-required");
            }
            var message = "";

            //if (!longDescription.trim()) {
            //    message = message + "<li>Long Description Is Required</li>";
            //    highlightEditors('#LongDescription');

            //}
            //if (!instructions.trim()) {
            //    message = message + "<li>Instructions Is Required</li>";
            //    highlightEditors('#Instructions');
            //    $(this).prop('disabled', false);
            //}

            var _form = $(this).closest("form");
            var isValid = _form.valid();

            if (errorMsg !== "") {
                $(".merchantDiv .field-validation-valid").text(errorMsg);
                $(".validation-summary-errors ul li:first").before("<li>" + errorMsg + "</li>");
                if ($(".validation-summary-errors").is("ul") === false) {
                    errorHighlight(this, "<ul><li>" + errorMsg + "</li></ul>");
                }

                $("span .k-dropdown-wrap").addClass("input-validation-error");
                $(this).prop('disabled', false);
            }
            else {
                $(".merchantDiv .field-validation-valid").empty();
                $("span .k-dropdown-wrap").removeClass("input-validation-error");
            }
            if (isValid && errorMsg === "" && message === "") {
                if (_form.serialize() !== formOriginaldata) {
                    $(".validation-summary-errors").empty();
                    saveOffer(_form);
                }
            }
            else {
                $(".spinner").hide();
                $(this).removeAttr("disabled");
                toastr.error("Unable to save offer as  some fields have invalid data.");
                $(this).prop('disabled', false);
            }
            var Ul = $(".validation-summary-errors").children();
            if (Ul !== null && Ul !== undefined && Ul !== "" && Ul.length > 0) {
                var ulmessage = Ul.append(message);
            }
            else {
                errorHighlight(this, "<ul>" + message + "</ul>");
            }
            $(this).prop('disabled', false);
        });

        $(".offerList #editbutton").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var id = $(this).attr("offerId");
                if (id > 0) {
                    window.location.href = "/Offer/Edit?Id=" + id;
                }
            });
        });

        $(".offerList #deletebutton").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var id = $(this).attr("offerId");
                if (id > 0) {
                    window.location.href = "/Offer/Delete?Id=" + id;
                }
            });
        });

        $("#btnCancelOffer").unbind().click(function (e) {
            e.preventDefault();
            if (inputsChanged) {
                $.when(deleteConfirmation("Cancel",
                    "Do you really want to discard changes?"))
                    .then(
                        function (status) {
                            if (status === "Yes") {
                                if ($("#Id").val() > 0) {
                                    toastr.warning("Edit offer was cancelled");
                                }
                                else {
                                    toastr.warning("Add offer was cancelled");
                                }
                                setTimeout(function () {
                                    window.history.back();
                                    //window.location.href = "/Offer/Index?setfromSession=" + true;
                                },
                                    1500);
                            }
                        }
                    );
            }
            else {
                window.history.back();
                //window.location.href = "/Offer/Index?setfromSession=" + true;
            }
        });

        window.addEventListener('popstate', function (event) {
            // The popstate event is fired each time when the current history entry changes.

            if (r === true) {
                // Call Back button programmatically as per user confirmation.
                history.back();
                // Uncomment below line to redirect to the previous page instead.
                // window.location = document.referrer // Note: IE11 is not supporting this.
            } else {
                // Stay on the current page.
                history.pushState(null, null, window.location.pathname);
            }

            history.pushState(null, null, window.location.pathname);
        }, false);

        $("input:text[name='MerchantId_input']").on('change', function () {
            $("span .k-dropdown-wrap").removeClass("input-validation-error");
            $(".merchantDiv .field-validation-valid").empty();
        });

        $("#MerchantId").on('change', function () {
            $("span .k-dropdown-wrap").removeClass("input-validation-error");
            $(".merchantDiv .field-validation-valid").empty();
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
        //Load spinner
        $('button').click(function () {
            $(this).toggleClass('navi');
        });

        $('.importbtn').click(function (e) {
            e.preventDefault();
            var formValidation = $(this).closest("form");
            var isValid = formValidation.valid();
            if (isValid && fileData !== null && fileData !== undefined) {
                $("#progress").show();
                $(function () {
                    var current_progress = 0;
                    var interval = setInterval(function () {
                        current_progress += 10;
                        $("#progressbar")
                            .css("width", current_progress + "%")
                            .attr("aria-valuenow", current_progress)
                            .text(current_progress + "% Completed");
                        if (current_progress >= 100) {
                            clearInterval(interval);
                            $("#progress").hide();
                            $(".importspinner").show();
                            $('#messageExists .errmsg').text('Import in progress');
                            $("#messageExists").removeClass("hidden");
                            getProcessingStatus();
                        }
                    }, 1000);
                });
                importFile();
            } else {
                $("#progress").hide();
                $(this).removeClass("navi");
                if (fileData !== null || fileData !== undefined) {
                    toastr.error("Please select file to import.");
                }
            }
        });

        $('.completebtn').click(function (e) {
            e.preventDefault();
            updateCompleteImport();
        });

        $("#AffiliateId").change(function (e) {
            e.preventDefault();
            $(".affiliateSpinner").show();
            var items = "<option value='0'>--Select File Type--</option>";
            var affiliate = $(this).find(":selected").val();
            if (affiliate > 0) {
                if (affiliateData !== null && affiliateData !== undefined) {
                    $.each(affiliateData,
                        function (i, item) {
                            if (item.AffiliateId === parseInt(affiliate)) {
                                if (item.FileTypes.length === 0) {
                                    items = "";
                                }
                                $.each(item.FileTypes,
                                    function (j, item1) {
                                        items += "<option value='" + item1.Value + "'>" + item1.Text + "</option>";
                                    });
                            }
                        });
                    $("#FileTypeId").html(items);
                }
            }
            $(".affiliateSpinner").hide();
        });

        $("#FileTypeId").change(function (e) {
            e.preventDefault();
            $(".affiliateSpinner").show();
            getFileImport();
        });

        $(".offerHeading").each(function () {
            $(this).click(function (e) {
                e.preventDefault();
                sortfield = this.childNodes[1].attributes["SelSortField"].value;//$(this).attr("SelSortField");

                if ($("#SortField").val() === sortfield) {
                    if ($("#SortDirection").val() === "asc") {
                        sortdir = "desc";
                    }
                    else {
                        sortdir = "asc";
                    }
                }
                else {
                    sortdir = "asc";
                }

                if (sortdir === "asc") {
                    if (sortfield === "ValidFrom" || sortfield === "ValidTo")
                        sorticon = "fa fa-sort-numeric-asc";
                    else
                        sorticon = "fa fa-sort-alpha-asc";
                }
                else {
                    if (sortfield === "ValidFrom" || sortfield === "ValidTo")
                        sorticon = "fa fa-sort-numeric-desc";
                    else
                        sorticon = "fa fa-sort-alpha-desc";
                }
                searchOffers(1);
            });
        });
        //Clear KendoUI Text if defaultValue Select
        $(document).on("click", "input:text[name='MerchantId_input']", function () {
            var merchantTxt = $("input:text[name='MerchantId_input']").val();
            //var defaultSel = $("#MerchantId option:first").text();
            var defaultSel = "Select Merchant";
            if (merchantTxt === defaultSel) {
                $("input:text[name='MerchantId_input']").val('');
            }
        });

        $(".note-editor").keydown(function () {
            var parent = $(this).parent();
            var textarea = parent.find("textarea").attr("Id");
            removehighlightEditors("#" + textarea + "");
        });

        $("#OfferListItemId").change(function () {
            var offerId = $(this).val();
            if (offerId !== "" && parseInt(offerId) > 0) {
                var countryCode = $("#CountryCode").val();
                if (countryCode !== "") {
                    searchOffersMaintain(1, 1);
                } else {
                    toastr.error("Please select country");
                }
            } else {
                toastr.error("Please select valid list name");
            }
        });

        $("#CountryCode").change(function () {
            var OfferListItemId = $("#OfferListItemId").val();
            if (OfferListItemId !== "") {
                searchOffersMaintain(1, 1);
            }
            else {
                toastr.error("Please select List name");
            }
        });

        /*Collapse Content Satrts here*/
        $(".readmore").on('click touchstart', function (event) {
            event.preventDefault();
            var txt = $(".more-content").is(':visible') ? 'Show more (+)' : 'Less (–)';
            $(this).html(txt);
            $(".more-content").toggleClass("cg-visible");
        });
        /*Collapse Content Ends here*/

        if ($("#lstBox2").has("li").length === 0) {
            $("#lstBox2").html("<span style='padding-left:10px;'>No Records</span>");
        }

        if ($("#lstBox1").has("li").length === 0) {
            $("#lstBox1").html("<span style='padding-left:10px;'>No Records</span>");
        }

        $("#offerListPaging #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var _form = $(this).closest("form");
                var isValid = _form.valid();
                if (isValid) {
                    var offerListId = $("#OfferListItemId").val();
                    //$("#ListofOfferListItems option:selected").text();
                    var countryCode = $("#CountryCode").val();
                    if (!offerListId.trim() || !countryCode.trim()) {
                        toastr.error("Please select both List Name and Country");
                        scrollfunction();
                    } else {
                        var pageNumber = $(this).attr('pagenumber');
                        getOffersSelectItems(pageNumber);
                        scrollfunction();
                    }
                } else {
                    toastr.error("Please select both List Name and Country");
                    scrollfunction();
                }
            });
        });

        /*add Offers to Offer List Item Starts*/
        $(".addOfferToListItem").unbind().click(function (e) {
            e.preventDefault();
            var offerObject = {};
            offerObject["offerId"] = $(this).attr("offerId");
            offerObject["validFrom"] = $(this).attr("fromDate");
            offerObject["validTo"] = $(this).attr("toDate");
            var content = "OfferId = " + $(this).attr("offerId");
            if ($(this).parent("tr").find("td.offerMerchantName").text() !== undefined) {
                offerObject["offerMerchantName"] = $(this).parent("tr").find("td.offerMerchantName").text();
            }
            if ($(this).closest("tr").find("td.offerShortDescription").text() !== undefined) {
                offerObject["shortDescription"] = $(this).closest("tr").find("td.offerShortDescription").text();
            }
            getNewOfferList(offerObject);
            //var newRow = createNewOfferList(offerObject);
            //console.log(newRow);
            //$('#OfferListItem').find("table tbody").append(newRow);
        });
        /*add Offers to Offer List Item Ends*/

        /*remove offerListItem Starts*/
        $(".removeOfferList").unbind().click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            $("#tblOfferMaintainList tbody tr").each(function () {
                $(this).removeClass("backgroundTableRow");
            });
            var currentOfferId = $(this).attr("offerId");
            $(this).parent("tr").addClass("backgroundTableRow");
            $.when(deleteConfirmation("Remove Offer?", "Are you sure you wish to remove this Offer from Offer List?")).then(
                function (status) {
                    if (status === "Yes") {
                        $(".removeOfferList[offerId=" + currentOfferId + "]").closest("tr").remove();
                        toastr.success("Removed offer from offer List, click save to update in Db.");
                        reDisplayOrder();
                    } else {
                        $(".removeOfferList[offerId=" + currentOfferId + "]").closest("tr").removeClass("backgroundTableRow");
                    }
                }
            );
        });
        /*remove offerListItem Ends*/

        /*Move tr up and down with addClass for highlighting Starts*/
        $(".upArrow,.downArrow").unbind().click(function (e) {
            e.preventDefault();
            $("#tblOfferMaintainList tbody tr").each(function () {
                $(this).removeClass("backgroundTableRow");
            });
            //setTimeout(function () {
            //    $(this).closest("tr").css("background-color", "goldenrod");
            //}, 800);
            var row = $(this).parents("tr:first");
            if ($(this).is(".upArrow")) {
                row.insertBefore(row.prev()).addClass("backgroundTableRow");
            } else {
                row.insertAfter(row.next()).addClass("backgroundTableRow");
            }
            reDisplayOrder();
        });
        /*Move tr up and down with addClass for highlighting Ends*/

        /*
         */
        $("#btnSaveOfferMaintain").unbind().click(function (e) {
            e.preventDefault();
            var _form = $(this).closest("form");
            var isValid = _form.valid();
            if (isValid) {  
                var offerListId = $("#OfferListItemId").val();
                //$("#ListofOfferListItems option:selected").text();
                var countryCode = $("#CountryCode").val();
                if (!offerListId.trim() || !countryCode.trim()) {
                    toastr.error("Please select both List Name and Country");
                    scrollfunction();
                } else {
                    $(this).prop('disabled', true);
                    saveNewOfferListItem();
                }
            } else {
                toastr.error("Please select both List Name and Country");
                scrollfunction();
            }
        });
        /*
         *
         */
    }

    function saveNewOfferListItem() {
        var offerListId = $("#OfferListItemId").val();
        var countryCode = $("#CountryCode").val();

        var formData = new FormData();
        formData.append("OfferListItemId", offerListId);
        formData.append("CountryCode", countryCode);
        if ($("#offersListPartial #tblOfferMaintainList").length > 0) {
            $("#tblOfferMaintainList tbody tr").each(function (i) {
                formData.append("OfferListItems[" + i + "].OfferId", $(this).find("td.removeOfferList").attr("offerId"));
                formData.append("OfferListItems[" + i + "].DisplayOrder", $(this).find("td span.upArrow").attr("displayOrder"));
                formData.append("OfferListItems[" + i + "].ValidFrom", $(this).find("td input[name='StartDate']").val());
                formData.append("OfferListItems[" + i + "].ValidTo", $(this).find("td input[name='EndDate']").val());
            });
        }

        $.ajax({
            url: "/Offer/UpdateOfferListItem",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
                $(this).removeAttr("disabled");
            },
            success: function (data) {
                if (data.success) {
                    toastr.success("Offer saved successfully.");
                    setTimeout(function () {
                        window.location.reload();
                    },
                        800);
                }
                else {
                    toastr.error(data.errorMessage);
                    $(this).removeAttr("disabled");
                }
            },

            error: function (xhr, status, error) {
                $(this).removeAttr("disabled");
                toastr.error(xhr.responseText);
            }
        });

        $(this).prop('disabled', false);
    }

    function getNewOfferList(offerObject) {
        var isSingleRow = false;
        var isOfferExists = false;
        //check if offerId exits
        if ($("#offersListPartial #tblOfferMaintainList").length > 0) {
            isSingleRow = true;
            $("#tblOfferMaintainList tbody tr").each(function () {
                if (offerObject["offerId"] === $(this).find("td.removeOfferList").attr("offerId")) {
                    isOfferExists = true;
                }
            });
        }
        if (isOfferExists) {
            toastr.error("Selected Offer for Merchant : " + offerObject["offerMerchantName"] + " is already exists.", "", { timeOut: 1050 });
        } else {
            offerObject["isSingleRow"] = isSingleRow;
            //ajax call to get partial view
            $.ajax({
                url: "/Offer/GetNewOfferList",
                type: "GET",
                data: offerObject,
                datatype: "json",
                cache: false,
                success: function (data) {
                    if (isSingleRow) {
                        $('#offersListPartial').find("table tbody").append(data);
                    } else {
                        $('#offersListPartial').html(data);
                    }
                    toastr.success("Added Offer to offerList", "", { timeOut: 750 });
                    setUpEvents();
                    reDisplayOrder();
                },
                error: function (xhr, status, error) {
                    toastr.error(error);
                }
            });
        }
    }

    function reDisplayOrder() {
        if ($("#offersListPartial #tblOfferMaintainList").length > 0) {
            $("#tblOfferMaintainList tbody tr").each(function (index) {
                $(this).find("td span.upArrow").attr("displayOrder", index + 1);
                if (index === 0) {
                    $(this).find("td span.upArrow").css({ "visibility": "hidden" });
                    $(this).find("td span.downArrow").css({ "visibility": "visible" });
                } else {
                    $(this).find("td span.upArrow").css({ "visibility": "visible" });
                    $(this).find("td span.downArrow").css({ "visibility": "visible" });
                }
                if (index === $("#tblOfferMaintainList tbody tr").length - 1) {
                    $(this).find("td span.downArrow").css({ "visibility": "hidden" });
                }
            });
        }
    }

    function rearrangeposition(offerPosition, selected, isdblclick) {
        var selectedlist;
        if (isdblclick) {
            selectedlist = $('#lstBox1 li')[selected];
        }
        else {
            selectedlist = $('#lstBox1 li.selected')[selected];
        }
        var merchantName = $(selectedlist).attr("merchantname");
        var shortdesp = "";
        if ($(selectedlist).attr("shortDescription") !== undefined) {
            shortdesp = $(selectedlist).attr("shortDescription");
        }

        var longdesp = "";
        if ($(selectedlist).attr("longdescription") !== undefined) {
            longdesp = $(selectedlist).attr("longdescription");
        }

        var offerId = $(selectedlist).attr("offerId");
        var selectdata = '<li data-z pos="' + (parseInt(offerPosition) + 1) + '" class="col-xs-12 navundline" offerid="' + offerId + '" merchantname="' + $.trim(merchantName) + '" id="offerId_' + (parseInt(offerPosition) + 1) + '" shortDescription="' + shortdesp + '" longdesp="' + longdesp + '" > <a class="col-xs-4">' + merchantName + '</a> <a class="col-xs-6 shtdsp"> ' + shortdesp + ' </a> <span id="upArrow" class="col-xs-1  btn-xs pointing"><i class="fa fa-arrow-up fa-xs" data-dir="up"></i></span> <span id="downArrow" class="col-xs-1  btn-xs pointing"> <i class="fa fa-arrow-down fa-xs" data-dir="down"></i> </span>  </li > ';
        if ($('#lstBox2 li').length === 0) {
            $("#lstBox2").empty();
            $("#lstBox2").append(selectdata);
        }
        else {
            $(selectdata).insertAfter($('#offerId_' + offerPosition));
        }
        offerListRearrange();
    }

    function loadBasicData(source, dest) {
        sourceData = source;
        destData = dest;
    }

    function offrPosition(position) {
        offerPosition = position;
    }

    function setPagination(set) {
        enablePaginaton = set;
    }
    return {
        setUpEvents: setUpEvents,
        initialiseFormData: initialiseFormData,
        setCategories: setCategories,
        setTags: setTags,
        setMerchants: setMerchants,
        setThirdPartySites: setThirdPartySites,
        tagAutoComplete: tagAutoComplete,
        kendoUIBind: kendoUIBind,
        setdefaultValue: setdefaultValue,
        registerSummernote: registerSummernote,
        assignToFileData: assignToFileData,
        loadAffiliate: loadAffiliate,
        displayToastr: displayToastr,
        pagingControl: pagingControl,
        offrPosition: offrPosition,
        loadBasicData: loadBasicData,
        setPagination: setPagination,
        getProcessingStatus: getProcessingStatus
    };
})();