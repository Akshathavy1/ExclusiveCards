var Newsletter = (function () {


  
  var message;
  function errorHighlight(e, message) {
    $(".validation-summary-valid").html(message);
  }


  $("#Id").on('change', function () {
    var id = $("#Id").val();
    getEmailTemplate(id);
  });

 

  $('#btnAddNewsLetter').on('click', function(e) {
    saveNewsLetters();
  });

 

  function saveNewsLetters() {
    var id = $("#Id").val();
    var newsLetterId = $("#NewsLetterId").val();
    var subject = $("#Subject").val();
    var name = $("#Id option:selected").eq(0).text();
    var enable = $("#chkEnable").is(':checked');
    var bodyText = $("#BodyText").val();
    var bodyHtml = $("#BodyHtml").val();
    var cronScheduled = $("#txtCron").val();
    var emailTemplateId = $("#EmailTemplateId").val();
    var offerListId = $("#OfferListId").val();
    var templateTypeId = $("#TemplateTypeId").val();
    var emailName = $("#EmailName").val();
    var headerHtml = $("#HeaderHtml").val();
    var footerHtml = $("#FooterHtml").val();

    var formData = new FormData();

    $("#whitelabels option[name='whitelabel-type']").each(function (index, value) {
      
      var checkbox = $("option[name='whitelabel-type']");
//      var NewsletterCampaignLinkId = $("#NewsletterCampaignLinkId_" + index).val();
      var isChecked = checkbox[index].selected;
        formData.append("MarketingCampaigns[" + index + "].Enabled", isChecked);
        formData.append("MarketingCampaigns[" + index + "].Id", $(this).val());
     
    });
   
    
    if (name === "Select") {
       message = "Name is required";
      //errorHighlight(this, message);
      toastr.error(message);
      return false;

    }
    if (subject === "") {
       message = "Subject is required";
      //errorHighlight(this, message);
      toastr.error(message);
      return false;

    }
    //if (bodyText === "") {
    //  message = "body of Text is required";
    //  //errorHighlight(this, message);
    //  toastr.error(message);
    //  return false;

    //}
    if (bodyHtml === "") {
      message = "body of Html is required";
      //errorHighlight(this, message);
      toastr.error(message);
      return false;

    }

    
    formData.append('Id', id);
    formData.append('NewsLetterId', newsLetterId);
    formData.append('Name', name);
    formData.append('Enable', enable);
    formData.append('Subject', subject);
    formData.append('BodyText', bodyText);
    formData.append('BodyHtml', bodyHtml);
    formData.append('Schedule', cronScheduled);
    formData.append('EmailTemplateId', emailTemplateId);
    formData.append('OfferListId', offerListId);
    formData.append('TemplateTypeId', templateTypeId);
    formData.append('EmailName', emailName);
    formData.append('HeaderHtml', headerHtml);
    formData.append('FooterHtml', footerHtml);
    $.ajax({
      url: "/Newsletter/Save",
        beforeSend: function () {
          $('#btnAddNewsLetter').prop('disabled', true);
            $('#btnAddNewsLetter').prop('spinner', true);
          $(".spinner").show();
      },
      type: "POST",
      data: formData,
      contentType: false,
      processData: false,
      cache: false,
      complete: function () {
          $(".spinner").hide();
          $('#btnAddNewsLetter').prop('disabled', false);
      },
      success: function (data) {
        if (data.success) {
          toastr.success("saved successfully.");
          
          setTimeout(function () {
          //  window.history.back();
            getEmailTemplate(0);
          },
            1500);
         
        }
        else {
          toastr.error(data.errorMessage);
          $(this).removeAttr("disabled");
        }
      },

        error: function (xhr, status, error) {
            $(".spinner").hide();
            $(this).removeAttr("disabled");
            toastr.error(xhr.responseText);
      }
    });
  }
  
 

  function getEmailTemplate(id) {

    var url = "/Newsletter/GetEmailTemplate?";
    if (id !== "" && id !== null) {
      url = url +
        "letterId=" +
        id;
    }
    window.location.href = url;

  }


  // for Preview Page
  $("#ddlListOfNewsletterName").on('change', function () {
    var id = $("#ddlListOfNewsletterName").val();
    getWhiteLabels(id);
  });

  function getWhiteLabels(id) {

    var url = "/Newsletter/GetWhiteLabels?";
    if (id !== "" && id !== null) {
      url = url +
        "letterId=" +
        id;
    }
    window.location.href = url;

  }


  
  $('#btnRender').on('click', function (e) {
    
    var newsletterId = $("#ddlListOfNewsletterName").val();
    var campaignId = $("#PreviewWhiteLabels option:selected").eq(0).val();
    if (newsletterId === "") {
      message = "Newsletter is required";
      toastr.error(message);
      return false;

    }
    if (campaignId ==="0") {
      message = "Whitelabel required";
      toastr.error(message);
      return false;

    }
    renderNewsletter(campaignId);
  });

  function renderNewsletter(campaignId) {
    
    $.ajax({
      url: "/Newsletter/RenderNewsletter?campaignId=" + campaignId,
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
        if (data !== null && data !== "") {
          $("#newsLetterHtml").html(data);
          $("#btnSend").prop('disabled', false);
        } else {
          $("#btnSend").prop('disabled', true);
        }

      },

      error: function (xhr, status, error) {
        $(this).removeAttr("disabled");
        toastr.error(xhr.responseText);
      }
    });
  }


  function isEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
      return false;
    } else {
      return true;
    }
  }

  $('#btnSend').on('click', function (e) {
    var newsletterId = $("#ddlListOfNewsletterName").val();
    var testEmailRecipient = $("#TestEmailRecipient").val();
    var campaignId = $("#PreviewWhiteLabels option:selected").eq(0).val();
    var response = isEmail(testEmailRecipient);
    if (response === false) {
      toastr.error("Please check email recipient");
      return false;
    }
    if (testEmailRecipient === "") {
      message = "email recipient required";
      toastr.error(message);
      return false;

    }
    if (newsletterId === 0) {
      message = "Newsletter is required";
      toastr.error(message);
      return false;

    }
    if (campaignId === 0) {
      message = "campaignId is required";
      toastr.error(message);
      return false;

    }
    sendTestEmail(testEmailRecipient, campaignId);
  });

  function sendTestEmail(testEmailRecipient, campaignId) {
    
    $.ajax({
      url: "/Newsletter/SendTestEmail?campaignId=" + campaignId + "&recipient=" + testEmailRecipient,
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
        debugger;
        if (data === "True") {
          toastr.success("Sent successfully");
          $("#TestEmailRecipient").val('');
        } else {
          toastr.error("An error occurred. Please try again.");
        }
      },

      error: function (xhr, status, error) {
        $(this).removeAttr("disabled");
        toastr.error(xhr.responseText);
      }
    });
  }


  return {
    //setUpEvents: setUpEvents,
    //initialiseFormData: initialiseFormData,
    //kendoUIBind: kendoUIBind,
    //setdefaultValue: setdefaultValue,
    //registerSummernote: registerSummernote,
    //assignToFileData: assignToFileData,
    //loadAffiliate: loadAffiliate,
    //displayToastr: displayToastr,
    //pagingControl: pagingControl,
    //loadBasicData: loadBasicData,
    //setPagination: setPagination,
    //getProcessingStatus: getProcessingStatus
  };
})();