var SiteClan = (function () {


  function setUpEvents() {

    $(".clan").click(function () {
      var description = $(this).attr("description");
      var clanId = $(this).attr("siteClanId");
      selectClub(description, clanId);
    });

    var league = $('#HdfDescription').val();
    if (league === 'Premier League') {
      $("#divCharity").show();
      $("#divClub").hide();
    } else {
      $("#divCharity").hide();
      $("#divClub").show();
    }


    $('#btnStandard').unbind().click(function () {
      window.location.href = "/Account/CreateAccount";

    });


    $('#btnClub').unbind().click(function () {
      window.location.href = "/Account/CreateAccount";

    });

  } 

  function selectClub(description, clanId) {
    var formData = new FormData();
    formData.append("Description", description);
    formData.append("Id", clanId);
    $.ajax({
      url: "/SiteClan/GetClub",
      type: "POST",
      data: formData,
      contentType: false,
      processData: false,
      cache: false,
      success: function(data) {
        if (data.success) {
          var url = "/SiteClan/Confirmation";
          window.location = url;
        } else {
          //alert(data.errorMessage);
        }
      },
      error: function(xhr, status, error) {
        //alert(error);
        $(this).removeAttr("disabled");
        toastr.error(xhr.responseText);
      }
    });
  }
  //

 

  return {
    setUpEvents: setUpEvents

  };
})();