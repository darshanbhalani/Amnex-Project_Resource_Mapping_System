var skillsDataSource = new kendo.data.DataSource({
    data: Skills,
    schema: {
        model: {
            id: "Skillid"
        }
    },
    pageSize: 10
});

$('#grid').kendoGrid({

    dataSource: skillsDataSource,
    columns: [
        {
            field: "Skillname",
            title: "Skill's Name",
            width: "40%",
            editable: false,
            attributes: {
                "class": "custom-column"
            },
            sortable: true
        },
        {
            title: "Details",
            width: "30%",
            template: function (dataItem) {
                return "<button class='btn btn-light btn-employee-details' data-skillid='" + dataItem.Skillid + "'>Employee Details</button> " +
                    "<button class='btn btn-light btn-project-details' data-skillid='" + dataItem.Skillid + "'>Project Details</button>";
            },
            attributes: {
                "class": "details"
            },
        },
        {
            title: "Actions",
            width: "30%",
            template: function (dataItem) {
                return "<i class='fas fa-edit fa-2x k-grid-edit' data-skillid='" + dataItem.Skillid + "' data-skillname = '" + dataItem.Skillname + "'> | </i>" +
                    "<i class='fas fa-trash fa-2x k-grid-delete' data-skillid='" + dataItem.Skillid + "' data-skillname = '" + dataItem.Skillname + "'></i>";
            },
            attributes: {
                "class": "actions"
            },
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
    dataBound: function () {
        $('#grid th').css('background-color', 'rgb(47 68 98)');
        $('#grid th').css('color', 'white');
        $('.k-grid-pdf').css('background-color', 'rgb(47 68 98)');
        $('.actions').css('text-align', 'center');
        $('.details').css('text-align', 'center');
        $('.btn-employee-details').off('click').on('click', function () {
            var skillId = $(this).data('skillid');
            console.log(skillId);
            var dataItem = skillsDataSource.get(skillId);
            if (dataItem) {
                displayEmployeeSkill(dataItem);
                $('#empSkillModal').modal('show');
            }
        });
        $('.k-grid-edit').off('click').on('click', function () {
            var skillId = $(this).data('skillid');
            var skillName = $(this).data('skillname');
            var dataItem = skillsDataSource.get(skillId);
            console.log(skillName);
            console.log(skillId);
            if (dataItem) {
                dataItem.skillId = skillId;
                dataItem.skillName = skillName;
                editSkill(dataItem);
                $('#editSkillModal').modal('show');
            }
        });
        $('.k-grid-delete').off('click').on('click', function () {
            var skillId = $(this).data('skillid');
            var skillName = $(this).data('skillname');
            var dataItem = skillsDataSource.get(skillId);
            if (dataItem) {
                dataItem.skillId = skillId;
                dataItem.skillName = skillName;
                deleteSkill(dataItem);
                $('#deleteSkillModal').modal('show');
            }
        });
        $('.btn-project-details').off('click').on('click', function () {
            var skillId = $(this).data('skillid');
            var dataItem = skillsDataSource.get(skillId);
            if (dataItem) {
                displayProjectSkill(dataItem);
                $('#projectSkillModal').modal('show');
            }

        });
    },
    selectable: "row",
    change: function (e) {
        var selectedRows = this.select();
        selectedRows.each(function () {
            var dataItem = $('#grid').data("kendoGrid").dataItem(this);
            if (dataItem) {
                $('#skillname').text(dataItem.Skillname);
                $('#editSkillName').text(dataItem.Skillid);
                displayEmployeeSkill(dataItem);
            }
        });
    },
});
function displayProjectSkill(dataItem) {
    $.ajax({
        url: '/Skills/GetProjectDetails',
        type: 'GET',
        data: { skillId: dataItem.Skillid },
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
                    //$('.k-grid-edit').attr("data-skillid", dataItem.Skillid);
                    //$('.k-grid-delete').attr("data-skillid", dataItem.Skillid);
                    if (e.sender.dataSource.view().length === 0) {
                        this.element.find(".k-grid-header").hide();
                    } else {
                        this.element.find(".k-grid-header").show();
                        var skillName = dataItem.Skillname;
                        this.element.find(".k-grid-header").find("th:first").text("Projects working on skill : " + skillName);
                    }
                },
                columns: [
                    { field: "projectName", title: "", width: "50%" }
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
        data: { skillId: dataItem.Skillid },
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
                    if (e.sender.dataSource.view().length === 0) {
                        this.element.find(".k-grid-header").hide();
                    } else {
                        this.element.find(".k-grid-header").show();
                        var skillName = dataItem.Skillname;
                        this.element.find(".k-grid-header").find("th:first").text("Employees having skill : " + skillName);
                    }
                },
                columns: [
                    { field: "employeeName", title: "Employee's Name", width: "50%" }
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

            });

        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

//Add skills
$(document).on('click', '.addSkillButton', function () {
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
    function checkAndSaveSkill(skillName) {
        var lowercaseSkillName = skillName.toLowerCase();
        var grid = $('#grid').data('kendoGrid');
        var dataSource = grid.dataSource;
        var existingSkill = dataSource.data().some(function (item) {
            return item.Skillname.toLowerCase() === lowercaseSkillName;
        });
        if (existingSkill) {
            showSnackbar('false', skillName + ' already exists');
            $('#skillName').val('');
            $('#addSkillModal').modal('hide');
            return;
        }
        else {
            saveSkill(skillName);
        }
    }

    $('#addSkillModal').modal('show');
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
        var skillName = $('#skillName').val();
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