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
        "order": [[0, 'asc']],
        "columns": [
            { "data": "GroupName", "name": "GroupName", "width": "30%", "orderable": true },
            { "data": "ProductName", "name": "ProductName", "width": "30%" },
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
                            toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại.", "Lỗi");
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
        //init_controlInputMarskNumber("#profitAmount");
        tableDetail = $('#material-item').DataTable({
            "processing": false, 
            "serverSide": false, 
            "paging":   false,
            "ordering": false,
            "info": false,
            "searching": false,
            "columnDefs": [
                { "width": "31%", "targets": 0 },
                { "width": "31%", "targets": 1 },
                { "width": "33%", "targets": 2},
                { "width": "5%", "targets": 3, "class": "dt-body-center" }
            ],
            "drawCallback": function (settings) {
                init_controlInputMarskNumber("input[name='quantityDetail']");
                //$("#material-item select[name='materialDetail']").select2({ dropdownAutoWidth: true });
                $("#material-item select[name='materialDetail']").select2();
                $("#material-item select[name='dvtDetail']").select2();
                //$("input[name='quantityDetail']").inputmask("999,999,999,999", { alias: "number", numericInput: true, integerOptional: true, showMaskOnFocus: true, jitMasking: true, removeMaskOnSubmit: true, rightAlign: true });
            }
        });
        if ($('#productID'))
        {
            addNewRowDetail(tableDetail, "newRow");
        }
        $('#addNewRow').click(function() {
            addNewRowDetail(tableDetail, "newRow");
        });
        $('#material-item tbody').on('click', "tr th button[name='addRemoveRow']", function () {
            tableDetail.row($(this).closest('tr')).remove().draw();
        });
        $("#sumbitbutton").click(function () {
            
            //$("form").valid();
            if (validator.checkAll($("form")))
            {
                var url = $('#productID').length > 0 ? "/Food/Edit" : "/Food/Create";
                var productDetail = [];
                $("tr[ name='detail']").each(function () {
                    $this = $(this);
                    if ($this.find("select[name='materialDetail']")[0].value != "") {
                        var object = new Object({
                            ProductMaterialID: $this.find("input[name='productMaterialID']")[0].value,
                            ProductID: $this.find("input[name='productID']").length > 0 ? $("input[name='productID']")[0].value : 0,
                            MaterialID: $this.find("select[name='materialDetail']")[0].value,
                            UOMID: $this.find("select[name='dvtDetail']")[0].value,
                            Quantity: $this.find("input[name='quantityDetail']")[0].value.replace(',', '')
                        });
                        productDetail.push(object);
                    }
                });
                var para = JSON.stringify({
                    "product" : new Object({
                        "ProductID": $('#productID').length > 0 ? $('#productID').val() : 0,
                        "ProductGroupID": $('#productGroupID').val(),
                        "ProductName": $('#productName').val(),
                        "Notes": $('#notes').val(),
                        "ProfitAmount": $('#profitAmount').val().replace(',', ''),
                        "IsActive": document.querySelector('.js-switch').checked,
                    }),
                    "productDetail": productDetail
                });
            
                $.ajax({
                    url: url,
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: para,
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "0") {
                            toastr["error"]("Thêm món ăn thất bại.", "Thông báo")
                            flag = false;
                        }
                        else {
                            toastr["success"]("Thêm món ăn thành công.", "Thông báo")
                            window.location.href = "/Food/Index";
                        }
                    }
                })
            }
        });
}
});