using Microsoft.AspNetCore.Mvc;
using Amnex_Project_Resource_Mapping_System.Models;
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
        public IActionResult AddSkill(Skill skill)
        {
            
            try
            {
                using (var command = new NpgsqlCommand("SELECT public.insertskill(@SkillName,@CreatedBy,@ModifyBy,@CreateTime,@ModifyTime,@in_is_deleted);", _connection))
                {
                    command.Parameters.AddWithValue("@SkillName", skill.Skillname);
                    command.Parameters.AddWithValue("@CreatedBy", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                    command.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(HttpContext.Session.GetString("userId")));
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
                    var isDeleted = CheckIfSkillIsDeleted(skill.Skillname);
                    int skillId = GetSkillId(skill.Skillname);
                    if (isDeleted)
                    {
                        UpdateSkillToNotDeleted(skill.Skillname, HttpContext.Session.GetString("userId"));
                        return Json(new { success = true, skillId = skillId });
                    }
                    else
                    {
                        return Json(new { success = false, message = skill.Skillname + " already exists." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "An error occurred while adding the skill. Please try again later." });
                }
            }
        }

        public IActionResult UpdateSkill(int skillId, string updatedSkillName)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT public.updateskill(@SkillId,@SkillName,@ModifyTime,@ModifyBy);", _connection))
                {
                    cmd.Parameters.AddWithValue("@SkillId", skillId);
                    cmd.Parameters.AddWithValue("@SkillName", updatedSkillName);
                    cmd.Parameters.AddWithValue("@ModifyTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(HttpContext.Session.GetString("userId"))); // ModifyBy could be the user who is updating the skill

                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true, message = "Skill updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the skill" });
            }
        }

        public IActionResult DeleteSkill(int skillId)
        {
            try
            {
                using (var command = new NpgsqlCommand("select deleteskill(@SkillId,@ModifyTime,@ModifyBy);", _connection))
                {
                    command.Parameters.AddWithValue("@SkillId", skillId); 
                    command.Parameters.AddWithValue("@ModifyTime", DateTime.Now); 
                    command.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(HttpContext.Session.GetString("userId"))); 

                    command.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private bool CheckIfSkillIsDeleted(string skillName)
        {
            try
            {
                using (var command = new NpgsqlCommand("SELECT isdeleted FROM skills WHERE skillname = @SkillName;", _connection))
                {
                    command.Parameters.AddWithValue("@SkillName", skillName);
                    return (bool)command.ExecuteScalar();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Error checking if skill is deleted: " + ex.Message);
                return false;
            }
        }
        private int GetSkillId(string skillName)
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
        private void UpdateSkillToNotDeleted(string skillName, string uid)
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
    }
}
