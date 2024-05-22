$(() => {
    // Display main data
    $("#displayMainDataGrid").kendoGrid({
        dataSource: {
            data: projectsData,
            pageSize: 10
        },
        height: "70vh",
        pageable: true,
        sortable: true,
        filterable: true,
        toolbar: [
            { name: "search" },
            { name: "pdf", margin: { top: "2cm", bottom: "2cm", left: "1cm", right: "1cm" }, paperSize: "A4" },
            { name: "excel", margin: { top: "2cm", bottom: "2cm", left: "1cm", right: "1cm" }, paperSize: "A4" },

        ],
        columns: [{
            field: "projectName",
            title: "Project Name",
            width: 160
        }, {
            field: "projectDepartment",
            title: "Department",
            width: 100
        },
        {
            field: "numberOfEmployees",
            title: "Employees",
            width: 100
        }, {
            field: "startDate",
            title: "Start Date",
            width: 160,
            template: function (dataItem) {
                let startDate = dataItem.startDate;
                return formatDate(startDate);
            }
        }, {
            field: "endDate",
            title: "End Date",
            width: 160,
            template: function (dataItem) {
                let endDate = dataItem.endDate;
                return formatDate(endDate);
            }
        },
        {
            title: "Actions",
            width: 80,
            template: `
                                <i class='fas fa-info-circle fa-lg projectInfo cursor m-2' title='Project Info'></i>
                                <span style='font-size: 1.5em; vertical-align: middle;'>|</span>
                                <i class='fas fa-user-plus fa-lg mapEmp cursor m-2' title='Assign Employee'></i>
                            `,
            //attributes: { style: "text-align: center;", class: "mapEmp cursor" }
        }],
        dataBound: function () {
            $("#displayMainDataGrid tbody[role='rowgroup']").attr("id", "dataRows");
            var grid = this;
            $('#displayMainDataGrid tbody tr').each(function () {
                var rowData = grid.dataItem(this);
                if (rowData) {
                    $(this).attr("data-projectid", rowData.projectId);
                    $(this).attr("data-projectskills", rowData.projectSkills);
                }
            });
        }
    });


    // display allocated employees data
    let projectId;
    let skills;
    // Event listener for row click to open modal
    //$(document).on('click', '#dataRows', '.k-master-row', function (event) {
    $(document).on('click', '.projectInfo', function (event) {
        $('#mainModalApply').prop('disabled', true);

        $('.detailsButton').click();
        $('.xlEmployeesData').css({ display: 'none' });
        $('.detailsButton').click(() => {
            $('.xlEmployeesData').css({ display: 'none' });
            $('#projectDetails').css({ display: 'block' });
        });
        $('.employeesButton').click(() => {
            $('.xlEmployeesData').css({ display: 'block' });
            $('#projectDetails').css({ display: 'none' });
        });

        if ($(event.target).closest('.mapEmp').length) {
            return;
        }
        const clickedRow = $(event.target).closest("tr");

        projectId = clickedRow.attr('data-projectid');
        let projectName = clickedRow.find('td:eq(0)').text();
        let department = clickedRow.find('td:eq(1)').text();
        skills = clickedRow.attr('data-projectskills');
        let employees = clickedRow.find('td:eq(2)').text();
        let projectStartDate = clickedRow.find('td:eq(3)').text();
        let projectEndDate = clickedRow.find('td:eq(4)').text();
        $('#mainModalLabel').text(`Details of "${projectName}"`);
        // Populate modal fields with the fetched data
        $('#pid').val(projectId);
        $('#projectName').val(projectName);
        $('#department').val(department);
        $('#skills').val(skills);
        $('#employees').val(employees);
        $('#startDate1').val(formatDateToYYYYMMDD(projectStartDate));
        $('#endDate1').val(formatDateToYYYYMMDD(projectEndDate));

        $('#mainModal').modal('show').off('shown.bs.modal').on('shown.bs.modal', function () {
            $('#xlModalErrorMsg').text('');

            $.ajax({
                url: '/Mapping/getAllocatedEmployeesData',
                type: 'POST',
                data: { ProjectId: projectId },
                dataType: 'json',
                success: function (data) {
                    populateEmployeeData(data);
                },
                error: function (error) {
                    console.log('Error fetching data:', error);
                }
            });
        });

        function populateEmployeeData(data) {
            var originalStartDates = {};
            var originalEndDates = {};
            let allocatedEmployeesData = JSON.parse(data);
            try {
                $("#displayAlloctedEmployeesGrid").kendoGrid({
                    dataSource: {
                        data: allocatedEmployeesData,
                        pageSize: 10
                    },
                    height: "40vh",
                    pageable: true,
                    sortable: true,
                    filterable: true,
                    toolbar: [
                        { name: "search" },
                        { name: "pdf", margin: { top: "2cm", bottom: "2cm", left: "1cm", right: "1cm" }, paperSize: "A4" },
                        { name: "excel", margin: { top: "2cm", bottom: "2cm", left: "1cm", right: "1cm" }, paperSize: "A4" },

                    ],
                    columns: [{
                        field: "EmployeeCode",
                        title: "Employee Code",
                        width: 80
                    }, {
                        field: "Employeename",
                        title: "Employee Name",
                        width: 160
                    },
                    {
                        field: "EmployeeSkills",
                        title: "Skills",
                        width: 160
                    },
                    {
                        field: "EmployeeStartDate",
                        title: "Start Date",
                        width: 130,
                        template: function (dataItem) {
                            let startDate = (dataItem.EmployeeStartDate).split('T')[0];
                            originalStartDates[dataItem.WorkingEmployeeId] = startDate;
                            let today = new Date().toISOString().split('T')[0];

                            return `<input type="date" class="form-control start_date" value="${startDate}" min=${today}>`;
                        }
                    }, {
                        field: "EmployeeEndDate",
                        title: "End Date",
                        width: 130,
                        template: function (dataItem) {
                            let endDate = (dataItem.EmployeeEndDate).split('T')[0];
                            originalEndDates[dataItem.WorkingEmployeeId] = endDate;
                            return `<input type="date" class="form-control end_date" value="${endDate}">`;
                        }
                    },
                    {
                        title: "Remove",
                        width: 80,
                        template: "<i class='fa fa-minus-square fa-lg down cursor' style='color: red' aria-hidden='true'></i>",
                        attributes: { style: "text-align: center;", class: " cursor" }

                    }],
                    dataBound: function () {
                        var grid = this;
                        $('#displayAlloctedEmployeesGrid tbody tr').each(function () {
                            var rowData = grid.dataItem(this);
                            if (rowData) {
                                $(this).attr("data-employeeid", rowData.WorkingEmployeeId);
                            }
                        });
                        $("#displayAlloctedEmployeesGrid .start_date").each(function () {
                            let employeeId = $(this).closest('tr').data('employeeid');
                            let startDateValue = $(this).val();
                            $(`.end_date[data-employeeid='${employeeId}']`).attr('min', startDateValue);
                        });
                    }
                });

            } catch (err) {
                console.log("Data not found " + err);
            }


            $('#displayAlloctedEmployeesGrid').off('change', '.start_date, .end_date').on('change', '.start_date, .end_date', function () {
                var row = $(this).closest('tr');
                var startDateInput = row.find('.start_date');
                var endDateInput = row.find('.end_date');

                var workingEmployeeId = row.attr('data-employeeid');
                var originalStartDate = new Date(originalStartDates[workingEmployeeId]);
                var originalEndDate = new Date(originalEndDates[workingEmployeeId]);

                var currentStartDate = new Date(row.find('.start_date').val());
                var currentEndDate = new Date(row.find('.end_date').val());

                var originalStartTimestamp = originalStartDate.getTime();
                var originalEndTimestamp = originalEndDate.getTime();
                var currentStartTimestamp = currentStartDate.getTime();
                var currentEndTimestamp = currentEndDate.getTime();

                if (startDateInput.val().trim() === '' || endDateInput.val().trim() === '') {
                    $('#mainModalApply').prop('disabled', true);
                } else if (currentStartTimestamp !== originalStartTimestamp || currentEndTimestamp !== originalEndTimestamp) {
                    $('#mainModalApply').prop('disabled', false);
                } else {
                    $('#mainModalApply').prop('disabled', true);
                }
                if ($(this).hasClass('start_date')) {
                    let startDateValue = startDateInput.val();
                    endDateInput.attr('min', startDateValue);
                }
            });

            $(document).on('change', '.start_date', function () {
                let employeeId = $(this).data('employeeid');
                let startDateValue = $(this).val();
                $(`.end_date[data-employeeid='${employeeId}']`).attr('min', startDateValue);
            });

            $('#mainModalApply').click(() => {
                let projectStartDateObj = parseDate(projectStartDate);
                let projectEndDateObj = parseDate(projectEndDate);
                let allValid = true;

                let today = new Date();
                today.setHours(0, 0, 0, 0);
                // Clear any previous error message
                $('#xlModalErrorMsg').text('');

                $('#displayAlloctedEmployeesGrid tbody tr').each(function () {
                    var row = $(this);
                    var startDate = resetTimeToMidnight(row.find('.start_date').val());
                    var endDate = resetTimeToMidnight(row.find('.end_date').val());

                    if (startDate < today) {
                        $('#xlModalErrorMsg').text('**Start date should not be before today\'s date');
                        allValid = false;
                        return false; // Break out of the loop
                    } else if (endDate < startDate) {
                        $('#xlModalErrorMsg').text('**End date should not be before start date');
                        allValid = false;
                        return false; // Break out of the loop
                    } else if (startDate < projectStartDateObj || endDate > projectEndDateObj) {
                        $('#xlModalErrorMsg').text('**Employee dates must be within the project range');
                        allValid = false;
                        return false; // Break out of the loop
                    }
                });

                if (allValid) {
                    var updatedData = [];
                    $('#displayAlloctedEmployeesGrid tbody tr').each(function () {
                        var row = $(this);
                        var workingEmployeeId = $(this).closest('tr').attr('data-employeeid');
                        var EmployeeStartDate = row.find('.start_date').val();
                        var EmployeeEndDate = row.find('.end_date').val();

                        updatedData.push({
                            workingEmployeeId: workingEmployeeId,
                            EmployeeStartDate: EmployeeStartDate,
                            EmployeeEndDate: EmployeeEndDate
                        });
                    });

                    $.ajax({
                        url: '/Mapping/UpdateAllocatedEmployeesData',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify(updatedData),
                        success: function (response) {
                            location.reload();
                        },
                        error: function (error) {
                            console.log('Error updating data:', error);
                            alert('An error occurred while updating data. Please try again.');
                        }
                    });

                }
            });
        }
    });

    //Event listener for click events on icons within the "Remove" column
    $('#displayAlloctedEmployeesGrid').on('click', '.fa-minus-square', function () {
        let $row = $(this).closest('tr');
        let employeeId = parseInt($row.attr('data-employeeid'));
        let employeeCode = $row.find('td:eq(0)').text();
        let employeeName = $row.find('td:eq(1)').text();
        let message = `Are you sure you want to remove <b>${employeeName}</b> having employee code <b>${employeeCode}</b> from the project?`;
        $('#employeeDetails').html(message);

        $('#confirmationModal').modal('show');
        let $button = $(this);

        $('#confirmDeletion').click(() => {
            $.ajax({
                url: '/Mapping/RemoveFromProject',
                type: 'POST',
                data: { employeeId: employeeId, projectId: projectId },
                success: function (result) {
                    $('#confirmationModal').modal('hide');
                    $button.closest('tr').remove();
                    $.ajax({
                        url: '/Mapping/getAllocatedEmployeesData',
                        type: 'POST',
                        data: { ProjectId: projectId },
                        dataType: 'json',
                        success: function (data) {
                            location.reload();
                            getData();
                            populateEmployeeData(data);
                        },
                        error: function (error) {
                            console.log('Error fetching data:', error);
                        }
                    });
                },
                error: function (error) {
                    console.error('Error removing employee from project:', error);
                }
            });
        });
    });
    $(".close-confirmation").click(() => {
        $('#confirmationModal').modal('hide');
    })

    var mainProjectStartDate;
    var mainProjectEndDate;
    let projectSkills
    // Event listener for click events on icons within the "Assign" column
    $('#displayMainDataGrid').on('click', '.mapEmp', function (event) {
        $('#validationMsg').text('');
        let $row = $(event.target).closest('tr');
        let projectId = $row.attr('data-projectid');
        let projectName = $row.find('td:eq(0)').text();
        projectSkills = $row.closest('tr').attr('data-projectskills')

        $('#assignEmployeeHeader').text(`Assign employee(s) to project "${projectName}"`);
        mainProjectStartDate = $row.find('td:eq(3)').text();
        mainProjectEndDate = $row.find('td:eq(4)').text();

        let departmentName = $row.find('td:eq(1)').text();
        $.ajax({
            url: '/Mapping/FetchNotAllocatedEmployees',
            type: 'POST',
            data: { departmentName: departmentName, projectId: projectId },
            success: function (data) {
                displayNotAllocatedData(data);
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });

        $('#mapModal').modal('show');
        $('#savechange').prop('disabled', true);
        $('#savechange').click(() => {
            saveChangesToDatabase(projectId);
        });

    });

    function saveChangesToDatabase(projectId) {
        let data = {
            ProjectId: parseInt(projectId),
            EmployeesId: [],
            AssignedSkillsOfEmployee: [],
            StartDate: [],
            EndDate: []
        };
        let allDatesValid = true;
        let today = new Date();
        today.setHours(0, 0, 0, 0);
        $('#displayNotAllocatedEmployeesGrid tbody tr:has(td:eq(6) input[type="checkbox"]:checked)').each(function () {
            let row = $(this);
            let employeeId = parseInt(row.attr('data-employeeid'));
            let assignedSkillsOfEmployee = [];
            row.find('td:eq(2) select option:selected').each(function () {
                assignedSkillsOfEmployee.push($(this).val()); // Push each selected skill to the array
            });
            let startDate = row.find('td:eq(4) input').val();
            let endDate = row.find('td:eq(5) input').val();

            var startDateObject = parseDate(formatDate(startDate));
            var endDateObject = parseDate(formatDate(endDate));

            var mainProjectStartDateObject = parseDate(mainProjectStartDate);
            var mainProjectEndDateObject = parseDate(mainProjectEndDate);
            if (assignedSkillsOfEmployee.length === 0) {
                $('#validationMsg').text('**Please select at least one skill for each employee.');
                allDatesValid = false;
                return false; // Break out of the loop
            }
            else if (startDateObject < today) {
                $('#validationMsg').text('**Start date should not be before today\'s date');
                allValid = false;
                return false; // Break out of the loop
            } else if (endDateObject < startDateObject) {
                $('#validationMsg').text('**End date should not be before start date');
                allDatesValid = false;
                return;
            } else if (!startDate || !endDate) {
                $('#validationMsg').text('**Both start date and end date are required.');
                allDatesValid = false;
                return;
            }
            else if (startDateObject < mainProjectStartDateObject || endDateObject > mainProjectEndDateObject) {
                $('#validationMsg').text('**Employee dates must be within the project range');
                allDatesValid = false;
                return;
            }
            else {
                data.EmployeesId.push(employeeId);
                data.AssignedSkillsOfEmployee.push(assignedSkillsOfEmployee);
                data.StartDate.push(startDate);
                data.EndDate.push(endDate);
            }
        });
        if (allDatesValid) {
            $.ajax({
                url: '/Mapping/AssignEmployees',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response) {
                    $('#mapModal').modal('hide');
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error('Error saving data:', error);
                }
            });
        }
    }

    function displayNotAllocatedData(Data) {
        let notAllocatedEmployeesData = JSON.parse(Data);
        $("#displayNotAllocatedEmployeesGrid").kendoGrid({
            dataSource: {
                data: notAllocatedEmployeesData,
                pageSize: 10
            },
            height: "40vh",
            pageable: true,
            sortable: true,
            filterable: true,
            toolbar: [
                { name: "search" },
                { name: "pdf", margin: { top: "2cm", bottom: "2cm", left: "1cm", right: "1cm" }, paperSize: "A4" },
                { name: "excel", margin: { top: "2cm", bottom: "2cm", left: "1cm", right: "1cm" }, paperSize: "A4" },

            ],
            columns: [{
                field: "EmployeeCode",
                title: "Employee Code",
                width: 100
            }, {
                field: "NotAllocatedEmployeeName",
                title: "Employee Name",
                width: 160
            }, {
                field: "NotAllocatedEmployeeSkills",
                title: "Skills",
                width: 180,
            },
            {
                field: "EmployeeDesignation",
                title: "Designation",
                width: 100
            }, {
                title: "Start Date",
                width: 125,
                template: 'not assigned'
            }, {
                title: "End Date",
                width: 125,
                template: 'not assigned'
            }, {
                title: "Add",
                width: 60,
                template: function (dataItem) {
                    return '<input type="checkbox" class="form-check-input addEmp cursor" id="addEmp' + dataItem.EmployeeId + '">';
                }
            }], dataBound: function () {
                $("<style>")
                    .prop("type", "text/css")
                    .html(".k-grid-content tbody tr { height: 80px; }") // Adjust height as needed karan
                    .appendTo("head");
                var grid = this;
                $('#displayNotAllocatedEmployeesGrid tbody tr').each(function () {
                    var rowData = grid.dataItem(this);
                    if (rowData) {
                        $(this).attr("data-employeeid", rowData.EmployeeId);
                    }
                });
            }
        });


        // Target only checkboxes within the table with ID 'empModal'
        $('#displayNotAllocatedEmployeesGrid .addEmp').change(function () {
            updateSaveButton();
            sortCheckedRowsEmpModal();
            updateSkillsToDropdown();
            updateDateFields();
        });

        function sortCheckedRowsEmpModal() {
            let rows = $('#displayNotAllocatedEmployeesGrid tbody tr').get();
            rows.sort(function (a, b) {
                let keyA = $(a).find('.addEmp').is(':checked') ? 0 : 1;
                let keyB = $(b).find('.addEmp').is(':checked') ? 0 : 1;
                return keyA - keyB;
            });
            $.each(rows, function (index, row) {
                $('#displayNotAllocatedEmployeesGrid tbody').append(row);
            });
        }

        function updateSkillsToDropdown() {
            $('#displayNotAllocatedEmployeesGrid .addEmp').each(function () {
                let row = $(this).closest('tr');
                let projectRoleCell = row.find('td').eq(2);
                let employeeSkills = row.find('td').eq(2).text();

                // Check if a dropdown already exists in the row
                let dropdownExists = projectRoleCell.find('.multipleEmployeeSkill').length > 0;
                if (!row.data('originalSkills')) {
                    row.data('originalSkills', employeeSkills);
                }

                if ($(this).is(':checked')) {
                    if (!dropdownExists) {
                        // If no dropdown exists, create and append a new dropdown
                        let fetchedSkills = projectSkills.split(',').map(skill => skill.trim());
                        let dropdown = $('<select class="form-control multipleEmployeeSkill" placeholder="Select skills" multiple></select>');

                        fetchedSkills.forEach(function (skill) {
                            dropdown.append($('<option></option>').attr("value", skill).text(skill));
                        });

                        projectRoleCell.empty().append(dropdown);
                        var multipleCancelButton = new Choices(dropdown[0], {
                            removeItemButton: true,
                        });
                    }
                } else {
                    if (projectRoleCell.find('select').length > 0) {
                        // Restore the original skills from the data attribute
                        let originalSkills = row.data('originalSkills');
                        projectRoleCell.empty().text(originalSkills);
                    }
                }
            });
        }
        function updateDateFields() {
            let projectStartDate = "not assigned";
            let projectEndDate = "not assigned";
            // Get today's date in YYYY-MM-DD format
            let today = new Date().toISOString().split('T')[0];

            $('#displayNotAllocatedEmployeesGrid .addEmp').each(function () {
                let row = $(this).closest('tr');
                let startDateCell = row.find('td').eq(4);
                let endDateCell = row.find('td').eq(5);

                if ($(this).is(':checked')) {
                    if (startDateCell.find('input').length === 0) {
                        startDateCell.html(`<input type="date" class="form-control startDate" value="${projectStartDate}" min="${today}">`);
                    }
                    if (endDateCell.find('input').length === 0) {
                        endDateCell.html(`<input type="date" class="form-control endDate" value="${projectEndDate}">`);
                    }
                    let startDateInput = startDateCell.find('input');
                    let endDateInput = endDateCell.find('input');

                    startDateInput.on('change', function () {
                        let startDateValue = startDateInput.val();
                        endDateInput.attr('min', startDateValue);
                    });

                    // Set initial min value for the end date
                    let startDateValue = startDateInput.val();
                    endDateInput.attr('min', startDateValue);
                }

                else {
                    if (startDateCell.find('input').length > 0) {
                        startDateCell.html(projectStartDate);
                    }
                    if (endDateCell.find('input').length > 0) {
                        endDateCell.html(projectEndDate);
                    }
                }
            });
        }
        function updateSaveButton() {
            let anyChecked = $('#displayNotAllocatedEmployeesGrid .addEmp:checked').length > 0;
            $('#savechange').prop('disabled', !anyChecked);
        }

    }

});

function formatDate(dateString) {
    let date = new Date(dateString);
    let day = date.getDate().toString().padStart(2, '0');
    let month = (date.getMonth() + 1).toString().padStart(2, '0'); // Month is zero-based, so we add 1
    let year = date.getFullYear();
    // Format the date as dd-mm-yyyy
    return day + '-' + month + '-' + year;
}
function formatDateToYYYYMMDD(dateString) {
    let parts = dateString.split('-');
    if (parts.length !== 3) {
        throw new Error('Invalid date format. Please provide a date in dd-mm-yyyy format.');
    }
    let year = parts[2];
    let month = parts[1];
    let day = parts[0];
    return `${year}-${month}-${day}`;
}
function parseDate(dateString) {
    var dateParts = dateString.split('-');
    var dateObject = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
    return dateObject;
}
function resetTimeToMidnight(dateString) {
    var date = new Date(dateString);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    date.setMilliseconds(0); 

    return date;
}