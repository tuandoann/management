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
            { "width": "30%", "targets": 0 },
            { "width": "15%", "targets": 1 },
            { "width": "50%", "targets": 2 },
            { "width": "5%", "targets": 3, "class": "dt-body-center" }
        ],
        "drawCallback": function (settings) {
            init_controlInputMarskNumber("#meal-item input[name='quantity']");
            $("#meal-item select[name='productID']").select2({ dropdownAutoWidth: true });
           }
    });
    if (!$("#partyID").length)
    {
        addNewRowDetail(tableMeal, "newRowMeal");
    }
    
    $('#addNewRowMeal').click(function () {
        addNewRowDetail(tableMeal, "newRowMeal");
    });
    $('#meal-item tbody').on('click', "tr th button[name='addRemoveRow']", function () {
        tableMeal.row($(this).closest('tr')).remove().draw();
    });
});