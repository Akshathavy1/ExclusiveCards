var Login = (function () {
  "use strict";
  var btnLabel = '';

  //Login Validation
  function validateLogin(formdata, isRedirect) {
    var data = "";
    btnLabel = $("#btnLogin")[0].innerText;
    window.buttonLoading(document.querySelector("#btnLogin"), true, "Logging in...");
    if (formdata !== null) {
      data = formdata.serialize();
    }
    $.ajax({
      url: "/Account/Login",
      beforeSend: function () {
      },
      type: "POST",
      data: data,
      cache: false,
      complete: function () {
      },
      success: function (data) {
        if (data.success) {
          if (isRedirect) {
            window.location.href = "/OfferHub/";
          } else {
            window.location.reload();
          }
        }
        else {
          $('#btnLogin').prop("disabled", false);
          window.buttonLoading(document.querySelector("#btnLogin"), false, btnLabel);
          $('#loginError').text(data.errorMessage);
          $('#loginError').show();
        }
      },
      error: function (xhr, status, error) {
        $('#btnLogin').prop("disabled", false);
        window.buttonLoading(document.querySelector("#btnLogin"), false, btnLabel);
        $('#loginError').text(xhr.responseText);
        $('#loginError').show();
      }
    });
  }

  //Account Register
  function validateRegistrationCode(code) {
    btnLabel = $("#btnAccountSetup")[0].innerText;
    window.buttonLoading(document.querySelector("#btnAccountSetup"), true, "Loading...");
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
          } else if (data.data.slug === "sport-rewards") {
            window.location.href = "/SportRewards/Index";
          }
          else {
            //window.location.href = "/Account/CreateAccount/?country=" +
            //  countrySelected +
            //  "&membershipPlanId=" +
            //  data.data.membershipPlanId +
            //  "&token=" +
            //  data.data.token;
            window.location.href = "/Account/CreateAccount";
          }
        }
        window.buttonLoading(document.querySelector("#btnAccountSetup"), false, btnLabel);
      }
      else {
        $('#AccountErrorMessage').text(data.errorMessage);
        $('#AccountErrorMessage').show();
        $('#btnAccountSetup').prop("disabled", false);
        window.buttonLoading(document.querySelector("#btnAccountSetup"), false, btnLabel);
      }
    });
  }

  function setUpEvents() {
    $(document).keypress(function (e) {
      if (e.which == 13 || event.keyCode == 13) {
        e.preventDefault();
        $("#btnLogin").click();
      }
    });

    $("#btnLogin").unbind().click(function (e) {
      e.preventDefault();
      $(this).prop("disabled", true);
      $('#loginError').hide();
      var isRedirect = false;
      if ($(this).attr("offerHubRedirect") === null || $(this).attr("offerHubRedirect") === undefined) {
        isRedirect = false;
      } else {
        isRedirect = $(this).attr("offerHubRedirect") === "true";
      }
      var _form = $(this).closest("form");
      _form.validate({
        rules: {
          Username: {
            required: true,
            email: true
          },
          LoginPassword: "required"
        },
        // Specify validation error messages
        messages: {
          Username: "",
          LoginPassword: ""
        },
        submitHandler: function (_form) {
          _form.submit();
        }
      });
      var isValid = _form.valid();
      if (isValid) {
        validateLogin(_form, isRedirect);
      } else {
        $(this).prop("disabled", false);
      }
    });

    $("#ForgotPassword").unbind().click(function (e) {
      e.preventDefault();
      $(this).prop("disabled", true);
      $('#errorNoticeMessage').hide();
      $('#successNoticeMessage').hide();
      var _form = $(this).closest("form");
      _form.validate({
        rules: {
          Username: {
            required: true
            // email: true
          }
        },
        // Specify validation error messages
        messages: {
          Username: ""
        },
        submitHandler: function (_form) {
          _form.submit();
        }
      });
      var isValid = _form.valid();
      if (isValid) {
        $.ajax({
          url: "/Account/ResetPasswordEmail?username=" + $("#Username").val().trim(),
          beforeSend: function () {
          },
          type: "POST",
          data: null,
          cache: false,
          complete: function () {
          },
          success: function (data) {
            $('#ForgotPassword').prop("disabled", false);
            if (data.success) {
              $('#successNoticeMessage').text(data.data);
              $('#successNoticeMessage').show();
            }
            else {
              $('#errorNoticeMessage').text(data.errorMessage);
              $('#errorNoticeMessage').show();
            }
          },
          error: function (xhr, status, error) {
            $('#errorNoticeMessage').text(xhr.responseText);
            $('#errorNoticeMessage').show();
            $('#ForgotPassword').prop("disabled", false);
          }
        });
      }
      else {
        $(this).prop("disabled", false);
      }
    });

    $("#btnAccountSetup").unbind().click(function (e) {
      e.preventDefault();
      $(this).prop("disabled", true);
      $("#AccountErrorMessage").hide();
      var _form = $(this).closest("form");
      _form.validate({
        rules: {
          registerCode: {
            required: true
          }
        },
        // Specify validation error messages
        messages: {
          registerCode: ""
        },
        submitHandler: function (_form) {
          _form.submit();
        }
      });
      var isValid = _form.valid();
      var registrationUrl = $("#RegistrationUrl").val();
      if (isValid) {
        validateRegistrationCode($("#registerCode").val().trim());
      }
      else if (!isValid && registrationUrl != null)
      {
          location.href = registrationUrl
      }
      else {
        $(this).prop("disabled", false);
      }
    });

    $("#SubmitPassword").unbind().click(function (e) {
      e.preventDefault();
      $(this).prop("disabled", true);
      if (!$("#ErrorMsg").hasClass("hideContent")) {
        $("#ErrorMsg").addClass("hideContent");
      }

      var username = $("#Username").val();
      var password = $("#NewPassword").val();
      var confirm = $("#ConfirmPassword").val();
      var token = $("#Token").val();
      var result = true;
      var passwordRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&^#])[A-Za-z\\d@$!%*?&^#]{8,48}$");

      if (password === null || password === undefined || password.trim() === "") {
        result = false;
        $("#ErrorMsg").val("The password entered does not follow the rules. Please enter a password as described below and then try again.");
        $("#NewPassword").addClass("--invalid");
      }

      if (confirm === null || confirm === undefined || confirm.trim() === "") {
        result = false;
        $("#ErrorMsg").val("The passwords entered do not match. Please enter the confirmation password again.");
        $("#ConfirmPassword").addClass("--invalid");
      }

      if (password !== confirm) {
        result = false;
      }

      if (!passwordRegex.test(password)) {
        result = false;
        $("#ErrorMsg").val("The password entered does not follow the rules. Please enter a password as described below and then try again.");
        if (!$("#ConfirmPassword").hasClass("--invalid")) {
          $("#ConfirmPassword").addClass("--invalid");
        }
        if (!$("#Password").hasClass("--invalid")) {
          $("#Password").addClass("--invalid");
        }
      }

      if (!result) {
        $("#ErrorMsg").removeClass("hideContent");
        $("#SubmitPassword").prop("disabled", false);
      } else {
        $.ajax({
          url: "/Account/SaveNewPassword?username=" + username + "&token=" + token + "&password=" + password,
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
              if ($("#successMsg").hasClass("hideContent")) {
                $("#successMsg").removeClass("hideContent");
              }
              if (!$("#ErrorMsg").hasClass("hideContent")) {
                $("#ErrorMsg").addClass("hideContent");
              }
              setTimeout(function () {
                window.location.href = "/Home";
              },
                1500);
            } else {
              $("#ErrorMsg").html("An error occurred attempting to reset your password. Please try again.");
              if ($("#ErrorMsg").hasClass("hideContent")) {
                $("#ErrorMsg").removeClass("hideContent");
              }
              $("#SubmitPassword").prop("disabled", false);
            }
          },

          error: function (xhr, status, error) {
            $("#ErrorMsg").html("An error occurred attempting to reset your password. Please try again.");
            if ($("#ErrorMsg").hasClass("hideContent")) {
              $("#ErrorMsg").removeClass("hideContent");
            }
            $("#SubmitPassword").prop("disabled", false);
          }
        });
      }
    });

    $("#NewPassword, #ConfirmPassword").keyup(checkPasswordMatch);
  }

  function checkPasswordMatch() {
    var password = $("#NewPassword").val();
    var confirmPassword = $("#ConfirmPassword").val();

    var passwordRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&^#])[A-Za-z\\d@$!%*?&^#]{8,48}$");

    if ($("#ConfirmPassword").is(":focus") && !passwordRegex.test(password)) {
      $("#ErrorMsg").html("The password entered does not follow the rules. Please enter a password as described below and then try again.");
      $("#NewPassword").addClass("--invalid");
      if ($("#ErrorMsg").hasClass("hideContent")) {
        $("#ErrorMsg").removeClass("hideContent");
      }
    } else if (confirmPassword !== null && confirmPassword !== undefined && confirmPassword.trim() !== "" && password !== confirmPassword) {
      $("#ErrorMsg").html("The passwords entered do not match. Please enter the confirmation password again.");
      $("#NewPassword").removeClass("--invalid");
      $("#ConfirmPassword").addClass("--invalid");
      if ($("#ErrorMsg").hasClass("hideContent")) {
        $("#ErrorMsg").removeClass("hideContent");
      }
    } else if (password === null || password === undefined || password.trim() === "") {
      $("#ErrorMsg").html("The password entered does not follow the rules. Please enter a password as described below and then try again.");
      $("#NewPassword").addClass("--invalid");
      if ($("#ErrorMsg").hasClass("hideContent")) {
        $("#ErrorMsg").removeClass("hideContent");
      }
    } else {
      $("#NewPassword").removeClass("--invalid");
      $("#ConfirmPassword").removeClass("--invalid");

      if (!$("#ErrorMsg").hasClass("hideContent")) {
        $("#ErrorMsg").addClass("hideContent");
      }
    }
  }

  return {
    setUpEvents: setUpEvents
  };
})();