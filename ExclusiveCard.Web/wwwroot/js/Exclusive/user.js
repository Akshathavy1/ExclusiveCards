var User = (function () {
    "use strict";
    var formData;

  function searchUsers(username, email) {
    $.ajax({
      url: "/User/GetUsers/?username=" + username + "&email=" + email,
      beforeSend: function () {
        $(".spinner").show();
      },
      type: "GET",
      cache: false,
      complete: function () {
        $(".spinner").hide();
      },
      success: function (data) {
          $('#userList').html(data);
      },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

  function create(form) {
    $.ajax({
      url: "/user/SaveUser",
      beforeSend: function () {
        $(".spinner").show();
      },
      type: "POST",
      data: form.serialize(),
      cache: false,
      dataType: "json",
      complete: function () {
        $(".spinner").hide();
        $(this).removeAttr("disabled");
      },
      success: function (data) {
        if (data.success) {
            setTimeout(
                function () {
                    window.location.href = "/User/";
                }, 5000);
        } else {
            $("#btnCreate").prop('disabled', false);
            toastr.error(data.errorMessage);
        }
      },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

  function disableUser(form) {
    $.ajax({
      url: "/user/DisableUser",
      beforeSend: function () {
        $(".spinner").show();
      },
      type: "POST",
      data: form.serialize(),
      cache: false,
      dataType: "json",
      complete: function () {
        $(".spinner").hide();
      },
      success: function (data) {
        if (data.success) {
          window.location.href = "/User/";
        } else {
          toastr.error("Could not disable user. Please try again.");
        }
      },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function setUpEvents() {
        formData = $("#btnCreate").closest("form").serialize();

        //Load Spinner
        $('a, button').click(function () {
            $(this).toggleClass('active');
        });

        $("#btnSearch").click(function (e) {
            e.preventDefault();
            var username = $("#Username").val();
            var email = $("#Email").val();
            searchUsers(username, email);
        });

        $("#btnCreate").click(function (e) {
            e.preventDefault();
            var _form = $(this).closest("form");
            var isvalid = _form.valid();
            if (isvalid) {
                $(this).prop('disabled', true);
                create(_form);
            }
        });

        $("#btnDisable").click(function (e) {
            e.preventDefault();
            var _form = $(this).closest("form");
            $(this).prop('disabled', true);
            disableUser(_form);
        });

        $("#btnCancels").click(function (e) {
            e.preventDefault();
            var _form = $(this).closest("form");

      if (formData !== _form.serialize()) {
        $("#userPop").dialog("open");
      } else {
        window.location.href = "/user";
      }
    });

    $("#userPop").dialog({
      autoOpen: false,
      draggable: false,
      autoResize: true,
      closeOnEscape: false,
      open: function (event, ui) {
        $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
      },
      position: {
        my: "center",
        at: "center",
        of: $("body"),
        within: $("body")
      },
      resizable: false,
      height: "auto",
      width: 400,
      modal: true,
      buttons: {
        "Confirm": function () {
          window.location.href = "/User";
          $(this).dialog("close");
        },
        "Cancel": function () {
          $(this).dialog("close");
        }
      }
    });
  }

    return {
        setUpEvents: setUpEvents
    };
}());