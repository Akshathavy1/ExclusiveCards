var Account = (function () {
    "use strict";

    function login(form) {
        $.ajax({
            url: "/Account/Login",
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: form.serialize(),
            cache: false,
            complete: function () {
                $(".spinner").hide();
                $("#btnConfirm").prop('disabled', false);
            },
            success: function (data) {
                if (data.success) {
                    window.location.href = "/Home/Index";
                }
                else {
                    $("#Username").val("");
                    $("#Password").val("");
                    $(".spinner").hide();
                    toastr.error(data.errorMessage);
                }
            },

            error: function (xhr, status, error) {
                $("#Username").val("");
                $("#Password").val("");
                toastr.error(xhr.responseText);
            }
        });
    }

    function popupmessageLogin(title, message) {
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
                Confirm: {
                    id: 'btnYesId',
                    text: 'Confirm',
                    click: function () {
                        $("#infoMessage").html("");
                        $(this).dialog("close");
                        def.resolve("Yes");
                    },
                    "class": "btnYes"
                },
                Cancel: {
                    id: 'btnNoId',
                    text: 'Cancel',
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

    function setUpEvents() {
        $("#btnConfirm").unbind().click(function (e) {
            //Login button
            e.preventDefault();
            var _form = $(this).closest("form");
            var isValid = _form.valid();
            if (isValid) {
                $(this).prop('disabled', true);
                login(_form);
            } else {
                $(".spinner").hide();
                toastr.error("Please provide username and password.");
            }
        });

        $("#btnCancels").unbind().click(function (e) {
            //Clear fields
            e.preventDefault();
            $("#Username").val("");
            $("#Password").val("");
        });

        //Load spineer
        $('button').click(function () {
            $(this).toggleClass('navi');

        });

        //SavePassword
        $("#btnSavePassword").click(function (e) {
            var _form = $(this).closest("form");
            var isValid = _form.valid();
            var oldPassword = $("#OldPassword").val();
            var newPassword = $("#NewPassword").val();
            var cnfmPassword = $("#ConfirmPassword").val();
            if (isValid) {

                if (newPassword !== cnfmPassword) {
                    $(".spinner").hide();
                    toastr.error("New password and confirm password does not match");
                }
                else if (oldPassword === newPassword) {
                    $(".spinner").hide();
                    toastr.error("Old password and New password cannot be same");
                }
                else {

                    $.ajax({
                        url: "/Account/SavePassword",
                        beforeSend: function () {
                            $(".spinner").show();
                        },
                        type: "POST",
                        data: _form.serialize(),
                        cache: false,
                        complete: function () {
                            $(".spinner").hide();
                        },
                        success: function (data) {
                            if (data.success) {
                                toastr.success("Password Changed Successfully");
                                $("#OldPassword").val("");
                                $("#NewPassword").val("");
                                $("#ConfirmPassword").val("");
                                setTimeout(function () {
                                    window.location.href = "/Home";
                                },
                                    1500);
                            }
                            else {

                                $(".spinner").hide();
                                toastr.error(data.errorMessage);
                            }
                        },

                        error: function (xhr, status, error) {
                            toastr.error(xhr.responseText);
                        }
                    });
                }

            }
            else {
                $(".spinner").hide();
                toastr.error("Please provide all the required fields");
            }
        });

        $("#btnCancelPassword").click(function (e) {
            if ($("#OldPassword").val() !== "" ||
                $("#NewPassword").val() !== "" ||
                $("#ConfirmPassword").val() !== "") {
                $.when(popupmessageLogin("Cancel",
                    "Do you really want to discard changes?"))
                    .then(
                        function (status) {
                            if (status === "Yes") {
                                window.location.href = "/Home";
                            } else {
                                $("#OldPassword").val("");
                                $("#NewPassword").val("");
                                $("#ConfirmPassword").val("");
                            }
                        }
                    );
            } else {
                window.location.href = "/Home";
            }
        });
  }

  //generate url

  $('#btnGenerate').on('click', function (e) {
    var whiteLabelSettings = $("#ddlGenrateUrlWhiteLabel").find(":selected").val();
    var registrationCode = $("#RegistrationCode").val();

    if (whiteLabelSettings === "0") {
      toastr.error("Please select a valid whiteLabel.");
      return false;
    }

    if (registrationCode === "" || registrationCode === undefined) {
      $(".spinner").hide();
      toastr.error("registration code is required");
    }
    else {
          $.ajax({
            url: "/Account/RegisterValidation?code=" + registrationCode + "&id=" + whiteLabelSettings,
            beforeSend: function () {
              $(".spinner").show();
            },
            type: "GET",
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
              $(".spinner").hide();
              $(this).removeAttr("disabled");
            },
            success: function (data) {
              if (data !== null || data !== undefined) {
                if (data.success === false) {
                  toastr.error(data.errorMessage);
                }
                $("#txtNewUrl").val(data.data);
              }
            },

            error: function (xhr, status, error) {
              $(this).removeAttr("disabled");
              toastr.error(xhr.responseText);
            }
          });

    }

  });

  //validate url
  function isUrl(s) {
    var regexp = /(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/;
    return regexp.test(s);
  }

  //Update Registration url
  $('#btnUpdateUrl').on('click', function (e) {
    var whiteLabelSettings = $("#ddlWhiteLabelSettings").find(":selected").val();
    var registrationCode = $("#txtRegistrationCode").val();

    if (whiteLabelSettings === "0") {
      toastr.error("Please select a valid whiteLabel.");
      return false;
    } 

    if (registrationCode === "" || registrationCode === undefined) {
      var isConfirm = confirm("Are you sure you want to remove the default registration for this site?");
      if (isConfirm === false) {
        return false;
      }
    }
    
      $.ajax({
        url: "/Account/UpdateRegistrationUrl?code=" + registrationCode + "&id=" + whiteLabelSettings,
        beforeSend: function () {
          $(".spinner").show();
        },
        type: "GET",
        contentType: false,
        processData: false,
        cache: false,
        complete: function () {
          $(".spinner").hide();
          $(this).removeAttr("disabled");
        },
        success: function (data) {
          if (data !== null || data !== undefined) {
            if (data.success === true) {
              toastr.success(data.data);

            } else {
              toastr.error(data.errorMessage);

            }
          }
        },

        error: function (xhr, status, error) {
          $(this).removeAttr("disabled");
          toastr.error(xhr.responseText);
        }
      });

  });

  //
  $("#txtUrl").change(function() {
    var text = $("#txtUrl").val();
    if (text === "" || text === undefined) {
      $('#btnUpdateUrl').prop('disabled', true);
    } else {
      $('#btnUpdateUrl').prop('disabled', false);
    }
   
  });
  //

  $('#btnCopytext').on('click', function (e) {

    copytext();
  });

  //for copy the text
  function copytext() {
    /* Get the text field */
    var copyText = document.getElementById("txtNewUrl");

    /* Select the text field */
    copyText.select();
    copyText.setSelectionRange(0, 99999); /*For mobile devices*/

    /* Copy the text inside the text field */
    document.execCommand("copy");

    /* Alert the copied text */
    alert("Copied the text: " + copyText.value);
  }

  $("#ddlWhiteLabelSettings").change(function () {
    var whiteLabelId = $("#ddlWhiteLabelSettings").find(":selected").val();

    if (whiteLabelId === "0") {
      toastr.error("Please select a valid whiteLabel.");
      return false;
    }
    $("#txtRegistrationCode").val('');
    $.ajax({
      url: "/Account/GetRegistrationCode?whiteLabelId=" + whiteLabelId,
      beforeSend: function () {
        $(".spinner").show();
      },
      type: "GET",
      contentType: false,
      processData: false,
      cache: false,
      complete: function () {
        $(".spinner").hide();
        $(this).removeAttr("disabled");
      },
      success: function (data) {
        if (data !== null || data !== undefined) {
          if (data.success === true) {
            $("#txtRegistrationCode").val(data.data);
          }
        }
      },
      error: function (xhr, status, error) {
        $(this).removeAttr("disabled");
        toastr.error(xhr.responseText);
      }
    });
  });


    return {
        setUpEvents: setUpEvents
    };
}());