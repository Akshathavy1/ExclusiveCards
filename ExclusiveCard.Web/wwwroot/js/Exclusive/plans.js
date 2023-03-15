var shouldClearAgent;
var Agent = {
    Id: 0,
    Name: "",
    ReportCode: "",
    Description: "",
    Commission: "50"
};
var Plans = (function () {
    function kendoUIBind() {
        // create ComboBox from select HTML element
        $("#WhiteLabelId").kendoComboBox({
            filter: 'contains'
        });
        $("#CardProviderId").kendoComboBox({
            filter: 'contains'
        });
        $("#MembershipPlanId").kendoComboBox({
            filter: 'contains'
        });
        $("#AgentId").kendoComboBox({
            filter: 'contains'
        });

        //function to change whitelabel
        $("#WhiteLabelId").unbind().change(function (e) {
            e.preventDefault();
            var whiteLabelId = $(this).val();
            if (whiteLabelId !== "" && parseInt(whiteLabelId) > 0) {
                $("#divPlans").show();
                $("#divMembershipPlans").hide();
                $("#CardProviderId").data("kendoComboBox").value("");
            } else {
                $("#WhiteLabelId").data("kendoComboBox").value("");
                $("#CardProviderId").data("kendoComboBox").value("");
                $("#divPlans").hide();
                $("#MembershipPlanContainer").hide();
                $("#divMembershipPlans").hide();
            }
            $("input:text[name^='CardProviderId_input'],input:text[name^='CardProviderId']").val('');
            $("#CardProviderIdVal").val();
            $("#CardProviderName").val("");
            hideContent(CardNameContainer);
            $("#divAgents").hide();
            $("#RegistrationCodes").hide();
            $("#createNewRegistrationCodeContainer").hide();
        });

        $('#WhiteLabelId').focusout(function () {
            var $elem = $("#WhiteLabelId").data("kendoComboBox");
            $elem.dataSource.filter([]); //reset filters
        });

        $('#AgentId').focusout(function () {
            var $elem = $("#AgentId").data("kendoComboBox");
            $elem.dataSource.filter([]); //reset filters
        });

        // function to change card provider
        $("#CardProviderId").unbind().change(function (e) {
            if (localStorage.getItem('cardProviderIdStorageKey') === null) {
                e.preventDefault();
                var CardProviderId = $(this).val();
                if (CardProviderId !== "" && parseInt(CardProviderId) > 0) {
                    showContent(CardNameContainer)
                    $("#MembershipPlanContainer").hide();
                    $("#CardProviderIdVal").val(CardProviderId);
                    $("#CardProviderName").val($("#CardProviderId option:selected").text());
                    localStorage.setItem('cardProviderIdStorageKey', 'true');
                    GetMembershipPlans($("#WhiteLabelId").val(), CardProviderId, 0, false);
                }
                else {
                    $("#CardProviderIdVal").val(0);
                    $("#CardProviderName").val("");
                    $("#CardProviderId").data("kendoComboBox").value("");
                    $("#MembershipPlanId").data("kendoComboBox").value("");
                    hideContent(CardNameContainer);
                    $("#divMembershipPlans").hide();
                }
                $("#divAgents").hide();
                $("#RegistrationCodes").hide();
                $("#createNewRegistrationCodeContainer").hide();
            }
        });

        //function to cancel Card Provider
        $(document).on("click", "#CancelCardProvider", function () {
            $("#CardProviderIdVal").val(0);
            $("#CardProviderName").val("");
            hideContent(CardNameContainer);
        });

        //for Membership
        //Clear KendoUI Text if defaultValue Select
        $(document).on("click", "input:text[aria-owns^='MembershipPlanId_listbox']", function () {
            if ($("input:text[aria-owns^='MembershipPlanId_listbox']").val() === "Start Typing") {
                $("input:text[aria-owns^='MembershipPlanId_listbox'],input:text[name^='MembershipPlanId']").val('');
            }
        });

        //function to change membership plans
        $("#MembershipPlanId").unbind().change(function (e) {
            e.preventDefault();
            var MembershiplPlanId = $(this).val();
            if (MembershiplPlanId !== "" && parseInt(MembershiplPlanId) > 0) {
                //Plan Name
                $("#PlanId").val(MembershipPlanId);
                $("#PlanName").val($("#MembershipPlanId option:selected").text());
                $("#MembershipPlanContainer").show();
                $("#CardNameContainer").hide();
                shouldClearAgent = false;
                GetMembershipPlans($("#WhiteLabelId").val(), $("#CardProviderId").val(), MembershiplPlanId, false);
                $('#btnSaveMembershipPlan').prop('disabled', true);
                $('#MembershipLevelId').prop("disabled", false);
            }
            else {
                $("#MembershipPlanContainer").hide();
                $("#divAgents").hide();
                $("#MembershipPlanId").data("kendoComboBox").value("");
                $("#AgentId").data("kendoComboBox").value("");
                $("#RegistrationCodes").hide();
                $("#createNewRegistrationCodeContainer").hide();
            }
        });

        //function to change Paid By Card Provider radio button
        $('input[type=radio][name=PaidByEmployer]').change(function () {
            if (this.value === "true") {
                $("#PartnerCardPrice").show();
                $("#DivPoundId").show();
                $("#PartnerCardPrice").val('19.99');
            }
            else if (this.value === "false") {
                $("#PartnerCardPrice").hide();
                $("#DivPoundId").hide();
                $("#errorMsg").hide();
                $("#PartnerCardPrice").val(0);
            }
        });

        //function to change Plan Type
        $(function () {
            $('#MembershipPlanTypeId').change(function () {
                var planTypeText = $("#MembershipPlanTypeId option:selected").text();
                if (planTypeText === "BenefitRewards") {
                    $("#BenefactorPercentageContainer").show();
                    $('#DeductionPercentage').val("20");
                    $('#CustomerCashbackPercentage').val("55");
                    $('#BenefactorPercentage').val("25");
                    $("#errorMsgCashback").hide();
                } else {
                    $("#BenefactorPercentageContainer").hide();
                    $('#DeductionPercentage').val("20");
                    $('#CustomerCashbackPercentage').val("80");
                    $("#errorMsgCashback").hide();
                }
            });
        });

        //function to change Membership Type
        $(function () {
            $('#MembershipLevelId').change(function () {
                var planTypeText = $("#MembershipLevelId option:selected").text();
                var startDate = $("#membershipValidFrom").datepicker('getDate');
                var endDate = new Date(startDate);
                if (planTypeText === "Standard") {
                    endDate.setDate(startDate.getDate() + 36500);
                } else {
                    endDate.setDate(startDate.getDate() + 365);
                }
                $('#membershipValidTo').datepicker("setDate", getDate(endDate));
            });
        });

        $("#PartnerCardPrice").keyup(function () {
            if ($('#PartnerCardPrice').val() < 1 || $('#PartnerCardPrice').val() > 99.99) {
                $('#errorMsg').show();
            }
            else {
                $('#errorMsg').hide();
            }
        });
        $("#MaximumNumberOfMemberships").keyup(function () {
            const div = document.getElementById('errorMsgMax');
            if ($("#MembershipLevelId option:selected").text() == "Standard"
                && ($('#MaximumNumberOfMemberships').val() < 1 || $('#MaximumNumberOfMemberships').val() > 1000000000))
            {
                div.textContent = "Enter a number between 1 and 1,000,000,000.";
                $('#errorMsgMax').show();
            }
            else if ($("#MembershipLevelId option:selected").text() != "Standard"
                && ($('#MaximumNumberOfMemberships').val() < 1 || $('#MaximumNumberOfMemberships').val() > 10000000))
            {
                div.textContent = "Enter a number between 1 and 10,000,000.";
                $('#errorMsgMax').show();
            }
            else {
                $('#errorMsgMax').hide();
            }
        });
        $("#CustomerCashbackPercentage").keyup(function () {
            maxSharingCheck();
        });
        $("#DeductionPercentage").keyup(function () {
            maxSharingCheck();
        });
        $("#BenefactorPercentage").keyup(function () {
            maxSharingCheck();
        });
        $("#MembershipLength").keyup(function () {
            if ($('#MembershipLength').val() < 7 || $('#MembershipLength').val() > 36500) {
                $('#errorMsgMembership').show();
            }
            else {
                $('#errorMsgMembership').hide();
            }
        });

        $(document).on("click", "#CancelMembershipPlan", function () {
            $("#MembershipPlanContainer").hide();
            $('#MembershipLevelId').prop("disabled", false);
        });

        $(document).on("click", "#MembershipValidIconFrom", function () {
            var fromDate = $("#membershipValidFrom").datepicker('getDate');
            var toDate = $("#membershipValidTo").datepicker('getDate');
            $('#ValidFrom').val(fromDate);
            $('#ValidTo').val(toDate);
            $("#membershipValidFrom").datepicker({
                dateFormat: 'dd/mm/yy',
                minDate: new Date(),
                onClose: function (selectedDate) {
                    // Set the minDate of 'to' as the selectedDate of 'from'
                    $("#membershipValidTo").datepicker("option", "minDate", selectedDate);
                }
            }).focus();
        });

        $('#frmMembershipPlanId')
            .each(function () {
                $(this).data('serialized', $(this).serialize())
            }).on('change input', function () {
                $(this).find('input:submit, button:submit')
                    .prop('disabled', $(this).serialize() == $(this).data('serialized'))
                    ;
            })
            .find('input:submit, button:submit')
            .prop('disabled', true);

        $(document).on("click", "input:text[aria-owns^='AgentId_listbox']", function () {
            if ($("input:text[aria-owns^='AgentId_listbox']").val() === "Start Typing") {
                $("input:text[aria-owns^='AgentId_listbox'],input:text[name^='AgentId']").val('');
            }
        });

        //function to change agent
        $("#AgentId").unbind().change(function (e) {
            e.preventDefault();
            var AgentId = $(this).val();
            if (AgentId !== "" && parseInt(AgentId) > 0) {
                $("#AgentContainer").show();
                $("#MembershipPlanContainer").hide();
                GetAgents(AgentId, true, true);
                shouldClearAgent = false;
            } else {
                $("#AgentId").data("kendoComboBox").value("");
                $("#AgentContainer").hide();
                //resetting agent
                $("#AgentIdVal").val(0);
                $("#AgentName").val("");
                $("#AgentReportCode").val("");
                $("#AgentDescription").val("");
                $("#CommissionPercent").val(0);
                $("#btnSaveAgent").prop('disabled', false);
                $("#MembershipPlanContainer").hide();
            }
        });

        //For showing agents details when clicking add new agents button
        $(document).on("click", "#addNewAgentBtn", function () {
            $("#MembershipPlanContainer").hide();
            $("#AgentContainer").show();
            $("#AgentFieldContainer").show();
            $("#AgentIdVal").val(0);
            $("#AgentName").val("");
            $("#AgentReportCode").val("");
            $("#AgentDescription").val("");
            $("#CommissionPercent").val(50);
            $("#AgentStartDate").val(null);
            $("#AgentEndDate").val(null);
            $("#AgentIsDeleted").val(0);
            $("#AgentId").data("kendoComboBox").value("");
            shouldClearAgent = false;
            $("input:text[name^='AgentId'],input:text[name^='AgentId']").val('Start Typing');
        });

        $("#CommissionPercent").keyup(function () {
            if ($('#CommissionPercent').val() < 0 || $('#CommissionPercent').val() > 100) {
                $('#errorMessageCommission').show();
            }
            else {
                $('#errorMessageCommission').hide();
            }
        });

        $(".allow-numeric").on("input", function (evt) {
            var self = $(this);
            self.val(self.val().replace(/\D/g, ""));
            if ((evt.which < 48 || evt.which > 57)) {
                evt.preventDefault();
            }
        });

        //function to show create new registration code
        $(document).on("click", "#addNewRegCodeBtn", function () {
            $('#createNewRegistrationCodeContainer').show();
            $("#fileSpinner").hide();
            var todayDate = new Date();
            var defaultEndDate = new Date(todayDate)
            defaultEndDate.setDate(todayDate.getDate() + 365);
            $('#validFrom').val(getDate(todayDate));
            $('#validTo').datepicker({
                minDate: new Date(),
                dateFormat: 'dd/mm/yy',
            });
            $('#validTo').datepicker("setDate", getDate(defaultEndDate));
            $("#validTo").datepicker("option", "minDate", todayDate);
            $("#validFrom").datepicker("option", "minDate", todayDate);
            $('#numberOfCodes').val(25);
            $("#NumberOfCodes").show();
            $("#NumberOfUses").hide();
            $('.codeType[value="unique"]').prop('checked', true);
            $('#errorMessageUses').hide();
            $('#errorMessageCode').hide();
        });

        $(document).on("click", "#cancelRegistrationCode", function () {
            $('#validFrom').datepicker("setDate", getDate(new Date()));
            $('#createNewRegistrationCodeContainer').hide();
        });

        //function to show cancel and save button and clear agent dropdown to remove agent
        $(document).on("click", "#removeAgent", function () {
            $("#AgentContainer").show();
            $("#AgentFieldContainer").hide();
            $("#AgentId").data("kendoComboBox").value("");
            $("#AgentIdVal").val(0);
            $("input:text[aria-owns^='AgentId_listbox'],input:text[name^='AgentId']").val('Start typing');
            shouldClearAgent = true;
        });

        //function to show date picker
        $(document).on("click", "#validIconFrom", function () {
            $("#validFrom").datepicker({
                minDate: new Date(),
                dateFormat: 'dd/mm/yy',
                onClose: function (selectedDate) {
                    // Set the minDate of 'to' as the selectedDate of 'from'
                    $("#validTo").datepicker("option", "minDate", selectedDate);
                }
            }).focus();
        });

        $(document).on("click", "#validIconTo", function () {
            $("#validTo").datepicker({
                onClose: function (selectedDate) {
                    // Set the maxDate of 'from' as the selectedDate of 'to'
                    $("#validFrom").datepicker("option", "maxDate", selectedDate);
                }
            }).focus();
        });

        //function to change code type radio button
        $('input[type=radio][name=codeType]').change(function () {
            if (this.value === "unique") {
                $("#NumberOfCodes").show();
                $("#NumberOfUses").hide();
                $("#numberOfCodes").val(25);
            }
            else if (this.value === "shared") {
                $("#NumberOfCodes").hide();
                $("#numberOfUses").val(25);
                $("#NumberOfUses").show();
            }
        });

        $("#numberOfCodes").keyup(function () {
            if ($('#numberOfCodes').val() < 1 || $('#numberOfCodes').val() > 10000) {
                $('#errorMessageCode').show();
            }
            else {
                $('#errorMessageCode').hide();
            }
            $('#errorMessageUses').hide();
        });

        $("#numberOfUses").keyup(function () {
            const sharedCode = $('input[name=codeType]:checked').val();
            const div = document.getElementById('errorMessageUses');

            if (sharedCode == "shared") {
                if ($('#numberOfUses').val() < 1 || $('#numberOfUses').val() > 100000000) {
                    div.textContent = "Enter a number between 1 and 100,000,000.";
                    $('#errorMessageUses').show();
                }
                else {
                    $('#errorMessageUses').hide();
                }
            }
            else {
                if ($('#numberOfUses').val() < 1 || $('#numberOfUses').val() > 10000) {
                    div.textContent = "Enter a number between 1 and 10,000.";

                    $('#errorMessageUses').show();
                }
                else {
                    $('#errorMessageUses').hide();
                }
            }
            $('#errorMessageCode').hide();
        });

        bindCardProviderControls();
        bindMembershiplanControls();
        bindAgentControls();
        bindRegestrationCodeControls();
    }

    //function to refresh card provider
    function RefreshCardProviders(cardProviderId) {
        //GetAllCardProviders
        $.ajax({
            type: "GET",
            url: "/Plans/GetAllCardProviders?cardProviderId=" + cardProviderId,
            beforeSend: function () {
                //TODO:Display loader
            },
            complete: function () {
                //TODO:Hide loader
            },
            success: function (data) {
                $("#divPlans").show();
                $("#divPlans").html(data);
                kendoUIBind();
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
        resetAgents();
        resetMembershipPlans();
    }

    //function to get membership plans
    function GetMembershipPlans(whiteLabelId, cardProviderId, membershipPlanId, isFromSaved) {
        $.ajax({
            type: "GET",
            url: "/Plans/GetMembershipPlans?whiteLabelId=" + whiteLabelId + "&cardProviderId=" + cardProviderId + "&membershipPlanId=" + membershipPlanId,
            beforeSend: function () {
                //TODO:Display loader
            },
            complete: function () {
                //TODO:Hide loader
            },
            success: function (data) {
                var $elem = $("#CardProviderId").data("kendoComboBox");
                $elem.dataSource.filter([]); //reset filters
                localStorage.removeItem('cardProviderIdStorageKey');
                $("#divMembershipPlans").show();
                $("#divMembershipPlans").html(data);
                if (membershipPlanId > 0) {
                    GetRegistrationCodes(membershipPlanId);
                }
                if (membershipPlanId > 0 && !isFromSaved) {
                    $("#MembershipPlanContainer").show();
                    var planTypeSelected = $("#MembershipPlanTypeId").val();
                    var AgentCodeId = $("#AgentCodeId").val();
                    const validFrom = $("#ValidFrom").val();
                    const validTo = $("#ValidTo").val();
                    $('#membershipValidFrom').datepicker({
                        dateFormat: 'dd/mm/yy',
                    });
                    $('#membershipValidTo').datepicker({
                        dateFormat: 'dd/mm/yy',
                    });
                    const from = validFrom.split("-").reverse().join("/");
                    const to = validTo.split("-").reverse().join("/");
                    console.log(from, to);
                    $('#membershipValidFrom').datepicker("setDate", getDate(new Date(from)));
                    $('#membershipValidTo').datepicker("setDate", getDate(new Date(to)));
                    var minDate = new Date(from);
                    minDate.setDate(minDate.getDate() + 1);
                    $("#membershipValidFrom").datepicker("option", "minDate", getDate(new Date(from)));
                    $("#membershipValidTo").datepicker("option", "minDate", minDate);
                    planTypeSelected == 3 ? $("#BenefactorPercentageContainer").show() : $("#BenefactorPercentageContainer").hide();
                    var partnerCardPrice = $("#PartnerCardPrice").val();
                    if (partnerCardPrice > 0) {
                        $("input[name=PaidByEmployer][value=" + true + "]").prop('checked', true);
                        $("#PartnerCardPrice").show();
                        $("#DivPoundId").show();
                    }
                    else {
                        $("input[name=PaidByEmployer][value=" + false + "]").prop('checked', true);
                        $("#PartnerCardPrice").hide();
                        $("#DivPoundId").hide();
                    }
                    if (AgentCodeId !== "" && parseInt(AgentCodeId) > 0) {
                        GetAgents(AgentCodeId, true, false);
                    }
                    else {
                        GetAgents(0, true, false);
                    }
                }
                else {
                    $("#MembershipPlanContainer").hide();
                }
                kendoUIBind();
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    }

    //function to save card provider
    function bindCardProviderControls() {
        $("#btnSaveCardProvider").unbind().on("click", function (e) {
            e.preventDefault();
            var whiteLabelId = $("#WhiteLabelId").val();
            var form = $("#frmSaveCardProvider");
            var url = form.attr("action");
            $.ajax({
                type: "POST",
                url: url,
                beforeSend: function () {
                    //TODO:Display loader
                    $('#btnSaveCardProvider').prop('disabled', true);
                },
                data: form.serialize(),
                complete: function () {
                    //TODO:Hide loader
                    $('#btnSaveCardProvider').prop('disabled', false);
                },
                success: function (data) {
                    if (data.success) {
                        RefreshCardProviders(data.data);
                        GetMembershipPlans(whiteLabelId, data.data, 0, false);
                    }
                    else {
                        toastr.error(data.errorMessage);
                    }
                },
                error: function (xhr, status, error) {
                    toastr.error(xhr.responseText);
                }
            });
        });
    }
    function resetMembershipPlans() {
    }
    function resetAgents() {
    }

    //function to show Card Name container on clicking add card provider button
    $(document).on("click", "#member-plan-btn", function () {
        showContent(CardNameContainer);
        $("input:text[name^='CardProviderId_input'],input:text[name^='CardProviderId']").val('Start Typing');
        $("#CardProviderId").data("kendoComboBox").value("");
        $("#CardProviderIdVal").val(0);
        $("#CardProviderName").val("");
        $("input:text[name^='Id_input'],input:text[name^='MembershipPlanId']").val('Start Typing');
        $("#MembershipPlanContainer").hide();
        $("#divAgents").hide();
    });

    $("[data-dismiss='cancel']").click(function () {
        hideContent($(this).closest('.card-provider'));
    });

    function hideContent(htmlId) {
        $(htmlId).css('display', 'none');
    }
    function showContent(htmlId) {
        $(htmlId).css('display', 'block');
    }

    function bindMembershiplanControls() {
        $("#btnSaveMembershipPlan").unbind().on("click", function (e) {
            e.preventDefault();
            const formValid = checkMembershipForm();
            if (formValid !== 'false') {
                toastr.error(formValid);
            }
            else {
                const MembershipLevelId = $('#MembershipLevelId').val();
                const MembershipPlanTypeId = $('#MembershipPlanTypeId').val();
                const Id = $('#PlanId').val();
                const Description = $('#Description').val();
                const Duration = $('#MembershipLength').val();
                const ValidFrom = $("#membershipValidFrom").val();
                const ValidTo = $("#membershipValidTo").val();
                const NumberOfCards = $('#MaximumNumberOfMemberships').val();
                const PaidByEmployer = $('input[id="PaidByEmployer"]:checked').val();
                const PartnerCardPrice = $('#PartnerCardPrice').val();
                const DeductionPercentage = $('#DeductionPercentage').val();
                const CustomerCashbackPercentage = $('#CustomerCashbackPercentage').val();
                const BenefactorPercentage = $('#BenefactorPercentage').val();
                const PartnerId = $('#PartnerId').val();
                const CurrencyCode = $('#CurrencyCode').val();
                const MinimumValue = $('#MinimumValue').val();
                const PaymentFee = $('#PaymentFee').val();
                const AgentCodeId = $('#AgentCodeId').val();
                const SiteCategoryId = $('#SiteCategoryId').val();
                var form = new FormData();
                form.append('MembershipLevelId', MembershipLevelId);
                form.append('MembershipPlanTypeId', MembershipPlanTypeId);
                form.append('Id', Id);
                form.append('Duration', Duration);
                form.append('Description', Description);
                form.append('ValidFrom', ValidFrom);
                form.append('ValidTo', ValidTo);
                form.append('NumberOfCards', NumberOfCards);
                form.append('PaidByEmployer', PaidByEmployer);
                form.append('PartnerCardPrice', PartnerCardPrice);
                form.append('DeductionPercentage', DeductionPercentage);
                form.append('CustomerCashbackPercentage', CustomerCashbackPercentage);
                form.append('BenefactorPercentage', BenefactorPercentage);
                form.append('PartnerId', PartnerId);
                form.append('CurrencyCode', CurrencyCode);
                form.append('MinimumValue', MinimumValue);
                form.append('PaymentFee', PaymentFee);
                form.append('AgentCodeId', AgentCodeId);
                form.append('SiteCategoryId', SiteCategoryId);
                var whiteLabelId = $("#WhiteLabelId").val();
                var cardProviderId = $("#CardProviderId").val();

                $.ajax({
                    type: "POST",
                    url: "/Plans/SaveMembershipPlans" + "?whiteLabelId=" + whiteLabelId + "&cardProviderId=" + cardProviderId,
                    beforeSend: function () {
                        //TODO:Display loader
                        $('#btnSaveMembershipPlan').prop('disabled', true);
                    },
                    data: form,
                    contentType: false,
                    processData: false,
                    complete: function () {
                        //TODO:Hide loader
                        $('#btnSaveMembershipPlan').prop('disabled', false);
                    },
                    success: function (data) {
                        if (data.success) {
                            GetMembershipPlans(whiteLabelId, cardProviderId, data.data, true);
                            GetAgents(0, false, false);
                        }
                        else {
                            toastr.error("Something Went Wrong");
                        }
                    },
                    error: function (xhr, status, error) {
                        toastr.error(xhr.responseText);
                    }
                });
            }
        });
    }

    function GetAgents(agentId, showAgentContainer, fromdropdown) {
        //GetAgents
        var url = "/Plans/GetAgents?agentId=" + agentId;

        $.ajax({
            type: "GET",
            url: url,
            beforeSend: function () {
                //TODO:Display loader
            },
            complete: function () {
                //TODO:Hide loader
            },
            success: function (data) {
                $("#divAgents").show();
                $("#divAgents").html(data);
                if (agentId > 0 && showAgentContainer) {
                    $("#AgentContainer").show();
                    $("#AgentFieldContainer").show();
                    Agent = {
                        Id: $("#AgentIdVal").val(), Name: $("#AgentName").val(), Description: $("#AgentDescription").val()
                        , CommissionPercent: $("#CommissionPercent").val(), ReportCode: $("#AgentReportCode").val()
                    };
                }
                if (!fromdropdown && agentId > 0) {
                    $('#removeAgent').prop('disabled', false);
                }
                else {
                    $('#removeAgent').prop('disabled', true);
                }
                kendoUIBind();
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    }

    function bindAgentControls() {
        $("#btnSaveAgent").unbind().on("click", function (e) {
            e.preventDefault();
            var membershipPlanId = $("#PlanId").val();
            const formValid = checkAgentForm();
            if (!shouldClearAgent) {
                if (formValid !== 'false') {
                    toastr.error(formValid);
                }
                else {
                    var form = $("#fromSaveAgent");
                    var url = form.attr("action");
                    $.ajax({
                        type: "POST",
                        url: url + "?membershipPlanId=" + membershipPlanId,
                        beforeSend: function () {
                            //TODO:Display loader
                            $('#btnSaveAgent').prop('disabled', true);
                        },
                        data: form.serialize(),
                        complete: function () {
                            //TODO:Hide loader
                            $('#btnSaveAgent').prop('disabled', false);
                        },
                        success: function (data) {
                            if (data.success) {
                                GetAgents(data.data, false);
                                $("#AgentContainer").hide();
                            }
                            else {
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
                var url = "/Plans/RemoveAgent?membershipPlanId=" + membershipPlanId;
                $.ajax({
                    type: "POST",
                    url: url,
                    beforeSend: function () {
                        //TODO:Display loader
                        $('#btnSaveAgent').prop('disabled', true);
                    },
                    complete: function () {
                        //TODO:Hide loader
                        $('#btnSaveAgent').prop('disabled', false);
                    },
                    success: function (data) {
                        if (data.success) {
                            $('#removeAgent').prop('disabled', true);
                            $("input:text[aria-owns^='AgentId_listbox'],input:text[name^='AgentId']").val('Start typing');
                            $("#AgentContainer").hide();
                        }
                        else {
                            $('#removeAgent').prop('disabled', false);
                            toastr.error(data.errorMessage);
                        }
                    },
                    error: function (xhr, status, error) {
                        toastr.error(xhr.responseText);
                    }
                });
            }
        });
    }

    function checkMembershipForm() {
        var deduction = $("#DeductionPercentage").val();
        var customerCashback = $("#CustomerCashbackPercentage").val();
        var benefactor = $("#BenefactorPercentage").val();
        var cardType = $("#MembershipPlanTypeId").val();
        const paidByEmployer = $('input[name=PaidByEmployer]:checked').val();
        const planName = $('#Description').val();
        if (planName == '') {
            return 'Plan name is required';
        }
        if ($('#MembershipLength').val() < 7 || $('#MembershipLength').val() > 36500) {
            return 'Invalid Membership Length';
        }
        if ($("#MembershipLevelId option:selected").text() == "Standard"
            && ($('#MaximumNumberOfMemberships').val() < 1 || $('#MaximumNumberOfMemberships').val() > 1000000000))
        {
            return 'Invalid Maximum Number of Memberships';
        }
        if ($("#MembershipLevelId option:selected").text() != "Standard"
            && ($('#MaximumNumberOfMemberships').val() < 1 || $('#MaximumNumberOfMemberships').val() > 10000000)) {
            return 'Invalid Maximum Number of Memberships';
        }
        if (paidByEmployer === 'true') {
            if ($('#PartnerCardPrice').val() < 1 || $('#PartnerCardPrice').val() > 99.99) {
                return 'Invalid Partner Card Price';
            }
        }
        if (cardType == 3) {
            if ((parseInt(deduction) + parseInt(customerCashback) + parseInt(benefactor)) != 100) {
                return 'Invalid Cashback Sharing';
            }
        }
        else {
            if ((parseInt(deduction) + parseInt(customerCashback)) != 100) {
                return 'Invalid Cashback Sharing';
            }
        }
        return 'false';
    }

    function checkAgentForm() {
        var reportCode = $("#AgentReportCode").val();
        var description = $("#AgentDescription").val();
        var commissionPercent = $("#CommissionPercent").val();

        if (reportCode == '') {
            return 'Report Code is required';
        }

        if (description == '') {
            return 'Description is required';
        }

        if (commissionPercent < 0 || commissionPercent > 100 || commissionPercent == '') {
            return 'Invalid commission percent';
        }

        return 'false';
    }

    //function to Get Registration Codes
    function GetRegistrationCodes(MembershipPlanId) {
        //GetAllRegistrationCode
        $.ajax({
            type: "GET",
            url: "/Plans/GetRegistrationCodes?membershipPlanId=" + MembershipPlanId,
            beforeSend: function () {
            },
            complete: function () {
            },
            success: function (data) {
                $("#RegistrationCodes").show();
                $("#createNewRegistrationCodeContainer").hide();
                $("#RegistrationCodes").html(data);
                kendoUIBind();
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    }

    function bindRegestrationCodeControls() {
        $("#btnSaveRegCode").unbind().on("click", function (e) {
            e.preventDefault();
            var NumberOfCodes;
            var isUnique = false;
            var codeType = $("input:radio[name=codeType]:checked").val()
            if (codeType == 'unique') {
                isUnique = true;
                NumberOfCodes = $('#numberOfCodes').val();
            }
            else {
                NumberOfCodes = $('#numberOfUses').val();
            }
            if (((parseInt(NumberOfCodes) < 1 || parseInt(NumberOfCodes) > 10000) && codeType == 'unique')
                || ((parseInt(NumberOfCodes) < 1 || parseInt(NumberOfCodes) > 100000000) && codeType == 'shared')) {
                toastr.error("Some fields are invalid");
            }
            else {
                var form = new FormData();
                var ValidFrom = $('#validFrom').val();
                var ValidTo = $('#validTo').val();
                var MembershipPlanId = $('#PlanId').val();
                form.append('ValidFrom', ValidFrom);
                form.append('ValidTo', ValidTo);
                form.append('MembershipPlanId', MembershipPlanId);
                form.append('NumberOfCodes', NumberOfCodes);
                $.ajax({
                    type: "POST",
                    url: "Plans/SaveRegistrationCode?isUnique=" + isUnique,
                    beforeSend: function () {
                        $("#fileSpinner").show();
                        $('#btnSaveRegCode').prop('disabled', true);
                        $('#cancelRegistrationCode').prop('disabled', true);
                    },
                    data: form,
                    contentType: false,
                    processData: false,
                    complete: function () {
                        $("#fileSpinner").hide();
                        $('#btnSaveRegCode').prop('disabled', false);
                        $('#cancelRegistrationCode').prop('disabled', false);
                    },
                    success: function (data) {
                        if (data.success) {
                            GetRegistrationCodes(MembershipPlanId);
                            toastr.success("Successfully created");
                        }
                        else {
                            toastr.error(data.errorMessage);
                        }
                    },
                    error: function (xhr, status, error) {
                        toastr.error(xhr.responseText);
                    }
                });
            }
        });
    }

    return {
        kendoUIBind: kendoUIBind
    };
})();

function clickToCopy(id) {
    $.ajax({
        type: "POST",
        url: "/Plans/CopyRegistrationCode?registrationCodeSummaryId=" + id,
        beforeSend: function () {
            //TODO:Display loader
        },
        complete: function () {
            //TODO:Hide loader
        },
        success: function (data) {
            if (data.success) {
                var text = data.data;
                var sampleTextarea = document.createElement("textarea");
                document.body.appendChild(sampleTextarea);
                sampleTextarea.value = text;
                sampleTextarea.select();
                document.execCommand("copy");
                document.body.removeChild(sampleTextarea);
                toastr.success("Registration code has been added to the clipboard ");
            }
            else {
                toastr.error(data.errorMessage);
            }
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function urlClick(id) {
    var whiteLabelId = $("#WhiteLabelId").val();
    $.ajax({
        type: "POST",
        url: "/Plans/GenerateRegistrationCodeURL?registrationCodeSummaryId=" + id + "&whiteLabelId=" + whiteLabelId,
        beforeSend: function () {
            //TODO:Display loader
        },
        complete: function () {
            //TODO:Hide loader
        },
        success: function (data) {
            if (data.success) {
                var text = data.data;
                var sampleTextarea = document.createElement("textarea");
                document.body.appendChild(sampleTextarea);
                sampleTextarea.value = text;
                sampleTextarea.select();
                document.execCommand("copy");
                document.body.removeChild(sampleTextarea);
                toastr.success("URL has been added to the clipboard");
            }
            else {
                toastr.error(data.errorMessage);
            }
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function cancelAgent() {
    if (!shouldClearAgent) {
        $("#AgentContainer").hide();
        $("#AgentName").val("");
        $("#AgentReportCode").val("");
        $("#AgentDescription").val("");
        $("#CommissionPercent").val(0);
    }
    // from remove button cancel
    else {
        $("#AgentIdVal").val(Agent.Id);
        $("#AgentName").val(Agent.Name);
        $("input:text[aria-owns^='AgentId_listbox'],input:text[name^='AgentId']").val(Agent.Name);
        $("#AgentReportCode").val(Agent.ReportCode);
        $("#CommissionPercent").val(Agent.CommissionPercent);
        $("#AgentDescription").val(Agent.Description);
        $("#AgentFieldContainer").show();
        shouldClearAgent = false;
    }
}

function getDate(date) {
    return date.toLocaleDateString("en-GB", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
    })
}

function membershipValidIconTo() {
    var fromDate = $("#membershipValidFrom").datepicker('getDate');
    var toDate = $("#membershipValidTo").datepicker('getDate');
    $('#ValidFrom').val(fromDate);
    $('#ValidTo').val(toDate);
    var minDate = $("#membershipValidFrom").datepicker('getDate');
    minDate.setDate(minDate.getDate() + 1);
    $("#membershipValidTo").datepicker("option", "minDate", minDate);
    $("#membershipValidTo").datepicker({
        dateFormat: 'dd/mm/yy',
        onClose: function (selectedDate) {
            // Set the maxDate of 'from' as the selectedDate of 'to'
            $("#membershipValidFrom").datepicker("option", "maxDate", selectedDate);
        }
    }).focus();
}

function maxSharingCheck() {
    var deduction = $("#DeductionPercentage").val();
    var customerCashback = $("#CustomerCashbackPercentage").val();
    var benefactor = $("#BenefactorPercentage").val();
    var cardType = $("#MembershipPlanTypeId").val()
    if (cardType == 3) {
        if ((parseInt(deduction) + parseInt(customerCashback) + parseInt(benefactor)) != 100) {
            $("#errorMsgCashback").show()
        } else {
            $("#errorMsgCashback").hide();
        }
    }
    else {
        if ((parseInt(deduction) + parseInt(customerCashback)) != 100) {
            $("#errorMsgCashback").show()
        } else {
            $("#errorMsgCashback").hide();
        }
    }
}

//function to show membership plan details when clicking add new membership button
function addNewMembershipPlan() {
    var hasStandard = false;
    var whiteLabelId = $("#WhiteLabelId").val();
    var cardProviderId = $("#CardProviderId").val();
    $.ajax({
        type: "GET",
        url: "/Plans/CheckHasStandardPlan" + "?whiteLabelId=" + whiteLabelId + "&cardProviderId=" + cardProviderId,
        beforeSend: function () {
        },
        complete: function () {
        },
        success: function (data) {
            hasStandard = data.data;
            $("#MembershipPlanId").data("kendoComboBox").value("");
            $("#CardNameContainer").hide();
            $("#AgentContainer").hide();
            $("#MembershipPlanContainer").show();
            $("input:text[name^='Id'],input:text[name^='AgentId']").val('Start Typing');
            $("input:text[name^='Id_input'],input:text[name^='MembershipPlanId']").val('Start Typing');
            $('#MembershipPlanTypeId').val('4');
            $('#Description').val('');
            $('#PlanId').val(0);
            $('#DeductionPercentage').val("20");
            $('#CustomerCashbackPercentage').val("80");
            maxSharingCheck();
            // for diamond
            if (hasStandard) {
                $('#MembershipLength').val('365');
                $('#MaximumNumberOfMemberships').val('1000');
                $('#MembershipLevelId').val('2');
                $("input[name=PaidByEmployer][value=" + true + "]").prop('checked', true);
                $("#PartnerCardPrice").show();
                $("#DivPoundId").show();
                $("#BenefactorPercentageContainer").hide();
                $('#PartnerCardPrice').val('19.99');
                var currentDate = new Date();
                var endDate = new Date(currentDate);
                endDate.setDate(currentDate.getDate() + 365);
                var minEndDate = new Date(currentDate);
                minEndDate.setDate(currentDate.getDate() + 1);
                $('#membershipValidFrom').datepicker({
                    dateFormat: 'dd/mm/yy',
                });
                $('#membershipValidTo').datepicker({
                    dateFormat: 'dd/mm/yy',
                });
                $("#membershipValidFrom").datepicker("option", "minDate", currentDate);
                $("#membershipValidTo").datepicker("option", "minDate", minEndDate);
                $('#membershipValidFrom').datepicker("setDate", getDate(currentDate));
                $('#membershipValidTo').datepicker("setDate", getDate(endDate));
                $('#MembershipLevelId').prop("disabled", false);
            }
            // for standard
            else {
                $('#MembershipLevelId').val('1');
                $('#MembershipLevelId').prop("disabled", true);
                $("input[name=PaidByEmployer][value=" + false + "]").prop('checked', true);
                $("input[name=PaidByEmployer][value=" + true + "]").prop('disabled', true);
                $('#MembershipLength').val('36500');
                $('#MaximumNumberOfMemberships').val('10000000');
                var currentDate = new Date();
                var endDate = new Date(currentDate);
                endDate.setDate(currentDate.getDate() + 36500);
                $('#membershipValidFrom').datepicker({
                    dateFormat: 'dd/mm/yy',
                });
                $('#membershipValidTo').datepicker({
                    dateFormat: 'dd/mm/yy',
                });
                var minEndDate = new Date(currentDate);
                minEndDate.setDate(currentDate.getDate() + 1);
                $("#membershipValidFrom").datepicker("option", "minDate", currentDate);
                $("#membershipValidTo").datepicker("option", "minDate", minEndDate);
                $('#membershipValidFrom').datepicker("setDate", getDate(currentDate));
                $('#membershipValidTo').datepicker("setDate", getDate(endDate));
                $("#PartnerCardPrice").hide();
                $("#DivPoundId").hide();
                $("#BenefactorPercentageContainer").hide();
            }
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
    $("#errorMsg").hide();
    $('#errorMsgMax').hide();
    $('#errorMsgMembership').hide();
    $("#RegistrationCodes").hide();
    $("#createNewRegistrationCodeContainer").hide();
}