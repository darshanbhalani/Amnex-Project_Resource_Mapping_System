var multipleCancelButton = new Choices('#choices-multiple-remove-button', {
    removeItemButton: true,
    maxItemCount: 50,
    searchResultLimit: 50,
    renderChoiceLimit: 50
});

function validateForm() {
    var isValid = true;

    // Reset error messages
    $('.error-message').empty();

    // Validate Employee Name
    var employeeName = $('#employeeName').val().trim();
    if (employeeName === "") {
        $('#employeeNameError').text('Employee Name is required.');
        isValid = false;
    }

    // Validate Employee Email
    var employeeEmailId = $('#employeeEmailId').val().trim();
    if (employeeEmailId === "") {
        $('#employeeEmailIdError').text('Employee Email is required.');
        isValid = false;
    } else if (!isValidEmail(employeeEmailId)) {
        $('#employeeEmailIdError').text('Invalid email format.');
        isValid = false;
    }

    // Validate Department
    var department = $('#department').val();
    if (department === "") {
        $('#departmentError').text('Department is required.');
        isValid = false;
    }

    // Validate User Role
    var userRole = $('#employeeLoginRole').val();
    if (userRole === "") {
        $('#employeeLoginRoleError').text('User Role is required.');
        isValid = false;
    }

    // Validate Designation
    var designation = $('#designation').val();
    if (designation === "") {
        $('#designationError').text('Designation is required.');
        isValid = false;
    }

    // Validate Skills
    var selectedSkills = $('#choices-multiple-remove-button').val();
    if (!selectedSkills || selectedSkills.length === 0) {
        $('#skillsError').text('At least one skill is required.');
        isValid = false;
    }

    return isValid;
}


// Function to validate email format
function isValidEmail(email) {
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function showError(message) {
    $('#errorMessages').text(message);
}

function addEmployee() {
    $('#addEmployeeBtn').show();
    $('#updateEmployeeBtn').hide();
    $('#actionEmployeeModalLabel').text('Add a new Employee');
    $('#employeeActionModal').modal('show');

    $('#addEmployeeBtn').click(function (e) {
        e.preventDefault();
        $('#errorMessages').empty();

        if (!validateForm()) {
            return;
        }

        var employeeObj = {
            EmployeeName: $('#employeeName').val().trim(),
            Email: $('#employeeEmailId').val().trim(),
            DepartmentId: $('#department').val(),
            LoginRoleId: $('#employeeLoginRole').val(),
            DesignationId: $('#designation').val(),
            SkillsId: $('#choices-multiple-remove-button').val().join(',')
        };

        loading();

        $.ajax({
            type: 'POST',
            url: '/Employees/AddEmployee',
            data: employeeObj,
            success: function (result) {
                location.reload(); // Not ideal, consider updating grid instead
                showSnackbar('true', 'Add New Employee successfully.');
            },
            error: function (errormessage) {
                loading();
                showSnackbar('false', errormessage.responseText);
            }
        });
    });

    $('.btn-close').click(function () {
        $('#addEmployeeForm').trigger('reset');
        $('.error-message').empty();
    });

    $('#employeeActionModal').on('hidden.bs.modal', function () {
        $('#addEmployeeForm').trigger('reset');
        $('.error-message').empty();
    });
}

function updateEmployee(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
    var employeeId = dataItem.employeeId;

    // Set values in the form for updating
    $('#employeeName').val(dataItem.employeeName);
    var emailId = dataItem.email;
    var employeeName = dataItem.employeeName;



    $('#employeeEmailId').val(dataItem.email);
    $('#department option').filter(function () {
        return $(this).text() == dataItem.departmentName;
    }).prop('selected', true);

    $('#designation option').filter(function () {
        return $(this).text() == dataItem.designationName;
    }).prop('selected', true);

    $('#employeeLoginRole option').filter(function () {
        return $(this).text() == dataItem.loginRole;
    }).prop('selected', true);

    // Set selected skills
    var skillIdsArray = dataItem.skillsId.split(',');
    skillIdsArray.forEach(function (skillId) {
        skillId = skillId.trim();
        multipleCancelButton.setChoiceByValue(skillId);
    });

    // Show modal for updating
    $('#addEmployeeBtn').hide();
    $('#updateEmployeeBtn').show();
    $('#actionEmployeeModalLabel').text('Update Employee Details');
    $('#employeeActionModal').modal('show');

    // Handle update button click
    $('#updateEmployeeBtn').click(function (e) {
        e.preventDefault();

        // Validate and collect form data
        if (!validateForm()) {
            return;
        }
        var tempemailId = $('#employeeEmailId').val().trim();
        if (emailId == tempemailId) {
            emailId = null;
        } else {
            emailId = tempemailId;
        }
        alert(emailId);

        var tempEmployeeName = $('#employeeName').val().trim();
        if (employeeName == tempEmployeeName) {
            employeeName = null;
        } else {
            employeeName = tempEmployeeName;
        }
        alert(employeeName)

        var employeeObj = {
            EmployeeId: employeeId,
            EmployeeName: employeeName,
            Email: emailId,
            DepartmentId: $('#department').val(),
            LoginRoleId: $('#employeeLoginRole').val(),
            DesignationId: $('#designation').val(),
            SkillsId: $('#choices-multiple-remove-button').val().join(',')
        };

        loading();

        $.ajax({
            type: 'POST',
            url: '/Employees/UpdateEmployee',
            data: employeeObj,
            success: function (result) {
                location.reload(); // Not ideal, consider updating grid instead
                showSnackbar('true', 'Employee updated successfully.');
            },
            error: function (errormessage) {
                loading();
                showSnackbar('false', errormessage.responseText);
            }
        });
    });
    $('.btn-close').click(function () {
        $('#addEmployeeForm').trigger('reset');
        $('.error-message').empty();
        emailId = null;
    });

    $('#employeeActionModal').on('hidden.bs.modal', function () {
        $('#addEmployeeForm').trigger('reset');
        $('.error-message').empty();
        emailid = null;
    });
}

function deleteEmployee(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
    var employeeId = dataItem.employeeId;

    $('#deleteModalError').empty();
    $('#confirmInput').val('');

    // Open delete confirmation modal
    $("#deleteEmployeeModal").modal('show');

    // Handle confirmation input
    $('#confirmInput').on('input', function () {
        var confirmInput = $('#confirmInput').val();
        var deleteButton = $('#deleteButton');

        if (confirmInput === 'CONFIRM') {
            deleteButton.prop('disabled', false).removeClass('btn-secondary').addClass('btn-danger');
        } else {
            deleteButton.prop('disabled', true).removeClass('btn-danger').addClass('btn-secondary');
        }
    });

    // Handle delete button click
    $('#deleteButton').click(function () {
        var confirmValue = $('#confirmInput').val().trim().toUpperCase();
        if (confirmValue === "CONFIRM") {
            var data = { EmployeeId: parseInt(employeeId) };

            $.ajax({
                type: 'POST',
                url: '/Employees/DeleteEmployee',
                data: data,
                success: function (result) {
                    if (result.success) {
                        $("#deleteModal").modal('hide');
                        window.location.href = '/Home/Employees';
                        showSnackbar('true', 'Employee deleted successfully.');
                    } else {
                        $('#deleteModalError').text(result.message).css('text-align', 'left');
                    }
                },
                error: function (errorMessage) {
                    $('#deleteModalError').text('Error deleting employee.').css('text-align', 'center');
                }
            });
        }
    });
}

function viewEmployee(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest('tr'));
    var employeeId = dataItem.employeeId;
    window.location.href = '/Employees/EmployeeDetails?EmployeeId=' + employeeId;
}

$(document).ready(function () {
    var employeeData = employeesModel.employees;
    console.log(employeeData);

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
            { field: "employeeId", hidden: true },
            { field: "employeeAipl", title: "AIPL Id", width: 100 },
            { field: "employeeName", title: "Name", width: 150 },
            { field: "email", hidden: true },
            { field: "departmentName", title: "Department", width: 150 },
            { field: "designationName", title: "Designation", width: 150 },
            { field: "skillsId", hidden: true },
            { field: "loginRole", title: "Login Role", width: 150 },

            {
                command: [
                    { name: "View", text: "", iconClass: "k-icon k-i-info-circle", click: viewEmployee },
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", click: updateEmployee },
                    { name: "delete", text: "", iconClass: "k-icon k-i-delete", click: deleteEmployee }
                ],
                title: "Action",
                width: "200px"
            }
        ],
        editable: false,

        selectable: 'row'
    });
});