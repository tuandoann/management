$(function () {
    //ini control
    init_controlInputMarskNumber("#moneyAndPayMethod input[name='pricePerTablePlan']");
    init_controlInputMarskNumber("#moneyAndPayMethod input[name='pricePerTableReal']");
    init_controlInputMarskNumber("#moneyAndPayMethod input[name='otherAmount']");
    init_controlInputMarskNumber("#moneyAndPayMethod input[name='depositAmount']");
    $('#depositDate').daterangepicker({
        "singleDatePicker": true,
        singleClasses: "picker_3",
        autoUpdateInput : false,
        startDate: $('#depositDate').val() ? new Date($('#depositDate').val().split('/')[1] + "/" +$('#depositDate').val().split('/')[0] + "/" + $('#depositDate').val().split('/')[2]) : new Date(),
        "locale": {
            "format": "DD/MM/YYYY",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    }, function (start, end, label) {
        debugger;
    });
    $('#depositDate').on('apply.daterangepicker', function (ev, picker) {
        $('#depositDate').val(picker.startDate._d.getDate() + "/" + (picker.startDate._d.getMonth() + 1) + "/" + picker.startDate._d.getFullYear());
    });
    $('#calculator_pricePerTablePlan').click(function () {
        if (confirm("Bạn có đồng ý tính lại không.")) {
            $("#meal-item tr[ name='detailMeal']").each(function () {
                $this = $(this);
                if ($this.find("select[name='productID']")[0].value != "0") {
                    $.ajax({
                        url: '/Party/GetUnitCostAndProfitAmountBaseProductID',
                        type: 'post',
                        datatype: 'json',
                        contentType: 'application/json',
                        data: JSON.stringify({
                            id: $this.find("select[name='productID']")[0].value
                        }),
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data != null) {
                                $this.find("input[name='unitCost']").val(data[0].GV == undefined ? 0 : data[0].GV);
                            }
                        }
                    })
                }
            });
            $("#sumUnitCostMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='unitCost']"], 1), 0, '', ','));
            sumMeal();
        }
    });
    var isPayCashAfterDoneParty = document.querySelector('#isPayCashAfterDoneParty');
    var switchIsPayCashAfterDoneParty = new Switchery(isPayCashAfterDoneParty);

    var isPayBank = document.querySelector('#isPayBank');
    var switchIsPayBank = new Switchery(isPayBank);

    var isVAT = document.querySelector('#isVAT');
    var switchIsVAT = new Switchery(isVAT);
});