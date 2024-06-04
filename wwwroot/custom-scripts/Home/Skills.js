function displaySkill(dataItem) {
    $.ajax({
        url: '/Skills/GetSkillsDetails',
        type: 'GET',
        data: { skillId: dataItem.skillid },
        success: function (response) {
            var skillData = response.data;
            $('#skillsgrid').kendoGrid({
                dataSource: {
                    data: skillData,
                    pageSize: 10
                },
                dataBound: function (e) {
                    $('#skillsgrid th').css('background-color', 'rgb(47 68 98)');
                    $('#skillsgrid th').css('color', 'white');
                    $('#skillsgrid .k-grid-content td').css('text-align', 'center');
                    $('.k-grid-pdf').css('background-color', 'rgb(47 68 98)');
                    $('.k-grid-edit').attr("data-skillid", dataItem.Skillid);
                    $('.k-grid-delete').attr("data-skillid", dataItem.Skillid);
                    $('.btn-employee-details').attr("data-skillid", dataItem.Skillid);
                    $('.btn-project-details').attr("data-skillid", dataItem.Skillid);

                },
                columns: [
                    { field: "skillname", title: "Skill's Name", width: "20%" },
                    { field: "employeecount", title: "Number of Employees", width: "20%" },
                    { field: "projectcount", title: "Number of Projects", width: "20%" }
                ],
                sortable: true,
                pageable: true,
                toolbar: [
                    {
                        name: "search",
                        template: "<button class='k-button searchButton'>Search</button>",
                        className: "custom-toolbar-button"
                    },
                    {
                        name: "excel",
                        text: "Export to Excel",
                        className: "k-grid-excel"
                    },
                    {
                        name: "pdf",
                        text: "Export to PDF",
                        className: "k-grid-pdf"
                    }
                ],
                pdf: {
                    fileName: "skills_details.pdf",
                    allPages: true,
                    avoidLinks: true,
                    paperSize: "A4",
                    margin: { top: "2cm", right: "1cm", bottom: "1cm", left: "1cm" },
                    fontSize: 12,
                },
                excel: {
                    fileName: "Skills_Details.xlsx",
                    filterable: true
                },

            });
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
function displayProjectSkill(dataItem) {

    $.ajax({
        url: '/Skills/GetProjectDetails',
        type: 'GET',
        data: { skillId: dataItem.skillid },
        success: function (response) {
            var projectData = response.data;
            $('#projgrid').kendoGrid({
                dataSource: {
                    data: projectData,
                    pageSize: 10
                },
                dataBound: function (e) {
                    $('#projgrid th').css('background-color', 'rgb(47 68 98)');
                    $('#projgrid th').css('color', 'white');
                    $('.k-grid-pdf').css('background-color', 'rgb(47 68 98)');
                },
                columns: [
                    { field: "projectName", title: "", width: "50%" },
                    { field: "startDate", title: "", width: "50%" },
                    { field: "endDate", title: "", width: "50%" },
                    { field: "departmentName", title: "", width: "50%" },
                    { field: "status", title: "", width: "50%" },
                ],
                sortable: true,
                pageable: true,
                toolbar: [
                    {
                        name: "search",
                        template: "<button class='k-button searchButton'>Search</button>",
                        className: "custom-toolbar-button"
                    },
                    {
                        name: "excel",
                        text: "Export to Excel",
                        className: "k-grid-excel"
                    },
                    {
                        name: "pdf",
                        text: "Export to PDF",
                        className: "k-grid-pdf"
                    }
                ],
                pdf: {
                    fileName: "Projects_in_skills.pdf",
                    allPages: true,
                    avoidLinks: true,
                    paperSize: "A4",
                    margin: { top: "2cm", right: "1cm", bottom: "1cm", left: "1cm" },
                    fontSize: 12,
                },
                excel: {
                    fileName: "Project_Details.xlsx",
                    filterable: true
                },

            });
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}
function displayEmployeeSkill(dataItem) {
    $.ajax({
        url: '/Skills/GetEmployeeDetails',
        type: 'GET',
        data: { skillId: dataItem.skillid },
        success: function (response) {
            var employeeData = response.data;
            $('#empgrid').kendoGrid({
                dataSource: {
                    data: employeeData,
                    pageSize: 10
                },
                dataBound: function (e) {
                    $('#empgrid th').css('background-color', 'rgb(47 68 98)');
                    $('#empgrid th').css('color', 'white');
                    $('.k-grid-pdf').css('background-color', 'rgb(47 68 98)');

                },
                columns: [
                    { field: "employeeAipl", title: "Employee's Aipl", width: "25%" },
                    { field: "employeeName", title: "Employee's Name", width: "25%" },
                    { field: "employeeDesignation", title: "Employee's Designation", width: "25%" },
                    { field: "departmentName", title: "Employee's Department", width: "25%" }
                    /* { field: "isAllocated", title: "Employee's Allocation", width: "25%" },*/
                ],
                sortable: true,
                pageable: true,
                toolbar: [
                    {
                        name: "search",
                        template: "<button class='k-button searchButton'>Search</button>",
                        className: "custom-toolbar-button"
                    },
                    {
                        name: "excel",
                        text: "Export to Excel",
                        className: "k-grid-excel"
                    },
                    {
                        name: "pdf",
                        text: "Export to PDF",
                        className: "k-grid-pdf"
                    }
                ],
                pdf: {
                    fileName: "Employees_in_skills.pdf",
                    allPages: true,
                    avoidLinks: true,
                    paperSize: "A4",
                    margin: { top: "2cm", right: "1cm", bottom: "1cm", left: "1cm" },
                    fontSize: 12,
                },
                excel: {
                    fileName: "Employee_details.xlsx",
                    filterable: true
                },

            });

        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

//Add skills

$(document).on('click', '.addSkillButton', function () {
    $('#addSkillModal').modal('show');
    function checkAndSaveSkill(skillName) {
        var lowercaseSkillName = skillName.toLowerCase();
        var grid = $('#grid').data('kendoGrid');
        var dataSource = grid.dataSource;
        var existingSkill = dataSource.data().some(function (item) {
            return item.skillname.toLowerCase() === lowercaseSkillName;
        });

        if (existingSkill) {
            showSnackbar('false', skillName + ' already exists');
            $('#skillName').val('');
            $('#addSkillModal').modal('hide');
            return;
        }
        saveSkill(skillName);

    }
    function saveSkill(skillName) {
        var data = {
            skillname: skillName,
        };
        loading();
        $.ajax({
            type: 'POST',
            url: '/Skills/AddSkill',
            data: data,
            success: function (response) {
                loading();
                if (response.success) {
                    showSnackbar('true', skillName + ' added successfully');
                    location.reload();
                } else {
                    $('#addSkillModal').modal('hide');
                    showSnackbar('false', response.message);

                }
            },
            error: function (xhr, status, error) {
                loading();
                console.error(error);
                showSnackbar('false', error);

            }
        });
    }
    $('#skillName').off('input').on('input', function () {
        var skillNameInput = $('#skillName').val();
        var saveAddSkillButton = $('#saveAddSkillButton');
        $('#addSkillModal').on('hidden.bs.modal', function () {
            $('#skillName').val('');
        });
        if (skillNameInput.trim() === '') {
            saveAddSkillButton.prop('disabled', true);
        } else {
            saveAddSkillButton.prop('disabled', false);

        }
    });
    $('#saveAddSkillButton').on('click', function () {
        $('#addSkillModal').modal('hide');
        var skillName = $('#skillName').val().trim();
        checkAndSaveSkill(skillName);
    });
});

//Edit skills
function editSkill(dataItem) {
    $('#editSkillName').val(dataItem.skillName);
    $('#editSkillName').val(dataItem.skillName).data('originalSkillName', dataItem.skillName);
    $('#editSkillModal').on('hidden.bs.modal', function () {
        $('#editSkillName').val('');
    });
    $('#editSkillName').on('input', function () {
        var editedSkillName = $(this).val().trim();
        var originalSkillName = $(this).data('originalSkillName');
        if (editedSkillName !== originalSkillName) {
            $('#saveEditSkillButton').prop('disabled', false);

        } else {
            $('#saveEditSkillButton').prop('disabled', true);
        }
    });
    $('#saveEditSkillButton').on('click', function () {
        var updatedSkillName = $('#editSkillName').val();
        loading();
        $.ajax({
            type: 'POST',
            url: '/Skills/UpdateSkill',
            data: { skillId: dataItem.skillId, updatedSkillName: updatedSkillName },
            success: function (response) {
                loading();
                if (response.success) {
                    showSnackbar('true', updatedSkillName + ' updated successfully');
                    location.reload();
                } else {
                    showSnackbar('false', response.error);

                }
            },
            error: function (xhr, status, error) {
                loading();
                console.error(error);
                showSnackbar('false', 'An error occurred while updating the skill. Please try again later.');

            }
        });
    });
}

//DELETE SKILL
function deleteSkill(dataItem) {
    $('#deleteSkillName').text(dataItem.skillName);
    var skillNameToDelete = dataItem.skillName;
    $('#deleteSkillModal').on('hidden.bs.modal', function () {
        $('#confirmInput').val('');
        $('#confirmDeleteButton').prop('disabled', true);
    });
    $('#confirmInput').on('input', function () {
        var confirmInputValue = $(this).val().trim();
        var confirmButton = $('#confirmDeleteButton');
        if (confirmInputValue === 'CONFIRM') {
            confirmButton.prop('disabled', false);
        } else {
            confirmButton.prop('disabled', true);
        }
    });
    $('#confirmDeleteButton').on('click', function () {
        var skillIdToDelete = dataItem.skillId;
        var confirmInputValue = $('#confirmInput').val().trim();
        if (confirmInputValue !== 'CONFIRM') {
            return;
        }
        loading();
        $.ajax({
            type: 'POST',
            url: '/Skills/DeleteSkill',
            data: { skillId: skillIdToDelete },
            success: function (response) {
                loading();
                if (response.success) {
                    $('#deleteSkillModal').modal('hide');
                    showSnackbar('true', skillNameToDelete + ' deleted successfully');
                    location.reload();
                } else {
                    showSnackbar('false', response.message);
                    $('#deleteSkillModal').modal('hide');

                }
            },
            error: function (xhr, status, error) {
                $('#deleteSkillModal').modal('hide');
                loading();
                console.error(error);
                showSnackbar('false', 'An error occurred while deleting the skill. Please try again later.');

            }
        });
    });


}

$(document).ready(function () {
    var skillsDataSource = new kendo.data.DataSource({
        data: Skills,
        schema: {
            model: {
                id: "skillid"
            }
        },
        pageSize: 10
    });
    $('#grid').kendoGrid({
        dataSource: skillsDataSource,
        columns: [
            {
                field: "skillname",
                title: "Skill's Name",
                width: "40%",
                editable: false,
                attributes: {
                    "class": "custom-column"
                },
                sortable: true
            },
            {
                command: [
                    {
                        name: "info",
                        text: "",
                        iconClass: "k-icon k-i-information k-grid-info"
                    },
                    {
                        name: "edit",
                        text: "",
                        iconClass: "k-icon k-i-edit k-grid-edit"
                    },
                    {
                        name: "destroy",
                        text: "",
                        iconClass: "k-icon k-i-delete k-grid-delete "
                    }
                ],
                title: "Actions",
                width: "10%"
            }
        ],
        sortable: true,
        editable: false,
        navigatable: true,
        pageable: {
            refresh: true,
            pageSizes: [10, 25, 50, 100],
            buttonCount: 5,
            pageSize: 10
        },
        toolbar: [
            {
                name: "create",
                template: "<button class='k-button addSkillButton' style='background-color:rgb(47 68 98);'>Add Skill</button>",
                className: "custom-toolbar-button"
            },
            {
                name: "search",
                template: "<button class='k-button searchButton'>Search</button>",
                className: "custom-toolbar-button"
            },
            {
                name: "excel",
                text: "Export to Excel",
                className: "k-grid-excel"
            },
            {
                name: "pdf",
                text: "Export to PDF",
                className: "k-grid-pdf"
            }
        ],
        pdf: {
            fileName: "Skills.pdf",
            allPages: true,
            avoidLinks: true,
            paperSize: "A4",
            margin: { top: "2cm", right: "1cm", bottom: "1cm", left: "1cm" },
            fontSize: 12,
            exportPDF: function (e) {
                e.sender.thead.find("th").each(function () {

                    if ($(this).text().trim() === "Actions") {
                        $(this).css("display", "none");
                    }
                });
            }
        },
        excel: {
            fileName: "Skills.xlsx",
            filterable: true
        },
        dataBound: function () {
            $('#grid th').css('background-color', 'rgb(47 68 98)');
            $('#grid th').css('color', 'white');
            $('.k-grid-pdf').css('background-color', 'rgb(47 68 98)');
            $('.actions').css('text-align', 'center');
            $('.details').css('text-align', 'center');
            var grid = this;
            grid.tbody.find(".k-i-information").each(function () {
                var dataItem = grid.dataItem($(this).closest("tr"));
                $(this).attr("data-skillid", dataItem.skillid);
                $(this).attr("data-skillname", dataItem.skillname);
            });
            grid.tbody.find(".k-i-edit").each(function () {
                var dataItem = grid.dataItem($(this).closest("tr"));
                $(this).attr("data-skillid", dataItem.skillid);
                $(this).attr("data-skillname", dataItem.skillname);
            });
            grid.tbody.find(".k-i-delete").each(function () {
                var dataItem = grid.dataItem($(this).closest("tr"));
                $(this).attr("data-skillid", dataItem.skillid);
                $(this).attr("data-skillname", dataItem.skillname);
            });
            $('.k-i-information').off('click').on('click', function () {
                var skillId = $(this).attr('data-skillid');
                var skillName = $(this).attr('data-skillname');
                var dataItem = skillsDataSource.get(skillId);
                if (dataItem) {
                    dataItem.skillid = skillId;
                    dataItem.Skillname = skillName;
                    displaySkill(dataItem);
                    displayEmployeeSkill(dataItem);
                    displayProjectSkill(dataItem);
                    $('#grid').hide();
                    $('#details').show();
                    $('#skillsgrid').show();
                }
            });
            $('.k-i-edit').off('click').on('click', function () {
                var skillId = $(this).attr('data-skillid');
                var skillName = $(this).attr('data-skillname');
                var dataItem = skillsDataSource.get(skillId);
                if (dataItem) {
                    dataItem.skillId = skillId;
                    dataItem.skillName = skillName;
                    editSkill(dataItem);
                    $('#editSkillModal').modal('show');
                }
            });
            $('.k-i-delete').off('click').on('click', function () {
                var skillId = $(this).attr('data-skillid');
                var skillName = $(this).attr('data-skillname');
                var dataItem = skillsDataSource.get(skillId);
                if (dataItem) {
                    dataItem.skillId = skillId;
                    dataItem.skillName = skillName;
                    deleteSkill(dataItem);
                    $('#deleteSkillModal').modal('show');
                }
            });

        },
        selectable: "row",
    });

    $('.btn-skill-details').off('click').on('click', function () {
        $('.btn-employee-details').removeClass('active');
        $('.btn-project-details').removeClass('active');
        $('.btn-skill-details').addClass('active');
        $('#grid').hide();
        $('#details').show();
        $('#empgrid').hide();
        $('#projgrid').hide();
        $('#skillsgrid').show();

    });
    $('.btn-employee-details').off('click').on('click', function () {
        $('.btn-skill-details').removeClass('active');
        $('.btn-project-details').removeClass('active');
        $('.btn-employee-details').addClass('active');
        $('#grid').hide();
        $('#details').show();
        $('#skillsgrid').hide();
        $('#projgrid').hide();
        $('#empgrid').show();

    });
    $('.btn-project-details').off('click').on('click', function () {
        $('.btn-skill-details').removeClass('active');
        $('.btn-employee-details').removeClass('active');
        $('.btn-project-details').addClass('active');
        $('#grid').hide();
        $('#details').show();
        $('#skillsgrid').hide();
        $('#empgrid').hide();
        $('#projgrid').show();
    });

});
