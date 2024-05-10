$(document).ready(function () {
    $("#departmentDetailsLink").click(function (event) {
        event.preventDefault(); // Prevent the default behavior of the link

        // Fetch the value of Model.DepartmentId
        //var departmentId = @Model.DepartmentId; // Ensure it's properly formatted for JavaScript

        // Construct the URL with the departmentId query parameter
        var url = "/Departments/GetDetails?departmentId=" + departmentId;

        // Navigate to the constructed URL
        window.location.href = url;
    });
});
