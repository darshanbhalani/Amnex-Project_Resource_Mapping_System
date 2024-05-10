$(document).ready(function () {
    $("#deptprojects").click(function () {
        // Get the value of the department ID input
        var departmentId = $("#deptid").val();

        // Make an Ajax request
        $.ajax({
            type: "POST",
            url: "/Departments/deptProjects", // Adjust the URL as per your route configuration
            contentType: "application/json",
            data: JSON.stringify({ departmentId: departmentId }),
            success: function (response) {
                // Redirect to deptProjects page with departmentId as a query parameter
                window.location.href = "/Departments/deptProjects?departmentId=" + departmentId;
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle errors
                console.error("Error:", textStatus, errorThrown);
            }
        });
    });
});
