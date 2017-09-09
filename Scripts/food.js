function loadDatatable(table) {
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
        "columnDefs": [
            { "visible": false, "targets": 0 }
        ],
        "ajax": {
            "url": "/Food/GetFoodDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },
        "formatNumber": function (toFormat) {
            return toFormat.toString().replace(
              /\B(?=(\d{3})+(?!\d))/g, "'"
            )
        },
        "columns": [
            { "data": "GroupName", "name": "GroupName", "width": "30%", "orderable": true },
            {
                "orderable": false, "data": null, "className": 'select-checkbox dt-head-center',
                "render": function (data) {
                    return "";
                }
            },
            { "data": "ProductName", "name": "ProductName", "width": "60%" },
             { "data": "ProfitAmount", "name": "ProfitAmount", "width": "10%", "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '', 0, '') },
              {
                  "data": "IsActive", "name": "IsActive", "width": "10%", "class": "dt-body-center",
                  "render": function (data) {
                      return "<th><input type='checkbox' name='isActive' " + (data == true ? "checked" : "") + " disabled /></th>";
                  }
              },
            {
                "targets": -1,
                "data": null,
                "width": "10%",
                "orderable": false,
                "class": "dt-body-center",
                "render": function (data) {
                    return "<button class=' btn btn-info btn-xs' name='edit'>Sửa</button><button class=' btn btn-danger btn-xs' name='delete' >Xóa</button>";
                }
            }
        ],
        order: [[0, 'asc']],
        select: {
            style: 'multi',
            blurable: true,
            selector: 'td:first-child'
        },
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;

            api.column(0, { page: 'current' }).data().each(function (group, i) {
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group"><td colspan="5"><h4><strong>' + group + '</strong></h4></td></tr>'
                    );
                    last = group;
                }
            });
            $("input[name='isActive']").iCheck({
                checkboxClass: 'icheckbox_flat-green',
                radioClass: 'iradio_flat-green'
            });
        }
    });
    return table;
}

$(function () {
    var table;
    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../Food/Index";
        }
    });
    if ($('#datatable-item').length) {
        $("#selectAll").iCheck({
            checkboxClass: 'icheckbox_flat-green',
            radioClass: 'iradio_flat-green'
        });
        $("#deleteMany").click(function () {
            if (confirm("Bạn có muốn xóa tất cả phần tử được chọn!")) {
                var itemArray = table.rows({ selected: true }).data();
                if (itemArray.length > 0) {
                    var lsIdItem = [];
                    $.each(itemArray, function (index, value) {
                        lsIdItem.push(value.ProductID);
                    });
                    $.ajax({
                        url: '/Food/deleteMany',
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
        table = loadDatatable(table);
        // Order by the grouping
        $('#datatable-item tbody').on('click', 'tr.group', function () {
            var currentOrder = table.order()[0];
            if (currentOrder[0] === 0 && currentOrder[1] === 'asc') {
                table.order([0, 'desc']).draw();
            }
            else {
                table.order([0, 'asc']).draw();
            }
        });
        $('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                var data = table.row($(this).closest('tr')).data();
                $.ajax({
                    url: '/Food/delete',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: data.ProductID
                    },
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "1") {
                            toastr["success"]("Xóa phần tử thành công.", "Thông báo");
                            table.row($(this).closest('tr')).remove().draw();
                        }
                        else {
                            toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại. Lỗi " + data, "Lỗi");
                        }
                    }
                })
            }
        });
        $('#datatable-item tbody').on('click', "tr td button[name='edit']", function () {
            var data = table.row($(this).closest('tr')).data();
            window.location.href = "../Food/Edit?id=" + data.ProductID;
        });
    } else {
        var tableDetail;
        $('#productGroupID').select2({ dropdownAutoWidth: true });
        $("#profitAmount").inputmask("999,999,999,999", { alias: "number", numericInput: true, integerOptional: true, showMaskOnFocus: true, jitMasking: true, removeMaskOnSubmit: true, rightAlign: true });
        tableDetail = $('#material-item').DataTable({
            "processing": false,
            "serverSide": false,
            "paging": false,
            "ordering": false,
            "info": false,
            "searching": false,
            "columnDefs": [
                { "width": "31%", "targets": 0 },
                { "width": "31%", "targets": 1 },
                { "width": "33%", "targets": 2 },
                { "width": "5%", "targets": 3, "class": "dt-body-center" }
            ],
            "drawCallback": function (settings) {
                $("input[name='quantityDetail']").autoNumeric('init', { vMin: -10000, mDec: 4 });//them autoNumeric
                $("#material-item select[name='materialDetail']").select2();
                $("#material-item select[name='dvtDetail']").select2();
                $("#material-item select[name='materialDetail']").on('change', function (evt) { //them  function get uom ID
                    $this = $(this);
                    $.ajax({
                        url: '/Material/GetUOMID',
                        type: 'post',
                        datatype: 'json',
                        data: {
                            id: $(this).val()
                        },
                        async: false,
                        cache: false,
                        success: function (data) {
                            $this.parent().parent().find("select[name='dvtDetail']").val(data).trigger('change');
                        }
                    })
                });
            }

        });
        $('#addNewRow').click(function () {
            addNewRowDetail(tableDetail, "newRow");
        });
        $('#material-item tbody').on('click', "tr th button[name='addRemoveRow']", function () {
            tableDetail.row($(this).closest('tr')).remove().draw();
        });
        $("#sumbitbutton").click(function () {
            if (validator.checkAll($("form"))) {
                var productDetail = [];
                $("tr[ name='detail']").each(function () {
                    $this = $(this);
                    if ($this.find("select[name='materialDetail']")[0].value != "0") {
                        var object = new Object({
                            ProductMaterialID: $this.find("input[name='productMaterialID']")[0].value,
                            // ProductID: $this.find("input[name='productID']").length > 0 ? $("input[name='productID']")[0].value : 0,
                            ProductID: $("#productID").val() == null ? 0 : $("#productID").val(),
                            MaterialID: $this.find("select[name='materialDetail']")[0].value,
                            UOMID: $this.find("select[name='dvtDetail']")[0].value,
                            Quantity: $this.find("input[name='quantityDetail']")[0].value.replace(/,/gi, '')
                        });
                        productDetail.push(object);
                    }
                });

                $.ajax({
                    url: "/Food/AddOrUpdateEntity",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "product": new Object({
                            "ProductID": $('#productID').length > 0 ? $('#productID').val() : 0,
                            "ProductGroupID": $('#productGroupID').val(),
                            "ProductName": $('#productName').val(),
                            "Notes": $('#notes').val(),
                            "ProfitAmount": $('#profitAmount').val().replace(/,/gi, ''),
                            "IsActive": document.querySelector('.js-switch').checked,
                        }),
                        "productDetail": productDetail,
                        "isEdit": $('#productID').length > 0 ? true : false
                    }),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "0") {
                            toastr["error"]("Món ăn lưu thất bại.", "Thông báo")
                            flag = false;
                        }
                        else {
                            toastr["success"]("Món ăn lưu thành công.", "Thông báo")
                            window.location.href = "/Food/Index";
                        }
                    }
                })
            }
        });
    }
});