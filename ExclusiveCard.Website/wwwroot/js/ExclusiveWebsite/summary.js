var Summary = (function () {
    "use strict";
    var sortField = "Date";
    var sortDir = "asc";
    var viewType;
    var orderFormSubmit = false;
    var btnLabel = '';

    //New Investment via payPal
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
            data: { MembershipPlanId: membershipPlanId },
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
                }
            },
            error: function (xhr, status, error) {
                $('#btnPayPalOrder').prop("disabled", false);
            }
        });
    }

    function loadView(pageNumber) {
        var cardId = $("#MembershipCardId").val();
        var type = $("#MembershipPlanType").val();
        var url = "Account Overview";
      if (viewType === "Account Overview" || viewType === "T_Account Overview") {
            url = "Transactions/?page=" + pageNumber + 
                "&sortField=" + sortField + "&sortDirection=" + sortDir + "&membershipPlanType=" + type;
        } else if (viewType === "Withdraw Funds" || viewType === "T_Withdraw Funds") {
              url = "Withdraw/?country=" + countrySelected + "&membershipCardId=" + cardId + "&membershipPlanType=" + type;
        } else if (viewType === "Preferences" || viewType === "T_Preferences") {
              url = "Account/?membershipCardId=" + cardId;
        } else if (viewType === "Help" || viewType === "T_Help") {
              url = "Help"; 
        } else if (viewType === "TAM Dashboard" || viewType === "T_TAM Dashboard") {
            url = "Dashboard?country=" + countrySelected + "&membershipCardId=" + cardId;
        } else if (viewType === "Account" || viewType === "T_Account") {
            url = "Account/?membershipCardId=" + cardId;
        } else if (viewType === "Account Package" || viewType === "T_Account Package") {
            url = "Package/";
        } else if (viewType === "Settings" || viewType === "T_Settings") {
            url = "Settings/";
        }
        $.ajax({
            url: "/MyAccount/" + url,
            beforeSend: function () {
                //$("#loader").show();
            },
            type: "GET",
            cache: false,
            complete: function () {
                //$("#loader").hide();
            },
            success: function (data) {
                if (data !== null) {
                    $("#divTransactions").html(data);
                    if (viewType === "Account Overview") {
                        $("#bannerHeader").css("display", "block");
                        $("#divWithdrawals").css("display", "block");
                    } else {
                        $("#bannerHeader").css("display", "none");
                        $("#divWithdrawals").css("display", "none");
                    }
                    if (viewType === "Withdraw Funds") {
                        var availableFund = parseFloat($("#AvailableFund").val());
                        var requestExists = $("#RequestExists").val();
                        if (availableFund === 0) {
                            $("#btnWithdraw").attr("disabled", true);
                        }
                        if (requestExists === "True" || requestExists === "true") {
                            $("#btnWithdraw").attr("disabled", true);
                            $("#successMsg").removeClass("hideContent");
                            $("#successMsg").html("Withdrawal request is in Processing. Cannot create new request.");
                        }
                        $("#error-name").hide();
                        $("#error-number").hide();
                        $("#error-sortcode").hide();
                        $("#error-password").hide();
                        $("#error-validSortCode").hide();
                    }
                }
                else {
                    alert("Some error occurred. Please try again.");
                }
                setUpEvents();
            },

            error: function (xhr, status, error) {
                alert(xhr.responseText);
            }
        });
    }

  function requestWithdrawal() {
    var formData = new FormData();
        var customerId = $("#CustomerId").val();
        var bankDetailId = $("#BankDetailId").val();
        var partnerRewardId = $("#PartnerRewardId").val();
        var amount = $("#withdrawal_amount").val();
        var name = $("#Name").val();
        var accNumber = $("#AccountNumber").val();
        var sortCode = $("#SortCode").val();
        var password = $("#Password").val();
        var type = $("#MembershipPlanType").val();
        var availableFund = $("#AvailableFund").val();
        if (availableFund >= 5.00) {
          $("#ErrorMsg").addClass("hideContent");
        }
        else {
          if ($("#ErrorMsg").hasClass("hideContent")) {
            $("#ErrorMsg").removeClass("hideContent");
          }
          $("#ErrorMsg").html("The minimum amount that can be withdrawn is £5.00");
          return false;
        }
           
        if (amount >= 5.00) {
          //$("#ErrorMsg").addClass("hideContent");
        }
        else {
          if ($("#ErrorMsg").hasClass("hideContent")) {
            $("#ErrorMsg").removeClass("hideContent");
          }
          $("#ErrorMsg").html("The minimum amount that can be withdrawn is £5.00");
          return false;
        }



        //if (amount >= 10.00) {
        //  $("#ErrorMsg").addClass("hideContent");
        //}
        //else {
        //  if ($("#ErrorMsg").hasClass("hideContent")) {
        //    $("#ErrorMsg").removeClass("hideContent");
        //  }
        //  $("#ErrorMsg").html("The minimum amount that can be withdrawn is £10.00");
        //  return false;
        //}


        formData.append('CustomerId', customerId);
        formData.append('BankDetailId', bankDetailId);
        formData.append('PartnerRewardId', partnerRewardId);
        formData.append('WithdrawAmount', amount);
        formData.append('Name', name);
        formData.append('AccountNumber', accNumber);
        formData.append('SortCode', sortCode);
        formData.append('Password', password);
        formData.append("MembershipPlanType", type);

        $.ajax({
            url: "/MyAccount/RequestWithdrawal",
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
                    $("#successMsg").removeClass("hideContent");
                    if (!$("#ErrorMsg").hasClass("hideContent")) {
                        $("#ErrorMsg").addClass("hideContent");
                  }
                  
                  $("#divWithdrawalForm").hide();
                }
                else {
                    if (!$("#successMsg").hasClass("hideContent")) {
                        $("#successMsg").addClass("hideContent");
                    }
                    if ($("#ErrorMsg").hasClass("hideContent")) {
                        $("#ErrorMsg").removeClass("hideContent");
                    }
                    window.buttonLoading(document.querySelector("#btnWithdraw"), false, btnLabel);
                }
            },

            error: function (xhr, status, error) {
                $(".spinner").hide();
                window.buttonLoading(document.querySelector("#btnWithdraw"), false, btnLabel);
            }
        });
    }

    function requestEmail() {
        $.ajax({
            url: "/Account/ResendConfirmation",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "Post",
            data: null,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
            },
            success: function (data) {
                if (data.success) {
                    $("#confirmEmailSent").removeClass("hideContent");
                    if (!$("#confirmEmailMsg").hasClass("hideContent")) {
                        $("#confirmEmailMsg").addClass("hideContent");
                    }
                    $("#resend").css("display", "none");
                }
                else {
                    if (!$("#confirmEmailMsg").hasClass("hideContent")) {
                        $("#confirmEmailMsg").addClass("hideContent");
                    }
                    if ($("#confirmEmailSent").hasClass("hideContent")) {
                        $("#confirmEmailSent").removeClass("hideContent");
                    }
                }
            },

            error: function (xhr, status, error) {
                $(".spinner").hide();
            }
        });
  }

  function validate() {
        var result = true;
        var name = $("#Name").val();
        var accNumber = $("#AccountNumber").val();
        var sortCode = $("#SortCode").val();
        var pass = $("#Password").val();
        var regex = /^(?!(?:0{6}|00-00-00))(?:\d{6}|\d\d-\d\d-\d\d)$/;

        if (name === undefined || name === null || name.trim() === "") {
            $("#Name").addClass("--invalid");
            result = false;
        } else {
            $("#Name").removeClass("--invalid");
        }
        if (accNumber === undefined || accNumber === null || accNumber.trim() === "") {
            $("#AccountNumber").addClass("--invalid");
            result = false;
        } else {
            $("#AccountNumber").removeClass("--invalid");
        }
        if (sortCode === undefined || sortCode === null || sortCode.trim() === "") {
            $("#SortCode").addClass("--invalid");
            result = false;
        } else {
            $("#SortCode").removeClass("--invalid");
        }
        if (pass === undefined || pass === null || pass.trim() === "") {
            $("#Password").addClass("--invalid");
            result = false;
        } else {
            $("#Password").removeClass("--invalid");
        }

        var withdraw = $("#withdrawal_amount").val();
        var balance = $("#AvailableFund").val();

        if (parseFloat(withdraw) > parseFloat(balance)) {
            $("#withdrawal_amount").addClass("--invalid");
            result = false;
        } else {
            $("#withdrawal_amount").removeClass("--invalid");
        }

        if (sortCode !== null && sortCode !== undefined && sortCode.trim() === "" && sortCode.length > 0) {
            sortCode.removeClass("--invalid");
            if (sortCode.length === 6) {
                var st = chunk(name, 2).join('-');
                $("#SortCode").val(st);
            }
            if (!regex.test(sortCode)) {
                result = false;
                $("#SortCode").addClass("--invalid");
                $("#error-validSortCode").show();
            }
        }
        return result;
    }

    function chunk(str, n) {
        var ret = [];
        var i;
        var len;

        for (i = 0, len = str.length; i < len; i += n) {
            ret.push(str.substr(i, n));
        }

        return ret;
    }

  function setUpEvents() {
    //Load Spinner
        $('a, button').click(function () {
            $(this).toggleClass('active');
        });

        $("._link").unbind().click(function(e) {
            e.preventDefault();
            $.each($("._link"),
                function(i, item) {
                    $("a").removeClass("--active");
                });
            $(this).addClass("--active");
            viewType = $(this)[0].text;
            if (viewType === "Settings") {
                $.each($("._sub-link"),
                    function(i, item) {
                        if (item.innerText === "Account") {
                            $(item).addClass("--active");
                        }
                    });
            }
          if (viewType === "Account Overview" || viewType === "T_Account Overview") {
                window.location.href = "/MyAccount/MyAccount?country=" + countrySelected + "&page=" + 1 +
                    "&sortField=" + sortField + "&sortDirection=" + sortDir;
            }
            else if (viewType === "TAM Dashboard") {
                window.location.href = "/MyAccount/Dashboard?country=" + countrySelected;
            }
            else if (viewType === "Resend Confirmation Email" || viewType === "T_Resend Confirmation Email") {
                requestEmail();
            } else if (viewType === "Account Boost" || viewType === "T_Account Boost") {
                window.location.href = "/MyAccount/Deposit?country=" + countrySelected;
            } else if (viewType === "Withdraw Funds" || viewType === "T_Withdraw Funds") {
                window.location.href = "/MyAccount/Withdraw?country=" + countrySelected;
            } else if (viewType === "Account Package" || viewType === "T_Account Package") {
              window.location.href = "/MyAccount/Package?country=" + countrySelected;
            } else if (viewType === "Settings" || viewType === "T_Settings") {
              window.location.href = "/MyAccount/Settings/?country=" + countrySelected;
            } else if (viewType !== "Logout") {
                loadView(1);
            }
        });

        $("._sub-link").unbind().click(function(e) {
            e.preventDefault();
            $.each($("._sub-link"), function(i, item) {
                $("a").removeClass("--active");
            });
            $(this).addClass("--active");
            viewType = $(this).context.innerText;
            var x = $(this).parent().parent().parent()[0].firstElementChild;
            $(x).addClass("--active");

            loadView(1);
        });

        $("#divTransactions #pager a").each(function () {
            $(this).click(function (e) {
                e.preventDefault();
                sortField = $("#SortField").val();
                sortDir = $("#SortDirection").val();
                var pageNumber = $(this).attr('pagenumber');
                viewType = "Transactions";
                //loadView(pageNumber);
                window.location.href = "/MyAccount/MyAccount?country=" + countrySelected + "&page=" + pageNumber +
                    "&sortField=" + sortField + "&sortDirection=" + sortDir;
            });
        });

        $(".transactionHeading").each(function () {
            $(this).click(function (e) {
                e.preventDefault();
                sortField = this.childNodes[0].attributes["SelSortField"].value;

                if ($("#SortField").val() === sortField) {
                    if ($("#SortDirection").val() === "asc") {
                        sortDir = "desc";
                    } else {
                        sortDir = "asc";
                    }
                } else {
                    sortDir = "asc";
                }
                viewType = "Transactions";
                loadView(1);
                //window.location.href = "/Account/myAccount?country=" + countrySelected + "&page=1" +
                //    "&sortField=" + sortField + "&sortDirection=" + sortDir;
            });
        });

        $("#btnLogout").unbind().click(function (e) {
            e.preventDefault();
            $.get('/Account/Logout', "", function (data, textStatus, jqXHR) {
                if (data.success) {
                    window.location.href = "/home";
                }
                else {
                    toastr.error(data.errorMessage);
                }
            });
        });

    $("#btnWithdraw").unbind().click(function (e) {
            e.preventDefault();
            btnLabel = $("#btnWithdraw")[0].innerText;
            window.buttonLoading(document.querySelector("#btnWithdraw"), true, "Submitting Request...");
            //validate fields
            var result = validate();

            var withdrawAmount = $("#withdrawal_amount").val();
            var availableFund = $("#AvailableFund").val();
            if (withdrawAmount !== null &&
                withdrawAmount !== undefined &&
                availableFund !== null &&
                availableFund !== undefined) {
                var d1 = parseFloat(withdrawAmount).toFixed(2);
                var d2 = parseFloat(availableFund).toFixed(2);
                if (parseFloat(d1) > parseFloat(d2)) {
                    $("#withdrawal_amount").addClass("--invalid");
                    $("#ErrorMsg").removeClass("hideContent");
                    result = false;
                    window.buttonLoading(document.querySelector("#btnWithdraw"), false, btnLabel);
                }
            }
            if (withdrawAmount === undefined || withdrawAmount === null || parseFloat(withdrawAmount) === 0) {
                $("#withdrawal_amount").addClass("--invalid");
                $("#ErrorMsg").removeClass("hideContent");
                result = false;
                window.buttonLoading(document.querySelector("#btnWithdraw"), false, btnLabel);
            }

            if (result === true) {
                requestWithdrawal();
                window.buttonLoading(document.querySelector("#btnWithdraw"), false, btnLabel);
            } else {
                window.buttonLoading(document.querySelector("#btnWithdraw"), false, btnLabel);
            }
            window.scrollTo(0, 100);
        });

        $("#Name").unbind().change(function(e) {
            e.preventDefault();
            var name = $(this).val();
            if (name.length > 0) {
                $(this).removeClass("border-red");
                $("#error-name").hide();
            } else {
                $(this).addClass("border-red");
                $("#error-name").show();
            }
        });

        $("#AccountNumber").unbind().change(function (e) {
            e.preventDefault();
            var name = $(this).val();
            if (name.length > 0) {
                $(this).removeClass("border-red");
                $("#error-number").hide();
            } else {
                $(this).addClass("border-red");
                $("#error-number").show();
            }
        });

        $("#SortCode").change(function (e) {
            e.preventDefault();
            var regex = /^(?!(?:0{6}|00-00-00))(?:\d{6}|\d\d-\d\d-\d\d)$/;
            var name = $(this).val();
            if (name.length > 0) {
                $(this).removeClass("--invalid");
                $("#ErrorMsg").addClass("hideContent");
                if (name.length === 6) {
                    var st = chunk(name, 2).join('-');
                    $(this).val(st);
                }
                if (!regex.test(name)) {
                    $(this).addClass("--invalid");
                    $("#ErrorMsg").removeClass("hideContent");
                }
            } else {
                $(this).addClass("--invalid");
                $("#ErrorMsg").removeClass("hideContent");
            }
        });

        $("#Password").unbind().change(function (e) {
            e.preventDefault();
            var name = $(this).val();
            if (name.length > 0) {
                $(this).removeClass("border-red");
                $("#error-password").hide();
            } else {
                $(this).addClass("border-red");
                $("#error-password").show();
            }
        });

        $("#withdrawal_amount").change(function(e) {
            e.preventDefault();
            var withdraw = $(this).val();
            var balance = $("#AvailableFund").val();

            if (parseFloat(withdraw) > parseFloat(balance)) {
                $("#withdrawal_amount").addClass("-invalid");
                $("#ErrorMsg").removeClass("hideContent");
            } else if(withdraw !== null && withdraw !== '') {
                if ($("#withdrawal_amount").hasClass("--invalid")) {
                    $("#withdrawal_amount").removeClass("--invalid");
                }
                if (!$("#ErrorMsg").hasClass("hideContent")) {
                    $("#ErrorMsg").addClass("hideContent");
                }
            }
        });

    $("#personal").unbind().click(function (e) {
     
            e.preventDefault();
            btnLabel = $("#personal")[0].innerText;
            window.buttonLoading(document.querySelector("#personal"), true, "Submitting form...");
            //$(this).prop("disabled", true);
            var formData = new FormData();

            var formValid = true;
            var niNumber = $("#NationalInsuranceNumber").val();
            var number = "";
            if (niNumber !== null && niNumber !== undefined && niNumber !== "") {
                number = niNumber.replace(/ /g, "");
                var valid = new RegExp('^[A-CEGHJ-PR-TW-Z]{1}[A-CEGHJ-NPR-TW-Z]{1}[0-9]{6}[A-D]{0,1}$');

                if (!valid.test(number)) {
                    $("#personalError").html("National insurance number is invalid.");
                    $("#NationalInsuranceNumber").focus();
                    $("#personalError").removeClass("hideContent");
                    formValid = false;
                    //$("#personal").prop("disabled", false);
                    window.buttonLoading(document.querySelector("#personal"), false, btnLabel);
                }
            }

            var postcode = $("#postcode").val();
            var postCodeRegex = new RegExp("^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$");
            if (postcode === null || postcode === undefined || postcode.trim() === "") {
                formValid = false;
                if (!$("#postcode").hasClass("--invalid")) {
                    $("#postcode").addClass("--invalid");
                }
            } else {
                $("#postcode").removeClass("--invalid");
            }

            if (!postCodeRegex.test(postcode)) {
                formValid = false;
                if (!$("#postcode").hasClass("--invalid")) {
                    $("#postcode").addClass("--invalid");
                }
          }

     
              var marketingNewsLetter =false;
              if ($("#marketing").is(':checked')==true) {
                marketingNewsLetter = true;
              }


     if (formValid) {
                formData.append('CustomerId', $("#CustomerId").val());
                formData.append('ContactDetailId', $("#ContactDetailId").val());
                formData.append('Forename', $("#full_name").val());
                formData.append('Surname', $("#sur_name").val());
                formData.append('Email', $("#email").val());
                formData.append('DateOfBirth', $("#DateOfBirth").val());
                formData.append('Postcode', $("#postcode").val());
                formData.append('NationalInsuranceNumber', number);
                formData.append('Address1', $("#Address1").val());
                formData.append('Address2', $("#Address2").val());
                formData.append('Address3', $("#Address3").val());
                formData.append('District', $("#District").val());
                formData.append('Town', $("#Town").val());
                formData.append('MarketingNewsLetter', marketingNewsLetter);

                $.ajax({
                    url: "/MyAccount/Update",
                    beforeSend: function() {
                        $(".spinner").show();
                    },
                    type: "Post",
                    data: formData,
                    contentType: false,
                    processData: false,
                    cache: false,
                    complete: function() {
                        $(".spinner").hide();
                    },
                    success: function(data) {
                        if (data.success) {
                            $("#personalSuccess").html("Personal details updated successfully.");
                            $("#personalSuccess").focus();
                            if ($("#personalSuccess").hasClass("hideContent")) {
                                $("#personalSuccess").removeClass("hideContent");
                            }
                            if (!$("#personalError").hasClass("hideContent")) {
                                $("#personalError").addClass("hideContent");
                            }
                            //$("#personal").prop("disabled", false);
                            window.buttonLoading(document.querySelector("#personal"), false, btnLabel);
                            if (number !== null && number !== undefined && number !== "") {
                                $("#niNNumber").addClass("--met");
                            }
                        } else {
                            $("#personalError").html(data.errorMessage);
                            $("#personalError").focus();
                            if ($("#personalError").hasClass("hideContent")) {
                                $("#personalError").removeClass("hideContent");
                            }
                            //$("#personal").prop("disabled", false);
                            window.buttonLoading(document.querySelector("#personal"), false, btnLabel);
                        }
                        window.scrollTo(0, 100);
                    },

                    error: function(xhr, status, error) {
                        $("#personalError").html(data.errorMessage);
                        $("#personalError").focus();
                        if ($("#personalError").hasClass("hideContent")) {
                            $("#personalError").removeClass("hideContent");
                        }
                        $("#personal").prop("disabled", false);
                        window.scrollTo(0, 100);
                    }
                });
            } else {
                window.buttonLoading(document.querySelector("#personal"), false, btnLabel);
            }
        });

        $("#password").unbind().click(function (e) {
            e.preventDefault();
            //$(this).prop("disabled", true);
            btnLabel = $("#password")[0].innerText;
            window.buttonLoading(document.querySelector("#password"), true, "Submitting form...");
            var formData = new FormData();
            var formValid = true;
            var error = "";
            var currentPass = $("#CurrentPassword").val();
            var newPass = $("#NewPassword").val();
            var confirmPass = $("#ConfirmPassword").val();

            if (currentPass === null || currentPass === undefined || currentPass === "") {
                error = "Current password is required.";
                formValid = false;
            }
            if (newPass === null || newPass === undefined || newPass === "") {
                if (error.length > 0) {
                    error += "<br />New password is required";
                } else {
                    error = "New password is required.";
                }
                formValid = false;
            }
            if (confirmPass === null || confirmPass === undefined || confirmPass === "") {
                if (error.length > 0) {
                    error += "<br />Confirm password is required";
                } else {
                    error = "Confirm password is required.";
                }
                formValid = false;
            }
            var passwordRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&^#])[A-Za-z\\d@$!%*?&^#]{8,48}$");
            if (!passwordRegex.test(newPass)) {
                formValid = false;
                if (!$("#ConfirmPassword").hasClass("--invalid")) {
                    $("#ConfirmPassword").addClass("--invalid");
                }
                if (!$("#Password").hasClass("--invalid")) {
                    $("#Password").addClass("--invalid");
                }
            }


            if (formValid) {
                formData.append('CurrentPassword', currentPass);
                formData.append('NewPassword', newPass);
                formData.append('ConfirmPassword', confirmPass);

                $.ajax({
                    url: "/MyAccount/UpdatePassword",
                    beforeSend: function() {
                        $(".spinner").show();
                    },
                    type: "Post",
                    data: formData,
                    contentType: false,
                    processData: false,
                    cache: false,
                    complete: function() {
                        $(".spinner").hide();
                    },
                    success: function(data) {
                        if (data.success) {
                            $("#passwordSuccess").html("Password changed successfully.");
                            $("#passwordSuccess").focus();
                            if ($("#passwordSuccess").hasClass("hideContent")) {
                                $("#passwordSuccess").removeClass("hideContent");
                            }
                            if (!$("#passwordError").hasClass("hideContent")) {
                                $("#passwordError").addClass("hideContent");
                            }
                        } else {
                            $("#passwordError").html(data.errorMessage);
                            if ($("#passwordError").hasClass("hideContent")) {
                                $("#passwordError").removeClass("hideContent");
                            }
                        }
                        window.buttonLoading(document.querySelector("#password"), false, btnLabel);
                    },

                    error: function(xhr, status, error) {
                        $("#passwordError").html(data.errorMessage);
                        $("#passwordError").focus();
                        if ($("#passwordError").hasClass("hideContent")) {
                            $("#passwordError").removeClass("hideContent");
                        }
                        //$("#password").prop("disabled", false);
                        window.buttonLoading(document.querySelector("#password"), false, btnLabel);
                    }
                });
            } else {
                $("#passwordError").html(error);
                $("#passwordError").focus();
                if ($("#passwordError").hasClass("hideContent")) {
                    $("#passwordError").removeClass("hideContent");
                }
                //$("#password").prop("disabled", false);
                window.buttonLoading(document.querySelector("#password"), false, btnLabel);
            }
        });


    //
    $("#security").unbind().click(function (e) {
      e.preventDefault();
      btnLabel = $("#security")[0].innerText;
      window.buttonLoading(document.querySelector("#security"), true, "Submitting form...");
      var formData = new FormData();
      var formValid = true;
      var error = "";
      var questionId = $("#QuestionId").val();
      var answer = $("#Answer").val();      

      if (questionId === null || questionId === undefined || questionId === "") {
        error = "Question is required.";
        formValid = false;
      }
      if (answer === null || answer === undefined || answer === "") {
        error = "Answer is required.";
        formValid = false;
      }
      

      if (formValid) {
        formData.append('CustomerId', $("#CustomerId").val());
        formData.append('QuestionId', questionId);
        formData.append('Answer', answer);

        $.ajax({
          url: "/MyAccount/UpdateSecurity",
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
              $("#securitySuccess").html("security details changed successfully.");
              $("#securitySuccess").focus();
              if ($("#securitySuccess").hasClass("hideContent")) {
                $("#securitySuccess").removeClass("hideContent");
              }
              if (!$("#securityError").hasClass("hideContent")) {
                $("#securityError").addClass("hideContent");
              }
            } else {
              $("#securityError").html(data.errorMessage);
              if ($("#securityError").hasClass("hideContent")) {
                $("#securityError").removeClass("hideContent");
              }
            }
            window.buttonLoading(document.querySelector("#security"), false, btnLabel);
            setTimeout(function () { window.location.reload(); }, 2000);
            
          },

          error: function (xhr, status, error) {
            $("#securityError").html(data.errorMessage);
            $("#securityError").focus();
            if ($("#securityError").hasClass("hideContent")) {
              $("#securityError").removeClass("hideContent");
            }
            window.buttonLoading(document.querySelector("#security"), false, btnLabel);
          }
        });
      } else {
        $("#securityError").html(error);
        $("#securityError").focus();
        if ($("#securityError").hasClass("hideContent")) {
          $("#securityError").removeClass("hideContent");
        }
        window.buttonLoading(document.querySelector("#security"), false, btnLabel);
      }
    });
    //


        $("#NewPassword, #ConfirmPassword").keyup(checkPasswordMatch);

        $(".niNumber").unbind().click(function(e) {
            e.preventDefault();
            $.each($("._link"),
                function (i, item) {
                    if (item.innerHTML === "Settings") {
                        $(this).addClass("--active");
                    } else {
                        $(this).removeClass("--active");
                    }
                });
            viewType = "Settings";
            loadView(1);
        });

        $("#submitOrderPayment").unbind().submit(function (e) {
            $("#btnPayPalOrder").prop("disabled", true);
            if (orderFormSubmit) {
                orderFormSubmit = false;
                return true;
            } else {
                validateOrderSummary();
            }
            return false;
        });

        $("#postcode").unbind().blur(function (e) {
            e.preventDefault();
            var postCodeRegex = new RegExp("^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$");
            var post = $(this).val().toUpperCase();
            if (post.length > 0) {
                $(this).val(post);
                if (!postCodeRegex.test(post)) {
                    //$("#personalError").html("Postcode is invalid.");
                    $("#personalError").removeClass("hideContent");
                    if (!$("#postcode").hasClass("--invalid")) {
                        $("#postcode").addClass("--invalid");
                    }
                } else {
                    if ($("#postcode").hasClass("--invalid")) {
                        $("#postcode").removeClass("--invalid");
                    }
                    if (!$("#personalError").hasClass("hideContent")) {
                        $("#personalError").addClass("hideContent");
                    }
                }
            }

        });

        $("#refresh").unbind().click(function(e) {
            e.preventDefault();
            window.location.reload();
        });

        $("#resendEmail").unbind().click(function(e) {
            e.preventDefault();
            requestEmail();
        });
    }

    function checkPasswordMatch() {
        var password = $("#NewPassword").val();
        var confirmPassword = $("#ConfirmPassword").val();

        var passwordRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&^#])[A-Za-z\\d@$!%*?&^#]{8,48}$");

        if ($("#ConfirmPassword").is(":focus") && !passwordRegex.test(password)) {
            $("#passwordError").html("The password entered does not follow the rules. Please enter a password as described below and then try again.");
            $("#NewPassword").addClass("--invalid");
            if ($("#passwordError").hasClass("hideContent")) {
                $("#passwordError").removeClass("hideContent");
            }
        } else if (confirmPassword !== null && confirmPassword !== undefined && confirmPassword.trim() !== "" && password !== confirmPassword) {
            $("#passwordError").html("The passwords entered do not match. Please enter the confirmation password again.");
            $("#NewPassword").removeClass("--invalid");
            $("#ConfirmPassword").addClass("--invalid");
            if ($("#passwordError").hasClass("hideContent")) {
                $("#passwordError").removeClass("hideContent");
            }
        } else if (password === null || password === undefined || password.trim() === "") {
            $("#passwordError").html("The password entered does not follow the rules. Please enter a password as described below and then try again.");
            $("#NewPassword").addClass("--invalid");
            if ($("#passwordError").hasClass("hideContent")) {
                $("#passwordError").removeClass("hideContent");
            }
        } else {
            $("#NewPassword").removeClass("--invalid");
            $("#ConfirmPassword").removeClass("--invalid");

            if (!$("#passwordError").hasClass("hideContent")) {
                $("#passwordError").addClass("hideContent");
            }
        }

        //if (password != confirmPassword) {
        //    $("#passwordError").html("Passwords do not match!");
        //    if ($("#passwordError").hasClass("hideContent")) {
        //        $("#passwordError").removeClass("hideContent");
        //    }
        //} else {
        //    if (!$("#passwordError").hasClass("hideContent")) {
        //        $("#passwordError").addClass("hideContent");
        //    }
        //}
    }

  function loadPartialView() {
    $.each($("._link"),
            function (i, item) {
                $("a").removeClass("--active");
            });
        $("a._link:contains('Account Package')").addClass("--active");
        viewType = "Account Package";
        loadView(1);
    }

    function paymentSuccess(paymentId) {
        $.ajax({
            url: "/MyAccount/Investment",
            beforeSend: function () {
                //$("#waitpaypal").show();
                //$("#loader").show();
            },
            type: "POST",
            data: { customerPaymentId: paymentId },
            cache: false,
            complete: function () {
                //$("#loader").hide();
                //$("#waitpaypal").hide();
            },
            success: function (data) {
                console.log(data);
            },
            error: function (xhr, status, error) {
                console.log(xhr + error);
            }
        });
    }

    return {
        setUpEvents: setUpEvents,
        loadPartialView: loadPartialView,
        paymentSuccess: paymentSuccess
    };
})();