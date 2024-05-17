$(document).ready(function () {
    $("#deptprojects").click(function () {
        var departmentId = $("#deptid").val();
        window.location.href = "/Departments/deptProjects?departmentId=" + departmentId;
    });
    $("#deptemployees").click(function () {
        var departmentId = parseInt($("#deptid").val());
        window.location.href = "/Departments/deptEmployees?departmentId=" + departmentId;
    });
    
});
