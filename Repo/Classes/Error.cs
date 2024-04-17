using Npgsql;
namespace Amnex_Project_Resource_Mapping_System.Repo.Classes
{
    public class Error 
    {
        internal static void recordError(Models.ErrorModel e,NpgsqlConnection connection)
        {
            using (var cmd = new NpgsqlCommand($"SELECT recordError('{DateTime.Now}','{e.ErrorMessage}','{e.StackTrace}','{e.ControllerName}','{e.ActionName}');", connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
