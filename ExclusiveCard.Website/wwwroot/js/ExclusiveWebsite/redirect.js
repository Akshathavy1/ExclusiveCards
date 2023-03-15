var Redirect = (function () {
    "use strict";

    function setUpEvents(offer, card, token) {
        $.ajax({
            url: "/Offers/Transfer",
            beforeSend: function () {
                //$("#loader").show();
            },
            type: "GET",
            data: { offerId: offer, membershipCard: card, token: token },
            cache: false,
            complete: function () {
                //$("#loader").hide();
            },
            success: function (data) {
                console.log(data);
                if (data.success) {
                    window.location.href = data.data;
                }
            },

            error: function (xhr, status, error) {
                //toastr.error(xhr.responseText);
            }
        });
    }

    return {
        setUpEvents: setUpEvents
    };
})();