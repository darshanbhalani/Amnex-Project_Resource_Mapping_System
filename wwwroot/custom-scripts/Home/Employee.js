var multipleCancelButton = new Choices('#choices-multiple-remove-button', {
    removeItemButton: true,
    maxItemCount: 50,
    searchResultLimit: 50,
    renderChoiceLimit: 50
});
function validateForm() {
    var isValid = true;

    // Validate Employee Name
    var employeeName = $('#employeeName').val().trim();
    if (employeeName === "") {
        $('#employeeNameError').text("Employee Name is required.");
        isValid = false;
    } else {
        $('#employeeNameError').text("");
    }

    // Validate Employee Email
    var employeeEmailId = $('#employeeEmailId').val().trim();
    if (employeeEmailId === "") {
        $('#employeeEmailIdError').text("Employee Email is required.");
        isValid = false;
    } else if (!isValidEmail(employeeEmailId)) {
        $('#employeeEmailIdError').text("Invalid email format.");
        isValid = false;
    } else {
        $('#employeeEmailIdError').text("");
    }

    // Validate Department
    var department = $('#department').val();
    if (department === "") {
        $('#departmentError').text("Department is required.");
        isValid = false;
    } else {
        $('#departmentError').text("");
    }

    // Validate User Role
    var userRole = $('#employeeLoginRole').val();
    if (userRole === "") {
        $('#employeeLoginRoleError').text("User Role is required.");
        isValid = false;
    } else {
        $('#employeeLoginRoleError').text("");
    }

    // Validate Designation
    var designation = $('#designation').val();
    if (designation === "") {
        $('#designationError').text("Designation is required.");
        isValid = false;
    } else {
        $('#designationError').text("");
    }

    // Validate Skills
    var selectedSkills = $('#choices-multiple-remove-button').val();
    if (!selectedSkills || selectedSkills.length === 0) {
        $('#skillsError').text("At least one skill is required.");
        isValid = false;
    } else {
        $('#skillsError').text("");
    }

    return isValid;
}

// Function to validate email format
function isValidEmail(email) {
    // Regular expression for email validation
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}



function addEmployee(e) {
    console.log("asdfgdsfghhf");

    $("#addEmployeeBtn").show();
    $("#updateEmployeeBtn").hide();
    $("#employeeActionModal").modal("show");

    $('#addEmployeeBtn').click(function (e) {
        e.preventDefault();
        $('#errorMessages').empty();

        if (!validateForm()) {
            return;
        }

        var employeeName = $('#employeeName').val().trim();
        var employeeEmailId = $('#employeeEmailId').val().trim();
        var department = $('#department').val();
        var userRole = $('#employeeLoginRole').val();
        var designation = $('#designation').val();
        var selectedSkills = $('#choices-multiple-remove-button').val();

        var employeeObj = {
            EmployeeName: employeeName,
            Email: employeeEmailId,
            DepartmentId: department,
            LoginRoleId: userRole,
            DesignationId: designation,
            SkillsId: selectedSkills.join(',')
        };

        loading();

        $.ajax({

            type: "POST",
            url: "/Employees/AddEmployee",
            data: employeeObj,
            success: function (result) {
                location.reload(); // Not ideal, consider updating grid instead
                showSnackbar("true", "Add New Employee successfully.");
            },
            error: function (errormessage) {
                loading();
                showSnackbar("false", errormessage.responseText);
            }
        });
    });

    $('.btn-close').click(function () {
        $('#addEmployeeForm').trigger("reset");
        $('.error-message').empty();
    });

    $('#employeeActionModal').on('hidden.bs.modal', function () {
        $('#addEmployeeForm').trigger("reset");
        $('.error-message').empty();
    });
}

function updateEmployee(e) {

    var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
    console.log(dataItem);
    var employeeId = dataItem.EmployeeId;
    var departmentName = dataItem.DepartmentName;
    var employeeLoginRole = dataItem.LoginRole;
    var designationName = dataItem.DesignationName;
    var skillsId = dataItem.SkillsId;

    $('#employeeName').val(dataItem.EmployeeName);
    $('#employeeEmailId').val(dataItem.Email);
    $('#department option').filter(function () {
        return $(this).text() == departmentName;
    }).prop('selected', true);

    $('#designation option').filter(function () {
        return $(this).text() == designationName;
    }).prop('selected', true);

    $('#employeeLoginRole option').filter(function () {
        return $(this).text() == employeeLoginRole;
    }).prop('selected', true);

    var skillIdsArray = skillsId.split(',');
    skillIdsArray.forEach(function (skillId) {

        skillId = skillId.trim();


        multipleCancelButton.setChoiceByValue(skillId);
    });

    $("#addEmployeeBtn").hide();
    $("#updateEmployeeBtn").show();
    $("#employeeActionModal").modal("show");
    $('#updateEmployeeBtn').click(function (e) {
        console.log("hello");
        e.preventDefault();
        console.log(employeeId);
        $('#errorMessages').empty();

        if (!validateForm()) {
            return;
        }

        var employeeName = $('#employeeName').val().trim();
        var employeeEmailId = $('#employeeEmailId').val().trim();
        var department = $('#department').val();
        var userRole = $('#employeeLoginRole').val();
        var designation = $('#designation').val();
        var selectedSkills = $('#choices-multiple-remove-button').val();

        var employeeObj = {
            employeeId: employeeId,
            EmployeeName: employeeName,
            Email: employeeEmailId,
            DepartmentId: department,
            LoginRoleId: userRole,
            DesignationId: designation,
            SkillsId: selectedSkills.join(',')
        };

        loading();

        $.ajax({

            type: "POST",
            url: "/Employees/UpdateEmployee",
            data: employeeObj,
            success: function (result) {
                location.reload(); // Not ideal, consider updating grid instead
                showSnackbar("true", "Add New Employee successfully.");
            },
            error: function (errormessage) {
                loading();
                showSnackbar("false", errormessage.responseText);
            }
        });
    });

}

function deleteEmployee(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
    console.log(dataItem);
    var employeeId = dataItem.EmployeeId;
    console.log(employeeId);
    $('#deleteModalError').empty();
    $('#confirmInput').val('');

    // Open delete confirmation modal
    $("#deleteEmployeeModal").modal('show');

    $('#confirmInput').on('input', function () {
        var confirmInput = $('#confirmInput').val();
        var deleteButton = $('#deleteButton');

        if (confirmInput === 'CONFIRM') {
            deleteButton.prop('disabled', false).removeClass('btn-secondary').addClass('btn-danger');
        } else {
            deleteButton.prop('disabled', true).removeClass('btn-danger').addClass('btn-secondary');
        }
    });

    $('#deleteButton').click(function () {
        var confirmValue = $('#confirmInput').val().trim().toUpperCase();
        if (confirmValue === "CONFIRM") {
            var data = { EmployeeId: parseInt(employeeId) };

            $.ajax({
                type: 'POST',
                url: "/Employees/DeleteEmployee",
                data: data,
                success: function (result) {
                    if (result.success) {

                        $("#deleteModal").modal('hide');


                        window.location.href = '/Home/Employees';


                        showSnackbar("true", "Employee deleted successfully.");
                    } else {

                        $('#deleteModalError').text(result.message).css('text-align', 'left');
                    }
                },
                error: function (errorMessage) {

                    $('#deleteModalError').text("Error deleting employee.").css('text-align', 'center');
                }
            });
        }
    });


}


function viewEmployee(e) {
    e.preventDefault();


    var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
    console.log(dataItem);
    var employeeId = dataItem.EmployeeId;
    alert(employeeId);

    window.location.href = '/Employees/EmployeeDetails?EmployeeId=' + employeeId;
}









$(document).ready(function () {
    //var employeesModel = @Html.Raw(ViewData["employeesModel"]); 

    console.log(employeesModel);

    var employeeData = employeesModel.Employees;

    console.log(employeeData);




    // Kendo Grid Initialization
    $("#grid1").kendoGrid({
        dataSource: {
            data: employeeData,
            pageSize: 10
        },
        height: 550,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: [10, 25, 50, 100, "all"],
            buttonCount: 10
        },
        navigatable: true,
        resizable: true,
        reorderable: true,
        filterable: true,
        toolbar: [
            { template: '<button class="k-button k-primary"  onclick="addEmployee()">Add Employee</button>' },
            "search",
            "excel",
            "pdf"
        ],
        excel: {
            fileName: "Employees.xlsx",
            filterable: true,
            allPages: true
        },
        pdf: {
            fileName: "Employees.pdf",
            allPages: true,
            avoidLinks: true,
            margin: { top: "2cm", left: "1cm", right: "1cm", bottom: "1cm" },
            landscape: true,
            repeatHeaders: true,
            scale: 0.8
        },
        columnMenu: true,
        columns: [
            { field: "EmployeeId", hidden: true },
            { field: "EmployeeAipl", title: "AIPL Id", width: 100 },
            { field: "EmployeeName", title: "Name", width: 150 },
            { field: "Email", hidden: true },
            { field: "DepartmentName", title: "Department", width: 150 },
            { field: "DesignationName", title: "Designation", width: 150 },
            { field: "SkillsId", hidden: true },
            { field: "LoginRole", title: "Login Role", width: 150 },
            { field: "IsAllocated", title: "Allocated", width: 150 },

            {
                command: [
                    { name: "View", text: "", iconClass: "k-icon k-i-info-circle", click: viewEmployee },
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", click: updateEmployee },
                    { name: "delete", text: "", iconClass: "k-icon k-i-delete", click: deleteEmployee }


                ],
                title: "Action",
                width: "100px"
            }

        ],
        editable: false,
        dataBound: function () {

        },
        selectable: 'row',

    });







});

