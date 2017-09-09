$(function () {
    var countCaLam = 2;
    var htmlCa1 = returnHtmlCalLam(1);
    var htmlCa2 = returnHtmlCalLam(2);
    $("div[name='lableCaLam']").last().after(htmlCa2);
    $("div[name='lableCaLam']").last().after(htmlCa1);
    $(".timepicki-input").inputmask("99:99");
    $('#employee').select2({ dropdownAutoWidth: true });
    $("#addNewRow").click(function () {
        countCaLam += 1;
        var htmlCa = returnHtmlCalLam(countCaLam);
        $("div[name='caLam']").last().after(htmlCa);
        $(".timepicki-input").inputmask("99:99");
        $("#caLamTu" + countCaLam).select();
    });
    $("#resetButton").click(function () {
        countCaLam = 2;
        $("#employee").select2().select2('val', '0');
        $("div[name='caLam']").remove();
        var htmlCa1 = returnHtmlCalLam(1);
        var htmlCa2 = returnHtmlCalLam(2);
        $("div[name='lableCaLam']").last().after(htmlCa2);
        $("div[name='lableCaLam']").last().after(htmlCa1);
        $('#sumSalary').html('');
        $(".timepicki-input").inputmask("99:99");
        $('#saveButton').prop("disabled", true);
    });
    $("#calculatorButton").click(function () {
        var sumSalary = 0;
        var allInputEmnty = true;
        if ($("#employee").val() == 0)
        {
            toastr["error"]("Vui lòng lựa chọn nhân viên.", "Thông báo");

            return;
        }
        $("div[ name='caLam']").each(function () {
            $this = $(this);
            var dateFrom = $this.find("input[name='caLamTu']")[0].value;
            var dateTo = $this.find("input[name='caLamDen']")[0].value;
            if (dateFrom == "" && dateTo != "") {
                toastr["error"]("Vui lòng nhập thời gian tới.", "Thông báo");
                $this.find("input[name='caLamTu']")[0].select();
                return;
            }
            if (dateFrom != "" && dateTo == "") {
                toastr["error"]("Vui lòng nhập thời gian đến.", "Thông báo");
                $this.find("input[name='caLamDen']")[0].select();
                return;
            }
            if (dateFrom != "" && dateTo != "")
                allInputEmnty *= false;
        });
        if (allInputEmnty)
        {
            toastr["error"]("Vui lòng nhập ít nhất một khoảng thời gian đến và thời gian đi.", "Thông báo");
            $("#caLamTu1").select();
            return;
        }
        $("div[ name='caLam']").each(function () {
            $this = $(this);
            var dateFrom = $this.find("input[name='caLamTu']")[0].value;
            var dateTo = $this.find("input[name='caLamDen']")[0].value;
            if (dateFrom != "" && dateTo != "") {
                $.ajax({
                    url: "/FastTimekeeping/CalculatorSalaryShift",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "dateFrom": dateFrom,
                        "dateTo": dateTo
                    }),
                    async: false,
                    cache: false,
                    success: function (data) {
                        $this.find("label[name='salary']")[0].innerText = $.number(data, 0, '', ',');
                        sumSalary += data;
                    }
                });
            }
        });
        $('#sumSalary').html($.number(sumSalary, 0, '', ','));
        $('#saveButton').prop("disabled", false);
    });
    $("#saveButton").click(function () {
        //validate before insert entity
        if ($('#employee').val() == 0)
        {
            toastr["error"]("Vui lòng chọn nhân viên.", "Thông báo");
            $('#employee').select2('open');
            return;
        }
        $("div[ name='caLam']").each(function () {
            $this = $(this);
            var dateFrom = $this.find("input[name='caLamTu']")[0].value;
            var dateTo = $this.find("input[name='caLamDen']")[0].value;
            if (dateFrom == "" && dateTo != "") {
                toastr["error"]("Vui lòng nhập thời gian tới.", "Thông báo");
                $this.find("input[name='caLamTu']")[0].select();
                return;
            }
            if (dateFrom != "" && dateTo == "") {
                toastr["error"]("Vui lòng nhập thời gian đến.", "Thông báo");
                $this.find("input[name='caLamDen']")[0].select();
                return;
            }
        });
        var lsEntity = [];
        var lsCheckIn = [];
        var lsCheckOut = [];
        $("div[ name='caLam']").each(function () {
            $this = $(this);
            var dateFrom = $this.find("input[name='caLamTu']")[0].value;
            var dateTo = $this.find("input[name='caLamDen']")[0].value;
            if (dateFrom != "" && dateTo != "") {
                var object = new Object({
                    FastPayRollID : 0,
                    EmployeeID: $("#employee").val(),
                    CheckIn: new Date(),
                    CheckOut: new Date(),
                    Amount: $this.find("label[name='salary']")[0].innerText.replace(/,/gi, ''),
                });
                lsCheckIn.push(dateFrom);
                lsCheckOut.push(dateTo);
                lsEntity.push(object);
            }
        });

        $.ajax({
            url: "/FastTimekeeping/AddOrUpdateListEntity",
            type: 'post',
            datatype: 'json',
            contentType: 'application/json',
            data: JSON.stringify({
                "lsEntity": lsEntity,
                "lsCheckIn": lsCheckIn,
                "lsCheckOut": lsCheckOut
            }),
            async: true,
            cache: false,
            success: function (data) {
                if (data == "1") {
                    toastr["success"]("Dữ liệu đã được lưu.", "Thông báo")
                    flag = false;
                    $("#resetButton").trigger("click");
                    $("#caLamTu1").select();
                }
            }
        })
    });
    $(document).on('change paste keyup', '.timepicki-input', function () {
        $('#saveButton').prop("disabled", true);
    });
    $(document).on('focusout', '.timepicki-input', function () {
        if ($(this).val() != "") {
            var idElement = $(this).attr('id');
            var idAnother = idElement.indexOf("caLamTu") == -1 ? "caLamTu" + idElement.split('caLamDen')[1] : "caLamDen" + idElement.split('caLamTu')[1];
            var nameCaCurrent = "ca" + ( idElement.indexOf("caLamTu") == -1 ?  idElement.split('caLamDen')[1] + " thời gian kết thúc ": idElement.split('caLamTu')[1] + " thời gian bắt đầu" );
            var valueInput = $(this).val();
            var hour = valueInput.split(':')[0];
            var minute = valueInput.split(':')[1];
            if (hour == '__' && minute != '__') {
                toastr["error"]("Vui lòng nhập giờ.", "Thông báo");
                $('#calculatorButton').prop("disabled", true);
                return;
            }
            if (minute == '__' && hour != '__') {
                toastr["error"]("Vui lòng nhập phút " + nameCaCurrent, "Thông báo");
                $('#calculatorButton').prop("disabled", true);
                return;
            }
            if (hour > 23) {
                toastr["error"]("Giờ không đươc lớn hơn 23 " + nameCaCurrent, "Thông báo");
                $('#calculatorButton').prop("disabled", true);
                return;
            }
            if (minute > 60) {
                toastr["error"]("Phút không đươc lớn hơn 60 " + nameCaCurrent, "Thông báo");
                $('#calculatorButton').prop("disabled", true);
                return;
            }
            var valueAnother = $('#' + idAnother).val();
            if (valueAnother != "") {
                var dateCurrent = new Date(2000, 1, 1, hour, minute);
                var dateAnother = new Date(2000, 1, 1, valueAnother.split(':')[0], valueAnother.split(':')[1]);
                if ((idElement.indexOf("caLamTu") == -1 && dateCurrent < dateAnother) || (idElement.indexOf("caLamDen") == -1 && dateCurrent > dateAnother)) {
                    toastr["error"]("Thời gian đến phải lớn hơn thời gian từ " + nameCaCurrent, "Thông báo");
                    $('#calculatorButton').prop("disabled", true);
                    return;
                }
            }
            $('#calculatorButton').prop("disabled", false);
        }
    });

});
function returnHtmlCalLam(soCaLam) {
    return "<div class='form-group' name='caLam'>"
                     + "<label class='col-md-2 col-sm-2 col-xs-12'>Ca " + soCaLam + "</label>"
                     + "<div class='col-md-2 col-sm-2 col-xs-12'>"
                     + "<input type='text' name='caLamTu' id='caLamTu" + soCaLam + "' class='form-control timepicki-input'/>"
                     + "</div>"
                     + "<div class='col-md-2 col-sm-2 col-xs-12'>"
                     + "<input type='text' name='caLamDen' id='caLamDen" + soCaLam + "' class='form-control timepicki-input' />"
                     + "</div>"
                     + "<label class='col-md-2 col-sm-2 col-xs-12' name='salary' style='text-align:right'></label>"
                     + "</div>";
}