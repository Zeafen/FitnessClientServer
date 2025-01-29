using FitnessAPI.Data.Helpers;
using FitnessAPI.domain.models;
using FitnessAPI.FitnessDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventSource;
using SomwApp.domain.models;
using System.IdentityModel.Tokens.Jwt;

namespace FitnessAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LessonsController : ControllerBase
    {
        private readonly GymContext _dbContext;
        public LessonsController(GymContext gymContext)
        {
            _dbContext = gymContext;
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех занятий
        /// </summary>
        /// <returns>Http-ответ, содержащий список всех занятий</returns>
        [HttpGet]
        public async Task<ActionResult<List<LessonModel>>> GetLessons()
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<LessonModel>? lessons = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                        lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                                   where lesson.IdCoaches == coachID
                                   select new LessonModel()
                                   {
                                       ID_Lessons = lesson.IdLessons,
                                       ID_Branches = lesson.IdBranches,
                                       ID_Coaches = lesson.IdCoaches,
                                       Date = lesson.Date,
                                       DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                       Title = lesson.Title,
                                       Time = lesson.Time,
                                       NumberOfPracticants = lesson.NumberOfPracticants,
                                   }).ToList();
                }
                if (lessons == null)
                    lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                               select new LessonModel()
                               {
                                   ID_Lessons = lesson.IdLessons,
                                   ID_Branches = lesson.IdBranches,
                                   ID_Coaches = lesson.IdCoaches,
                                   Date = lesson.Date,
                                   DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                   Title = lesson.Title,
                                   Time = lesson.Time,
                                   NumberOfPracticants = lesson.NumberOfPracticants,
                               }).ToList();

                return lessons;
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение занятий в ближайшее время
        /// </summary>
        /// <returns>Http-ответ, содержащий список всех занятий, отсортированых по дате проведения</returns>
        [HttpGet("recent")]
        public async Task<ActionResult<List<LessonModel>>> GetRecentLessons()
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<LessonModel>? lessons = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                        lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                                   where lesson.IdCoaches == coachID
                                   && lesson.Date.CompareTo(DateOnly.FromDateTime(DateTime.Now)) == 0
                                   && int.Abs(lesson.Time.Hour - DateTime.Now.Hour) <= 4
                                   select new LessonModel()
                                   {
                                       ID_Lessons = lesson.IdLessons,
                                       ID_Branches = lesson.IdBranches,
                                       ID_Coaches = lesson.IdCoaches,
                                       Date = lesson.Date,
                                       DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                       Title = lesson.Title,
                                       Time = lesson.Time,
                                       NumberOfPracticants = lesson.NumberOfPracticants,
                                   }).ToList();
                }
                if (lessons == null)
                    lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                               where lesson.Date.CompareTo(DateOnly.FromDateTime(DateTime.Now)) == 0
                                   && int.Abs(lesson.Time.Hour - DateTime.Now.Hour) <= 4
                               select new LessonModel()
                               {
                                   ID_Lessons = lesson.IdLessons,
                                   ID_Branches = lesson.IdBranches,
                                   ID_Coaches = lesson.IdCoaches,
                                   Date = lesson.Date,
                                   DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                   Title = lesson.Title,
                                   Time = lesson.Time,
                                   NumberOfPracticants = lesson.NumberOfPracticants,
                               }).ToList();

                return lessons;
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение занятий по идентификатору тренера
        /// </summary>
        /// <param name="id">Идентификатор тренера</param>
        /// <returns>Http-ответ, содержащий список занятий, отсортированных по идентификатору тренера</returns>
        [HttpGet("bycoach/{id}")]
        public async Task<ActionResult<List<LessonModel>>> GetLessonsByCoach(int id)
        {
            try
            {
                var exists = await _dbContext.Coaches.AnyAsync(c => c.IdCoaches == id);
                if(!exists)
                    return NotFound();

                var lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                           where lesson.IdCoaches == id
                           select new LessonModel()
                           {
                               ID_Lessons = lesson.IdLessons,
                               ID_Branches = lesson.IdBranches,
                               ID_Coaches = lesson.IdCoaches,
                               Date = lesson.Date,
                               DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                               Title = lesson.Title,
                               Time = lesson.Time,
                               NumberOfPracticants = lesson.NumberOfPracticants,
                           }).ToList();

                return lessons;

            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение занятий, отсортированных по дате проведения
        /// </summary>
        /// <param name="date">Дата проведения</param>
        /// <returns>Http-ответ, содержащий список занятий, отсортированных по дате проведения</returns>
        [HttpGet("bydate")]
        public async Task<ActionResult<List<LessonModel>>> GetLessonsByDate([FromQuery(Name ="date")]DateOnly date)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<LessonModel>? lessons = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                        lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                                   where lesson.IdCoaches == coachID && lesson.Date.CompareTo(date) == 0
                                   select new LessonModel()
                                   {
                                       ID_Lessons = lesson.IdLessons,
                                       ID_Branches = lesson.IdBranches,
                                       ID_Coaches = lesson.IdCoaches,
                                       Date = lesson.Date,
                                       DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                       Title = lesson.Title,
                                       Time = lesson.Time,
                                       NumberOfPracticants = lesson.NumberOfPracticants,
                                   }).ToList();
                }
                if (lessons == null)
                    lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                               where lesson.Date.CompareTo(date) == 0
                               select new LessonModel()
                               {
                                   ID_Lessons = lesson.IdLessons,
                                   ID_Branches = lesson.IdBranches,
                                   ID_Coaches = lesson.IdCoaches,
                                   Date = lesson.Date,
                                   DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                   Title = lesson.Title,
                                   Time = lesson.Time,
                                   NumberOfPracticants = lesson.NumberOfPracticants,
                               }).ToList();

                return lessons;

            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }


        /// <summary>
        /// Обрабатывает запрос на получение занятий, отсортироавнных по дате проведеиня в передаваемый период
        /// </summary>
        /// <param name="request">Период, в который проводились занятия</param>
        /// <returns>Http-ответ, содержащий список занятий, отсортироавнных по дате проведеиня в передаваемый период</returns>
        [HttpPost("inperiod")]
        public async Task<ActionResult<List<LessonModel>>> GetLessonsInPeriod([FromBody]DatePeriodRequest request)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<LessonModel>? lessons = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                        lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                                   where lesson.IdCoaches == coachID 
                                   && lesson.Date.CompareTo(request.DateFrom) >= 0 && lesson.Date.CompareTo(request.DateTo) <= 0
                                   select new LessonModel()
                                   {
                                       ID_Lessons = lesson.IdLessons,
                                       ID_Branches = lesson.IdBranches,
                                       ID_Coaches = lesson.IdCoaches,
                                       Date = lesson.Date,
                                       DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                       Title = lesson.Title,
                                       Time = lesson.Time,
                                       NumberOfPracticants = lesson.NumberOfPracticants,
                                   }).ToList();
                }
                if (lessons == null)
                    lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                               where lesson.Date.CompareTo(request.DateFrom) >= 0 && lesson.Date.CompareTo(request.DateTo) <= 0
                               select new LessonModel()
                               {
                                   ID_Lessons = lesson.IdLessons,
                                   ID_Branches = lesson.IdBranches,
                                   ID_Coaches = lesson.IdCoaches,
                                   Date = lesson.Date,
                                   DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                   Title = lesson.Title,
                                   Time = lesson.Time,
                                   NumberOfPracticants = lesson.NumberOfPracticants,
                               }).ToList();

                return lessons;

            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение занятий, отсортированных по времени проведения
        /// </summary>
        /// <param name="time">Время проведения</param>
        /// <returns>Http-ответ, содержащий список занятий, отсортированных по времени проведения</returns>
        [HttpGet("bytime")]
        public async Task<ActionResult<List<LessonModel>>> GetLessonsByTime([FromQuery(Name ="time")]TimeOnly time)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<LessonModel>? lessons = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                        lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                                   where lesson.IdCoaches == coachID
                                   && lesson.Time.CompareTo(time) == 0
                                   select new LessonModel()
                                   {
                                       ID_Lessons = lesson.IdLessons,
                                       ID_Branches = lesson.IdBranches,
                                       ID_Coaches = lesson.IdCoaches,
                                       Date = lesson.Date,
                                       DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                       Title = lesson.Title,
                                       Time = lesson.Time,
                                       NumberOfPracticants = lesson.NumberOfPracticants,
                                   }).ToList();
                }
                if (lessons == null)
                    lessons = (from lesson in await _dbContext.Lessons.ToListAsync()
                               where lesson.Time.CompareTo(time) == 0
                               select new LessonModel()
                               {
                                   ID_Lessons = lesson.IdLessons,
                                   ID_Branches = lesson.IdBranches,
                                   ID_Coaches = lesson.IdCoaches,
                                   Date = lesson.Date,
                                   DurationClasses = decimal.ToDouble(lesson.DurationClasses),
                                   Title = lesson.Title,
                                   Time = lesson.Time,
                                   NumberOfPracticants = lesson.NumberOfPracticants,
                               }).ToList();

                return lessons;

            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение занятия по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор занятия</param>
        /// <returns>Http-ответ, содержащий занятие с ID, соответствующим переданному</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LessonModel>> GetLessonsByID(int id)
        {
            try
            {
                if (!await _dbContext.Lessons.AnyAsync(l => l.IdLessons == id))
                    return NotFound();
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                LessonModel? lesson = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                    {
                        var searched = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == id);
                        if(searched!.IdCoaches != coachID.Value)
                            return Conflict("This lesson belong to another coach");
                        lesson = new LessonModel()
                        {
                            ID_Lessons = searched.IdLessons,
                            ID_Branches = searched.IdBranches,
                            ID_Coaches = searched.IdCoaches,
                            Date = searched.Date,
                            DurationClasses = decimal.ToDouble(searched.DurationClasses),
                            Title = searched.Title,
                            Time = searched.Time,
                            NumberOfPracticants = searched.NumberOfPracticants,
                        };
                    }
                }
                if (lesson == null)
                {
                    var searched = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == id);
                    lesson = new LessonModel()
                    {
                        ID_Lessons = searched!.IdLessons,
                        ID_Branches = searched.IdBranches,
                        ID_Coaches = searched.IdCoaches,
                        Date = searched.Date,
                        DurationClasses = decimal.ToDouble(searched.DurationClasses),
                        Title = searched.Title,
                        Time = searched.Time,
                        NumberOfPracticants = searched.NumberOfPracticants,
                    };
                }
                return lesson;
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на добавление занятия
        /// </summary>
        /// <param name="lesson">Добавляемое занятие</param>
        /// <returns>Http-ответ, обозначающий результат добавления</returns>
        [HttpPost]
        public async Task<ActionResult> AddLesson(LessonModel lesson)
        {
            try
            {
                var emptyFields = string.IsNullOrEmpty(lesson.Title);
                var illegalNumbers = lesson.NumberOfPracticants < 0 || lesson.DurationClasses < 0 || lesson.DurationClasses > 5;
                if (illegalNumbers || emptyFields)
                    return Conflict("Illegal data format");

                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                Lesson? newLesson = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID.HasValue)
                    {
                        var hasAlready = await _dbContext.Lessons.AnyAsync(l =>
                        l.IdCoaches == coachID
                        && l.IdBranches == lesson.ID_Branches
                        && l.Time.CompareTo(lesson.Time) == 0
                        && l.Date.CompareTo(lesson.Date) == 0);
                        if (hasAlready)
                            return Conflict("Lesson already exists");

                        var coachBusy = (await _dbContext.Lessons.ToListAsync()).Any(l =>
                        l != null && l.IdCoaches == coachID
                        && DateTimeCrossChecker.DoDatesCross(
                            l.Date.ToDateTime(l.Time), l.Date.ToDateTime(l.Time.AddHours(Convert.ToDouble(l.DurationClasses))),
                            lesson.Date.ToDateTime(lesson.Time), lesson.Date.ToDateTime(lesson.Time.AddHours(Convert.ToDouble(lesson.DurationClasses)))
                            ));

                        if (coachBusy)
                            return Conflict("Coach is busy at this time");

                        var coachCantGetTo = await _dbContext.Lessons.AnyAsync(l =>
                        l.IdCoaches == coachID
                        && l.IdBranches != lesson.ID_Branches);
                        if (coachCantGetTo)
                            return Conflict("Coach cant get to this branch");

                        newLesson = new Lesson()
                        {
                            Date = lesson.Date,
                            Time = lesson.Time,
                            DurationClasses = Convert.ToDecimal(lesson.DurationClasses),
                            IdBranches = lesson.ID_Branches,
                            IdCoaches = coachID.Value,
                            NumberOfPracticants = lesson.NumberOfPracticants,
                            Title = lesson.Title,
                        };
                    }
                }
                if (newLesson == null)
                {
                    var hasAlready = await _dbContext.Lessons.AnyAsync(l =>
                    l.IdCoaches == lesson.ID_Coaches
                    && l.IdBranches == lesson.ID_Branches
                    && l.Time.CompareTo(lesson.Time) == 0
                    && l.Date.CompareTo(lesson.Date) == 0);
                    if (hasAlready)
                        return Conflict("Lesson already exists");

                    var coachBusy = (await _dbContext.Lessons.ToListAsync()).Any(l =>
                    l != null && l.IdCoaches == lesson.ID_Coaches
                    && DateTimeCrossChecker.DoDatesCross(
                        l.Date.ToDateTime(l.Time), l.Date.ToDateTime(l.Time.AddHours(Convert.ToDouble(l.DurationClasses))),
                        lesson.Date.ToDateTime(lesson.Time), lesson.Date.ToDateTime(lesson.Time.AddHours(Convert.ToDouble(lesson.DurationClasses)))
                        ));

                    if (coachBusy)
                        return Conflict("Coach is busy at this time");

                    var coachCantGetTo = await _dbContext.Lessons.AnyAsync(l =>
                    l.IdCoaches == lesson.ID_Coaches
                    && l.IdBranches != lesson.ID_Branches);
                    if (coachCantGetTo)
                        return Conflict("Coach cant get to this branch");

                    newLesson = new Lesson()
                    {
                        Date = lesson.Date,
                        Time = lesson.Time,
                        DurationClasses = Convert.ToDecimal(lesson.DurationClasses),
                        IdBranches = lesson.ID_Branches,
                        IdCoaches = lesson.ID_Coaches,
                        NumberOfPracticants = lesson.NumberOfPracticants,
                        Title = lesson.Title,
                    };
                }


                _dbContext.Lessons.Add(newLesson);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на изменение занятия
        /// </summary>
        /// <param name="lesson">Измененное занятие</param>
        /// <returns>Http-ответ, обозначающий результат изменения</returns>
        [HttpPut]
        public async Task<ActionResult> EditLesson(LessonModel lesson)
        {
            try
            {
                if(!await _dbContext.Lessons.AnyAsync(l => l.IdLessons == lesson.ID_Lessons))
                    return NotFound();

                var emptyFields = string.IsNullOrEmpty(lesson.Title);
                var illegalNumbers = lesson.NumberOfPracticants < 0 || lesson.DurationClasses < 0 || lesson.DurationClasses > 5;
                if (illegalNumbers || emptyFields)
                    return Conflict("Illegal data format");

                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                Lesson? editedLesson = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID.HasValue)
                    {
                        var hasAlready = await _dbContext.Lessons.AnyAsync(l =>
                        l.IdLessons != lesson.ID_Lessons &&
                        l.IdCoaches == coachID
                        && l.IdBranches == lesson.ID_Branches
                        && l.Time.CompareTo(lesson.Time) == 0
                        && l.Date.CompareTo(lesson.Date) == 0);
                        if (hasAlready)
                            return Conflict("Lesson already exists");


                        var coachBusy = (await _dbContext.Lessons.ToListAsync()).Any(l =>
                    l != null &&
                    l.IdLessons != lesson.ID_Lessons && l.IdCoaches == coachID
                    && DateTimeCrossChecker.DoDatesCross(
                        l.Date.ToDateTime(l.Time), l.Date.ToDateTime(l.Time.AddHours(Convert.ToDouble(l.DurationClasses))),
                        lesson.Date.ToDateTime(lesson.Time), lesson.Date.ToDateTime(lesson.Time.AddHours(Convert.ToDouble(lesson.DurationClasses)))
                        ));

                        if (coachBusy)
                            return Conflict("Coach is busy at this time");

                        var coachCantGetTo = await _dbContext.Lessons.AnyAsync(l =>
                        l.IdLessons != lesson.ID_Lessons &&
                        l.IdCoaches == coachID
                        && l.IdBranches != lesson.ID_Branches);
                        if (coachCantGetTo)
                            return Conflict("Coach cant get to this branch");

                        editedLesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == lesson.ID_Lessons);

                        if (editedLesson!.IdCoaches != coachID)
                            return Conflict("This lesson belongs to another coach");

                        editedLesson.Date = lesson.Date;
                        editedLesson.Time = lesson.Time;
                        editedLesson.DurationClasses = Convert.ToDecimal(lesson.DurationClasses);
                        editedLesson.IdBranches = lesson.ID_Branches;
                        editedLesson.NumberOfPracticants = lesson.NumberOfPracticants;
                        editedLesson.Title = lesson.Title;
                    }
                }
                if (editedLesson == null)
                {
                    if (!await _dbContext.Coaches.AnyAsync(c => c.IdCoaches == lesson.ID_Coaches))
                        return Conflict("Coach not found");

                    var hasAlready = await _dbContext.Lessons.AnyAsync(l =>
                    l.IdLessons != lesson.ID_Lessons &&
                    l.IdCoaches == lesson.ID_Coaches
                    && l.IdBranches == lesson.ID_Branches
                    && l.Time.CompareTo(lesson.Time) == 0
                    && l.Date.CompareTo(lesson.Date) == 0);
                    if (hasAlready)
                        return Conflict("Lesson already exists");

                    var coachBusy = (await _dbContext.Lessons.ToListAsync()).Any(l =>
                    l != null &&
                    l.IdLessons != lesson.ID_Lessons
                    && l.IdCoaches == lesson.ID_Coaches
                    && DateTimeCrossChecker.DoDatesCross(
                        l.Date.ToDateTime(l.Time), l.Date.ToDateTime(l.Time.AddHours(Convert.ToDouble(l.DurationClasses))),
                        lesson.Date.ToDateTime(lesson.Time), lesson.Date.ToDateTime(lesson.Time.AddHours(Convert.ToDouble(lesson.DurationClasses)))
                        ));

                    if (coachBusy)
                        return Conflict("Coach is busy at this time");

                    var coachCantGetTo = await _dbContext.Lessons.AnyAsync(l =>
                    l.IdLessons != lesson.ID_Lessons &&
                    l.IdCoaches == lesson.ID_Coaches
                    && l.IdBranches != lesson.ID_Branches);
                    if (coachCantGetTo)
                        return Conflict("Coach cant get to this branch");

                    editedLesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == lesson.ID_Lessons);

                    editedLesson!.Date = lesson.Date;
                    editedLesson.Time = lesson.Time;
                    editedLesson.DurationClasses = Convert.ToDecimal(lesson.DurationClasses);
                    editedLesson.IdBranches = lesson.ID_Branches;
                    editedLesson.IdCoaches = lesson.ID_Coaches;
                    editedLesson.NumberOfPracticants = lesson.NumberOfPracticants;
                    editedLesson.Title = lesson.Title;
                }

                _dbContext.Lessons.Update(editedLesson);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на удаление занятия по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор занятия</param>
        /// <returns>Http-ответ, обозначающий результат удаления</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLesson(int id)
        {
            try
            {
                if(!await _dbContext.Lessons.AnyAsync(l => l.IdLessons == id))
                    return NotFound();

                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                Lesson? deletedLesson = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID.HasValue)
                    {
                        deletedLesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == id);

                        if (deletedLesson!.IdCoaches != coachID)
                            return Conflict("This lesson belongs to another coach");
                    }
                }
                if (deletedLesson == null)
                {
                    deletedLesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == id);
                }

                _dbContext.Lessons.Remove(deletedLesson!);
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
