$(document).ready(function () { 
    function createKendoGrid(selector, dataSource, name) {
        $(selector).kendoGrid({
            dataSource: {
                data: dataSource,
                pageSize: 10,
            },
            columns: [
                { field: "projectName", title: "Name", width: 60, editable: false },
                { field: "startDate", title: "StartDate", width: 120, editable: false },
                { field: "endDate", title: "EndDate", width: 120, editable: false },
                {
                    title: "Progress",
                    width: 200,
                    template: function (dataItem) {
                        var progressPercentage = calculateProgress(new Date(dataItem.startDate), new Date(dataItem.endDate));
                        return `
      <div class="progress-container">
                <div class="progress-bar" style="width: ${progressPercentage}%; animation: progressAnimation 2s;">
                    ${progressPercentage.toFixed(2)}%
                </div>
            </div>
                        `;
                    }
                },
            ],
            editable: false,
            navigatable: true,
            selectable: "row",
            sortable: true,
            pageable: true,
            filterable: true,
            toolbar: [
                {template:`<span class="k-text text-align-center" style="font-size:20px;">${name}</span>`},
            ],
        });
        $(selector).on("click", "tbody > tr", function () {
            var dataItem = $(selector).data("kendoGrid").dataItem($(this));
            // Redirect to new page with project details
            console.log(dataItem);
            console.log(dataItem.projectId);

            window.location.href = "/User/UserProjectDetails?projectId=" + dataItem.projectId;
        });
    }
    function calculateProgress(startDate, endDate) {
        var now = new Date();
        var totalDays = (endDate - startDate) / (1000 * 60 * 60 * 24);
        var passedDays = (now - startDate) / (1000 * 60 * 60 * 24);
        var progressPercentage = (passedDays / totalDays) * 100;
        return Math.min(Math.max(progressPercentage, 0), 100); 
    }


    createKendoGrid("#workingProjectsGrid", workingProjects, "Current Projects");

    createKendoGrid("#notWorkingProjectsGrid", notWorkingProjects,"Past Projects");
});
