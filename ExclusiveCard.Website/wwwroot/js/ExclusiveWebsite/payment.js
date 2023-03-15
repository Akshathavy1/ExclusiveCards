var Payment = (function () {
    "use strict";

    function setUpEvents(data) {
        console.log(data);
        $.ajax({
            url: "/PayPal/PayPalAccountRegister",
            beforeSend: function () {
                //$("#loader").show();
            },
            type: "POST",
            data: { customerPaymentId : data },
            cache: false,
            complete: function () {
                //$("#loader").hide();
            },
            success: function (data) {
                console.log(data);
                if (data.success) {
                    $("#processingPayment").hide();
                    $("#paymentSuccessMessage").show();
                } else {
                    console.log(data);
                }
            },

            error: function (xhr, status, error) {
                toastr.error(xhr.responseText);
            }
        });
    }

    return {
        setUpEvents: setUpEvents
    };
})();