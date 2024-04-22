using Microsoft.AspNetCore.Mvc.Rendering;

namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class TempList
    {
        public List<SelectListItem> DropdownOptions { get; set; }
        public List<string> PreselectedValues { get; set; }

    }
}
