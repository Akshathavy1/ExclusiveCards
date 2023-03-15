var SportRewards = (function () {


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

    //$('#btnStandard').unbind().click(function () {
    //  validateTalkSportRegistrationCode(false, 0);
    //});

    

    //$('#btnClub').unbind().click(function () {

    //  var siteClanId = $('#Id').val();
    //  if (siteClanId === undefined) {

    //    $("#customError").html("Please select a club or press the skip this step button.");
    //    $("#customError").removeClass("hideContent");
    //    window.scrollTo(0, 500);
    //    return false;
    //  }
    //  validateTalkSportRegistrationCode(true, siteClanId);
    //});


  }



  function createAccount() {


    //var membershipPlanId = $('#membershipPlanId').val();
    //var token = $('#token').val();
    //var siteClanId = $('#siteClanId').val();

    //var formData = new FormData();
    //formData.append('MembershipPlanId', membershipPlanId);
    //formData.append('Token', token);
    //formData.append('SiteClanId', siteClanId);

   
      //$.ajax({
      //  url: "/Account/CreateAccount",
      //  type: "POST",
      //  data: formData,
      //  contentType: false,
      //  processData: false,
      //  cache: false,
      //  complete: function () {
      //    $(".spinner").hide();
      //  },
      //  //success: function (data) {
      //  //  if (data) {
      //  //    alert();
      //  //  }
          
      //  //},
      //  error: function (xhr, status, error) {
      //    console.log(xhr.responseText);
      //  }
      //});
    
   
  }

  function selectClub(description, clanId) {
    var formData = new FormData();
    formData.append("Description", description);
    formData.append("Id", clanId);
    $.ajax({
      url: "/SportRewards/GetClub",
      type: "POST",
      data: formData,
      contentType: false,
      processData: false,
      cache: false,
      success: function(data) {
        if (data.success) {
          var url = "/SportRewards/Club";
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