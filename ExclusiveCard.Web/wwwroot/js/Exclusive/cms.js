var Cms = (function () {
    "use strict";

    function searchAdvert(form) {
        $.ajax({
            url: "/Cms/Search",
            beforeSend: function () {
                $(".bannerSpinner").show();
            },
            type: "GET",
            data: form.serialize(),
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".bannerSpinner").hide();
            },
            success: function (data) {
                if (data) {
                    $("#advertsearch").html(data);
                    setUpEvents();
                }
                else {
                    toastr.error(data);
                }
            },
            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function scrollfunction() {
        $('.offrcontent').animate({
            scrollTop: $("#advertop").offset().top
        }, 500);
    }

    function deleteAdvert(id) {
        $.ajax({
            url: "/Cms/Delete/?id=" + id,
            beforeSend: function () {
                $(".spinner").show();
            },
            type: "POST",
            data: null,
            contentType: false,
            processData: false,
            cache: false,
            complete: function () {
                $(".spinner").hide();
            },
            success: function (data) {
                if (data.success) {
                    toastr.success("Advert deleted successfully.");
                    setTimeout(function () {
                        window.location.reload();
                    }, 1500);
                } else {
                    toastr.error(data.errorMessage);
                }
            },
            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    function kendoUIBind() {
         $("#Name").kendoComboBox({
            filter: "contains"
        });
     }

    function setUpEvents() {

        $("#AdvertList #pager a").each(function () {
            $(this).unbind().click(function (e) {
                e.preventDefault();
                var pageNumber = $(this).attr('pagenumber');
                $("#CurrentPage").val(pageNumber);
                var _form = $("#SearchAdverts").closest("form");
                var isvalid = _form.valid();
                if (isvalid) {
                    searchAdvert(_form);
                }
                scrollfunction();
            });
        });

        $('button').click(function () {
            $(this).toggleClass('navi');
        });

        $("#SearchAdverts").unbind().click(function (e) {
            e.preventDefault();
            $("#SearchAdverts").addClass("active");
            var _form = $(this).closest("form");
            var isvalid = _form.valid();
            if (isvalid) {
                searchAdvert(_form);
            }
            $("#SearchAdverts").removeClass("active");
         });

        //merchant list shorting
        $(".cardheaders").each(function () {
            $(this).click(function (e) {
                e.preventDefault();

                var sortfield = this.childNodes[1].attributes["SelSortField"].value;
                var sortdir = "asc";
                var sorticon = "fa fa-sort-alpha-desc";

                if ($("#SortField").val() === sortfield) {
                    if ($("#SortDirection").val() === "asc") {
                        sortdir = "desc";
                        sorticon = "fa fa-sort-alpha-desc";
                    }
                    else {
                        sortdir = "asc";
                        sorticon = "fa fa-sort-alpha-asc";
                    }
                }
                $("#SortDirection").val(sortdir);
                var _form = $("#SearchAdverts").closest("form");
                var isvalid = _form.valid();
                if (isvalid) {
                    searchAdvert(_form);
                }
            });
        });

        $(".deleteAdvert").click(function (e) {
            e.preventDefault();
            var id = $(this).attr("advertId");
            if (id !== "") {
                deleteAdvert(id);
            }
        });
 //Clear KendoUI Text if defaultValue Select
        $(document).on("click", "input:text[name='Name_input']", function () {
            var advertTxt = $("input:text[name='Name_input']").val();
            var defaultSel = "Select Advert";
            if (advertTxt === defaultSel) {
                $("input:text[name='Name_input']").val('');
            }
        });
    }

    return {
        setUpEvents: setUpEvents,
        kendoUIBind: kendoUIBind
    };
}());