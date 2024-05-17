$(document).ready(function () {

    function getDeptName() {
        $.ajax({
            url: '/Departments/GetDepartmentname',
            type: 'GET',
            data: { departmentId: deptid },
            success: function (response) {
                console.log(response);
                deptname = response.departmentname; 
                initializeGrid(deptname); // Call the function to initialize the grid with the department name
                console.log(deptname);
            },
            error: function (xhr, status, error) {
                console.error(error); 
            }
        });
        return deptname;
    }
    function initializeGrid(deptname) {


        $('#deptprojectgrid').kendoGrid({
            dataSource: {
                data: dataSource,
                pageSize: 10,
            },
            columns: [
                { field: "projectName", title: "Name", width: 60, editable: false },
                { field: "startDate", title: "StartDate", width: 120, editable: false },
                { field: "endDate", title: "EndDate", width: 120, editable: false },
                { field: "skillName", title: "Skills", width: 100, editable: false },
                { field: "status", title: "Status", width: 100, editable: false }
            ],
            editable: false,
            navigatable: true,
            selectable: "row",
            sortable: true,
            pageable: true,
            filterable: true,
            toolbar: [
                "search",
                "pdf",
            ],
            pdf: {
                allPages: true,
                fileName: deptname ? "DepartmentProjects_" + deptname + ".pdf" : "DepartmentProjects.pdf", // Set the PDF filename with department name if available
                paperSize: "A4",
                landscape: true,
                margin: { top: "2cm", left: "1cm", right: "1cm", bottom: "2cm" },
                scale: 0.8,
                repeatHeaders: true,
                template: $("#page-template").html(),
                forcePageBreak: ".k-grid"
            }
        });
    }





    $("#departmentDetailsLink").click(function (event) {
        event.preventDefault();
        var departmentId = $("#deptid").val();

        // Make an Ajax request
        $.ajax({
            type: "POST",
            url: "/Departments/GetDetails", // Adjust the URL as per your route configuration
            contentType: "application/json",
            data: JSON.stringify({ departmentId: deptid }),
            success: function (response) {
                // Redirect to deptProjects page with departmentId as a query parameter
                window.location.href = "/Departments/GetDetails?departmentId=" + deptid;
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle errors
                console.error("Error:", textStatus, errorThrown);
            }
        });
    });
    $("#detailsbtn").click(function () {
        // Get the value of the department ID input
        var departmentId = $("#deptid").val();

        // Make an Ajax request
        $.ajax({
            type: "POST",
            url: "/Departments/GetDetails", // Adjust the URL as per your route configuration
            contentType: "application/json",
            data: JSON.stringify({ departmentId: deptid }),
            success: function (response) {
                // Redirect to deptProjects page with departmentId as a query parameter
                window.location.href = "/Departments/GetDetails?departmentId=" + deptid;
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle errors
                console.error("Error:", textStatus, errorThrown);
            }
        });
    });
    $("#deptemployees").click(function () {
        window.location.href = "/Departments/deptEmployees?departmentId=" + deptid;
    });
        getDeptName();

});
