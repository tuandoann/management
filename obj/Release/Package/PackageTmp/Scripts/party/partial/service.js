$(function () {
    //ini control
    var tableService = $('#service-item').DataTable({
        "processing": false,
        "serverSide": false,
        "paging": false,
        "ordering": false,
        "info": false,
        "searching": false,
        "bAutoWidth": false,
        "columnDefs": [
            { "width": "25%", "targets": 0 },
            { "width": "15%", "targets": 1 },
            { "width": "15%", "targets": 2 },
            { "width": "40%", "targets": 3 },
            { "width": "5%", "targets": 4, "class": "dt-body-center" }
        ],
        "drawCallback": function (settings) {
            init_controlInputMarskNumber("#service-item input[name='quantity']");
            init_controlInputMarskNumber("#service-item input[name='unitPrice']");
            $("#service-item select[name='serviceID']").select2({ dropdownAutoWidth: true });
            $("#service-item select[name='serviceID']").on('change', function (evt) {
                $this = $(this);
                $.ajax({
                    url: '/Service/GetUnitPrice',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: $(this).val()
                    },
                    async: false,
                    cache: false,
                    success: function (data) {
                        $this.parent().parent().find("input[name='unitPrice']").val(data);
                    }
                })
            });
        }
    });
    if (!$("#partyID").length) {
        addNewRowDetail(tableService, "newRowService");
    }
    
    $('#addNewRowService').click(function () {
        addNewRowDetail(tableService, "newRowService");
    });
    $('#service-item tbody').on('click', "tr th button[name='addRemoveRow']", function () {
        tableService.row($(this).closest('tr')).remove().draw();
    });
});