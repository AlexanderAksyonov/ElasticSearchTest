function getData() {
    $('#content').empty();
    $.ajax({
        type: "GET",
        url: "/api/DepartmentAPI/GetRootDepartment/0",
        success: function (data) {
            $('#content').prepend('<div id="TreeView"><ul id="MyTree" class="Tree"></ul></div>');
            for (var i = 0; i < data.length; i++) {
                if (!data[i].ParentId) {
                    $('#MyTree').append('<li class="TreeNode"  onclick = "depClick(' + data[i].ID +')"  >' + data[i].Name + '</li>')
                        .append(getChildDep(data[i].ID))
                        .append(getChildPers(data[i].ID));
                };
            }
            
        }
    });
    $('#content').append('<button id="AddRootDep" onclick = "AddDep(0, \'\')">Add root department</button>');
}

function getChildDep(depId) {
    var subElements = $('<ul></ul>');
    $.ajax({
        type: "GET",
        url: "/api/DepartmentAPI/GetChildDepartment/" + depId,
        success: function (data) {
            
            for (var i = 0; i < data.length; i++) {
                subElements.append('<li class="TreeNode" onclick = "depClick('+ data[i].ID +')">' + data[i].Name + '</li>')
                    .append(getChildDep(data[i].ID))
                    .append(getChildPers(data[i].ID));
            }
        }
    });
    return subElements;
}

function getChildPers(depId) {
    var subElements = $('<ul></ul>');
    $.ajax({
        type: "GET",
        url: "/api/PersonAPI/GetForDepartment/" + depId,
        success: function (data) {

            for (var i = 0; i < data.length; i++) {
                subElements.append('<li class= "TreeSheet" onclick = "personClick(' + data[i].Id +')">' + data[i].Name + " " + data[i].Surname + '</li>');
            }
        }
    });
    return subElements;
}

function AddDep(parentId, parentName) {
    $('#content').empty();
    $('#content').append('<div id = "addDisplay" class="display"></div>');
    $('#addDisplay').append('<h4>Add new Department</h4>').append('<form id="FormAddDep"></form>');
    $('#FormAddDep').append('<p><label>Name</label><input id="editName" name="Name"/></p>')
        .append('<input id="editID" type ="hidden" name="ID" value ="0" />')
        .append('<input id="editParentID" type ="hidden" name="ParentId" value ="' + parentId+'" />')
        .append('<p><label>Parent name</label><label>'+parentName+'</label></p>');
    $('#addDisplay').append('<button id="btnSaveDepartment" onclick ="saveDep()" >Save</button>');
    
}

function saveDep() {
    if ($('#FormAddDep')) {
        $.ajax({
            type: "POST",
            url: "/api/DepartmentAPI/",
            data: $('#FormAddDep').serialize(),
            success: function (result) {
                getData();
            }
        });
    }
}
function deleteDep(depID) {
        $.ajax({
            type: "DELETE",
            url: "/api/DepartmentAPI/"+depID,
            success: function (result) {
                getData();
            }
        });

}

function depClick(id) {
    if ($('#info').length>0) {
        $('#info').empty();
    }
    else {
        $('#content').append('<div id="info"></div>');
    }
  
    $.ajax({
        type: "GET",
        url: "/api/DepartmentAPI/"+ id,
        success: function (data) {
            if (data) {             
                var table = $(' <table id="concrete"> <thead><tr>'
                    + '<th class="nameCol"> Имя</th > '
                    + '<th class="idCol" >ID</th >'
                    + '<th class="parentCol" > Родительский Отдел</th > '
                    + '</tr>'
                    + '</thead >'
                    + '<tbody id="tableBody">'
                    + '</tbody>'
                    + '</table >');

                $('#info').append(table);

                $('#tableBody').append('<tr>'
                    + '<td>' + data.Name + '</td>'
                    + '<td>' + data.ID + '</td>'            
                    + '<td>' + data.ParentId + '</td></tr>');

                $('#info').append('<button id="AddDep" onclick = "AddDep(' + data.ID + ', \'' + data.Name + '\')">Add child department</button>')
                    .append('<button id="deleteDep" onclick = "deleteDep(' + data.ID + ')">Delete department</button>');
            }
        }
    });
    

}

function personClick(id) {
    if ($('#info')) {
        $('#info').empty();
    }
    else {
        $('#content').append('<div id="info"></div>');
    }
    $.ajax({
        type: "GET",
        url: "/api/PersonAPI/" + id,
        success: function (data) {
            if (data) {
                var table = $(' <table id="concrete"> <thead><tr>'
                    + '<th class="idCol" >ID</th >' 
                    + '<th class="nameCol"> Имя</th > '
                    + '<th class="surNameCol">Фамилия</th > '
                    + '<th class="departnentCol" >Отдел</th > '
                    + '</tr>'
                    + '</thead >'
                    + '<tbody id="tableBody">'
                    + '</tbody>'
                    + '</table >');

                $('#info').append(table);

                $('#tableBody').append('<tr>'
                    + '<td>' + data.Id + '</td>'
                    + '<td>' + data.Name + '</td>'
                    + '<td>' + data.Surname + '</td>'
                    + '<td>' + data.DepartmentID + '</td></tr>');
            }
        }
    });
}



$(document).ready(function () {
    getData();
});

