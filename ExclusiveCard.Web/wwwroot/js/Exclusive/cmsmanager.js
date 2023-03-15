var CmsManager = (function() {
    "use strict";
    var panels;
    var advertImage = null;
    var bgImage = null;
    var formData;
    var countrySelected = null;
    var storeData = null;
    var slotId = null;

    function errorHighlight(e, message) {
        $(".validation-summary-valid").html(message);
    }
    function loadSlots(type) {
        if (type === "1") {
            $("#Slot").html("<option value='0'>0 slots</option>");
        } else if (type === "2") {
            if (panels !== null && panels !== undefined) {
                var merchant = $("#Merchantpage").is(":checked");
                var search = $("#SearchResults").is(":checked");
                var home = $("#Homepage").is(":checked");
                var merchantSlot = $("#Merchantpage").attr("slots");
                var searchSlot = $('#SearchResults').attr("slots");
                var homeSlot = $('#Homepage').attr("slots");
                if (merchantSlot !== "" && searchSlot !== "" && homeSlot !== "") {
                    var min = 5;
                    if (merchant === true && search === true && home === true) {
                        min = Math.min(parseInt(merchantSlot), parseInt(searchSlot), parseInt(homeSlot));
                    } else if (merchant === true && search === true && home === false) {
                        min = Math.min(parseInt(merchantSlot), parseInt(searchSlot));
                    } else if (merchant === true && search === false && home === true) {
                        min = Math.min(parseInt(merchantSlot), parseInt(homeSlot));
                    } else if (merchant === false && search === true && home === true) {
                        min = Math.min(parseInt(homeSlot), parseInt(searchSlot));
                    } else if (merchant === false && search === false && home === true) {
                        min = parseInt(homeSlot);
                    } else if (merchant === true && search === false && home === false) {
                        min = parseInt(merchantSlot);
                    } else if (merchant === false && search === true && home === false) {
                        min = parseInt(searchSlot);
                    }
                    var item = "";

                    for (var u = 1; u <= min; u++) {
                        item = item + "<option value='" + u + "'>Slot " + u + "</option>";
                    }
                    $("#Slot").html(item);
                    $('#NumberOfSlots').val(min);
                } else {
                    toastr.error("Couldn't find the slots.");
                }
            }
        }
    }

    function checkUniqueName(name) {
        $.ajax({
            url: "/Cms/CheckName/?Name=" + name,
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "GET",
            data: null,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
            },
            success: function (data) {
                if (!data.success) {
                    toastr.error(data.errorMessage);
                } else {
                    $(".bannerSpinner").hide();
                }
            },
            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function storeToSession() {
        var homePage = $("#Homepage").is(":checked");
        var searchPage = $("#SearchResults").is(":checked");
        var merchantPage = $("#Merchantpage").is(":checked");
        if (homePage == false && searchPage == false && merchantPage == false) {
            toastr.error("Please select at least one panel.");
        }

        $("#Type").attr('disabled', false);
        if (countrySelected == null || countrySelected == undefined) {
            countrySelected = $("#CountryCode").find(":selected").val();
        }
        if (slotId == null || slotId == undefined) {
            slotId = $("#Slot").find(":selected").val();
            if (slotId == undefined) {
                slotId = 0;
            }
        }
        var fData = new FormData();
        fData.append('Name', $("#Name").val());
        fData.append('Url', $("#Url").val());
        fData.append('Type', $("#Type").val());
        fData.append('ImageFile', advertImage);
        fData.append('BgImageFile', bgImage);
        fData.append('Homepage', $('#Homepage').is(":checked"));
        fData.append('SearchResults', $('#SearchResults').is(":checked"));
        fData.append('Merchantpage', $('#Merchantpage').is(":checked"));
        fData.append('CountryCode', countrySelected);
        fData.append('NumberOfSlots', $('#NumberOfSlots').val());
        fData.append('Slot', slotId);
        fData.append('Image', $("#Image").val());
        fData.append('Background', $("#Background").val());

        $.ajax({
            url: "/Cms/AddToStore",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: fData,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
            },
            success: function (data) {
                if (data.success) {
                    storeData = data.data;
                    if (storeData !== null && storeData !== undefined) {
                        bindDataToView();
                    }
                } else {
                    toastr.error(data.errorMessage);
                    $("#Slot").val(slotId);
                }
            },
            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
                $("#Slot").val(slotId);
            }
        });
        if (formType == "Edit") {
            $("#Type").attr('disabled', true);
        }
    }

    function bindDataToView() {
        var selectedCountry = $("#CountryCode").find(":selected").val();
        //Check if store data has got data for selected country
        $.each(storeData,
            function(i, item) {
                $.each(item.panelAdverts, function (j, ad) {
                    if (ad.countryCode === selectedCountry) {
                        if (ad.panelId === 1) {
                            $("Homepage").prop('checked', true);
                        }
                        if (ad.panelId === 2) {
                            $("Merchantpage").prop('checked', true);
                        }
                        if (ad.panelId === 3) {
                            $("SearchResults").prop('checked', true);
                        }
                        //Set values in the store to view
                        $("#Name").val(item.name);
                        $("#Type").val(item.advertTypeId);
                        if (item.advertTypeId === 1) {
                            $("#Url").val(item.url);
                            $("#Slot").val(item.numberOfSlots);
                            if (item.ImagePath.includes("Avert/")) {
                                $("#imageDis").attr('src', "/Image/GetImage?path=" + item.imagePath);
                            } else {
                                $("#imageDis").attr('src', item.imagePath);
                            }
                            $("#Image").val(item.imagePath);
                        } else {
                            var slot = $('#Slot').find(":selected").val();
                            var index = -1;
                            $.each(item.advertSlots, function (e, item1) {
                                if (item1.slotId == parseInt(slot)) {
                                    index = 1;
                                    $("#Url").val(item1.url);
                                    if (item1.imagePath.includes("Avert/")) {
                                        $("#imageDis").attr('src',
                                            "/Image/GetImage?path=" +
                                            item1.imagePath);
                                    } else {
                                        $("#imageDis").attr('src',
                                            item1.imagePath);
                                    }
                                    $("#Image").val(item1.imagePath);

                                    if (item.imagePath.includes("Avert/")) {
                                        $("#bgImage").attr('src', "/Image/GetImage?path=" + item.imagePath);
                                    } else {
                                        $("#bgImage").attr('src', item.imagePath);
                                    }
                                    $("#Background").val(item.imagePath);
                                }
                            });
                            if (index === -1) {
                                $("#Url").val('');
                                $("#image-file").val('');
                            }
                        }
                    }
                });
            });
    }

    function setUpEvents() {
        $("#btnCancel").unbind().click(function(e) {
            e.preventDefault();
            window.history.back();
        });

        //Load spinner
        $('button').click(function () {
            $(this).toggleClass('navi');
        });

        $("#btnSave").unbind().click(function (e) {
            e.preventDefault();

            var homePage = $("#Homepage").is(":checked");
            var searchPage = $("#SearchResults").is(":checked");
            var merchantPage = $("#Merchantpage").is(":checked");
            if (homePage == false && searchPage == false && merchantPage == false) {
                toastr.error("Please select at least one panel.");
                $(this).toggleClass('navi');
                return;
            }

            $("#btnSave").addClass("active");
            formData = $(this).closest("form");
            var isValid = formData.valid();
            if (!isValid) {
                toastr.error("Please provide the required fields.");
                $(this).toggleClass('navi');
                return;
            } else {
                $("#Type").attr('disabled', false);
                if (countrySelected == null || countrySelected == undefined) {
                    countrySelected = $("#CountryCode").find(":selected").val();
                }
                if (slotId == null || slotId == undefined) {
                    slotId = $("#Slot").find(":selected").val();
                    if (slotId == undefined) {
                        slotId = 0;
                    }
                }
                //Store to Session and then save
                var fData = new FormData();
                fData.append('Name', $("#Name").val());
                fData.append('Url', $("#Url").val());
                fData.append('Type', $("#Type").val());
                fData.append('ImageFile', advertImage);
                fData.append('BgImageFile', bgImage);
                fData.append('Homepage', $('#Homepage').is(":checked"));
                fData.append('SearchResults', $('#SearchResults').is(":checked"));
                fData.append('Merchantpage', $('#Merchantpage').is(":checked"));
                fData.append('CountryCode', countrySelected);
                fData.append('NumberOfSlots', $('#NumberOfSlots').val());
                fData.append('Slot', slotId);
                fData.append('Image', $("#Image").val());
                fData.append('Background', $("#Background").val());

                $.ajax({
                    url: "/Cms/AddToStore",
                    beforeSend: function () {
                        $(".spinner").show();
                    },
                    type: "POST",
                    data: fData,
                    contentType: false,
                    processData: false,
                    cache: false,
                    complete: function () {
                        $(".spinner").hide();
                    },
                    success: function (data) {
                        if (data.success) {
                            storeData = data.data;
                            if (storeData !== null && storeData !== undefined) {
                                $.ajax({
                                    url: "/CMS/Save",
                                    beforeSend: function () {
                                        $(".spinner").show();
                                    },
                                    type: "Post",
                                    data: null,
                                    contentType: false,
                                    processData: false,
                                    cache: false,
                                    complete: function () {
                                    },
                                    success: function (data) {
                                        if (data.success) {
                                            toastr.success("Saving Advert Successful");
                                            setTimeout(function () {
                                                    window.history.back();
                                                },
                                                1500);
                                        } else {
                                            toastr.error(data.errorMessage);
                                        }
                                    },

                                    error: function (xhr, status, error) {
                                        toastr.error(xhr.responseText);
                                    }
                                });
                            }
                        } else {
                            toastr.error(data.errorMessage);
                        }
                    },
                    error: function (xhr, status, error) {
                        toastr.error(xhr.responseText);
                    }
                });
            }
        });
        
        $("#Type").unbind().change(function(e) {
            e.preventDefault();
            var selection = $(this).find(":selected").val();
            $('.divSlot').css('display', 'none');
            if (selection === '2') {
                $('.divSlot').css('display', 'block');
            }
            if (selection !== "") {
                loadSlots(selection);
            }
        });

        $("#CountryCode").click(function (e) {
            e.preventDefault();
            countrySelected = $(this).val();
            formData = $("#btnSave").closest("form");
            var isValid = formData.valid();
            if (isValid) {
                //Call method to save
            } else {
                toastr.error("Please provide the required fields.");
                return;
            }
        }).change(function(e) {
            var isValid = formData.valid();
            if (isValid) {
                storeToSession(formData);
            } else {
                toastr.error("Please provide the required fields.");
                return;
            } 
        });
        
        $("#Name").blur(function (e) {
            e.preventDefault();
            var name = $(this).val();
            if (name !== undefined && name !== null && name.trim() !== "") {
                checkUniqueName(name);
                $(".bannerSpinner").show();
            } else {
                $(".bannerSpinner").hide();
               toastr.error("Name is required");
            }
        });

        $('#Type').on('change', function () {
            $('.divSlot').css('display', 'none');
            if ($(this).val() === '2') {
                $('.divSlot').css('display', 'block');
            }
        });

        $("#Slot").click(function(e) {
            e.preventDefault();
            slotId = $(this).val();
        }).change(function(e) {
            e.preventDefault();
            if (bgImage == null) {
                toastr.error("Background image is required");
                $(this).val(slotId);
                return;
            }
            var selected = $(this).find(":selected").val();
            var url = $("#Url").val();
            var countrySelected = $("#CountryCode").find(':selected').val();
            var name = $("#Name").val();
            if (name !== '' && url !== null) {
                var error = false;
                //check for the image in store for the slot
                if (storeData == null || storeData == undefined) {
                    if (advertImage == null) {
                        error = true;
                        toastr.error("Slot image required.");
                    }
                } else {
                    $.each(storeData,
                        function(i, item) {
                            $.each(item.PanelAdverts,
                                function(j, ad) {
                                    if (ad.CountryCode === countrySelected) {
                                        $.each(item.AdvertSlots,
                                            function (j, item1) {
                                                if (item1.SlotId == parseInt(selected) && (item1.ImagePath == null || item1.ImagePath == undefined)) {
                                                    error = true;
                                                    toastr.error("Slot image required.");
                                                    $(this).val(slotId);
                                                }
                                            });
                                    }
                                });
                        });
                }
                if (error === false) {
                    storeToSession();
                }
            }
            });

        $("#Homepage").change(function(e) {
            e.preventDefault();
            var type = $("#Type").find(":selected").val();
            if (type == "2") {
                loadSlots(type);
            }
        });

        $("#SearchResults").change(function (e) {
            e.preventDefault();
            var type = $("#Type").find(":selected").val();
            if (type == "2") {
                loadSlots(type);
            }
        });

        $("#Merchantpage").change(function (e) {
            e.preventDefault();
            var type = $("#Type").find(":selected").val();
            if (type == "2") {
                loadSlots(type);
            }
        });
    }

    function initializePanels(panelData) {
        panels = panelData;
    }

    function initializeStoreData(data) {
        storeData = data;
    }

    function addAdvertImage(imageData) {
        advertImage = imageData;
    }

    function addBgImage(imgData) {
        bgImage = imgData;
    }

    return {
        setUpEvents: setUpEvents,
        initializePanels: initializePanels,
        addAdvertImage: addAdvertImage,
        addBgImage: addBgImage,
        initializeStoreData: initializeStoreData
    };
}());