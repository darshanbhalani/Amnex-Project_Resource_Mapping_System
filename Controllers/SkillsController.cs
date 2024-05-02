using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class SkillsController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public SkillsController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        
        [HttpPost]
        public IActionResult GetSkills()
        {
            List<Skill> skills = new List<Skill>();
            using (var cmd = new NpgsqlCommand("SELECT s.skillid, s.skillname FROM skills s WHERE isdeleted=false ORDER BY s.skillid;", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        skills.Add(new Skill
                        {
                            Skillid = reader.GetInt32(0),
                            Skillname = reader.GetString(1)
                        });
                    }
                }
            }
            return Json(new { success = true, data = skills });
        }


        [HttpPost]
        public IActionResult AddSkill(Skill model)
        {
            string uid = HttpContext.Session.GetString("userId")!;
            var skillname = model.Skillname;
            try
            {
                using (var command = new NpgsqlCommand("SELECT public.insertskill(@SkillName,@CreatedBy,@ModifyBy,@CreateTime,@ModifyTime,@in_is_deleted);", _connection))
                {
                    command.Parameters.AddWithValue("@SkillName", model.Skillname);
                    command.Parameters.AddWithValue("@CreatedBy", Convert.ToInt32(uid));
                    command.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(uid));
                    command.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                    command.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                    command.Parameters.AddWithValue("@in_is_deleted", false);
                    command.ExecuteNonQuery();
                    return Json(new { success = true });

                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.SqlState == "23505")
                {
                    var isDeleted = checkIfSkillIsDeleted(skillname);
                    int skillId = getSkillId(skillname);
                    if (isDeleted)
                    {
                        updateSkillToNotDeleted(skillname, uid);
                        return Json(new { success = true, skillId = skillId });
                    }
                    else
                    {
                        return Json(new { success = false, message = skillname + " already exists." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "An error occurred while adding the skill. Please try again later." });
                }
            }
        }


        [HttpPost]
        public IActionResult UpdateSkill(int skillId, string updatedSkillName)
        {
            string uid = HttpContext.Session.GetString("userId")!;
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT public.updateskill(@SkillId,@SkillName,@ModifyTime,@ModifyBy);", _connection))
                {
                    cmd.Parameters.AddWithValue("@SkillId", skillId);
                    cmd.Parameters.AddWithValue("@SkillName", updatedSkillName);
                    cmd.Parameters.AddWithValue("@ModifyTime", DateTime.Now); 
                    cmd.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(uid)); 

                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true, data = updatedSkillName });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

                return Json(new { success = false, message = "An error occurred while updating the skill" });
            }
        }


        [HttpPost]
        public IActionResult DeleteSkill(int skillId)
        {
                using (var command = new NpgsqlCommand("select deleteskill(@SkillId,@ModifyTime,@ModifyBy);", _connection))
                {
                    command.Parameters.AddWithValue("@SkillId", skillId); 
                    command.Parameters.AddWithValue("@ModifyTime", DateTime.Now); 
                    command.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(HttpContext.Session.GetString("userId")!)); 

                    command.ExecuteNonQuery();
                }
                return Json(new { success = true });
        }


        private bool checkIfSkillIsDeleted(string skillName)
        {
            try
            {
                using (var command = new NpgsqlCommand("SELECT isdeleted FROM skills WHERE skillname = @SkillName;", _connection))
                {
                    command.Parameters.AddWithValue("@SkillName", skillName);
                    var isDeleted = (bool)command.ExecuteScalar()!;
                    return isDeleted;
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Error checking if skill is deleted: " + ex.Message);
                return false;
            }
        }


        private int getSkillId(string skillName)
        {
            try
            {
                using (var command = new NpgsqlCommand("SELECT skillid FROM skills WHERE skillname = @SkillName;", _connection))
                {
                    command.Parameters.AddWithValue("@SkillName", skillName);
                    var skillId = command.ExecuteScalar();
                    return skillId == null ? -1 : Convert.ToInt32(skillId);
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Error getting skill ID: " + ex.Message);
                return -1;
            }
        }


        private void updateSkillToNotDeleted(string skillName, string uid)
        {
            try
            {
                using (var command = new NpgsqlCommand("UPDATE skills SET isdeleted = false, modifyby = @ModifyBy, modifytime = @ModifyTime WHERE skillname = @SkillName;", _connection))
                {
                    command.Parameters.AddWithValue("@SkillName", skillName);
                    command.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(uid));
                    command.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Error updating skill to not deleted: " + ex.Message);
            }
        }


        internal static List<dynamic> getSkillList(NpgsqlConnection _connection)
        {
            List<dynamic> skills = [];
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.displayallskills();", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<dynamic> x = [];
                        x.Add(Convert.ToInt32(reader.GetValue(0)));
                        x.Add(reader.GetValue(1));
                        skills.Add(x);
                    }
                }
            }
            return skills;
        }

    }
}
