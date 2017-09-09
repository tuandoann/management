$(function () {
    var partyDate;
    //ini control
    $("#partyType").select2({ dropdownAutoWidth: true });
    init_controlInputMarskNumber("#numberTablePlan");
    init_controlInputMarskNumber("#numberTableReal");
    init_controlInputMarskNumber("#numberTableException");
    init_controlInputMarskNumber("#numberTableVegetarian");
    init_controlInputMarskNumber("#priceVegetarian");
    debugger;
    $('#bookingDate').daterangepicker({
        "singleDatePicker": true,
        singleClasses: "picker_3",
        startDate :$('#bookingDate').val() ?  new Date($('#bookingDate').val()) : new Date(),
        "locale": {
            "format": "DD/MM/YYYY",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    }, function (start, end, label) {
        debugger;
    });
    $('#partyDate').daterangepicker({
        "singleDatePicker": true,
        startDate: $('#partyDate').val() ? new Date($('#partyDate').val().replace('_',' ')) : new Date(),
        "timePicker": true,
        singleClasses: "picker_2",
        "locale": {
            "format": "DD/MM/YYYY HH:mm",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    }, function (start, end, label) {
        debugger;
    });
    if (!$('#negativeDate').val())
    {
        $('#partyDate').val("");
    }
    $('#partyDate').on('apply.daterangepicker', function (ev, picker) {
        var array = convertSolar2Lunar(picker.startDate._d.getDate(), picker.startDate._d.getMonth() + 1, picker.startDate._d.getFullYear(), 7);
        $('#negativeDate').val((array[0].toString().length == 1 ? "0" + array[0].toString() : array[0].toString()) + "/" + (array[1].toString().length == 1 ? "0" + array[1].toString() : array[1].toString()) + "/" + array[2]);
    });
})