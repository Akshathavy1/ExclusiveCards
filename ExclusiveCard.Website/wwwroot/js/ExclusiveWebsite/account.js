var Account = (function () {
    "use strict";
    var FromDetails;
    var form_submit = false;
    var orderFormSubmit = false;
    var btnLabel = '';

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

    function popupoPayment(title, message) {
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

    function createAccount(formData) {
        var paidByEmployer = $("#PaidByEmployer").val();
        var upgrade = $("#DiamondUpgrade").val();
        var data = "";
        if (formData !== null) {
            data = formData.serialize();
        }
        $.ajax({
            url: "/Account/Save",
            beforeSend: function () {
                //$("#loader").show();
            },
            type: "POST",
            data: data,
            cache: false,
            complete: function () {
                //$("#loader").hide();
            },
            success: function (data) {
                if (data.success) {
                    //data.data is userToken object
                    if (data.data.id > 0) {
                        window.buttonLoading(document.querySelector("#btnContinue"), false, btnLabel);
                    }
                    setTimeout(function () {
                            if ((paidByEmployer === "false" || paidByEmployer === "False") && 
                              (upgrade === "true" || upgrade === "True")) {
                              window.location.href = "/Home";
                                //window.location.href = "/PayPal/Payment?isDiamondUpgrade=true";
                            } else {
                                window.location.href = "/Home";
                            }
                        },
                        1500);
                }
                else {
                    window.buttonLoading(document.querySelector("#btnContinue"), false, btnLabel);
                    $("#customError").html(data.errorMessage);
                    $("#customError").removeClass("hideContent");
                    window.scrollTo(0, 1150);
                    //setTimeout(function () {
                    //    window.location.href = "/Account";
                    //},
                    //    1500);
                }
            },

            error: function (xhr, status, error) {
                window.buttonLoading(document.querySelector("#btnContinue"), false, btnLabel);
                $("#customError").val(data.errorMessage);
                $("#customError").removeClass("hideContent");
                window.scrollTo(0, 1150);
            }
        });
    }

    function createAddOnFree(countryCode, customerId, membershipPlan, token) {
        $.ajax({
            url: "/Account/CreateAddOnFree/?countryCode=" +
                countryCode +
                "&customerId=" +
                customerId +
                "&membershipPlanId=" +
                membershipPlan +
                "&token=" +
                token,
            beforeSend: function () {
                //$("#loader").show();
            },
            type: "POST",
            data: null,
            cache: false,
            complete: function () {
                //$("#loader").hide();
            },
            success: function (data) {
                if (data.success) {
                    //data.data is userToken object
                    if (data.data.id > 0) {
                        toastr.success("Membership card created succesfully.");
                    }
                    setTimeout(function () {
                            window.location.href = "/Account/AccountSummary";
                        },
                        1500);
                }
                else {
                    if (data.errorMessage !== null) {
                        toastr.error(data.errorMessage);
                    }
                    //setTimeout(function () {
                    //    window.location.href = "/Account";
                    //},
                    //    1500);
                }
            },

            error: function (xhr, status, error) {
                if (xhr.responseText === "") {
                    toastr.success("Membership card created succesfully.");
                    setTimeout(function () {
                            window.location.href = "/Account/AccountSummary";
                        },
                        1500);
                } else {
                    $('#btnAccountSetup').prop("disabled", false);
                    toastr.error(xhr.responseText);
                }
            }
        });
    }

    function createAddOn(formdata) {
        $.ajax({
            url: "/Account/CreateAddOn",
            beforeSend: function () {
                //$("#loader").show();
            },
            type: "POST",
            data: formdata.serialize(),
            cache: false,
            complete: function () {
                //$("#loader").hide();
            },
            success: function (data) {
                if (data.success) {
                    //data.data is userToken object
                    if (data.data.id > 0) {
                        toastr.success("Membership card created succesfully.");
                    }
                    setTimeout(function () {
                            window.location.href = "/Account/AccountSummary";
                        },
                        1500);
                }
                else {
                    if (data.errorMessage !== null) {
                        $("#btnContinuePayment").prop("disabled", false);
                        toastr.error(data.errorMessage);
                    }
                    //setTimeout(function () {
                    //    window.location.href = "/Account";
                    //},
                    //    1500);
                }
            },

            error: function (xhr, status, error) {
                if (xhr.responseText === "") {
                    toastr.success("Membership card created succesfully.");
                    setTimeout(function () {
                            window.location.href = "/Account/AccountSummary";
                        },
                        1500);
                } else {
                    toastr.error(xhr.responseText);
                }
            }
        });
    }

    function getAccountSummary(pageNumber, customer, card, status) {
        window.location.href = "/Account/AccountStatement/?country=" + countrySelected + "&customerId=" + customer + "&cardId=" + card + "&currentPage=" + pageNumber + "&accountStatus" + status;
    }

    function termsHidden() {
        $(".search").hide();
        $("#myTopnav").hide();
        $(".mm-dropdown").hide();
    }

    function validateRegistrationCode(code) {
        //$("#loader").show();
        var customer = $("#CustomerId").val();
        if (customer === "") {
            customer = null;
        }
        $.get("/account/RegisterValidation", { code: code, customerId: customer }, function (data, textStatus, jqXHR) {
            if (data.success) {
                
                if (data.data.Name !== "") {
                    var cardCost = data.data.cardCost;
                    if (customer !== "" && customer !== undefined && customer !== null &&
                        (parseFloat(cardCost).toFixed(2) == "0.00" || parseFloat(cardCost).toFixed(2) == 0.00 || parseFloat(cardCost).toFixed(2) == 0)) {
                        createAddOnFree(countrySelected,
                            parseInt(customer),
                            data.data.membershipPlanId,
                            data.data.token);
                    }
                    else if (customer !== "" && customer !== undefined && customer !== null && cardCost > 0) {
                        window.location.href = "/Account/SelectPayment/?country=" +
                            countrySelected +
                            "&customerId=" + parseInt(customer) +
                            "&membershipPlanId=" +
                            data.data.membershipPlanId +
                            "&token=" +
                            data.data.token;
                    } else {
                        window.location.href = "/Account/CreateAccount/?country=" +
                            countrySelected +
                            "&membershipPlanId=" +
                            data.data.membershipPlanId +
                            "&token=" +
                            data.data.token;
                    }
                }
                $("#btnAccountSetup").removeClass("active");
                $('#btnAccountSetup').prop("disabled", false);
            }
            else {
                //toastr.error(data.errorMessage);
                $.when(popupoffers("The registration code entered is invalid.",
                    data.errorMessage));
                $("#btnAccountSetup").removeClass("active");
                $('#btnAccountSetup').prop("disabled", false);
                //$("#loader").hide();
            }
        });
    }

    function validateDefaultPlan(customer) {
        $.get("/account/ValidateDefaultPlan",
            { customerId: customer },
            function(data,
                textStatus,
                jqXHR) {
                if (data.success) {
                    window.location.href = "/Account/SelectPayment/?country=" +
                        countrySelected +
                        "&customerId=" +
                        parseInt(customer) +
                        "&membershipPlanId=" +
                        defaultMembershipPlan;
                } else {
                    $.when(popupoffers("Warning!",
                        data.errorMessage));
                    $("#btnBuyNow").removeClass("active");
                    $('#btnBuyNow').prop("disabled", false);
                }
            });
    }

    //before paypal redirect validate the account details
    function accountValidation() {
        if (!$("#Check").is(":checked")) {
            $('#submitAccountForm').prop("disabled", false);
            toastr.error("Please select agree terms and continue");
        }
        else {
            var data = "";
            if (FromDetails !== null && FromDetails !== undefined) {
                data = FromDetails.serialize();
            } else {
                var _form = $("#btnContinuePayment").closest("form");
                data = _form.serialize();
            }
            //Check if customerId > 0
            var cust = $("#Id").val();
            if (cust === "" || cust === "0" || cust === undefined) {
                $.ajax({
                    url: "/PayPal/PayPalRequest",
                    beforeSend: function () {
                        $("#waitpaypal").show();
                        //$("#loader").show();
                    },
                    type: "POST",
                    data: data,
                    cache: false,
                    complete: function () {
                        //$("#loader").hide();
                        $("#waitpaypal").hide();
                    },
                    success: function (data) {
                        if (data.success) {
                            $("#customerPaymentProviderId").val(data.data);
                            form_submit = true;
                            $('#submitAccountForm').submit();
                        } else {
                            $('#submitAccountForm').prop("disabled", false);
                            toastr.error(data.errorMessage);
                        }
                    },
                    error: function (xhr, status, error) {
                        $('#submitAccountForm').prop("disabled", false);
                        toastr.error(xhr.responseText);
                    }
                });
            } else if (cust !== "") {
                $.ajax({
                    url: "/PayPal/AddOnPayPalRequest",
                    beforeSend: function () {
                        $("#waitpaypal").show();
                        //$("#loader").show();
                    },
                    type: "POST",
                    data: data,
                    cache: false,
                    complete: function () {
                        //$("#loader").hide();
                        $("#waitpaypal").hide();
                    },
                    success: function (data) {
                        if (data.success) {
                            $("#customerPaymentProviderId").val(data.data);
                            form_submit = true;
                            $('#submitAccountForm').submit();
                        } else {
                            $('#submitAccountForm').prop("disabled", false);
                            toastr.error(data.errorMessage);
                        }
                    },
                    error: function (xhr, status, error) {
                        $('#submitAccountForm').prop("disabled", false);
                        toastr.error(xhr.responseText);
                    }
                });
            }
        }
    }

    function reloadAccountSummary(card, customer) {
        window.location.href = "/Account/AccountSummary/?country=" + countrySelected + "&customerId=" + customer + "&cardId=" + card;
    }

    /*Plan description READMORE STARTS HERE*/
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

    function planDescription() {
        readMoreLess(".planDescDiv", 200);
    }
    /*Plan Description READMORE ENDS HERE*/
    const items = document.querySelectorAll(".accordion a");

    function toggleAccordion() {
        this.classList.toggle('active');
        this.nextElementSibling.classList.toggle('active');
    }
    //New Upgrade Membership Plan via payPal
    function validateOrderSummary() {
        var membershipPlanId = $("#MembershipPlanId").val();
        var formData = {};// empty JSON object
        formData["MembershipPlanId"] = membershipPlanId;
        $.ajax({
            url: "/PayPal/PayPalRequestUpgradeAccount",
            beforeSend: function () {
                //$("#waitpaypal").show();
                //$("#loader").show();
            },
            type: "POST",
            data: { MembershipPlanId : membershipPlanId},
            cache: false,
            complete: function () {
                //$("#loader").hide();
                //$("#waitpaypal").hide();
            },
            success: function (data) {
                console.log(data);
                if (data.success) {
                    $("#customerPaymentProviderId").val(data.data);
                    orderFormSubmit = true;
                    $('#submitOrderPayment').submit();
                } else {
                    $('#btnPayPalOrder').prop("disabled", false);
                    console.log(data.errorMessage);
                }
            },
            error: function (xhr, status, error) {
                $('#btnPayPalOrder').prop("disabled", false);
                console.log(xhr.responseText);
            }
        });
    }

    function validateRegistrationForm() {
        var result = true;
        //var title = $("#Title").val();
        var forename = $("#Forename").val();
        var surname = $("#Surname").val();
        var email = $("#Email").val();
        var confirmEmail = $("#Confirmemail").val();
        var password = $("#Password").val();
        var confirmPassword = $("#ConfirmPassword").val();
        //var dob = $("#Dateofbirth").val();
        //var postcode = $("#Postcode").val();
        //var question = $("#QuestionId").val();
        //var answer = $("#Answer").val();

        var postCodeRegex = new RegExp("^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$");
        var passwordRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&^#])[A-Za-z\\d@$!%*?&^#]{8,48}$");

        //if (title === null || title === undefined || title.trim() === "") {
        //    result = false;
        //    if (!$("#Title").hasClass("--invalid")) {
        //        $("#Title").addClass("--invalid");
        //    }
        //} else {
        //    $("#Title").removeClass("--invalid");
        //}

        if (forename === null || forename === undefined || forename.trim() === "") {
            result = false;
            if (!$("#Forename").hasClass("--invalid")) {
                $("#Forename").addClass("--invalid");
            }
        } else {
            $("#Forename").removeClass("--invalid");
        }

        if (surname === null || surname === undefined || surname.trim() === "") {
            result = false;
            if (!$("#Surname").hasClass("--invalid")) {
                $("#Surname").addClass("--invalid");
            }
        } else {
            $("#Surname").removeClass("--invalid");
        }

        if (email === null || email === undefined || email.trim() === "") {
            result = false;
            if (!$("#Email").hasClass("--invalid")) {
                $("#Email").addClass("--invalid");
            }
        } else {
            $("#Email").removeClass("--invalid");
        }

        if (confirmEmail === null || confirmEmail === undefined || confirmEmail.trim() === "") {
            result = false;
            if (!$("#Confirmemail").hasClass("--invalid")) {
                $("#Confirmemail").addClass("--invalid");
            }
        } else {
            $("#Confirmemail").removeClass("--invalid");
        }

        if (password === null || password === undefined || password.trim() === "") {
            result = false;
            if (!$("#Password").hasClass("--invalid")) {
                $("#Password").addClass("--invalid");
            }
        } else {
            $("#Password").removeClass("--invalid");
        }

        if (confirmPassword === null || confirmPassword === undefined || confirmPassword.trim() === "") {
            result = false;
            if (!$("#ConfirmPassword").hasClass("--invalid")) {
                $("#ConfirmPassword").addClass("--invalid");
            }
        } else {
            $("#ConfirmPassword").removeClass("--invalid");
        }

        //if (dob === null || dob === undefined || dob.trim() === "") {
        //    result = false;
        //    if (!$("#Dateofbirth").hasClass("--invalid")) {
        //        $("#Dateofbirth").addClass("--invalid");
        //    }
        //} else {
        //    $("#Dateofbirth").removeClass("--invalid");
        //}

        //if (postcode === null || postcode === undefined || postcode.trim() === "") {
        //    result = false;
        //    if (!$("#Postcode").hasClass("--invalid")) {
        //        $("#Postcode").addClass("--invalid");
        //    }
        //} else {
        //    $("#Postcode").removeClass("--invalid");
        //}

        //if (question === null || question === undefined || question.trim() === "") {
        //    result = false;
        //    if (!$("#QuestionId").hasClass("--invalid")) {
        //        $("#QuestionId").addClass("--invalid");
        //    }
        //} else {
        //    $("#QuestionId").removeClass("--invalid");
        //}

        //if (answer === null || answer === undefined || answer.trim() === "") {
        //    result = false;
        //    if (!$("#Answer").hasClass("--invalid")) {
        //        $("#Answer").addClass("--invalid");
        //    }
        //} else {
        //    $("#Answer").removeClass("--invalid");
        //}

        if (email.trim() !== confirmEmail) {
            result = false;
            if (!$("#Confirmemail").hasClass("--invalid")) {
                $("#Confirmemail").addClass("--invalid");
            }
            if (!$("#Email").hasClass("--invalid")) {
                $("#Email").addClass("--invalid");
            }
        }

        if (password.trim() !== confirmPassword) {
            result = false;
            if (!$("#ConfirmPassword").hasClass("--invalid")) {
                $("#ConfirmPassword").addClass("--invalid");
            }
            if (!$("#Password").hasClass("--invalid")) {
                $("#Password").addClass("--invalid");
            }
        }

        if (!passwordRegex.test(password)) {
            result = false;
            if (!$("#ConfirmPassword").hasClass("--invalid")) {
                $("#ConfirmPassword").addClass("--invalid");
            }
            if (!$("#Password").hasClass("--invalid")) {
                $("#Password").addClass("--invalid");
            }
        }

        //if (!postCodeRegex.test(postcode)) {
        //    result = false;
        //    if (!$("#Postcode").hasClass("--invalid")) {
        //        $("#Postcode").addClass("--invalid");
        //    }
        //}

        return result;
    }


    function setUpEvents() {

    //$(window).unload(function () { window.location.href = "/Account/Logout"; });
        //Terms Condition Page Js
        $('#close_window').on('click', function () {
            window.close();
        });

        //Index Page Js
        $("#btnBuyNow").unbind().click(function () {
            $(this).toggleClass('active');
            $(this).prop("disabled", true);
            var customer = $("#CustomerId").val();
            if (customer === "") {
                window.location.href =
                    "/Account/CreateAccount/?country=" +
                    countrySelected +
                    "&membershipPlanId=" +
                    defaultMembershipPlan;
            } else if (customer !== "") {
                validateDefaultPlan(customer);
            }
        });

        $("#btnAccountSetup").unbind().click(function (e) {
            e.preventDefault();
            $(this).toggleClass('active');
            $(this).prop("disabled", true);
            
            var code = $("#registerCode").val();
            if (code !== null && code !== undefined && code !== "") {
                validateRegistrationCode(code);
            } else {
                toastr.error("Please enter registration code and continue");
                $(this).toggleClass('active');
                $(this).prop("disabled", false);
            }
        });

        //CreateAccount Page Js
        if (!$("#Tick").is(":checked")) {
            $("#Addressone").attr("disabled", true);
            $("#Addresstwo").attr("disabled", true);
            $("#Addressthree").attr("disabled", true);
            $("#Town").attr("disabled", true);
            $("#County").attr("disabled", true);
            //$("#CountryId").attr("disabled", true);
            $("#hostedbuttonid").val($("#SubscribeAppRef").val());
            $(".bothcards").hide();
            $(".singlecard").show();

        } else {
            $(".bothcards").show();
            $(".singlecard").hide();
            $("#hostedbuttonid").val($("#SubscribeAppAndCardRef").val());
        }

        $("#Tick").change(function (e) {
            e.preventDefault();
            if ($("#Tick").is(":checked")) {
                $("#Addressone").removeAttr("disabled");
                $("#Addresstwo").removeAttr("disabled");
                $("#Addressthree").removeAttr("disabled");
                $("#Town").removeAttr("disabled");
                $("#County").removeAttr("disabled");
                //$("#CountryId").removeAttr("disabled");
                $("#hostedbuttonid").val($("#SubscribeAppAndCardRef").val());
                $(".bothcards").show();
                $(".singlecard").hide();
            }
            else {
                $("#Addressone").attr("disabled", true);
                $("#Addresstwo").attr("disabled", true);
                $("#Addressthree").attr("disabled", true);
                $("#Town").attr("disabled", true);
                $("#County").attr("disabled", true);
                //$("#CountryId").attr("disabled", true);
                $("#hostedbuttonid").val($("#SubscribeAppRef").val());
                $(".bothcards").hide();
                $(".singlecard").show();
            }
        });

        $("#marketing").change(function(e) {
            e.preventDefault();
            var checked = $(this).is(":checked");
            $("#MarketingPreference").val(checked);
        });

        $("#btnContinue").unbind().click(function (e) {
            e.preventDefault();
            btnLabel = $('#btnContinue')[0].innerText;
            window.buttonLoading(document.querySelector("#btnContinue"), true, "Creating Account");
            if (!$("#customError").hasClass("hideContent")) {
                $("#customError").addClass("hideContent");
            }
            if (!$("#msgError").hasClass("hideContent")) {
                $("#msgError").addClass("hideContent");
            }
            if ($("#remember").hasClass("--invalid")) {
                $("#remember").removeClass("--invalid");
            }
            var _form = $(this).closest("form");
            var isValid = validateRegistrationForm();
            if (isValid) {
                if ($("#Email").val() !== $("#Confirmemail").val()) {
                    window.buttonLoading(document.querySelector("#btnContinue"), false, btnLabel);
                    if ($("#msgError").hasClass("hideContent")) {
                        $("#msgError").removeClass("hideContent");
                    }
                    window.scrollTo(0, 1150);
                }
                else if ($("#Password").val() !== $("#ConfirmPassword").val()) {
                    window.buttonLoading(document.querySelector("#btnContinue"), false, btnLabel);
                    if ($("#msgError").hasClass("hideContent")) {
                        $("#msgError").removeClass("hideContent");
                    }
                    window.scrollTo(0, 1150);
                }
                else if (!$("#remember").is(":checked")) {
                    window.buttonLoading(document.querySelector("#btnContinue"), false, btnLabel);
                    if ($("#customError").hasClass("hideContent")) {
                        $("#customError").removeClass("hideContent");
                        $("#customError").html("Please accept Terms and Conditions to continue.");
                    }
                    window.scrollTo(0, 1150);
                }
                else {
                    var token = $("#Token").val();
                    var cardPrice = parseFloat($("#CardCost").val()).toFixed(2);
                    if (token !== null && token !== undefined && token !== "" && (cardPrice === "0.00" || cardPrice === 0.00 || cardPrice === 0)) {
                        createAccount(_form);
                    } else {
                        FromDetails = _form;
                        $(".accountFormDiv").addClass("hidden");
                        $("#ChoosePaymentScreen").removeClass("hidden");
                        window.scrollTo(0, 1150);
                    }
                    //$(this).toggleClass('active');
                    //$(this).prop("disabled", false);
                }
            } else {
                if ($("#msgError").hasClass("hideContent")) {
                    $("#msgError").removeClass("hideContent");
                }
                
                window.buttonLoading(document.querySelector("#btnContinue"), false, btnLabel);
                window.scrollTo(0, 1150);
            }
        });

        //Load spinner
        $('a, button').click(function () {
            $(this).toggleClass('active');
        });

        /****************chart Starts Here****************************/
        (function ($) {

            $.fn.circliful = function (options, callback) {

                var settings = $.extend({
                    // These are the defaults.
                    fgcolor: "#556b2f",
                    bgcolor: "#eee",
                    fill: false,
                    width: 15,
                    dimension: 200,
                    fontsize: 15,
                    percent: 50,
                    animationstep: 1.0,
                    iconsize: '20px',
                    iconcolor: '#999',
                    border: 'default',
                    complete: null
                }, options);

                return this.each(function () {
                    var customSettings = ["fgcolor", "bgcolor", "fill", "width", "dimension", "fontsize", "animationstep", "endPercent", "icon", "iconcolor", "iconsize", "border"];
                    var customSettingsObj = {};
                    var icon = '';
                    var endPercent = 0;
                    var obj = $(this);
                    var fill = false;
                    var text, info;
                    var percent;

                    obj.addClass('circliful');

                    checkDataAttributes(obj);

                    if (obj.data('text') !== undefined) {
                        text = obj.data('text');

                        if (obj.data('icon') !== undefined) {
                            icon = $('<i></i>')
                                .addClass('fa ' + $(this).data('icon'))
                                .css({
                                    'color': customSettingsObj.iconcolor,
                                    'font-size': customSettingsObj.iconsize
                                });
                        }

                        if (obj.data('type') !== undefined) {
                            type = $(this).data('type');

                            if (type === 'half') {
                                addCircleText(obj, 'circle-text-half', (customSettingsObj.dimension / 1.45));
                            } else {
                                addCircleText(obj, 'circle-text', customSettingsObj.dimension);
                            }
                        } else {
                            addCircleText(obj, 'circle-text', customSettingsObj.dimension);
                        }
                    }

                    if ($(this).data("total") !== undefined && $(this).data("part") != undefined) {
                        var total = $(this).data("total") / 100;

                        percent = (($(this).data("part") / total) / 100).toFixed(3);
                        endPercent = ($(this).data("part") / total).toFixed(3)
                    } else {
                        if ($(this).data("percent") !== undefined) {
                            percent = $(this).data("percent") / 100;
                            endPercent = $(this).data("percent");
                        } else {
                            percent = settings.percent / 100;
                        }
                    }

                    if ($(this).data('info') !== undefined) {
                        info = $(this).data('info');

                        if ($(this).data('type') !== undefined) {
                            type = $(this).data('type');

                            if (type === 'half') {
                                addInfoText(obj, 0.9);
                            } else {
                                addInfoText(obj, 1.25);
                            }
                        } else {
                            addInfoText(obj, 1.25);
                        }
                    }

                    $(this).width(customSettingsObj.dimension + 'px');

                    var canvas = $('<canvas></canvas>').attr({
                        width: customSettingsObj.dimension,
                        height: customSettingsObj.dimension
                    }).appendTo($(this)).get(0);

                    var context = canvas.getContext('2d');
                    var x = canvas.width / 2;
                    var y = canvas.height / 2;
                    var degrees = customSettingsObj.percent * 360.0;
                    var radians = degrees * (Math.PI / 180);
                    var radius = canvas.width / 2.5;
                    var startAngle = 2.3 * Math.PI;
                    var endAngle = 0;
                    var counterClockwise = false;
                    var curPerc = customSettingsObj.animationstep === 0.0 ? endPercent : 0.0;
                    var curStep = Math.max(customSettingsObj.animationstep, 0.0);
                    var circ = Math.PI * 2;
                    var quart = Math.PI / 2;
                    var type = '';
                    var fireCallback = true;

                    if ($(this).data('type') !== undefined) {
                        type = $(this).data('type');

                        if (type === 'half') {
                            startAngle = 2.0 * Math.PI;
                            endAngle = 3.13;
                            circ = Math.PI * 1.0;
                            quart = Math.PI / 0.996;
                        }
                    }

                    /**
                     * adds text to circle
                     * 
                     * @param obj
                     * @param cssClass
                     * @param lineHeight
                     */
                    function addCircleText(obj, cssClass, lineHeight) {
                        $("<span></span>")
                            .appendTo(obj)
                            .addClass(cssClass)
                            .text(text)
                            .prepend(icon)
                            .css({
                                'line-height': lineHeight + 'px',
                                'font-size': customSettingsObj.fontsize + 'px'
                            });
                    }

                    /**
                     * adds info text to circle
                     * 
                     * @param obj
                     * @param factor
                     */
                    function addInfoText(obj, factor) {
                        $('<span></span>')
                            .appendTo(obj)
                            .addClass('circle-info-half')
                            .css(
                                'line-height', (customSettingsObj.dimension * factor) + 'px'
                            );
                    }

                    /**
                     * checks which data attributes are defined
                     * @param obj
                     */
                    function checkDataAttributes(obj) {
                        $.each(customSettings, function (index, attribute) {
                            if (obj.data(attribute) !== undefined) {
                                customSettingsObj[attribute] = obj.data(attribute);
                            } else {
                                customSettingsObj[attribute] = $(settings).attr(attribute);
                            }

                            if (attribute == 'fill' && obj.data('fill') !== undefined) {
                                fill = true;
                            }
                        });
                    }


                    function animate(current) {
                        context.clearRect(0, 0, canvas.width, canvas.height);

                        context.beginPath();
                        context.arc(x, y, radius, endAngle, startAngle, false);

                        context.lineWidth = customSettingsObj.width + 1;

                        context.strokeStyle = customSettingsObj.bgcolor;
                        context.stroke();

                        if (fill) {
                            context.fillStyle = customSettingsObj.fill;
                            context.fill();
                        }

                        context.beginPath();
                        context.arc(x, y, radius, -(quart), ((circ) * current) - quart, false);

                        if (customSettingsObj.border === 'outline') {
                            context.lineWidth = customSettingsObj.width + 13;
                        } else if (customSettingsObj.border === 'inline') {
                            context.lineWidth = customSettingsObj.width - 13;
                        }

                        context.strokeStyle = customSettingsObj.fgcolor;
                        context.stroke();

                        if (curPerc < endPercent) {
                            curPerc += curStep;
                            requestAnimationFrame(function () {
                                animate(Math.min(curPerc, endPercent) / 100);
                            }, obj);
                        }

                        if (curPerc === endPercent && fireCallback && typeof (options) !== "undefined") {
                            if ($.isFunction(options.complete)) {
                                options.complete();

                                fireCallback = false;
                            }
                        }
                    }

                    animate(curPerc / 100);

                });
            };
        }(jQuery));

        $(document).ready(function () {
            $('.pendingCh').circliful();
            $('.confirmedCh').circliful();
            $('.receivedCh').circliful();

        });
        /****************chart Ends Here****************************/

        $("#question-mark-pending").click(function () {
            $.when(popupoffers("Pending Title", "Pending Message"));
        });

        $("#question-mark-confirmed").click(function () {
            $.when(popupoffers("Confirmed Title", " Confirmed Message"));
        });

        $("#question-mark-received").click(function () {
            $.when(popupoffers("Received Title", " Received Message"));
        });

        $('input[type=radio][name=PaymentType]').change(function () {
            if (this.value === 'PayPal') {
                $("#paypalbutton").show();
                $("#btnContinuePayment").hide();
            }
            else {
                $("#paypalbutton").hide();
                $("#btnContinuePayment").show();
            }
        });

        $("#btnContinuePayment").unbind().click(function (e) {
            e.preventDefault();
            $(this).prop("disabled", true);
            if ($('input[type=radio][name=PaymentType]').prev().length > 0) {
                var paymentType = $('input[type=radio][name=PaymentType]:checked').val();
                if (paymentType === undefined || paymentType === "") {
                    $(this).prop("disabled", false);
                    $(this).toggleClass('active');
                    toastr.error("Please select Payment Option");
                }
                else if (!$("#Check").is(":checked")) {
                    $(this).prop("disabled", false);
                    $(this).toggleClass('active');
                    toastr.error("Please select agree terms and continue");
                }
                else if (paymentType.trim() === "Cashback") {
                    if (FromDetails === undefined) {
                        var _form = $(this).closest("form");
                        createAddOn(_form);
                    } else {
                        createAccount(FromDetails);
                    }
                }
            }
            else if (!$("#Check").is(":checked")) {
                $(this).prop("disabled", false);
                $(this).toggleClass('active');
                toastr.error("Please select agree terms and continue");
            }
            else if (paymentType.trim() === "Cashback") {
                //$.when(popupoPayment("Pay using your earned cashback?", "You will not get access to full membership benefits including restaurant and high street offers until your card has been paid for.  Are you sure you wish to continue with this choice?")).then(
                //    function (status) {
                //        if (status === "Yes") {
                //            createAccount(FromDetails);
                //        }
                //        else if (status === "No") {
                //            $('input[type=radio][name=PaymentType]').attr('checked', false);
                //        }
                //    }
                //);
                if (FromDetails === undefined) {
                    var _form1 = $(this).closest("form");
                    createAddOn(_form1);
                } else {
                    createAccount(FromDetails);
                }
            }
            //else {
            //    createAccount(FromDetails);
            //}
        });

        $("#submitAccountForm").unbind().submit(function () {
            $(this).prop("disabled", true);
            if (form_submit) {
                form_submit = false;
                return true;
            } else {
                if ($('input[type=radio][name=PaymentType]').prev().length > 0) {
                    var paymentType = $('input[type=radio][name=PaymentType]:checked').val();
                    if (paymentType === undefined || paymentType === "") {
                        $(this).prop("disabled", false);
                        toastr.error("Please select Payment Option");
                    } else {
                        accountValidation();
                    }
                } else {
                    accountValidation();
                }
                return false;
            }
        });

        $("#btnback").click(function (e) {
            $("#AccountFormDiv").removeClass("hidden");
            $("#ChoosePaymentScreen").addClass("hidden");
            $("#btnContinue").toggleClass('active');
            window.scrollTo(0, 0);
        });

        $("#btnPaymentback").click(function(e) {
            e.preventDefault();
            window.history.back();
        });

        $("#statementPaging #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pagenumber');
                var customer = $(this).closest("#CustomerId");
                var card = $(this).closest("#CardId");
                var status = $(this).closest("#AccountStatus");
                getAccountSummary(pageNumber, customer, card, status);

            });
        });

        $("#CardId").change(function (e) {
            e.preventDefault();
            var card = $(this).find(":selected").val();
            var customer = $("#CustomerId").val();

            if (card !== '' && customer !== '') {
                reloadAccountSummary(card, customer);
            }
        });

        $(".paylatercash").hide();

        $("#submitOrderPayment").unbind().submit(function(e) {
            $("#btnPayPalOrder").prop("disabled", true);
            if (orderFormSubmit) {
                orderFormSubmit = false;
                return true;
            } else {
                validateOrderSummary();
            }
            return false;
        });

        $("#diamond-upgrade").unbind().change(function(e) {
            e.preventDefault();
            if ($("#diamond-upgrade").is(":checked")) {
                $("#DiamondUpgrade").val(true);
            } else {
                $("#DiamondUpgrade").val(false);
            }
        });

        $("#Postcode").unbind().blur(function(e) {
            e.preventDefault();
            var postCodeRegex = new RegExp("^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$");
            var post = $(this).val().toUpperCase();
            if (post.length > 0) {
                $(this).val(post);
                if (!postCodeRegex.test(post)) {
                    $("#customError").html("Postcode is invalid.");
                    $("#customError").removeClass("hideContent");
                    if (!$("#Postcode").hasClass("--invalid")) {
                        $("#Postcode").addClass("--invalid");
                    }
                } else {
                    if ($("#Postcode").hasClass("--invalid")) {
                        $("#Postcode").removeClass("--invalid");
                    }
                    if (!$("#customError").hasClass("hideContent")) {
                        $("#customError").addClass("hideContent");
                    }
                }
            }

        });
    }


  

  
    return {
        setUpEvents: setUpEvents,
        termsHidden: termsHidden,
        planDescription: planDescription
    };
})();