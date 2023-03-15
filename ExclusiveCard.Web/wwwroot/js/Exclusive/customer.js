var Customer = (function() {
    "use strict";
    var formOriginalData;
    var type;
    var inputsChanged = true;

    function searchCustomer(pageNumber) {
        var username = $("#Username").val();
        var dob = $("#Dob").val();
        var forename = $("#Forename").val();
        var surname = $("#Surname").val();
        var cardnumber = $("#CardNumber").val();
        var carddateissued = $("#CardDateIssued").val();
        var cardstatus = $("#CardStatus").val();
        var registrationCode = $("#RegistrationCode").val();
        var postcode = $("#Postcode").val();

        var formData = new FormData();
        formData.append('Username', username);
        formData.append('Dob', dob);
        formData.append('Forename', forename);
        formData.append('Surname', surname);
        formData.append('CardNumber', cardnumber);
        formData.append('CardDateIssued', carddateissued);
        formData.append('CardStatus', cardstatus);
        formData.append('RegistrationCode', registrationCode);
        formData.append('Postcode', postcode);
        formData.append('PageNumber', pageNumber);

        $.ajax({
            url: "/Customer/Search",
            beforeSend: function() {
                $(".spinner").show();
            },
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            complete: function() {
                $(".spinner").hide();
            },
            success: function(data) {
                if (data) {
                    $("#customsearch").html(data);
                    setUpEvents();
                } else {
                    toastr.error(data);
                }
            },
            error: function(xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function getValidCards(id, validCards) {
        $.ajax({
            url: "/Customer/GetCards/?customerId=" + id + "&onlyValidCards=" + validCards,
            beforeSend: function() {
                $(".spinner").show();
            },
            type: "Get",
            data: null,
            datatype: "json",
            contentType: false,
            processData: false,
            cache: false,
            complete: function() {
                $(".spinner").hide();
            },
            success: function(data) {
                if (data) {
                    $("#membershipcarddetails").html(data);
                    setUpEvents();
                } else {
                    toastr.error(data);
                }
            },
            error: function(xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function updatePhysicalCardStatus(cardId, selected) {
        $.ajax({
            url: "/Customer/UpdatePhysicalStatus/?cardId=" + cardId + "&physicalStatus=" + selected,
            beforeSend: function() {
                $(".spinner").show();
            },
            type: "POST",
            data: null,
            datatype: "json",
            contentType: false,
            processData: false,
            cache: false,
            complete: function() {
                $(".spinner").hide();
            },
            success: function(data) {
                if (!data.success) {
                    toastr.error(data);
                }
            },
            error: function(xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function scrollfunction() {
        $('.offrcontent').animate({
                scrollTop: $("#mainDiv").offset().top
            },
            500);
    }

    function enableDisableCard(customerId, cardId, activate, validcard) {
        $.ajax({
            url: "/Customer/EnableDisableCard/?customerId=" + customerId + "&cardId=" + cardId + "&activate=" + activate + "&validCard=" + validcard,
            beforeSend: function () {
                //$(".spinner").show();
            },
            type: "POST",
            data: null,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                //$(".spinner").hide();
            },
            success: function (data) {
                if (data) {
                    $("#membershipcarddetails").html(data);
                    setUpEvents();
                } else {
                    toastr.error(data);
                }
                if (activate === true || activate === "true") {
                    $(".btnEnable").toggleClass('active');
                } else {
                    $(".btnDisable").toggleClass('active');
                }
            },
            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
                $(".btnDisable").toggleClass('active');
            }
        });
    }

    function disableConfirmation(title, message) {
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
                        $(".btnDisable").toggleClass('active');
                    },
                    "class": "btnNo"
                }
            }
        });
        return def.promise();
    }

    function setUpEvents() {
        formOriginalData = $("#btnSaveCustomer").closest("form").serialize();

        $("#btnSearchCustomer").unbind().click(function(e) {
            e.preventDefault();
            searchCustomer(1);
        });

        $(".customerList #editbutton").each(function() {
            $(this).unbind().click(function(e) {
                e.preventDefault();
                var id = $(this).attr("customerId");
                if (id > 0) {
                    window.location.href = "/Customer/Edit?id=" + id;
                }
            });
        });

        $(".cardList #viewbutton").each(function() {
            $(this).unbind().click(function(e) {
                e.preventDefault();
                var id = $(this).attr("cardId");
                if (id > 0) {
                    window.location.href = "/Customer/Edit?id=" + id + "&type=View";
                }
            });
        });

        $("#divCustomerList #pager a").each(function() {
            $(this).unbind().click(function(e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pagenumber');
                var form = $("#btnSearchOffer").closest("form");
                searchCustomer(pageNumber);
                scrollfunction();
            });
        });

        $("#btnCancelCustomer").unbind().click(function(e) {
            e.preventDefault();
            var _form = $(this).closest("form");
            if (_form.serialize() !== formOriginalData) {
                toastr.warning("The form will be closed.");
                setTimeout(function() {
                        window.history.back();
                        //if (type === "Edit") {
                        //    window.location.href = "/Customer";
                        //} else if (type === "View") {
                        //    window.location.href = "/Customer/Cards";
                        //}
                    },
                    1500);
            } else {
                window.history.back();
                //if (type === "Edit") {
                //    window.location.href = "/Customer";
                //} else if (type === "View") {
                //    window.location.href = "/Customer/Cards";
                //}
            }
        });

        $(".carddisplay").unbind().click(function(e) {
            e.preventDefault();
            var validcards = true;
            var lable = $(".carddisplay").text();

            if (lable.trim() === "Show all cards") {
                $(".carddisplay").text("Hide old cards");
                $(".membershipcarddetails").hide();
                validcards = false;
            } else {
                $(".carddisplay").text("Show all cards");
                $(".membershipcarddetails").show();
                validcards = true;
            }
            var customerId = $("#Id").val();
            if (customerId !== "") {
                getValidCards(customerId, validcards);
            } else {
                toastr.error("Customer not found.");
            }
        });

        $("#btnSaveCustomer").unbind().click(function(e) {
            e.preventDefault();
            $(this).prop('disabled', true);
            var id = $("#Id").val();
            var username = $("#Username").val();
            var forename = $("#Forename").val();
            var surname = $("#Surname").val();
            var dob = $("#Dob").val();
            var dateadd = $("#Dateadd").val();
            var newsletter = $("#MarketingNewsLetter").is(":checked");
            var thirdparty = $("#MarketingThirdParty").is(":checked");
            var address1 = $("#Address1").val();
            var address2 = $("#Address2").val();
            var address3 = $("#Address3").val();
            var phone = $("#Phone").val();
            var email = $("#EmailAddress").val();
            var formData = new FormData();

            formData.append("Id", id);
            formData.append("Username", username);
            formData.append("Forename", forename);
            formData.append("Surname", surname);
            formData.append("Dob", dob);
            formData.append("Dateadd", dateadd);
            formData.append("MarketingNewsLetter", newsletter);
            formData.append("MarketingThirdParty", thirdparty);
            formData.append("Address1", address1);
            formData.append("Address2", address2);
            formData.append("Address3", address3);
            formData.append("Phone", phone);
            formData.append("EmailAddress", email);
            formData.append("PostCode", $("#PostCode").val());
            formData.append("Town", $("#Town").val());
            formData.append("District", $("#District").val());
            formData.append("CountryCode", $("#CountryCode").val());

            $.ajax({
                url: "/Customer/Save",
                beforeSend: function() {
                    $(".spinner").show();
                },
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                complete: function() {
                    $(".spinner").hide();
                    $("#btnSaveCustomer").prop('disabled', false);
                },
                success: function(data) {
                    if (data.success) {
                        toastr.success("Update Customer Successful");
                    } else {
                        toastr.error(data.errorMessage);
                    }
                },
                error: function(xhr, status, error) {
                    $(".spinner").hide();
                    toastr.error(xhr.responseText);
                }
            });
        });

        $(".dropdownInputs").change(function(e) {
            e.preventDefault();
            var cardId = $(this).attr("cardId");
            var selected = $(this).find(":selected").val();
            if (cardId !== "" && selected !== "") {
                updatePhysicalCardStatus(cardId, selected);
            } else {
                toastr.error("Could not update membership phyical card status. Try again.");
            }
        });

        $('button').click(function() {
            if ($(this).hasClass('navi') === false) {
                $(this).toggleClass('navi');
            }
        });

        $(".btnEnable").click(function (e) {
            e.preventDefault();
            $(this).toggleClass('active');
            var customerId = $("#Id").val();
            var cardId = $(".dropdownInputs").attr("cardId");
            var lable = $(".carddisplay").text();
            var validCard = true;
            if (lable.trim() === "Show all cards") {
                validCard = false;
            }
            if (cardId !== "" && cardId !== undefined && cardId !== null && customerId !== "") {
                enableDisableCard(customerId, cardId, true, validCard);
            }
        });

        //$(".btnDisable").click(function (e) {
        //    e.preventDefault();
        //    var customerId = $("#Id").val();
        //    var cardId = $(".dropdownInputs").attr("cardId");
        //    //Expect to display all the cards so, validCard will be set to false
        //    if (cardId !== "" && cardId !== undefined && cardId !== null && customerId !== "") {
        //        enableDisableCard(customerId, cardId, false, false);
        //    }
        //});

        $(".btnDisable").unbind().click(function (e) {
            e.preventDefault();
            $(this).toggleClass('active');
            var customerId = $("#Id").val();
            var cardId = $(".dropdownInputs").attr("cardId");
            $.when(disableConfirmation("Card Status",
                    "Do you really want to Cancel Card?"))
                    .then(
                        function (status) {
                            if (status === "Yes") {
                               if(cardId !== "" && cardId !== undefined && cardId !== null && customerId !== "")
                                enableDisableCard(customerId, cardId, false, false);
                               
                            }
                        }
                    );
            

        });
    }

    function initialiseType(data) {
        type = data;
    }

    return {
        setUpEvents: setUpEvents,
        initialiseType: initialiseType
    };
}());