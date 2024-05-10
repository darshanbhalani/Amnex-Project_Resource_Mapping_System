$(document).ready(function () {
    console.log(DepartmentData);
    $('#grid').kendoGrid({
        dataSource: {
            data: DepartmentData,
            pageSize: 10,
        },

        columns: [
            {
                field: "DepartmentName", title: "Name", width: 60, editable: false,
                template: '<a href="javascript:void(0)" class="department-link">#= DepartmentName #</a>'
            },
            {
                command: [
                    {
                        name: "edit", text: "", click: openEditModal
                    },
                    {
                        name: "delete", text: "", iconClass: "k-icon k-i-delete", click: openDeleteModal
                    }
                ], title: "Actions", width: "10px"
            }
        ],
        editable: false,
        navigatable: true,
        selectable: "row",
        sortable: true,
        pageable: true,
        filterable: true,
        toolbar: [
            { template: '<button class="k-button k-primary" id="AddDepartment">Add Department</button>' },
            "search",
            "pdf",
        ],
        dataBound: function () {
            $('.department-link').click(function (e) {
                e.preventDefault();
                console.log("HELO");
                var grid = $('#grid').data("kendoGrid");
                var dataItem = grid.dataItem($(this).closest("tr"));
                console.log(dataItem);
                var departmentName = $(this).text();
                console.log(departmentName);
                //$('#deptid').val(dataItem.DepartmentId);
                //$('#deptname').val(departmentName);
                var data = {
                    DepartmentId: dataItem.DepartmentId
                }
                window.location.href = "/Departments/GetDetails?departmentId=" + dataItem.DepartmentId;


            });
        }
    });






    function openEditModal(e) {
        e.preventDefault();
        $('#editDepartmentModal').modal('show');
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#editDepartment').val(dataItem.DepartmentName);
        $("#editDepartment").on("input", function () {

            var newDepartment = $(this).val();
            if (newDepartment !== dataItem.DepartmentName) {
                $("#EditChangesBtn").removeAttr("disabled");
                $("#EditChangesBtn").addClass('btn-success');
            } else {
                $("#EditChangesBtn").attr("disabled", true);
                $("#EditChangesBtn").removeClass('btn-success');
            }
        });
        $(document).on("click", "#EditChangesBtn", function () {
            var editeddepartmentName = $("#editDepartment").val();
            var data = {
                DepartmentId: dataItem.DepartmentId,
                DepartmentName: editeddepartmentName,
            };
            $.ajax({
                type: "PUT",
                url: "/Departments/Editdepartment",
                data: data,
                success: function (result) {
                    $("#editDepartment").val('');
                    $('#editDepartmentModal').modal('hide');
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error(error);

                }
            });
        });
    }

    function openDeleteModal(e) {
        e.preventDefault();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        console.log("In delete fun" + dataItem);
        $("#departmentNamePlaceholder").text(dataItem.DepartmentName);
        $("#deleteDepartmentconfirm").val('');
        $("#deleteDepartmentModal").modal('show');

        $("#deleteDepartmentconfirm").on("input", function () {
            var newDepartment = $(this).val();

            if (newDepartment === 'CONFIRM') {
                $("#deleteBtn").removeAttr("disabled");
                $("#deleteBtn").addClass('btn-danger');
                $("#deleteBtn").click(function () {
                    var data = {
                        DepartmentId: dataItem.DepartmentId,
                    };
                    $.ajax({
                        type: "POST",
                        url: "/Departments/Deletedepartment",
                        data: data,
                        success: function (result) {
                            if (result.success) {
                                $('#deleteDepartmentModal').modal('hide');
                                location.reload();
                            } else {
                                $('#deleteDepartmentModal').modal('hide');
                            }
                        },
                        error: function (xhr, status, error) {
                            loading();
                            console.error(errorMessage);
                            showSnackbar('false', 'Something went wrong');

                        }
                    });
                });


            } else {
                $("#deleteBtn").attr("disabled", true);
                $("#deleteBtn").removeClass('btn-danger');
            }
        });
    }
    $('#AddDepartment').click(function () {
        $('#departmentModal').modal('show');
    });
    $('#addsaveChangesBtn').click(function () {
        var departmentName = $("#department").val();
        var data = {
            DepartmentName: departmentName,
        };
        $.ajax({
            type: "POST",
            url: "/Departments/AddDepartment",
            data: data,
            success: function (result) {
                if (result.success) {
                    $("#department").val('');
                    $('#departmentModal').modal('hide');
                    location.reload();
                }
                else {
                    $('#departmentModal').modal('hide');
                }
            },
            error: function (xhr, status, error) {
                // Handle error
                $("#department").val('');
                $('#departmentModal').modal('hide');


            }
        });
    });

});
