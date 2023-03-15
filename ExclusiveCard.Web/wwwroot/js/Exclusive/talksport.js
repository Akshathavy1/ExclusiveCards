var talkSport = (function () {
  "use strict";
 
  $('#btnSearchOffer').on('click', function (e) {
    
    var leagueId = $("#LeagueId").val();
    var siteCategoryId = $("#SiteCategoryId option:selected").eq(0).val();

  
    if (leagueId === "") {
      toastr.error("League is required");
      return false;
    }
    if (siteCategoryId === "") {
      toastr.error("SiteCategory is required");
      return false;
    }
    var pageNumber = 1;
    var url = "/TalkSport/SearchClub?leagueId=" + leagueId + "&siteCategoryId=" + siteCategoryId + "&pageNumber=" + pageNumber;
    window.location.href = url;

  });

  //Edit event handler.
  $("body").on("click", "#clubList #btnEdit", function() {
    
    var row = $(this).closest("tr");
    var tbody = $(this).closest("tbody");

    var btnText = row.find("#btnEdit").text();
    if (btnText.trim() === "Edit") {
     
      $("tr", tbody).each(function () {
        if ($(this).find("input").length === 0) {
          $(this).find("input").show();
          $(this).find("span").hide();
          $(this).find("#ddlLeagueId").prop("disabled", true);
          $(this).find("#ddlCharityId").prop("disabled", true);
          $(this).find("#btnEdit").text("Edit");

        }
      });

      $("td", row).each(function () {
        if ($(this).find("input").length === 0) {
          $(this).find("input").show();
          $(this).find("span").hide();
        }
      });

      row.find("#ddlLeagueId").removeAttr("disabled");
      row.find("#ddlCharityId").removeAttr("disabled");
      row.find("#btnEdit").text("Update");
    }
    else if (btnText.trim() === "Update") {
      
      var league = row.find("#ddlLeagueId option:selected").val();
      var charity = row.find("#ddlCharityId option:selected").val();
      var siteClanId = $(this).val();
      if (league === "") {
        toastr.error("League is required");
        return false;
      }
      if (charity === "") {
        toastr.error("Charity is required");
        return false;
      }
      
      updateSiteClan(siteClanId, league, charity);
      row.find("#ddlLeagueId").prop("disabled", true);
      row.find("#ddlCharityId").prop("disabled", true);
      row.find("#btnEdit").text("Edit");
    }


  });

  function updateSiteClan(id, league, charity) {

    $.ajax({
      url: "/TalkSport/UpdateSiteClan?Id=" + id + "&league=" + league + "&charity=" + charity,
      beforeSend: function () {
        $(".spinner").show();
      },
      type: "POST",
      contentType: false,
      processData: false,
      cache: false,
      complete: function () {
        $(".spinner").hide();
        $(this).removeAttr("disabled");
      },
      success: function (data) {
        if (data.success) {
          toastr.success("Updated successfully.");
        }

      },

      error: function (xhr, status, error) {
        $(this).removeAttr("disabled");
        toastr.error(xhr.responseText);
      }
    });
  }

  function updateSiteClanImage(id, file) {
    var formData = new FormData();
    formData.append('id', id);
    formData.append('imageFile', file);
    if (id > 0) {
        $.ajax({
        url: "/TalkSport/UpdateSiteClanImage",
        type: "POST",
        beforeSend: function () {
          $(".spinner").show();
        },
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        complete: function () {
          $(".spinner").hide();
          $('#Image-file').val(null);
        },
        success: function (data) {
          if (data.success === true) {
            toastr.success("Updated Successfully");
            setTimeout(function () {
                window.location.reload();
              },
              1500);
          }
          setUpEvents();
        },
        error: function (xhr, status, error) {
          console.log(xhr.responseText);
        }
      });
    } else {
      toastr.error("something went wrong");
    }
   
  }


  //delete site clan image
  function deleteSiteClanImage(id) {
    var isConfirm = confirm("Are you sure you want to delete the image for this club?");
    if (isConfirm === false) {
      return false;
    }
    var formData = new FormData();
    formData.append('id', id);
    if (id > 0) {
      $.ajax({
        url: "/TalkSport/DeleteSiteClanImage",
        type: "POST",
        beforeSend: function () {
          $(".spinner").show();
        },
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        complete: function () {
          $(".spinner").hide();
          $('#Image-file').val(null);
        },
        success: function (data) {
          if (data.success === true) {
            toastr.success(data.data);
            setTimeout(function () {
                window.location.reload();
              },
              1500);
          }
          setUpEvents();
        },
        error: function (xhr, status, error) {
          console.log(xhr.responseText);
        }
      });
    } else {
      toastr.error("something went wrong");
    }

  }
  


  function search(leagueId, siteCategoryId, pageNumber) {
    var url = "/TalkSport/SearchClub?leagueId=" + leagueId + "&siteCategoryId=" + siteCategoryId + "&pageNumber=" + pageNumber;
    window.location.href = url;
  }


  function setUpEvents() {
    $('a, button').click(function() {
      $(this).toggleClass('active');
    });

    //
    $("#secondDiv #pager a").each(function () {
      $(this).unbind().click(function (e) {
        e.preventDefault();
        
        var pageNumber = $(this).attr('pagenumber');
        var form = $("#btnSearchOffer").closest("form");

        var leagueId = $("#LeagueId").val();
        var siteCategoryId = $("#SiteCategoryId option:selected").eq(0).val();
        search(leagueId, siteCategoryId, pageNumber);
      });
    });
    //
  }


  return {
    setUpEvents: setUpEvents,
    updateSiteClanImage: updateSiteClanImage,
    deleteSiteClanImage: deleteSiteClanImage
  };
})();