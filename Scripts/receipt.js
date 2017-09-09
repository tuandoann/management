//ĐỊNH DẠNG NGÀY
function getFormattedDate(date) {
    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return day + '/' + month + '/' + year;
}
//BẢNG/TABLE
function loadDatable(table, fromDate, toDate) {
    table = $('#datatable-item').DataTable({
        "processing": true, // for show processing bar
        "serverSide": true, // for process on server side
        "orderMulti": false, // for disable multi column order
        "iDisplayLength": 20,
        "lengthMenu": [[10, 20, 50, 100], [10, 20, 50, 100]],
        "info": false,
        "language": {
            "url": languageDatatable
        },
        "ajax": {
            "url": "/Receipt/GetReceiptDatatableIndex",
            "type": "POST",
            "datatype": "json",
            "data": {
                "fromDate": fromDate,
                "toDate": toDate
            }
        },
        "columns": [
            {
                "orderable": false, "data": null, "width": "10%", "className": 'select-checkbox dt-head-center',
                "render": function (data) {
                    return "";
                }
            },
           { "data": "ReceiptNo", "name": "ReceiptNo", "width": "20%", "orderable": true, "class": " dt-head-center ", "class": "dt-body-center", },
           {
               "data": "ReceiptDate", "name": "ReceiptDate", "width": "15%", "orderable": true, "class": " dt-head-center ", "class": "dt-body-center",
               "render": function (data) {
                   return (data.split('/')[0].length == 1 ? "0" + data.split('/')[0] : data.split('/')[0]) + "/" + (data.split('/')[1].length == 1 ? "0" + data.split('/')[1] : data.split('/')[1]) + "/" + data.split('/')[2];
               }
           },
          { "data": "CustomerName", "name": "CustomerName", "width": "30%", "orderable": true, "class": " dt-head-center ", "class": "dt-body-left", },
          { "data": "Amount", "name": "Amount", "width": "15%", "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '', 0, '') },
          {
              "targets": -1,
              "data": null,
              "width": "10%",
              "orderable": false,
              "class": "dt-body-center",
              "render": function (data) {
                  return "<button class=' btn btn-info btn-xs' type='button' name='edit'>Sửa</button><button class=' btn btn-danger btn-xs' name='delete' >Xóa</button>";
              }
          }
        ],
        order: [[1, 'asc']],
        select: {
            style: 'multi',
            blurable: true,
            selector: 'td:first-child'
        },
    });
    return table;
}

$(function () {
    var table;

    $("#selectAll").iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });
    var date = new Date();
    var dateEdit = $('#dateReceipt').val();

    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);

    //ngay trong create/edit
    $('#dateReceipt').daterangepicker({
        "singleDatePicker": true,
        "startDate": dateEdit == "" ? date : dateEdit,
        "singleClasses": "picker_3",
        "locale": {
            "format": "DD/MM/YYYY",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    }, function (start, end, label) {
    });

    //ngay di trong index
    $('#fromDate').daterangepicker({
        "singleDatePicker": true,
        "startDate": new Date(),
        "startDate": firstDay,
        "singleClasses": "picker_3",
        "locale": {
            "format": "DD/MM/YYYY",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    }, function (start, end, label) {
    });
    //ngay den trong index
    $('#toDate').daterangepicker({
        "singleDatePicker": true,
        "startDate": lastDay,
        "singleClasses": "picker_3",
        "locale": {
            "format": "DD/MM/YYYY",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    }, function (start, end, label) {
    });

    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../Receipt/Index";
        }
    });

    //Tìm trong bảng
    //$("#searchFrom").click(function () {
    //    table.destroy();
    //    table = loadDatable(table, $('#fromDate').val(), $('#toDate').val());
    //});
    $("#searchFrom").click(function () {
        var DateFrom = new Date($('#fromDate').val().split('/')[2], ($('#fromDate').val().split('/')[1] - 1), $('#fromDate').val().split('/')[0]);
        var DateTo = new Date($('#toDate').val().split('/')[2], ($('#toDate').val().split('/')[1] - 1), $('#toDate').val().split('/')[0]);
        if (DateFrom.getTime() > DateTo.getTime()) {
            $("#fromDate").trigger('click');
            toastr["error"]("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.", "Lỗi");
        }
        else {
            table.destroy();
            table = loadDatable(table, $('#fromDate').val(), $('#toDate').val());
            $("#searchFrom").trigger('submit');
        }
    })

    //Xóa dữ liệu
    if ($('#datatable-item').length) {


        $("#deleteMany").click(function () {
            if (confirm("Bạn có muốn xóa tất cả phần tử được chọn!")) {
                var itemArray = table.rows({
                    selected: true
                }).data();
                if (itemArray.length > 0) {
                    var lsIdItem = [];
                    $.each(itemArray, function (index, value) {
                        lsIdItem.push(value.ReceiptID);
                    });
                    $.ajax({
                        url: '/Receipt/deleteMany',
                        type: 'post',
                        datatype: 'json',
                        contentType: 'application/json',
                        data: JSON.stringify({
                            "lsIdItem": lsIdItem
                        }),
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data == "1") {
                                toastr["success"]("Xóa phần tử được chọn thành công.", "Thông báo");
                                table.ajax.reload();
                            }
                            else {
                                toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại. Lỗi " + data, "Lỗi");
                            }
                        }
                    })
                }
                else {
                    toastr["success"]("Vui lòng chọn ít nhất 1 phần tử.", "Thông báo");
                }
            }
        });

        $('#selectAll').on('ifChanged', function (event) {
            if (event.target.checked)
                table.rows().select();
            else
                table.rows().deselect();
        });

        table = loadDatable(table, getFormattedDate(firstDay), getFormattedDate(lastDay));
        $('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                var data = table.row($(this).closest('tr')).data();
                $.ajax({
                    url: '/Receipt/delete',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: data.ReceiptID,
                    },
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "1") {
                            toastr["success"]("Xóa phần tử thành công.", "Thông báo");
                            table.row($(this).closest('tr')).remove().draw();
                        }
                        else {
                            toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại.", "Lỗi");
                        }
                    }
                })
            }
        });
        $('#datatable-item tbody').on('click', "tr td button[name='edit']", function () {
            var data = table.row($(this).closest('tr')).data();
            window.location.href = "/Receipt/Edit?id=" + data.ReceiptID;
        });
    }
    else {
        //ding dang 
        $('#partyID').select2({ dropdownAutoWidth: true });
        $('#employeeID').select2({ dropdownAutoWidth: true });
        $("#amount").inputmask("999,999,999,999", { alias: "number", numericInput: true, integerOptional: true, showMaskOnFocus: true, jitMasking: true, removeMaskOnSubmit: true, rightAlign: true });
        //Gui đi, luu lại
        $("#sumbitbutton").click(function () {
            if (validator.checkAll($("form"), true)) {
                $.ajax({
                    url: "/Receipt/CheckReceiptNo",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "receiptNo": $('#receiptNo').val(),
                        "receiptID": ($('#receiptID').length > 0 ? $('#receiptID').val() : 0),
                        "isEdit": ($('#receiptID').length > 0 ? true : false)
                    }),
                    async: true,
                    cache: false,
                    success: function (data) {
                        if (data == false) {
                            addOrUpdateEntity();
                        }
                        else {
                            toastr["error"]("Trùng phiếu thu xin vui lòng nhập lại.", "Thông báo");
                            $("#receiptNo").select();
                        }
                    }
                })

            }
        });
    }
});
function addOrUpdateEntity() {
    $.ajax({
        url: "/Receipt/AddOrUpdateEntity",
        type: 'post',
        datatype: 'json',
        contentType: 'application/json',
        data: JSON.stringify({
            "item": new Object({
                "ReceiptID": $('#receiptID').length > 0 ? $('#receiptID').val() : 0,
                "ReceiptDate": $('#dateReceipt').val(),
                "ReceiptNo": $('#receiptNo').val(),
                "PartyID": $('#partyID').val(),
                "Amount": $('#amount').val().replace(/,/gi, ''),
                "EmployeeID": $('#employeeID').val(),
                "Notes": $('#notesReceipt').val(),
            }),
            "isEdit": $('#receiptID').length > 0 ? true : false,
            "receiptDate": $('#dateReceipt').val(),
        }),
        async: true,
        cache: false,
        success: function (data) {
            if (data == "0") {
                toastr["error"]("Phiếu thu lưu thất bại.", "Thông báo")
                flag = false;
            }
            else {
                toastr["success"]("Phiếu thu lưu thành công.", "Thông báo")
                window.location.href = "/Receipt/Index";
            }
        }
    })
}

