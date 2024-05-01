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
        public IActionResult Skills()
        {
            return View();
        }
        [HttpGet]
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
            string uid = HttpContext.Session.GetString("userId");
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
                if (ex.SqlState == "23505") // Unique constraint violation error code
                {
                    var isDeleted = CheckIfSkillIsDeleted(skillname);
                    int skillId = GetSkillId(skillname);
                    if (isDeleted)
                    {
                        // Update the skill to mark it as not deleted
                        UpdateSkillToNotDeleted(skillname, uid);
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

        private bool CheckIfSkillIsDeleted(string skillName)
        {
            try
            {
                using (var command = new NpgsqlCommand("SELECT isdeleted FROM skills WHERE skillname = @SkillName;", _connection))
                {
                    command.Parameters.AddWithValue("@SkillName", skillName);
                    var isDeleted = (bool)command.ExecuteScalar();
                    return isDeleted;
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
                // Handle the exception if necessary
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
                // Handle the exception if necessary
                Console.WriteLine("Error updating skill to not deleted: " + ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateSkill(int skillId, string updatedSkillName)
        {
            string uid = HttpContext.Session.GetString("userId");
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT public.updateskill(@SkillId,@SkillName,@ModifyTime,@ModifyBy);", _connection))
                {
                    cmd.Parameters.AddWithValue("@SkillId", skillId);
                    cmd.Parameters.AddWithValue("@SkillName", updatedSkillName);
                    cmd.Parameters.AddWithValue("@ModifyTime", DateTime.Now); // Assuming ModifyTime is a timestamp
                    cmd.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(uid)); // ModifyBy could be the user who is updating the skill

                    // Execute the SQL command
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true, data = updatedSkillName });
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine("Error: " + ex.Message);

                // Return an error response
                return Json(new { success = false, message = "An error occurred while updating the skill" });
            }
        }

        [HttpDelete]
        public IActionResult DeleteSkill(int skillId)
        {
            string uid = HttpContext.Session.GetString("userId");

            try
            {
                using (var command = new NpgsqlCommand("select deleteskill(@SkillId,@ModifyTime,@ModifyBy);", _connection))
                {
                    command.Parameters.AddWithValue("@SkillId", skillId); // Assuming skillId is the ID of the skill to delete
                    command.Parameters.AddWithValue("@ModifyTime", DateTime.Now); // Assuming ModifyTime is the current time
                    command.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(uid)); // Assuming modifyBy is the ID of the user performing the modification

                    // Execute the command
                    command.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

    }
}
