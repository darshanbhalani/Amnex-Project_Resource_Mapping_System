$(function () {
   


    var projects = {
        labels: projectsLabel,
        datasets: [{
            label: "Projects",
            data: projectsData,
            backgroundColor: ["rgba(0, 255, 0, 0.2)"],
            borderColor: ["rgba(0, 255, 0, 1)"],
            borderWidth: 1
        }]
    };

    var employees = {
        labels: employeesLabel,
        datasets: [{
            label: "Employees",
            data: employeesData,
            backgroundColor: ["rgba(255, 0, 0, 0.2)"],
            borderColor: ["rgba(255, 0, 0, 1)"],
            borderWidth: 1
        }]
    };

    var departments = {
        labels: departmentsLabel,
        datasets: [{
            label: "Departments",
            data: departmentsData,
            backgroundColor: ["rgba(0, 0, 255, 0.2)"],
            borderColor: ["rgba(0, 0, 255, 1)"],
            borderWidth: 1
        }]
    };

    var combine = {
        labels: departmentsLabel,
        datasets: [{
            label: "Employees",
            data: employeesData,
            backgroundColor: ["rgba(255, 0, 0, 0.2)"],
            borderColor: ["rgba(255, 0, 0, 1)"],
            borderWidth: 1
        }, {
            label: "Projects",
            data: projectsData,
            backgroundColor: ["rgba(0, 255, 0, 0.2)"],
            borderColor: ["rgba(0, 255, 0, 1)"],
            borderWidth: 1
        }, {
            label: "Departments",
            data: departmentsData,
            backgroundColor: ["rgba(0, 0, 255, 0.2)"],
            borderColor: ["rgba(0, 0, 255, 1)"],
            borderWidth: 1
        }]
    };

    var logs = {
        labels: dateLable,
        datasets: [{
            label: "Inserts",
            data: insertData,
            backgroundColor: ["rgba(0, 255, 0, 0.2)"],
            borderColor: ["rgba(0, 255, 0, 1)"],
            borderWidth: 1
        }, {
            label: "Updates",
            data: updatetData,
            backgroundColor: ["rgba(0, 0, 255, 0.2)"],
            borderColor: ["rgba(0, 0, 255, 1)"],
            borderWidth: 1
        }, {
            label: "Deletes",
            data: deleteData,
            backgroundColor: ["rgba(255, 0, 0, 0.2)"],
            borderColor: ["rgba(255, 0, 0, 1)"],
            borderWidth: 1
        }]
    };

    var projectStatus = {
        labels: DepartmentLable,
        datasets: [{
            label: "Pending Projects",
            data: pendingProjects,
            backgroundColor: ["rgba(0, 255, 0, 0.2)"],
            borderColor: ["rgba(0, 255, 0, 1)"],
            borderWidth: 1
        }, {
            label: "Running Projects",
            data: runningProjects,
            backgroundColor: ["rgba(0, 0, 255, 0.2)"],
            borderColor: ["rgba(0, 0, 255, 1)"],
            borderWidth: 1
        }, {
            label: "Completed Projects",
            data: completedProjects,
            backgroundColor: ["rgba(255, 0, 0, 0.2)"],
            borderColor: ["rgba(255, 0, 0, 1)"],
            borderWidth: 1
        }]
    };

    var employeeStatus = {
        labels: DepartmentLable2,
        datasets: [{
            label: "Allocated Employees",
            data: allocatedEmployees,
            backgroundColor: ["rgba(0, 255, 0, 0.2)"],
            borderColor: ["rgba(0, 255, 0, 1)"],
            borderWidth: 1
        }, {
            label: "Unallocated Employees",
            data: unAllocatedEmployees,
            backgroundColor: ["rgba(0, 0, 255, 0.2)"],
            borderColor: ["rgba(0, 0, 255, 1)"],
            borderWidth: 1
        }]
    };

    var predefinedColors = [
        "#80FFFF", "#FFC080", "#C080FF", "#80C0FF", "#C0FF80",
        "#00FF80", "#FF0080", "#FF8000", "#0080FF", "#8000FF",
        "#FF8080", "#80FF80", "#8080FF", "#FFFF80", "#FF80FF",
        "#80FFFF", "#FFC080", "#C080FF", "#80C0FF", "#C0FF80",
        "#80FFC0", "#FF80C0", "#FFC080", "#80C0FF", "#C080FF",
        "#FF8080", "#80FF80", "#8080FF", "#FFFF80", "#FF80FF",
        "#FF0000", "#00FF00", "#0000FF", "#FFFF00", "#FF00FF",
        "#00FFFF", "#FF8000", "#8000FF", "#0080FF", "#80FF00",
        "#80FFC0", "#FF80C0", "#FFC080", "#80C0FF", "#C080FF",
        "#FF8080", "#80FF80", "#8080FF", "#FFFF80", "#FF80FF"
    ];


    var currentIndex = 0;

    function getRandomColor() {
        if (currentIndex < predefinedColors.length) {
            return predefinedColors[currentIndex++];
        } else {
            var letters = '0123456789ABCDEF';
            var color = '#';
            for (var i = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            return color;
        }
    }

    var departmentProject = {
        labels: departmentProjecLable,
        datasets: [{
            label: "Projects",
            data: departmentProjectData,
            backgroundColor: [],
            borderColor: [],
            borderWidth: 0
        }]
    };
    var departmentEmployee = {
        labels: departmentEmployeeLable,
        datasets: [{
            label: "Employees",
            data: departmentEmployeeData,
            backgroundColor: [],
            borderColor: [],
            borderWidth: 0
        }]
    };
    // add random color in list
    departmentProject.labels.forEach(function (label, index) {
        departmentProject.datasets[0].backgroundColor.push(getRandomColor());
        departmentProject.datasets[0].borderColor.push(getRandomColor());
    });
    departmentEmployee.labels.forEach(function (label, index) {
        departmentEmployee.datasets[0].backgroundColor.push(getRandomColor());
        departmentEmployee.datasets[0].borderColor.push(getRandomColor());
    });

    var totalDepartmentProject = departmentProject.datasets[0].data.reduce((a, b) => a + b, 0);
    departmentProject.labels.forEach((label, index) => {
        var percentage = ((departmentProject.datasets[0].data[index] / totalDepartmentProject) * 100).toFixed(2) + "%";
        departmentProject.labels[index] = label + " (" + percentage + ")";
    });

    var totalDepartmentEmployee = departmentEmployee.datasets[0].data.reduce((a, b) => a + b, 0);
    departmentEmployee.labels.forEach((label, index) => {
        var percentage = ((departmentEmployee.datasets[0].data[index] / totalDepartmentEmployee) * 100).toFixed(2) + "%";
        departmentEmployee.labels[index] = label + " (" + percentage + ")";
    });

    // var employeesCtx = document.getElementById("employeesChart").getContext("2d");
    // var projectsCtx = document.getElementById("projectsChart").getContext("2d");
    // var departmentsCtx = document.getElementById("departmentsChart").getContext("2d");
    var combineCtx = document.getElementById("combineChart").getContext("2d");
    var logsCtx = document.getElementById("logsChart").getContext("2d");
    var departmentProjectCtx = document.getElementById("departmentProjectChart").getContext("2d");
    var departmentEmployeeCtx = document.getElementById("departmentEmployeeChart").getContext("2d");
    var projectStatusCtx = document.getElementById("projectStatusChart").getContext("2d");
    var employeeStatusCtx = document.getElementById("employeeStatusChart").getContext("2d");

    // Create different types of charts
    // var employeesChart = new Chart(employeesCtx, {
    //     type: "line",
    //     data: employees
    // });
    // var projectsChart = new Chart(projectsCtx, {
    //     type: "line",
    //     data: projects
    // });
    // var departmentsChart = new Chart(departmentsCtx, {
    //     type: "line",
    //     data: departments
    // });
    var combineChart = new Chart(combineCtx, {
        type: "line",
        data: combine
    });


    var departmentProjectChart = new Chart(departmentProjectCtx, {
        type: "doughnut",
        data: departmentProject,
        options: {
            cutoutPercentage: 80, // Adjust as needed
            animation: {
                animateScale: true
            },
            legend: {
                display: true,
                position: 'bottom',
                labels: {
                    generateLabels: function (chart) {
                        return chart.data.labels.map(function (label, index) {
                            return {
                                text: label,
                                fillStyle: chart.data.datasets[0].backgroundColor[index],
                                hidden: isNaN(chart.data.datasets[0].data[index]) || chart.data.datasets[0].data[index] === 0
                            };
                        });
                    }
                }
            }
        }
    });

    var departmentEmployeeChart = new Chart(departmentEmployeeCtx, {
        type: "doughnut",
        data: departmentEmployee,
        options: {
            cutoutPercentage: 80, // Adjust as needed
            animation: {
                animateScale: true
            },
            legend: {
                display: true,
                position: 'bottom',
                labels: {
                    generateLabels: function (chart) {
                        return chart.data.labels.map(function (label, index) {
                            return {
                                text: label,
                                fillStyle: chart.data.datasets[0].backgroundColor[index],
                                hidden: isNaN(chart.data.datasets[0].data[index]) || chart.data.datasets[0].data[index] === 0
                            };
                        });
                    }
                }
            }
        }
    });

    var logsChart = new Chart(logsCtx, {
        type: "bar",
        data: logs,
    });

    var projectStatusChart = new Chart(projectStatusCtx, {
        type: "bar",
        data: projectStatus
    });

    var employeeStatusChart = new Chart(employeeStatusCtx, {
        type: "bar",
        data: employeeStatus
    });

});
