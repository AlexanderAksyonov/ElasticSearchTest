function selectView(view) {
    $('.display').not('#' + view + "Display").hide();
    $('#' + view + "Display").show();
}
function getData() {
    $.ajax({
        type: "GET",
        url: "/api/PersonAPI",
        success: function (data) {
            $('#tableBody').empty();
            for (var i = 0; i < data.length; i++) {
                $('#tableBody').append('<tr><td><input id="id" name="id" type="radio"'
                + 'value="' + data[i].Id + '" /></td>'
                + '<td>' + data[i].Name + '</td>'
                + '<td>' + data[i].Surname + '</td>'
                + '<td>' + data[i].DepartmentID + '</td></tr>');
            }
            $('input:radio')[0].checked = "checked";
            selectView("summary");
        }
    });
}
$(document).ready(function () {
    selectView("summary");
    getData();
    $("button").click(function (e) {
        var selectedRadio = $('input:radio:checked')
        switch (e.target.id) {
            case "refresh":
                getData();
                break;
            case "delete":
                $.ajax({
                    type: "DELETE",
                    url: "/api/PersonAPI/" + selectedRadio.attr('value'),
                    success: function (data) {
                        selectedRadio.closest('tr').remove();
                    }
                });
                break;
            case "add":
                selectView("add");
                break;
            case "edit":
                $.ajax({
                    type: "GET",
                    url: "/api/PersonAPI/" + selectedRadio.attr('value'),
                    success: function (data) {
                        $('#editId').val(data.Id);
                        $('#editName').val(data.Name);
                        $('#editSurname').val(data.Surname);
                        $('#editDepartmentID').val(data.DepartmentID);
                        selectView("edit");
                    }
                });
                break;
            case "submitEdit":
                $.ajax({
                    type: "PUT",
                    url: "/api/PersonAPI/" + selectedRadio.attr('value'),
                    data: $('#editForm').serialize(),
                    success: function (result) {
                        if (result) {
                            var cells = selectedRadio.closest('tr').children();
                            cells[1].innerText = $('#editName').val();
                            cells[2].innerText = $('#editSurname').val();
                            cells[3].innerText = $('#editDepartmentID').val();
                            selectView("summary");
                        }
                    }
                });
                break;
        }
    });
});