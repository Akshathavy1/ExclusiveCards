var Offers = (function () {
    "use strict";
    var categories, tags, offerTypes,finalSearchInput;
    var tagNameList = [];
    var inputChanged = false;

    function setCategories(data) {
        categories = data;
        categoryTree();

        //offerType tree initial bind
        $(".customTreeView a[href = 'javascript:;']").click(function (e) {
            inputChanged = true;
            if ($(this).children("i").hasClass("checked")) {
                $(this).children("i").removeClass("checked");
            }
            else {
                $(this).children("i").addClass("checked");
            }
        });
    }

    function categoryTree() {
        simTree({

            linkParent: true,

            // custom key
            response: {
                name: 'Name',
                id: 'Id',
                pid: 'ParentId',
                checked: 'IsActive',
                expand: 'expand',
                disabled: 'disabled',
                slug: 'UrlSlug'
            },

            el: '#categoryTree',
            data: categories,
            check: true,
            childNodeAsy: false,
            onChange: function () {
                //searchOffers();
                inputChanged = true;
            }
        });
    }


    function setTags(data) {

        tags = data;

        $.each(tags, function (i) {
            tagNameList.push(tags[i].Tags);
        });
    }

    function tagAutoComplete() {
        // create autocomplete UI
        $("#Keywords").autocomplete({
            source: tagNameList
        });
    }

    function addKeywords(keywords) {
        var keywordHtml = "<span class='keywordCard col-md-12'><label class='control-label col-md-9 keyword'>" + keywords + "</label><button class='col-md-2'><i class='fa fa-times'></i></button></span>";
        $("#keywordList").append(keywordHtml);
    }


    function kendoUIBind() {
        // create ComboBox from select HTML element
        $("#MerchantName").kendoComboBox({
            filter: "contains"

        });
    }

    //function for dialog message
    function popupoffers(title, message) {
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
                Close: {
                    id: 'btnYesId',
                    text: 'Close',
                    click: function () {
                        $("#infoMessage").html("");
                        $(this).dialog("close");
                        def.resolve("Yes");
                    },
                    "class": "btnYes"
                }
            }
        });
        return def.promise();
    }

    function setOfferTypes(data) {
        offerTypes = data;
        offerTypesTree();
    }

    function offerTypesTree() {
        simTree({

            linkParent: false,

            // custom key
            response: {
                name: 'Description',
                id: 'Id',
                pid: 0,
                checked: 'IsActive',
                expand: 'expand',
                disabled: 'disabled'
            },

            el: '#offerTypeTree',
            data: offerTypes,
            check: true,
            childNodeAsy: false,
            onChange: function () {
                //searchOffers();
                inputChanged = true;
            }
        });
    }

    //function for searchOffers window location
    function searchOffers(pageNumber) {
      
        var Url = "/Offers/DisplayOffer?country=" +
            countrySelected +
            "&parentCategoryId=" +
            $("#ParentCategoryId").val() +
            "&parentCategoryName=" +
            $("#ParentCategoryName").val().replace(/&/, '%26');

        var searchCategory = "";
        var searchOfferType = "";

        $("#displaySidebarFilter input[name='searchCategories']:checked").each(
            function () {
                searchCategory = searchCategory + $(this).attr("id") + ",";
            });
        if (searchCategory.length > 0) {
            searchCategory = searchCategory.substring(0, searchCategory.length - 1);
            Url = Url + "&searchCategories=" + searchCategory;
        }
        $("#displaySidebarFilter input[name='offer-type']:checked").each(function () {
            if ($.trim($(this).val()) === 'Cashback') {
                //amend the search
                searchOfferType = searchOfferType + "Standard Cashback,";
            }
            searchOfferType = searchOfferType + $(this).val() + ",";
        });
        if (searchOfferType.length > 0) {
            searchOfferType = searchOfferType.substring(0, searchOfferType.length - 1);
            Url = Url + "&searchOfferTypes=" + searchOfferType;
        }
        if ($("#displaySidebarFilter input[name='MerchantName']").val() !== null && $("#displaySidebarFilter input[name='MerchantName']").val() !== "" && $("#displaySidebarFilter input[name='MerchantName']").val() !== undefined) {
            //Url = Url + "&merchant=" + $("#displaySidebarFilter input[name='MerchantName']").val().replace(/&/, '%26');
            mainSearchInputValidating($("#displaySidebarFilter input[name='MerchantName']").val());
        }
        if (finalSearchInput !== null && finalSearchInput !== "" && finalSearchInput !== undefined) {
           finalSearchInput = finalSearchInput.replace('amp;', '');
            Url = Url + "&merchant=" + encodeURIComponent(finalSearchInput);
          }

        Url = Url + "&pageNumber=" + pageNumber;
        
        //if ($("#Keywords").val() !== null && $("#Keywords").val() !== "") {
        //    Url = Url + "&keyword=" + $("#Keywords").val();
        //}

        if ($("#offerCount").val() !== undefined && $("#offerCount").val() !== null) {
            Url = Url + "&pageSize=" + $("#offerCount").val();
        }
        if ($("#offerSort").val() !== undefined && $("#offerSort").val() !== null) {
            Url = Url + "&offerSort=" + $("#offerSort").val();
        }

        window.location.href = Url;
    }

    function searchMainOffers(pageNumber) {
        // $("#mainSearch").val()
        var Url = "/Offers/SearchByText?country=" + countrySelected;
        var searchCategory = "";
        var searchOfferType = "";
        $("#searchSidebarFilter input[name='searchCategories']:checked").each(
            function () {
                searchCategory = searchCategory + $(this).attr("id") + ",";
            });
        if (searchCategory.length > 0) {
            searchCategory = searchCategory.substring(0, searchCategory.length - 1);
            Url = Url + "&searchCategories=" + searchCategory;
        }
        $("#searchSidebarFilter input[name='offer-type']:checked").each(function () {
            if ($.trim($(this).val()) === 'Cashback') {
                //amend the search
                searchOfferType = searchOfferType + "Standard Cashback,";
            }
            searchOfferType = searchOfferType + $(this).val() + ",";
        });
        if (searchOfferType.length > 0) {
            searchOfferType = searchOfferType.substring(0, searchOfferType.length - 1);
            Url = Url + "&searchOfferTypes=" + searchOfferType;
        }
        if ($("#offerCount").val() !== undefined && $("#offerCount").val() !== null) {
            Url = Url + "&pageSize=" + $("#offerCount").val();
        }
        if ($("#offerSort").val() !== undefined && $("#offerSort").val() !== null) {
            Url = Url + "&offerSort=" + $("#offerSort").val();
        }
        //Url = Url + "&pageNumber=" + pageNumber + "&mainSearchTerm=" + $("input[name = 'mainSearchTerm']").val();
        if (finalSearchInput !== null && finalSearchInput !== "" && finalSearchInput !== undefined) {
            finalSearchInput = finalSearchInput.replace('amp;', '');
        }

        Url = Url + "&pageNumber=" + pageNumber + "&mainSearchTerm=" + encodeURIComponent(finalSearchInput);
        window.location.href = Url;
    }

    function mainSearchSorting() {
        searchMainOffers(1);
        return false;
    }

  function mainSearchInputValidating(searchInput) {
    // All clients
    var clientSideUntrustedInputOldStyle = searchInput;
      //injectedData.getAttribute("data-untrustedinput");

      // HTML 5 clients only
    var clientSideUntrustedInputHtml5 = searchInput;
      //injectedData.dataset.untrustedinput;

      // Put the injected, untrusted data into the scriptedWrite div tag.
      // Do NOT use document.write() on dynamically generated data as it
      // can lead to XSS.

      document.getElementById("scriptedWrite").innerText += clientSideUntrustedInputOldStyle;

      // Or you can use createElement() to dynamically create document elements
      // This time we're using textContent to ensure the data is properly encoded.
      var x = document.createElement("div");
      x.textContent = clientSideUntrustedInputHtml5;
      document.body.appendChild(x);

      // You can also use createTextNode on an element to ensure data is properly encoded.
      var y = document.createElement("div");
      y.appendChild(document.createTextNode(clientSideUntrustedInputHtml5));
      document.body.appendChild(y);
      finalSearchInput = y.innerHTML;
    }

    $("#btnmainSearchTerm").unbind().click(function (e) {
      var mainSearchTerm=$("input[name='mainSearchTerm']").val();
      mainSearchInputValidating(mainSearchTerm);
      searchMainOffers(1);
      return false;
    });

  $("input[name='mainSearchTerms']").keypress(function (e) {

    // Enter pressed?
      if (e.which === 10 || e.which === 13) {
        var mainSearchTerm = $("input[name='mainSearchTerms']").val();
        mainSearchInputValidating(mainSearchTerm);
        searchMainOffers(1);
        return false;
      }
    });


    function loadDisplayOffer(pageNumber, offerTypeId, offertypeName) {
        var formData = new FormData();

        $("#categoryTree .sim-tree-checkbox.checked").each(function (i) {
            formData.append("categories[" + i + "]", $(this).closest("li").attr("data-id"));
        });
        formData.append("offerTypeId", offerTypeId);
        formData.append("pageNumber", pageNumber);
        formData.append("keywords", $("#Keywords").val());
        formData.append("merchantName", $("#MerchantName").val());
        formData.append("offerTypeName", offertypeName);
        formData.append("listName", $("#listName").val());
        $.ajax({
            url: "/Offers/Search/?country=" + countrySelected,
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
            },
            success: function (data) {
                $("#offersResult [typeId=" + offerTypeId + "]").html(data);
                setUpEvents();
            },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });

    }

    function loadMainDisplayOffer(pageNumber, offerTypeId, offertypeName) {
        var formData = new FormData();
        formData.append("offerTypeId", offerTypeId);
        formData.append("pageNumber", pageNumber);
        formData.append("keyword", $("#mainSearch").val());
        formData.append("offerTypeName", offertypeName);
        $.ajax({
            url: "/Offers/PagedSearchTerm/?country=" + countrySelected,
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
            },
            success: function (data) {
                $("#offersResult [typeId=" + offerTypeId + "]").html(data);
                setUpEvents();
            },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function branchcontactpaging(currentpage, merchantid) {

        $.ajax({
            url: "/Offers/GetPagedBranchContact?merchantId=" + merchantid + "&currentPage=" + currentpage,
            beforeSend: function () {
                //$(".spinner").show();
            },
            type: "GET",

            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
            },
            success: function (data) {
                if (data) {
                    $(".merchtbranchfnt").html(data);
                    setUpEvents();
                    readMoreLess('.rdfull', 50);
                }
                else {
                    toastr.error(data);
                }
            },
            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function branchcontactscrollfunction() {
        $('html, body').animate({
            scrollTop: $(".newmerchbck").offset().top
        }, 500);
    }


    /*MERCHANT READMORE STARTS HERE*/
    function readMoreLess(className, length) {
        var visibleCharacters = length;
        var paragraph = $(className);


        paragraph.each(function () {
            var text = $(this).text().trim();
            var wholeText = text.slice(0, visibleCharacters) + "<span class='ellipsis'>&nbsp;</span><a href='#' class='more'>Read more</a>" + "<span style='display:none'>" + text.slice(visibleCharacters, text.length) + "<a href='#' class='less'>Hide</a></span>"

            if (text.length < visibleCharacters) {
                return;
            } else {
                $(this).html(wholeText);
            }
        });
        $(".more").click(function (e) {
            e.preventDefault();
            $(this).hide().prev().hide();
            $(this).next().show();
        });
        $(".less").click(function (e) {
            e.preventDefault();
            $(this).parent().hide().prev().show().prev().show();
        });
    };

    function merchantmoreLess() {
        readMoreLess('.merchtrdfull', 600);
    }
    /*MERCHANT READMORE ENDS HERE*/
    const items = document.querySelectorAll(".accordion a");

    function toggleAccordion() {
        this.classList.toggle('active');
        this.nextElementSibling.classList.toggle('active');
    }

    items.forEach(item => item.addEventListener('click', toggleAccordion));

    function validateAdddress() {
        var result = true;

        var add1 = $("#Address1").val();
        var postcode = $("#Postcode").val();
        var town = $("#Town").val();
        var country = $("#Country").val();
        var postCodeRegex = new RegExp("^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$");

        if (add1 === null || add1 === undefined || add1.trim() === "") {
            result = false;
            if (!$("#Address1").hasClass("--invalid")) {
                $("#Address1").addClass("--invalid");
            }
        } else {
            $("#Address1").removeClass("--invalid");
        }

        if (town === null || town === undefined || town.trim() === "") {
            result = false;
            if (!$("#Town").hasClass("--invalid")) {
                $("#Town").addClass("--invalid");
            }
        } else {
            $("#Town").removeClass("--invalid");
        }

        if (country === null || country === undefined || country.trim() === "") {
            result = false;
            if (!$("#Country").hasClass("--invalid")) {
                $("#Country").addClass("--invalid");
            }
        } else {
            $("#Country").removeClass("--invalid");
        }

        if (postcode === null || postcode === undefined || postcode.trim() === "") {
            result = false;
            if (!$("#Postcode").hasClass("--invalid")) {
                $("#Postcode").addClass("--invalid");
            }
        } else {
            $("#Postcode").removeClass("--invalid");
        }

        if (!postCodeRegex.test(postcode)) {
            result = false;
            if (!$("#Postcode").hasClass("--invalid")) {
                $("#Postcode").addClass("--invalid");
            }
        }

        return result;
    }

    function setUpEvents() {
        $("#searchSidebarFilter #btnApply").unbind().click(function (e) {
            e.preventDefault();
            $(this).prop("disabled", true);
            searchMainOffers(1);
        });

        $("#searchContentFilter #offerCount").on('change',
            function () {
                searchMainOffers(1);
            });

        $("#searchContentFilter #offerSort").on('change',
            function () {
                searchMainOffers(1);
            });

        $("#searchContentFilter .c_pagination a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pageNumber');
                if (pageNumber !== "0") {
                  if ($("input[name='mainSearchTerm']").val() !== "") {
                        var mainSearchTerm = $("input[name='mainSearchTerm']").val();
                        mainSearchInputValidating(mainSearchTerm);
                        searchMainOffers(pageNumber);
                    } else {
                        window.location.href = "/Home/Index?country=" + countrySelected;
                    }
                }
            });
        });

        $("#displaySidebarFilter #btnApply").unbind().click(function(e) {
            e.preventDefault();
            $(this).prop("disabled", true);
            searchOffers(1);
        });

        $("#displayContentFilter #offerCount").on('change',
            function() {
                searchOffers(1);
            });

        $("#displayContentFilter #offerSort").on('change',
            function() {
                searchOffers(1);
            });

        $("#displayContentFilter .c_pagination a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pageNumber');
                if (pageNumber !== "0") {
                    searchOffers(pageNumber);
                }
            });
        });

        $(".merchtbranchfnt #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pagenumber');
                var merchantId = $(".merchtbranchfnt").attr('MerchantId');
                branchcontactpaging(pageNumber, merchantId);
                branchcontactscrollfunction();

            });
        });
        $(".termpop").unbind().click(function (e) {
            e.preventDefault();
            $(this).addClass("visitlink");
            var message = $(this).children('#Terms')[0].value;
            if (message !== null && message !== "") {
                $.when(popupoffers("Terms",
                    message));
            }
        });

        $(".inspop").unbind().click(function (e) {
            e.preventDefault();
            $(this).addClass("visitlink");
            var message = $(this).children('#Instructions')[0].value;
            if (message != null && message != "") {
                $.when(popupoffers("Instructions",
                    message));
            }
        });

        $(".exlpop").unbind().click(function (e) {
            e.preventDefault();
            $(this).addClass("visitlink");
            var message = $(this).children('#Exclusions')[0].value;
            if (message != null && message != "") {
                $.when(popupoffers("Exculsion",
                    message));
            }
        });

        $(".codepop").unbind().click(function (e) {
            e.preventDefault();
            var message = $(this).children('#Code')[0].value;
            if (message != null && message != "") {
                $.when(popupoffers("Get Code",
                    message));
            }
        });

        //Load Spinner
        $('a, button').click(function () {
            $(this).toggleClass('active');
        });

        //$("#MerchantName").on('change', function () {
        //    inputChanged = true;
        //});

        //$("#Keywords").on('change', function () {
        //    inputChanged = true;
        //});

        $(".category").unbind().click(function (e) {
            e.preventDefault();
            //var categoryId = $(this).attr("categoryId");
            var categoryName = $(this).text();

            //window.location.href = "/Offers/DisplayOffer/?categories[0]=" + categoryId;
            window.location.href = "/Offers/DisplayOffer/?country=" + countrySelected + "&searchCategories=" + encodeURIComponent(categoryName);
        });

        $(".btn-cashback").unbind().click(function (e) {
            e.preventDefault();
            var merchantId = $(this).attr("merchantId");
            window.location.href = "/Offers/Index/?country=" + countrySelected + "&merchantId=" + merchantId;
        });

        $(".btnDetailCashback").unbind().click(function (e) {
            e.preventDefault();

            var deepLink = $(this).attr("linkurl");

            if (userSignedIn && deepLink === "1") {
                var offerId = $(this).attr("offerId");
                $('<a href="' + "/Offers/Redirect/?country=" + countrySelected + "&offerId=" + offerId + '" target="_blank"></a>')[0].click();
            } else {
                if (!userSignedIn) {
                    toastr.error("Please Login/Signup to continue.");
                } else if (deepLink === "0") {
                    toastr.error("Merchant url missing.");
                }
            }
        });

        $(".btnStandardCashback").unbind().click(function (e) {
            e.preventDefault();

            var deepLink = $('#Standard_0__DeepLinkAvailable').val();

                if (userSignedIn && deepLink.toLowerCase() === "true") {
                    var offerId = $(this).attr("offerId");
                    $('<a href="' + "/Offers/Redirect/?country=" + countrySelected + "&offerId=" + offerId + '" target="_blank"></a>')[0].click();
                } else {
                    if (!userSignedIn) {
                        toastr.error("Please Login/Signup to continue.");
                    } else if (deepLink.toLowerCase() === "false") {
                        toastr.error("Merchant url missing.");
                    }
                }
        });

        $("#btnShop").unbind().click(function(e) {
            e.preventDefault();
            $("#btnShop").prop("disabled", true);
            var valid = validateAdddress();

            if (valid) {
                var formData = new FormData();
                formData.append("Id", $("#Id").val());
                formData.append("UserId", $("#UserId").val());
                formData.append("MembershipCardId", $("#MembershipCardId").val());
                formData.append("OfferId", $("#OfferId").val());
                formData.append("Address1", $("#Address1").val());
                formData.append("Address2", $("#Address2").val());
                formData.append("Address3", $("#Address3").val());
                formData.append("Town", $("#Town").val());
                formData.append("County", $("#County").val());
                formData.append("Postcode", $("#Postcode").val());
                formData.append("Country", $("#Country").val());
                $.ajax({
                    url: "/Offers/RedeemOffer",
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
                    },
                    success: function (data) {
                        if (data.success) {
                            $("#successRedeem").html("Congratulations, your Love2Shop card will be posted to you shortly.");
                            $("#successRedeem").focus();
                            if ($("#successRedeem").hasClass("hideContent")) {
                                $("#successRedeem").removeClass("hideContent");
                            }
                            if (!$("#errorRedeem").hasClass("hideContent")) {
                                $("#errorRedeem").addClass("hideContent");
                            }
                            window.location.href = '/Offers/RedeemSuccess';
                        } else {
                            $("#errorRedeem").html("Error redeeming the offer.");
                            if ($("#errorRedeem").hasClass("hideContent")) {
                                $("#errorRedeem").removeClass("hideContent");
                            }
                            $("#btnShop").prop("disabled", false);
                            window.location.href = '/Offers/RedeemFail';
                        }
                    },

                    error: function (xhr, status, error) {
                        $("#errorRedeem").html("Error redeeming the offer.");
                        $("#errorRedeem").focus();
                        if ($("#errorRedeem").hasClass("hideContent")) {
                            $("#errorRedeem").removeClass("hideContent");
                        }
                        $("#btnShop").prop("disabled", false);
                        window.location.href = '/Offers/RedeemFail';
                    }
                });
            } else {
                if ($("#errorRedeem").hasClass("hideContent")) {
                    $("#errorRedeem").removeClass("hideContent");
                }
                $("#btnShop").prop("disabled", false);
                //window.location.href = '/Offers/RedeemFail';
            }
        });
    }

  function shortSearch(searchTerm) {
    $.ajax({
            url: "/Offers/ShortSearch?country=" + countrySelected +"&searchTerm=" + searchTerm,
            beforeSend: function () {
                //$("#loader").show();
            },
            type: "GET",
            cache: false,
            complete: function () {
                //$("#loader").hide();
            },
          success: function (data) {
            if (data.success) {
                    if (data.data !== null) {
                        //var tagData = data.data.merchantName.trim();
                        var tagData = data.data[0].merchantName;
                        if (data.data.offerShortDescription !== null && data.data.offerShortDescription !== undefined) {
                            tagData = tagData + " - " + data.data.offerShortDescription.trim();
                        }
                        var tag = "<select><option value=\"" + tagData + "\" merchant=" + data.data[0].merchantId + " offer=" + data.data[0].offerId + "></option></select>";
                        $("#merchants").html(tag);
                        var datalist = document.querySelector('datalist');
                        var select = document.querySelector('select');
                        var options = select.options;
                        /* if DDL is hidden */
                        if (datalist.style.display === '') {
                            /* show DDL */
                           // datalist.style.display = 'block';
                            for (var i = 0; i < options.length; i++) {
                                if (options[i].value.trim() == tagData) {
                                    select.selectedIndex = i;
                                    $("input[name='mainSearchTerm']").trigger("keydown");
                                    break;
                                }
                            }
                        }
                    }
                }
                else {
                    alert("Some error occurred. Please try again.");
                }
            },

            error: function (xhr, status, error) {
                alert(xhr.responseText);
            }
        });
    }

    return {
        setUpEvents: setUpEvents,
        shortSearch: shortSearch
    };
})();