using Amnex_Project_Resource_Mapping_System.Models;
using Amnex_Project_Resource_Mapping_System.Repo.Classes;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public DepartmentsController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }
        public IActionResult AddDepartment(Department department)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("select public.insertdepartment(@in_department_name,@in_created_by,@in_modify_by,@in_create_time,@in_modify_time,@in_is_deleted)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_name", department.DepartmentName);
                    cmd.Parameters.AddWithValue("in_created_by", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                    cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                    cmd.Parameters.AddWithValue("in_create_time", DateTime.Now);
                    cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                    cmd.Parameters.AddWithValue("in_is_deleted", false);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch (NpgsqlException ex)
            {
                if (ex.SqlState == "23505")
                {
                    var fetchdeptid = CheckId(department.DepartmentName);
                    var checkdata = Checkdelete(department.DepartmentName);
                    if (checkdata)
                    {
                        changeIsDeleted(fetchdeptid);
                        return Json(new { success = true });

                    }

                    Response.StatusCode = 400;
                    return Json(new { success = false, error = "Department already exists" });
                }
                else
                {
                    ErrorModel error = new ErrorModel{
                        ErrorMessage= ex.Message,
                        ErrorTime = DateTime.Now,
                        StackTrace = ex.StackTrace,
                        ControllerName = ControllerContext.RouteData.Values["controller"].ToString(),
                        ActionName = ControllerContext.RouteData.Values["action"].ToString()
                };
                    Error.recordError(error, _connection);
                    Response.StatusCode = 500;
                    return Json(new { success = false, error = ex.Message });
                }
            }
        }

        public IActionResult UpdateDepartment(Department department)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("select public.updatedepartment(@in_department_id,@in_department_name,@in_modify_time,@in_modify_by)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_id", department.DepartmentId);
                    cmd.Parameters.AddWithValue("in_department_name", department.DepartmentName);
                    cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                    cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorModel error = new ErrorModel
                {
                    ErrorMessage = ex.Message,
                    ErrorTime = DateTime.Now,
                    StackTrace = ex.StackTrace,
                    ControllerName = ControllerContext.RouteData.Values["controller"].ToString(),
                    ActionName = ControllerContext.RouteData.Values["action"].ToString()
                };
                Error.recordError(error,_connection);
                return Json(new { success = false, error = ex.Message });
            }
        }

        public IActionResult DeleteDepartment(Department department)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("select public.deletedepartment(@in_department_id,@in_modify_time,@in_modify_by)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_id", department.DepartmentId);
                    cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                    cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorModel error = new ErrorModel
                {
                    ErrorMessage = ex.Message,
                    ErrorTime = DateTime.Now,
                    StackTrace = ex.StackTrace,
                    ControllerName = ControllerContext.RouteData.Values["controller"].ToString(),
                    ActionName = ControllerContext.RouteData.Values["action"].ToString()
                };
                Error.recordError(error, _connection);
                return Json(new { success = false, error = ex.Message });
            }
        }

        public int CheckId(string departmentname)
        {
            int departmentId = 0;

            using (var cmd = new NpgsqlCommand("select departmentid from departments where departmentname = @departmentname", _connection))
            {
                cmd.Parameters.AddWithValue("departmentname", departmentname);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    departmentId = Convert.ToInt32(result);
                    return departmentId;
                }
                else
                {
                    return 0;
                }
            }

        }
        public void changeIsDeleted(int DepartmentId)
        {
            using (var cmd = new NpgsqlCommand("update departments set modifyby = @modifyby,modifytime = @modifytime, isDeleted = false where departmentid = @departmentid", _connection))
            {
                cmd.Parameters.AddWithValue("departmentid", DepartmentId);
                cmd.Parameters.AddWithValue("modifyby", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                cmd.Parameters.AddWithValue("modifytime", DateTime.Now);
                cmd.Parameters.AddWithValue("isDeleted", false);
                cmd.ExecuteNonQuery();
            }
        }
        public bool Checkdelete(string departmentname)
        {
            using (var cmd = new NpgsqlCommand("select isdeleted from departments where departmentname = @departmentname", _connection))
            {
                cmd.Parameters.AddWithValue("departmentname", departmentname);
                return (bool)cmd.ExecuteScalar();
            }
        }
    }
}
