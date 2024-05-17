$(document).ready(function () {

    console.log("bbbbb");
    console.log(dataSource);
    //var dataSource = @Html.Raw(Json.Serialize(Model));
    function getDeptName() {
        $.ajax({
            url: '/Departments/GetDepartmentname',
            type: 'GET',
            data: { departmentId: deptid },
            success: function (response) {
                console.log(response);
                deptname = response.departmentname; 
                initializeGrid(deptname); 
                console.log(deptname);
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
        return deptname;
    }

    function initializeGrid(deptname) {
        $('#deptemployeesgrid').kendoGrid({
            dataSource: {
                data: dataSource,
                pageSize: 10,
            },
            columns: [
                { field: "employeeAipl", title: "AIPL", width: 100 },
                { field: "employeeName", title: "Name", width: 120 },
                { field: "employeeDesignation", title: "Designation", width: 150 },
                { field: "employeeSkills", title: "Skills", width: 150 },
                { field: "isAllocated", title: "Is Allocated", width: 100, template: "#= isAllocated ? 'Yes' : 'No' #" }
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
                fileName: deptname ? "DepartmentEmployees_" + deptname + ".pdf" : "DepartmentEmployees.pdf", // Set the PDF filename with department name if available
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
        event.preventDefault(); // Prevent the default behavior of the link
        // Construct the URL with the departmentId query parameter
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
    $('#departmentProjectLink').click(function (event) {
        var url = "/Departments/deptProjects?departmentId=" + deptid;
        $.ajax({
            type: "POST",
            url: "/Departments/deptProjects", // Adjust the URL as per your route configuration
            contentType: "application/json",
            data: JSON.stringify({ departmentId: deptid }),
            success: function (response) {
                // Redirect to deptProjects page with departmentId as a query parameter
                window.location.href = "/Departments/deptProjects?departmentId=" + deptid;
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle errors
                console.error("Error:", textStatus, errorThrown);
            }
        });
    });
    $("#deptprojects").click(function () {
        window.location.href = "/Departments/deptProjects?departmentId=" + deptid;

        //// Get the value of the department ID input
        //var departmentId = $("#deptid").val();
        //console.log("DEptid" + deptid);
        //// Make an Ajax request
        //$.ajax({
        //    type: "POST",
        //    url: "/Departments/deptProjects", // Adjust the URL as per your route configuration
        //    contentType: "application/json",
        //    data: JSON.stringify({ departmentId: deptid }),
        //    success: function (response) {
        //        // Redirect to deptProjects page with departmentId as a query parameter
        //        window.location.href = "/Departments/deptProjects?departmentId=" + deptid;
        //    },
        //    error: function (xhr, textStatus, errorThrown) {
        //        // Handle errors
        //        console.error("Error:", textStatus, errorThrown);
        //    }
        //});
    });
    getDeptName();

});
