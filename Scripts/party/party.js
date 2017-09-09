function validatorPartyForm() {
    var flagValidator = false;
    if ($('#bookingDate').val() == "") {
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
    if ($('#negativeDate').val() == "" && !$('#negativeDate').text().isDate()) {
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
                $this.find("select[name='productID']").select2('open');
            }
            flagValidator = true;
            toastr["error"]("Vui lòng nhập món ăn.", "Thông báo");
            return false;
        }
    });

    //$("#service-item tr[ name='detailService']").each(function () {
    //    $this = $(this);
    //    if ($this.find("select[name='serviceID']")[0].value == "0") {
    //        if (!flagValidator) {
    //            $('a[href="#service"]').trigger('click');
    //            $this.find("select[name='serviceID']").select2('open');
    //        }
    //        flagValidator = true;
    //        toastr["error"]("Vui lòng nhập dịch vụ.", "Thông báo");
    //        return false;
    //    }
    //});

    if ($('#depositDate').val() && $('#depositDate').text().isDate() ) {
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
    if ($('#partyFullCalendar').length > 0) {
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
                                    title: r.CustomerName + ' ' + (r.PartyAddress == null ? "" : r.PartyAddress),
                                    start: new Date(r.PartyDate),
                                    end: new Date(r.PartyDate).addHours(2),
                                });
                            });
                        }
                        callback(events);
                    }
                });
            }
        })
    }
    $("#deleteForm").click(function () {
        if (confirm("Bạn có chắc muốn xóa tiệc. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            $.ajax({
                url: "/Party/DeleteEntity",
                type: 'post',
                datatype: 'json',
                contentType: 'application/json',
                data: JSON.stringify({
                    "id": $("#partyID").val(),
                }),
                async: false,
                cache: false,
                success: function (data) {
                    if (data == "1") {
                        toastr["success"]("Xóa lịch tiệc thành công.", "Thông báo")
                        window.location.href = "/Party/PartyScheduler";
                    }
                    else {
                        toastr["success"]("Xóa lịch tiệc thất bại.", "Thông báo")
                    }
                }
            })
        }
    });
    //reset all form
    $("#resetAllForm").click(function () {
        $("form[name='partyFrom']").trigger('reset');
        $('#bookingDate').data('daterangepicker').setStartDate($('#bookingDate').val() ? new Date($('#bookingDate').val()) : new Date());
        $('#partyDate').data('daterangepicker').setStartDate($('#partyDate').val() ? new Date($('#partyDate').val().replace('_', ' ')) : new Date());
    });
    //cancel 
    $("#cancelAllForm").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../Party/PartyScheduler";
        }
    });
    //sumbit
    $("#sumbitAllForm").click(function () {
        if (validatorPartyForm()) {
            var objectParty = new Object({
                "PartyID": $('#partyID').length > 0 ? $('#partyID').val() : 0,
                //"BookingDate": $('#bookingDate').data('daterangepicker').startDate._d,
                //"PartyDate": $('#partyDate').data('daterangepicker').endDate._d,
                //"NegativeDate": new Date($('#negativeDate').val().split('/')[2], $('#negativeDate').val().split('/')[1] - 1, $('#negativeDate').val().split('/')[0]),
                "BookingDate": Date.now,
                "PartyDate": Date.now,
                "NegativeDate": Date.now,
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
                "DepositAmount": $('#depositAmount').val().replace(/,/gi, ''),
                //"DepositDate": $('#depositDate').val().split.length >=3 ? new Date($('#depositDate').val().split('/')[2], $('#depositDate').val().split('/')[1] - 1, $('#depositDate').val().split('/')[0]) : null ,
                "DepositDate": Date.now,
                "OtherAmount": $('#otherAmount').val().replace(/,/gi, ''),
                "IsPayCashAfterDoneParty": isPayCashAfterDoneParty.checked,
                "IsPayBank": isPayBank.checked,
                "IsVAT": isVAT.checked,
                "Notes": $('#notes').val(),
                "UserCreate": 0
            });
            var bookingDate = $('#bookingDate').data('daterangepicker').startDate._d;
            var partyDate = $('#partyDate').data('daterangepicker').endDate._d;
            var negativaDate = $('#negativeDate').val().split('/');
            var depositDate = $('#depositDate').val().split('/')
            var objectDate = [[bookingDate.getDate(), bookingDate.getMonth() + 1, bookingDate.getFullYear()],
                [partyDate.getDate(), partyDate.getMonth() + 1, partyDate.getFullYear(), partyDate.getHours(), partyDate.getMinutes()],
                 [negativaDate[0], negativaDate[1], negativaDate[2]],
                (depositDate.length >= 3 ? [depositDate[0], depositDate[1], depositDate[2]] : []),
            ];
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
                        "UnitCost": $this.find("input[name='unitCost']")[0].value.replace(/,/gi, ''),
                        "ProfitAmount": $this.find("input[name='profitAmount']")[0].value.replace(/,/gi, ''),
                        
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
                        "UnitPrice": $this.find("input[name='unitPrice']")[0].value.replace(/,/gi, ''),
                        "Notes": $this.find("input[name='notes']")[0].value,
                    });
                    lsObjecService.push(object);
                }
            }); 
            
            if (lsObjectMeal.length == 0)
            {
                toastr["error"]("Vui lòng thêm món ăn.", "Thông báo");
                $('a[href="#meal"]').trigger('click');
                return;
            }
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
                    "objectDate": objectDate,
                }),
                async: false,
                cache: false,
                success: function (data) {
                    if (data == "0") {
                        toastr["error"]("Thêm lịch tiệc thất bại.", "Thông báo")
                        flag = false;
                    }
                    else {
                        toastr["success"]("Thêm lịch tiệc thành công.", "Thông báo")
                        window.location.href = "/Party/PartyScheduler";
                    }
                }
            })
        }
    });
});