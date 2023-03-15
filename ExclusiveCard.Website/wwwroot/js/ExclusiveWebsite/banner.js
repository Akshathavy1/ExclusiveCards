var Banner = (function () {
    "use strict";
    var slots = 0;
    var full = 0;

    /*Slotted-Banner 5 Starts Here */
    function Slotted_banner_slider_init(slottedId) {

        var Slotted_banner_options = {
            $SlideWidth: 154,
            $SlideHeight: 80,
            $SlideSpacing: 35,
            $ArrowNavigatorOptions: {
                $Class: $JssorArrowNavigator$
                //$Steps: 5
            }
        };

        var Slotted_banner_slider = new $JssorSlider$(slottedId, Slotted_banner_options);

        /*#region responsive code begin*/

        var MAX_WIDTH = 1920;

        function ScaleSlider() {
            var containerElement = Slotted_banner_slider.$Elmt.parentNode;
            var containerWidth = containerElement.clientWidth;

            if (containerWidth) {

                var expectedWidth = Math.min(MAX_WIDTH || containerWidth, containerWidth);

                Slotted_banner_slider.$ScaleWidth(expectedWidth);
            }
            else {
                window.setTimeout(ScaleSlider, 30);
            }
        }

        ScaleSlider();

        $Jssor$.$AddEvent(window, "load", ScaleSlider);
        $Jssor$.$AddEvent(window, "resize", ScaleSlider);
        $Jssor$.$AddEvent(window, "orientationchange", ScaleSlider);
        /*#endregion responsive code end*/
    };
    /*Slotted-Banner Ends Here */

    /*Slotted-Banner 6 Starts Here */
    function Slotted_bannerpage_slider_init(slottedId) {

        var Slotted_bannerpage_options = {
            $SlideWidth: 134,
            $SlideHeight: 80,
            $SlideSpacing: 30,
            $ArrowNavigatorOptions: {
                $Class: $JssorArrowNavigator$
                //$Steps: 5
            }
        };

        var Slotted_bannerpage_slider = new $JssorSlider$(slottedId, Slotted_bannerpage_options);

        /*#region responsive code begin*/

        var MAX_WIDTH = 1920;

        function ScaleSlider() {
            var containerElement = Slotted_bannerpage_slider.$Elmt.parentNode;
            var containerWidth = containerElement.clientWidth;

            if (containerWidth) {

                var expectedWidth = Math.min(MAX_WIDTH || containerWidth, containerWidth);

                Slotted_bannerpage_slider.$ScaleWidth(expectedWidth);
            }
            else {
                window.setTimeout(ScaleSlider, 30);
            }
        }

        ScaleSlider();

        $Jssor$.$AddEvent(window, "load", ScaleSlider);
        $Jssor$.$AddEvent(window, "resize", ScaleSlider);
        $Jssor$.$AddEvent(window, "orientationchange", ScaleSlider);
        /*#endregion responsive code end*/
    };
    /*Slotted-Banner 6 Ends Here */

    /*Fullwidth-Banner Starts Here */
    function full_width_banner_slider_init(fullbannerId) {

        var full_width_banner_options = {
            $SlideWidth: 980,
            $SlideSpacing: 0,
            $ArrowNavigatorOptions: {
                $Class: $JssorArrowNavigator$
                //$Steps: 1
            }
        };

        var full_width_banner_slider = new $JssorSlider$(fullbannerId, full_width_banner_options);

        /*#region responsive code begin*/

        var MAX_WIDTH = 1920;

        function ScaleSlider() {
            var containerElement = full_width_banner_slider.$Elmt.parentNode;
            var containerWidth = containerElement.clientWidth;

            if (containerWidth) {

                var expectedWidth = Math.min(MAX_WIDTH || containerWidth, containerWidth);

                full_width_banner_slider.$ScaleWidth(expectedWidth);
            }
            else {
                window.setTimeout(ScaleSlider, 30);
            }
        }

        ScaleSlider();

        $Jssor$.$AddEvent(window, "load", ScaleSlider);
        $Jssor$.$AddEvent(window, "resize", ScaleSlider);
        $Jssor$.$AddEvent(window, "orientationchange", ScaleSlider);
        /*#endregion responsive code end*/
    };
    /*Fullwidth-Banner Ends Here */

    function bannerdisplay(fullbanner, slotbanner) {
        full = fullbanner;
        slots = slotbanner;
        var i;
        var j;
        for (i = 1; i < fullbanner; i++) {
            full_width_banner_slider_init("full_width_banner" + i);
        }
        for (j = 1; j < slotbanner; j++) {
            Slotted_banner_slider_init("Slotted_banner" + j);
        }
    }

    function bannerpagedisplay(fullbanner, slotbanner) {
        full = fullbanner;
        slots = slotbanner;
        var i;
        var j;
        for (i = 1; i < fullbanner; i++) {
            full_width_banner_slider_init("full_width_banner" + i);
        }
        for (j = 1; j < slotbanner; j++) {
            Slotted_bannerpage_slider_init("Slotted_banner" + j);
        }
    }

    function setUpEvents() {
     }

    return {
        setUpEvents: setUpEvents,
        Slotted_banner_slider_init: Slotted_banner_slider_init,
        bannerdisplay: bannerdisplay,
        bannerpagedisplay: bannerpagedisplay
    };
})();