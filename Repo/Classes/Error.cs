using Amnex_Project_Resource_Mapping_System.Repo.Interfaces;
using Npgsql;
namespace Amnex_Project_Resource_Mapping_System.Repo.Classes
{
    public class Error : IError
    {
        private readonly NpgsqlConnection _connection;
        public Error(NpgsqlConnection connection)
        {
            _connection = connection;
        }
        public void recordError(Models.ErrorModel e)
        {
            using (var cmd = new NpgsqlCommand($"SELECT recordError('{DateTime.Now}','{e.ErrorMessage}','{e.StackTrace}','{e.ControllerName}','{e.ActionName}');", _connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
