$(function () {
    //ini control
    var tableMeal;
    tableMeal = $('#meal-item').DataTable({
        "processing": false,
        "serverSide": false,
        "paging": false,
        "ordering": false,
        "info": false,
        "searching": false,
        "bAutoWidth": false,
        "columnDefs": [
            { "width": "20%", "targets": 0 },
            { "width": "15%", "targets": 1 },
            { "width": "15%", "targets": 2 },
            { "width": "15%", "targets": 3 },
            { "width": "30%", "targets": 4 },
            { "width": "5%", "targets": 5, "class": "dt-body-center" }
        ],
        "drawCallback": function (settings) {
            init_controlInputMarskNumber("#meal-item input[name='quantity']");
            init_controlInputMarskNumber("#meal-item input[name='unitCost']");
            init_controlInputMarskNumber("#meal-item input[name='profitAmount']");
            if ($("#meal-item input[name='quantity']").length && $("#meal-item input[name='unitCost']").length && $("#meal-item input[name='profitAmount']").length) {
                $("#sumUnitCostMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='unitCost']"], 1), 0, '', ','));
                $("#sumProfitAmountMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='profitAmount']"], 1), 0, '', ','));
                sumMeal();
            }
            $("#meal-item select[name='productID']").select2({ dropdownAutoWidth: true });
            $("#meal-item select[name='productID']").on('change', function (evt) { 
                $this = $(this);
                var gv;
                var profitAmount;
                if ($this.val() == 0)
                {
                    gv = 0;
                    profitAmount = 0;
                } else
                {
                    $.ajax({
                        url: '/Party/GetUnitCostAndProfitAmountBaseProductID',
                        type: 'post',
                        datatype: 'json',
                        contentType: 'application/json',
                        data: JSON.stringify({
                            id: $this.val()
                        }),
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data != null) {
                                gv = data[0].GV;
                                profitAmount = data[0].ProfitAmount;
                            }
                        }
                    })
                }
                $this.parent().parent().find("input[name='unitCost']").val(gv == undefined ? 0 : ( gv > 9999999999 ? 9999999999 : gv) );
                $this.parent().parent().find("input[name='profitAmount']").val(profitAmount == undefined ? 0 : profitAmount);
                $("#sumUnitCostMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='unitCost']"], 1), 0, '', ','));
                $("#sumProfitAmountMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='profitAmount']"], 1), 0, '', ','));
                sumMeal();
            });
            $("#meal-item input[name='quantity']").focusout(function () {
                $("#sumUnitCostMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='unitCost']"], 1), 0, '', ','));
                $("#sumProfitAmountMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='profitAmount']"], 1), 0, '', ','));
                sumMeal();
            });
            $("#meal-item input[name='unitCost']").focusout(function () {
                $("#sumUnitCostMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='unitCost']"], 1), 0, '', ','));
                sumMeal();
            });
            $("#meal-item input[name='profitAmount']").focusout(function () {
                $("#sumProfitAmountMeal").text($.number(calculateSumItemDetailTable("meal-item", ["input[name='quantity']", "input[name='profitAmount']"], 1), 0, '', ','));
                sumMeal();
            });
        }
    });
    //if (!$("#partyID").length) {
    //    addNewRowDetail(tableMeal, "newRowMeal");
    //}
    $('#addNewRowMeal').click(function () {
        addNewRowDetail(tableMeal, "newRowMeal");
    });
    $('#meal-item tbody').on('click', "tr th button[name='addRemoveRow']", function () {
        tableMeal.row($(this).closest('tr')).remove().draw();
    });
});
function sumMeal() {
    var sumUnitCostMeal = parseInt($("#sumUnitCostMeal").text() == "" ? "0" : $("#sumUnitCostMeal").text().replace(/,/gi, ''));
    var sumProfitAmountMeal = (parseInt($("#sumProfitAmountMeal").text() == "" ? "0" : $("#sumProfitAmountMeal").text().replace(/,/gi, '')));
    $("#sumMeal").text($.number((sumUnitCostMeal + sumProfitAmountMeal), 0, '', ','));
    $("#pricePerTablePlan").val($.number((sumUnitCostMeal + sumProfitAmountMeal) > 9999999999 ? 9999999999 : (sumUnitCostMeal + sumProfitAmountMeal), 0, '', ','));
}