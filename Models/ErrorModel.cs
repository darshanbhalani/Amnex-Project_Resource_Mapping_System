using System.ComponentModel.DataAnnotations;

namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class ErrorModel
    {
        public int ErrorId { get; set; }

        public DateTime ErrorTime { get; set; }

        public string ErrorMessage { get; set; }

        public string? StackTrace { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }
    }
}
