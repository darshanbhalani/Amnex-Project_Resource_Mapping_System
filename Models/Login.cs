﻿namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Login
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public int employeeid { get; set; }
    public string employeename   { get; set; }
    public int employeeloginrole {  get; set; }

}
