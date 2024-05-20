$(document).ready(function () {
    console.log(DepartmentData);
    $('#grid').kendoGrid({
        dataSource: {
            data: DepartmentData,
            pageSize: 10,
        },

        columns: [
            {
                field: "departmentName", title: "Name", width: 500, editable: false
                //    template: '<a href="javascript:void(0)" class="department-link">#= DepartmentName #</a>'
            },
            {
                command: [
                    { name: "View", text: "", iconClass: "k-icon k-i-info-circle", click: openGetDetails },
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", click: openEditModal },
                    { name: "delete", text: "", iconClass: "k-icon k-i-delete", click: openDeleteModal }
                ],
                title: "Action",
                width: "80px"
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
    });
    function openEditModal(e) {
        e.preventDefault();
        $('#addsaveChangesBtn').addClass('k-hidden');
        $('#EditChangesBtn').removeClass('k-hidden');
        $('#deleteBtn').addClass('k-hidden');

        $('#departmentlabel').removeClass('k-hidden');
        $('#deletedeptlabel').addClass('k-hidden');
        $('#departmentModalLabel').text('Edit Department');
        //$('#department').removeAttr('placeholder');

        $('#departmentModal').modal('show');
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        $('#department').val(dataItem.DepartmentName);
        $("#department").on("input", function () {

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
            var editeddepartmentName = $("#department").val();
            var data = {
                DepartmentId: dataItem.DepartmentId,
                DepartmentName: editeddepartmentName,
            };
            $.ajax({
                type: "PUT",
                url: "/Departments/Editdepartment",
                data: data,
                success: function (result) {
                    $("#department").val('');
                    $('#departmentModal').modal('hide');
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
        $("#departmentNamePlaceholder").text(dataItem.DepartmentName);
        $('#departmentModalLabel').text('Delete Department');

        $("#deleteDepartmentconfirm").val('');

        $('#departmentlabel').addClass('k-hidden');
        $('#deletedeptlabel').removeClass('k-hidden');

        $('#EditChangesBtn').addClass('k-hidden');
        $('#addsaveChangesBtn').addClass('k-hidden');
        $('#deleteBtn').removeClass('k-hidden');
        $('#department').val('');
        $('#department').attr('placeholder', 'Type Confirm');

        $("#departmentModal").modal('show');

        $("#department").on("input", function () {
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
        $('#EditChangesBtn').addClass('k-hidden');
        $('#addsaveChangesBtn').removeClass('k-hidden');
        $('#deleteBtn').addClass('k-hidden');

        $('#departmentlabel').removeClass('k-hidden');
        $('#deletedeptlabel').addClass('k-hidden');
        $('#departmentModalLabel').text('Add Department');
        $('#depart ment').removeAttr('placeholder');

        $('#department').attr('placeholder', 'Enter Department');
        $('#department').val('');


        $('#departmentModal').modal('show');
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

    function openGetDetails(e) {
        e.preventDefault();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        console.log(dataItem);
        var departmentName = dataItem.departmentId;
        console.log("ID" + departmentName);
        var data = {
            DepartmentId: dataItem.departmentId
        }
        window.location.href = "/Departments/GetDetails?departmentId=" + dataItem.departmentId;
    }
});
