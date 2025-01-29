using FitnessAPI.FitnessDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomwApp.domain.models;
using System.Diagnostics;

namespace FitnessAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BranchesController : ControllerBase
    {
        private readonly GymContext _dbContext;

        public BranchesController(GymContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        ///  Обрабатывает запрос на получение всех записей филиалов
        /// </summary>
        /// <returns>Возвращает все записи филиалов, которые содержатся в бд</returns>
        [HttpGet]
        public async Task<ActionResult<List<BranchesModel>>> GetBranches()
        {

            try
            {
                var branches = from branch in await _dbContext.Branches.ToListAsync()
                               select new BranchesModel()
                               {
                                   ID_Branches = branch.IdBracnches,
                                   AddressBranches = branch.AddressBranches,
                                   Name_Branches = branch.NameBranches,
                               };
                return branches.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение филиала по id
        /// </summary>
        /// <param name="id"> Идентификатор филиала</param>
        /// <returns>Запсись с ID, соответствующую запрошенному</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BranchesModel>> GetBranch(int id)
        {
            try
            {
                var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.IdBracnches == id);
                if (branch == null)
                    return NotFound();
                return new BranchesModel()
                {
                    ID_Branches = branch.IdBracnches,
                    AddressBranches = branch.AddressBranches,
                    Name_Branches = branch.NameBranches,
                };
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на добавление филиала
        /// </summary>
        /// <param name="branch">Добавляемый филиал</param>
        /// <returns>Http-ответ, обозначающий результат добавления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<ActionResult<BranchesModel>> AddBranch(BranchesModel branch)
        {
            try
            {
                var exists = await _dbContext.Branches.AnyAsync(b => b.AddressBranches == branch.AddressBranches);
                if(exists
                    || string.IsNullOrEmpty(branch.AddressBranches)
                    ||string.IsNullOrEmpty(branch.Name_Branches))
                    return Conflict("Such branch already exists. Check if name or(and) address are correct");
                var newBranch = new Branch()
                {
                    AddressBranches = branch.AddressBranches,
                    NameBranches = branch.Name_Branches,
                };
                _dbContext.Branches.Add(newBranch);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на обновление филиала
        /// </summary>
        /// <param name="branch">Обновляемы филиал</param>
        /// <returns>Http-ответ, обозначающий результат изменения</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPut]
        public async Task<ActionResult<BranchesModel>> EditBranch(BranchesModel branch)
        {
            try
            {
                var editedBranch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.IdBracnches == branch.ID_Branches);
                if (editedBranch == null)
                    return NotFound();
                var exists = await _dbContext.Branches.AnyAsync(b => b.AddressBranches == branch.AddressBranches);
                if (exists
                    || string.IsNullOrEmpty(branch.AddressBranches)
                    || string.IsNullOrEmpty(branch.Name_Branches))
                    return Conflict();

                editedBranch.AddressBranches = branch.AddressBranches;
                editedBranch.NameBranches = branch.Name_Branches;

                _dbContext.Branches.Update(editedBranch);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на удаление филиала
        /// </summary>
        /// <param name="id">Идентификатор филиала</param>
        /// <returns>Http-ответ, обозначающий результат удаления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BranchesModel>> DeleteBranch(int id)
        {
            try
            {
                var deletedBranch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.IdBracnches == id);
                if (deletedBranch == null)
                    return NotFound();

                _dbContext.Branches.Remove(deletedBranch);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        } 
    }
}
