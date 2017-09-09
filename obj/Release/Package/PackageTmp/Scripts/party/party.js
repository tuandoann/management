function validatorPartyForm() {
    var flagValidator = false;
    if ($('#bookingDate').val()  == "") {
        $("#bookingDate").trigger('click');
        $('a[href="#informationParty"]').trigger('click');
        flagValidator = true;
        toastr["error"]("Vui lòng nhập ngày lập", "Thông báo");
    }
    if ($('#partyDate').val() == "") {
        if (!flagValidator) {
            $("#partyDate").trigger('click');
            $('a[href="#informationParty"]').trigger('click');
        }
        flagValidator = true;
        toastr["error"]("Vui lòng nhập ngày đặt dương lịch", "Thông báo");
    }
    if ($('#customerName').val() == "") {
        if (!flagValidator) {
            $("#customerName").focus();
            $('a[href="#informationParty"]').trigger('click');
        }
        flagValidator = true;
        toastr["error"]("Vui lòng nhập tên khách hàng", "Thông báo");
    }
    if ($('#negativeDate').val() == "" && !validator.tests.date($('#negativeDate').val())) {
        if (!flagValidator) {
            $('a[href="#informationParty"]').trigger('click');
            $("#negativeDate").focus();
        }
        flagValidator = true;
        toastr["error"]("Vui lòng nhập ngày đặt âm lịch", "Thông báo");
    }

    $("#meal-item tr[ name='detailMeal']").each(function () {
        $this = $(this);
        if ($this.find("select[name='productID']")[0].value == "0") {
            if (!flagValidator) {
                $('a[href="#meal"]').trigger('click');
                $this.select2('open');
            }
            flagValidator = true;
            toastr["error"]("Vui lòng nhập món ăn.", "Thông báo");
            return false;
        }
    });

    $("#service-item tr[ name='detailService']").each(function () {
        $this = $(this);
        if ($this.find("select[name='serviceID']")[0].value == "0") {
            if (!flagValidator) {
                $('a[href="#service"]').trigger('click');
                $this.select2('open');
            }
            flagValidator = true;
            toastr["error"]("Vui lòng nhập dịch vụ.", "Thông báo");
        }
    });

    if ($('#depositDate').val() && !validator.tests.date($('#depositDate').val())) {
        if (!flagValidator) {
            $("#depositDate").focus();
            $('a[href="#moneyAndPayMethod"]').trigger('click');
        }
        flagValidator = true;
        toastr["error"]("Ngày không hợp lệ. Xin vui lòng nhập lại!", "Thông báo");
    }
    return !flagValidator;
}
$(function () {
    if ($('#partyFullCalendar').length > 0)
    {
        $('#partyFullCalendar').fullCalendar({
            lang: 'vi',
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,agendaWeek,agendaDay,listMonth'
            },
            selectable: true,
            selectHelper: true,
            eventClick: function (calEvent, jsEvent, view) {
                window.location.href = "/Party/EditParty?id=" + calEvent.id;
            },
            editable: true,
            events: function (start, end, timezone, callback) {
                jQuery.ajax({
                    url: '/Party/GetPartyForFullCalendar',
                    type: 'POST',
                    dataType: 'json',
                    data: {
                        start: start.format(),
                        end: end.format()
                    },
                    success: function (doc) {
                        var events = [];
                        debugger;
                        if (!!doc) {
                            $.map(doc, function (r) {
                                events.push({
                                    id: r.PartyID,
                                    title: r.CustomerName + ' ' + ( r.PartyAddress == null ? "" : r.PartyAddress ),
                                    start: new Date(JSON.dateParser(r.PartyDate)),
                                    end: new Date(JSON.dateParser(r.PartyDate)).addHours(2),
                                });
                            });
                        }
                        callback(events);
                    }
                });
            }
        })
    }
    //reset all form
    $("#resetAllForm").click(function () {
        $("form[name='partyFrom']").trigger('reset')
    });
    //cancel 
    $("#cancelAllForm").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../Party/PartyScheduler";
        }
    });
    //sumbit
    $("#sumbitAllForm").click(function () {
        if (validatorPartyForm())
        {
            var objectParty = new Object({
                "PartyID": $('#partyID').length > 0 ? $('#partyID').val() : 0,
                "BookingDate": $('#bookingDate').data('daterangepicker').startDate._d,
                "PartyDate": $('#partyDate').data('daterangepicker').endDate._d,
                "NegativeDate": new Date($('#negativeDate').val().split('/')[2], $('#negativeDate').val().split('/')[1] - 1, $('#negativeDate').val().split('/')[0]),
                "CustomerName": $('#customerName').val(),
                "PhoneNumber1": $('#phoneNumber1').val(),
                "PhoneNumber2": $('#phoneNumber2').val(),
                "PartyAddress": $('#partyAddress').val(),
                "PartyType": $('#partyType').val(),
                "NumberTablePlan": $('#numberTablePlan').val().replace(/,/gi, ''),
                "NumberTableException": $('#numberTableException').val().replace(/,/gi, ''),
                "NumberTableReal": $('#numberTableReal').val().replace(/,/gi, ''),
                "NumberTableVegetarian": $('#numberTableVegetarian').val().replace(/,/gi, ''),
                "PriceVegetarian": $('#priceVegetarian').val().replace(/,/gi, ''),
                "PricePerTablePlan": $('#pricePerTablePlan').val().replace(/,/gi, ''),
                "PricePerTableReal": $('#pricePerTableReal').val().replace(/,/gi, ''),
                "DepositAmount": $('#depositAmount').val().replace(',', ''),
                "DepositDate": $('#depositDate').val().split.length >=3 ? new Date($('#depositDate').val().split('/')[2], $('#depositDate').val().split('/')[1] - 1, $('#depositDate').val().split('/')[0]) : null ,
                "OtherAmount": $('#otherAmount').val().replace(/,/gi, ''),
                "IsPayCashAfterDoneParty": isPayCashAfterDoneParty.checked,
                "IsPayBank": isPayBank.checked,
                "IsVAT": isVAT.checked,
                "Notes": $('#notes').val(),
                "UserCreate": 0
            });
            var lsObjectMeal = [];
            $("#meal-item tr[ name='detailMeal']").each(function () {
                $this = $(this);
                if ($this.find("select[name='productID']")[0].value != "0") {
                    var object = new Object({
                        "PartyProductID": $this.find("input[name='partyProductID']").length > 0 ? $this.find("input[name='partyProductID']")[0].value : 0,
                        "PartyID": $('#partyID').length > 0 ? $('#partyID').val() : 0,
                        "ProductID": $this.find("select[name='productID']")[0].value,
                        "Quantity": $this.find("input[name='quantity']")[0].value.replace(/,/gi, ''),
                        "Notes": $this.find("input[name='notes']")[0].value,
                    });
                    lsObjectMeal.push(object);
                }
            });
            var lsObjecService = [];
            $("#service-item tr[ name='detailService']").each(function () {
                $this = $(this);
                if ($this.find("select[name='serviceID']")[0].value != "0") {
                    var object = new Object({
                        "PartyServiceID": $this.find("input[name='partyServiceID']").length > 0 ? $this.find("input[name='partyServiceID']")[0].value : 0,
                        "PartyID": $('#partyID').length > 0 ? $('#partyID').val() : 0,
                        "ServiceID": $this.find("select[name='serviceID']")[0].value,
                        "Quantity": $this.find("input[name='quantity']")[0].value.replace(/,/gi, ''),
                        "UnitPrice": $this.find("input[name='unitPrice']")[0].value.replace(/,/gi,''),
                        "Notes": $this.find("input[name='notes']")[0].value,
                    });
                    lsObjecService.push(object);
                }
            });

            $.ajax({
                url: "/Party/AddOrUpdateEntity",
                type: 'post',
                datatype: 'json',
                contentType: 'application/json',
                data: JSON.stringify({
                    "objectParty": objectParty,
                    "lsObjectMeal": lsObjectMeal,
                    "lsObjecService": lsObjecService,
                    "isEdit": $('#partyID').length > 0 ? true : false,
                }),
                async: false,
                cache: false,
                success: function (data) {
                    if (data == "0") {
                        toastr["error"]("Thêm món ăn thất bại.", "Thông báo")
                        flag = false;
                    }
                    else {
                        toastr["success"]("Thêm món ăn thành công.", "Thông báo")
                        window.location.href = "/Party/PartyScheduler";
                    }
                }
            })
        }else
        {
            
        }
    });
});