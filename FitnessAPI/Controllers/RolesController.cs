using FitnessAPI.FitnessDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomwApp.domain.models;

namespace FitnessAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolesController : ControllerBase
    {
        private readonly GymContext _dbContext;
        public RolesController(GymContext dbContext) {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Обрабатывает запрос на получение ролей
        /// </summary>
        /// <returns>Http-ответ, содержащий Список ролей</returns>
        [HttpGet]
        public async Task<ActionResult<List<RoleModel>>> GetRoles()
        {
            try
            {
                var roles = from role in await _dbContext.Roles.ToListAsync()
                            select new RoleModel()
                            {
                                ID_Role = role.IdRoles,
                                RoleName = role.RoleName,
                            };
                return roles.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Обрабатывает запрос на получение роли по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns>Http-ответ, содержащий роль с ID. соответствующим переданному</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleModel>> GetRoleByID(int id)
        {
            try
            {
                var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.IdRoles == id);
                if(role == null)
                    return NotFound();
                return new RoleModel()
                {
                    ID_Role = role.IdRoles,
                    RoleName = role.RoleName,
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
