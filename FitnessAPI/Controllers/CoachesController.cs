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
    public class CoachesController : ControllerBase
    {
        private readonly GymContext _dbContext;
        public CoachesController(GymContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Обрабатывает запрос на получение тренеров
        /// </summary>
        /// <returns>Http-ответ, содержащий список тренеров</returns>
        [HttpGet]
        public async Task<ActionResult<List<CoachModel>>> GetCoaches()
        {
            try
            {
                var coaches = from coach in await _dbContext.Coaches.ToListAsync()
                              select new CoachModel()
                              {
                                  ID_Coaches = coach.IdCoaches,
                                  ID_UserAccounts = coach.IdUserAccounts,
                                  LessonsSchedule = decimal.ToDouble(coach.LessonsSchedule),
                                  MiddleName = coach.MiddleName,
                                  Name = coach.Name,
                                  PhoneNumber = coach.PhoneNumber,
                                  Specialization = coach.Specialization,
                                  Surname = coach.Surname
                              };
                return coaches.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение тренеров, отсротрованных по специальности
        /// </summary>
        /// <param name="specialization">Специальность тренера</param>
        /// <returns>Http-ответ, содержащий  список тренеров, отсротрованных по специальности</returns>
        [HttpGet("byspecialization")]
        public async Task<ActionResult<List<CoachModel>>> GetCoachesBySpecialization([FromQuery(Name = "specialization")] string specialization)
        {
            try
            {
                var coaches = from coach in await _dbContext.Coaches.ToListAsync()
                              where coach.Specialization.Contains(specialization, StringComparison.OrdinalIgnoreCase)
                              select new CoachModel()
                              {
                                  ID_Coaches = coach.IdCoaches,
                                  ID_UserAccounts = coach.IdUserAccounts,
                                  LessonsSchedule = decimal.ToDouble(coach.LessonsSchedule),
                                  MiddleName = coach.MiddleName,
                                  Name = coach.Name,
                                  PhoneNumber = coach.PhoneNumber,
                                  Specialization = coach.Specialization,
                                  Surname = coach.Surname
                              };
                return coaches.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение тренера по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор тренера</param>
        /// <returns>Http-ответ, содержащий тренера с ID, равным переданному</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CoachModel>> GetCoachByID(int id)
        {
            try
            {
                var coach = await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdCoaches == id);
                if (coach == null)
                    return NotFound();
                return new CoachModel()
                {
                    ID_Coaches = coach.IdCoaches,
                    ID_UserAccounts = coach.IdUserAccounts,
                    LessonsSchedule = decimal.ToDouble(coach.LessonsSchedule),
                    MiddleName = coach.MiddleName,
                    Name = coach.Name,
                    PhoneNumber = coach.PhoneNumber,
                    Specialization = coach.Specialization,
                    Surname = coach.Surname
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на добавление тренера
        /// </summary>
        /// <param name="coach">Добаляемый тренер</param>
        /// <returns>Http-ответ, обозначающий результат добавления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<ActionResult> AddCoach(CoachModel coach)
        {
            try
            {
                var maxCoaches = _dbContext.Coaches.Count() == 10;
                var hasAccount = coach.ID_UserAccounts != null && (await _dbContext.Coaches.AnyAsync(c => c.IdUserAccounts == coach.ID_UserAccounts));

                if (maxCoaches || (coach.ID_UserAccounts != null && hasAccount)
                    || string.IsNullOrEmpty(coach.Name)
                    || string.IsNullOrEmpty(coach.Surname)
                    || string.IsNullOrEmpty(coach.Specialization)
                    || string.IsNullOrEmpty(coach.PhoneNumber)
                    || coach.LessonsSchedule <= 0)
                    return Conflict("Coach with that account already exists");

                var newCoach = new Coach()
                {
                    Name = coach.Name,
                    Surname = coach.Surname,
                    MiddleName = coach.MiddleName,
                    Specialization = coach.Specialization,
                    PhoneNumber = coach.PhoneNumber,
                    LessonsSchedule = Convert.ToDecimal(coach.LessonsSchedule),
                    IdUserAccounts = coach.ID_UserAccounts,
                };

                _dbContext.Coaches.Add(newCoach);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на изменение тренера
        /// </summary>
        /// <param name="coach">Измененный тренер</param>
        /// <returns>Http-ответ, обозначающий результат изменения</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPut]
        public async Task<ActionResult> EditCoach(CoachModel coach)
        {
            try
            {
                var editedCoach = await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdCoaches == coach.ID_Coaches);
                if (editedCoach == null)
                    return NotFound();
                var hasAccount = coach.ID_UserAccounts != null && (await _dbContext.Coaches.AnyAsync(c => c.IdUserAccounts == coach.ID_UserAccounts && c.IdCoaches != coach.ID_Coaches));

                if (hasAccount
                    || string.IsNullOrEmpty(coach.Name)
                    || string.IsNullOrEmpty(coach.Surname)
                    || string.IsNullOrEmpty(coach.Specialization)
                    || string.IsNullOrEmpty(coach.PhoneNumber)
                    || coach.LessonsSchedule <= 0)
                    return Conflict();

                editedCoach.Name = coach.Name;
                editedCoach.Surname = coach.Surname;
                editedCoach.MiddleName = coach.MiddleName;
                editedCoach.Specialization = coach.Specialization;
                editedCoach.PhoneNumber = coach.PhoneNumber;
                editedCoach.LessonsSchedule = Convert.ToDecimal(coach.LessonsSchedule);
                editedCoach.IdUserAccounts = coach.ID_UserAccounts;

                _dbContext.Coaches.Update(editedCoach);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на удаление тренера по идентифиактору
        /// </summary>
        /// <param name="id">Идентификатор тренера</param>
        /// <returns>Http-ответ, обозначающий результат удаления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoach(int id)
        {
            try
            {
                var deletedCoach = await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdCoaches == id);
                if (deletedCoach == null)
                    return NotFound();

                _dbContext.Coaches.Remove(deletedCoach);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
