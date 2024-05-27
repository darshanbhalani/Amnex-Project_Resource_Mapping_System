using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class Log
    {
        public string Action { get; set; }
        public string Description {  get; set; }
        public string TableName { get; set; }
        public Dictionary<string, dynamic> OldValue { get; set; }
        public Dictionary<string, dynamic> NewValue { get; set; }
        public string CreatedTime { get; set; }
    }
}
