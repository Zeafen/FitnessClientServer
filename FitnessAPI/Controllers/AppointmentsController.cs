using FitnessAPI.Data.Helpers;
using FitnessAPI.domain.models;
using FitnessAPI.FitnessDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomwApp.domain.models;
using System.IdentityModel.Tokens.Jwt;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitnessAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly GymContext _dbContext;
        public AppointmentsController(GymContext gymContext)
        {
            _dbContext = gymContext;
        }

        private async Task<bool> CheckCustomerAbleToBeSigned(int customerID)
        {
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomers == customerID);
            if (customer == null)
                return false;

            var payments = await _dbContext.Payments.Where(p => p.IdCustomers == customer.IdCustomers).ToListAsync();
            if (payments.Count == 0) return false;

            var lastActivePay = payments.MaxBy(p => p.ValidityEndDate);
            if (lastActivePay == null || lastActivePay.ValidityEndDate.CompareTo(DateOnly.FromDateTime(DateTime.Now)) < 0) return false;

            var lastActiveSub = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.IdSubscription == lastActivePay.IdSubscription);
            if (lastActiveSub == null) return false;

            var appointmentsNum = await _dbContext.AppointmentsForClasses.CountAsync(ap =>
            ap.IdCustomers == customer.IdCustomers && ap.DateRecording.CompareTo(lastActivePay.PaymentDate) > 0);

            if (appointmentsNum == 0 || appointmentsNum < lastActiveSub.NumberofVisits)
                return true;

            return false;
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех посещений
        /// </summary>
        /// <returns>Http-ответ, содержащий список всех посещений</returns>
        [HttpGet]
        public async Task<ActionResult<List<AppointmentsForClassesModel>>> GetAppointments()
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<AppointmentsForClassesModel>? appointments = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null){
                        var lessons = from lesson in await _dbContext.Lessons.ToListAsync()
                                      where lesson.IdCoaches == coachID
                                      select lesson.IdLessons;
                        appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                   where lessons.Contains(appointment.IdLessons)
                                   select new AppointmentsForClassesModel()
                                   {
                                       ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                       Date_recording = appointment.DateRecording,
                                       ID_Customers = appointment.IdCustomers,
                                       ID_Lessons = appointment.IdLessons,
                                       Status_recording = appointment.StatusRecording,
                                   }).ToList();
                    }
                }
                if (appointments == null)
                    appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                    select new AppointmentsForClassesModel()
                                    {
                                        ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                        Date_recording = appointment.DateRecording,
                                        ID_Customers = appointment.IdCustomers,
                                        ID_Lessons = appointment.IdLessons,
                                        Status_recording = appointment.StatusRecording,
                                    }).ToList();

                return appointments;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение посещений, отсортированных по идентификатору клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Http-ответ, содержащий список посещений, отсортированных по идентификатору клиента</returns>
        [HttpGet("bycustomer/{id}")]
        public async Task<ActionResult<List<AppointmentsForClassesModel>>> GetAppointmentsByCustomer(int id)
        {
            try
            {
                if (!(await _dbContext.Customers.AllAsync(c => c.IdCustomers == id)))
                    return NotFound();
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<AppointmentsForClassesModel>? appointments = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                    {
                        var lessons = from lesson in await _dbContext.Lessons.ToListAsync()
                                      where lesson.IdCoaches == coachID
                                      select lesson.IdLessons;
                        appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                        where lessons.Contains(appointment.IdLessons)
                                        && appointment.IdCustomers == id
                                        select new AppointmentsForClassesModel()
                                        {
                                            ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                            Date_recording = appointment.DateRecording,
                                            ID_Customers = appointment.IdCustomers,
                                            ID_Lessons = appointment.IdLessons,
                                            Status_recording = appointment.StatusRecording,
                                        }).ToList();
                    }
                }
                if (appointments == null)
                    appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                    where appointment.IdCustomers == id
                                    select new AppointmentsForClassesModel()
                                    {
                                        ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                        Date_recording = appointment.DateRecording,
                                        ID_Customers = appointment.IdCustomers,
                                        ID_Lessons = appointment.IdLessons,
                                        Status_recording = appointment.StatusRecording,
                                    }).ToList();

                return appointments;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение посещений, отсортированных по идентификатору занятия
        /// </summary>
        /// <param name="id">Идентификатор занятия</param>
        /// <returns>Http-ответ, содержащий список посещений, отсортированных по идентификатору занятия</returns>
        [HttpGet("bylesson/{id}")]
        public async Task<ActionResult<List<AppointmentsForClassesModel>>> GetAppointmentsByLesson(int id)
        {
            try
            {
                if (!(await _dbContext.Lessons.AnyAsync(l => l.IdLessons == id)))
                    return NotFound();

                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<AppointmentsForClassesModel>? appointments = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                    {
                        if (await _dbContext.Lessons.AnyAsync(l => l.IdLessons == id && l.IdCoaches != coachID.Value))
                            return Conflict("This appointment belongs to another coach");

                        appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                        where appointment.IdLessons == id
                                        select new AppointmentsForClassesModel()
                                        {
                                            ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                            Date_recording = appointment.DateRecording,
                                            ID_Customers = appointment.IdCustomers,
                                            ID_Lessons = appointment.IdLessons,
                                            Status_recording = appointment.StatusRecording,
                                        }).ToList();
                    }
                }
                if (appointments == null)
                    appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                    where appointment.IdLessons == id
                                    select new AppointmentsForClassesModel()
                                    {
                                        ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                        Date_recording = appointment.DateRecording,
                                        ID_Customers = appointment.IdCustomers,
                                        ID_Lessons = appointment.IdLessons,
                                        Status_recording = appointment.StatusRecording,
                                    }).ToList();

                return appointments;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение посещений, отсортированных по идентификатору тренера
        /// </summary>
        /// <param name="id">Идентификатор тренера</param>
        /// <returns>Http-ответ, содержащий список посещений, отсортированных по идентификатору тренера</returns>
        [HttpGet("bycoach/{id}")]
        public async Task<ActionResult<List<AppointmentsForClassesModel>>> GetAppointmentsByCoach(int id)
        {
            try
            {
                if (!(await _dbContext.Lessons.AnyAsync(l => l.IdLessons == id)))
                    return NotFound();

                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<AppointmentsForClassesModel>? appointments = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                    {
                        if (coachID != id)
                            return Conflict("These appointments belong to another coach");
                        var lessons = from lesson in await _dbContext.Lessons.ToListAsync()
                                      where lesson.IdCoaches == coachID
                                      select lesson.IdLessons;

                        appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                        where lessons.Contains(appointment.IdLessons)
                                        select new AppointmentsForClassesModel()
                                        {
                                            ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                            Date_recording = appointment.DateRecording,
                                            ID_Customers = appointment.IdCustomers,
                                            ID_Lessons = appointment.IdLessons,
                                            Status_recording = appointment.StatusRecording,
                                        }).ToList();
                    }
                }
                if (appointments == null){

                    var lessons = from lesson in await _dbContext.Lessons.ToListAsync()
                                  where lesson.IdCoaches == id
                                  select lesson.IdLessons;
                    appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                    where lessons.Contains(appointment.IdLessons)
                                    select new AppointmentsForClassesModel()
                                    {
                                        ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                        Date_recording = appointment.DateRecording,
                                        ID_Customers = appointment.IdCustomers,
                                        ID_Lessons = appointment.IdLessons,
                                        Status_recording = appointment.StatusRecording,
                                    }).ToList();
                }
                return appointments;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение посещений, отсортированных по дате отметки
        /// </summary>
        /// <param name="date">Дата отметки</param>
        /// <returns>Http-ответ, содержащий список посещений, отсортированных по дате отметки</returns>
        [HttpGet("bydate")]
        public async Task<ActionResult<List<AppointmentsForClassesModel>>> GetAppointmentsByDate([FromQuery(Name ="date")] DateOnly date)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<AppointmentsForClassesModel>? appointments = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                    {
                        var lessons = from lesson in await _dbContext.Lessons.ToListAsync()
                                      where lesson.IdCoaches == coachID
                                      select lesson.IdLessons;

                        appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                        where lessons.Contains(appointment.IdLessons)
                                        && appointment.DateRecording.CompareTo(date) == 0
                                        select new AppointmentsForClassesModel()
                                        {
                                            ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                            Date_recording = appointment.DateRecording,
                                            ID_Customers = appointment.IdCustomers,
                                            ID_Lessons = appointment.IdLessons,
                                            Status_recording = appointment.StatusRecording,
                                        }).ToList();
                    }
                }
                if (appointments == null)
                    appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                    where appointment.DateRecording.CompareTo(date) == 0
                                    select new AppointmentsForClassesModel()
                                    {
                                        ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                        Date_recording = appointment.DateRecording,
                                        ID_Customers = appointment.IdCustomers,
                                        ID_Lessons = appointment.IdLessons,
                                        Status_recording = appointment.StatusRecording,
                                    }).ToList();

                return appointments;

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение посещений, отсортированных по дате отметки
        /// </summary>
        /// <param name="request">Период, в который посещение отмечалось</param>
        /// <returns>Http-ответ, содержащий список  посещений, отсортированных по дате отметки</returns>
        [HttpPost("inperiod")]
        public async Task<ActionResult<List<AppointmentsForClassesModel>>> GetAppointmentsInPeriod([FromBody] DatePeriodRequest request)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                List<AppointmentsForClassesModel>? appointments = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID != null)
                    {
                        var lessons = from lesson in await _dbContext.Lessons.ToListAsync()
                                      where lesson.IdCoaches == coachID
                                      select lesson.IdLessons;

                        appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                        where lessons.Contains(appointment.IdLessons)
                                        && appointment.DateRecording.CompareTo(request.DateFrom) >= 0
                                        && appointment.DateRecording.CompareTo(request.DateTo) <= 0
                                        select new AppointmentsForClassesModel()
                                        {
                                            ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                            Date_recording = appointment.DateRecording,
                                            ID_Customers = appointment.IdCustomers,
                                            ID_Lessons = appointment.IdLessons,
                                            Status_recording = appointment.StatusRecording,
                                        }).ToList();
                    }
                }
                if (appointments == null)
                    appointments = (from appointment in await _dbContext.AppointmentsForClasses.ToListAsync()
                                    where appointment.DateRecording.CompareTo(request.DateFrom) >= 0
                                        && appointment.DateRecording.CompareTo(request.DateTo) <= 0
                                    select new AppointmentsForClassesModel()
                                    {
                                        ID_AppointmentsForClasses = appointment.IdAppointmentsForClasses,
                                        Date_recording = appointment.DateRecording,
                                        ID_Customers = appointment.IdCustomers,
                                        ID_Lessons = appointment.IdLessons,
                                        Status_recording = appointment.StatusRecording,
                                    }).ToList();

                return appointments;

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Обрабатывает запрос на добавление посещения
        /// </summary>
        /// <param name="appointment">Добавляемое посещение</param>
        /// <returns>Http-ответ, обозначающий результат добавления</returns>
        [HttpPost]
        public async Task<ActionResult> AddAppointment(AppointmentsForClassesModel appointment)
        {
            try
            {
                var requestedLesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == appointment.ID_Lessons);
                if (requestedLesson == null)
                    return Conflict("Lesson not found");
                if (!(await _dbContext.Customers.AnyAsync(c => c.IdCustomers == appointment.ID_Customers)))
                    return Conflict("Customer not found");
                if(!(await CheckCustomerAbleToBeSigned(appointment.ID_Customers)))
                    return Conflict("Customer does not have active subscription");


                var emptyFields = string.IsNullOrEmpty(appointment.Status_recording);
                var maxPracAlready = (await _dbContext.AppointmentsForClasses.CountAsync(l => l.IdLessons == appointment.ID_Lessons)) == requestedLesson.NumberOfPracticants;
                var custAlready = await _dbContext.AppointmentsForClasses.AnyAsync(ap => ap.IdLessons == requestedLesson.IdLessons && ap.IdCustomers == appointment.ID_Customers);

                //Getting other lessons, that goes on within the requested lesson time
                var otherLessonsInTime = (await _dbContext.Lessons.ToListAsync()).Where(l => l.IdLessons != requestedLesson.IdLessons
                && DateTimeCrossChecker.DoDatesCross(
                    l.Date.ToDateTime(l.Time), l.Date.ToDateTime(l.Time.AddHours(decimal.ToDouble(l.DurationClasses))),
                    requestedLesson.Date.ToDateTime(requestedLesson.Time), requestedLesson.Date.ToDateTime(requestedLesson.Time.AddHours(decimal.ToDouble(requestedLesson.DurationClasses))))
                ).Select(l => l.IdLessons).ToList();

                //checking if requested customer was signed in those lessons
                var custBusyInTime = await _dbContext.AppointmentsForClasses.AnyAsync(ap => otherLessonsInTime.Contains(ap.IdLessons) && ap.IdCustomers == appointment.ID_Customers);

                if (emptyFields || maxPracAlready || custAlready || custBusyInTime)
                    return Conflict("Invalid data format");

                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                AppointmentsForClass? newAppointment = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID.HasValue)
                    {
                        if (requestedLesson.IdCoaches != coachID.Value)
                            return Conflict("This lesson belongs to another coach");
                        newAppointment = new AppointmentsForClass()
                        {
                            DateRecording = appointment.Date_recording,
                            IdCustomers = appointment.ID_Customers,
                            IdLessons = appointment.ID_Lessons,
                            StatusRecording = appointment.Status_recording,
                        };
                    }
                }
                if (newAppointment == null)
                    newAppointment = new AppointmentsForClass()
                    {
                        DateRecording = appointment.Date_recording,
                        IdCustomers = appointment.ID_Customers,
                        IdLessons = appointment.ID_Lessons,
                        StatusRecording = appointment.Status_recording,
                    };


                _dbContext.AppointmentsForClasses.Add(newAppointment);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на изменение поещения
        /// </summary>
        /// <param name="appointment">Измененное посещение</param>
        /// <returns>Http-ответ, обозначающий результат изменения</returns>
        [HttpPut]
        public async Task<ActionResult> EditAppointment(AppointmentsForClassesModel appointment)
        {
            try
            {
                var requestedAppointment = await _dbContext.AppointmentsForClasses.FirstOrDefaultAsync(ap => ap.IdAppointmentsForClasses == appointment.ID_AppointmentsForClasses);
                if (requestedAppointment == null)
                    return Conflict("Appointment not found");

                var requestedLesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == appointment.ID_Lessons);
                if (requestedLesson == null)
                    return Conflict("Lesson not found");

                if (!(await _dbContext.Customers.AnyAsync(c => c.IdCustomers == appointment.ID_Customers)))
                    return Conflict("Customer not found");

                //Conditions
                var emptyFields = string.IsNullOrEmpty(appointment.Status_recording);
                var maxPracAlready = (await _dbContext.AppointmentsForClasses.CountAsync(ap => ap.IdLessons == appointment.ID_Lessons && ap.IdAppointmentsForClasses != appointment.ID_AppointmentsForClasses)) == requestedLesson.NumberOfPracticants;
                var custAlready = await _dbContext.AppointmentsForClasses.AnyAsync(ap => ap.IdLessons == requestedLesson.IdLessons && ap.IdCustomers == appointment.ID_Customers && ap.IdAppointmentsForClasses != appointment.ID_AppointmentsForClasses);

                //Getting other lessons, that goes on within the requested lesson time
                var otherLessonsInTime = await _dbContext.Lessons.Where(l => l.IdLessons != requestedLesson.IdLessons
                && DateTimeCrossChecker.DoDatesCross(
                    l.Date.ToDateTime(l.Time), l.Date.ToDateTime(l.Time.AddHours(decimal.ToDouble(l.DurationClasses))),
                    requestedLesson.Date.ToDateTime(requestedLesson.Time), requestedLesson.Date.ToDateTime(requestedLesson.Time.AddHours(decimal.ToDouble(requestedLesson.DurationClasses))))
                ).Select(l => l.IdLessons).ToListAsync();

                //checking if requested customer was signed in those lessons
                var custBusyInTime = await _dbContext.AppointmentsForClasses.AnyAsync(ap => otherLessonsInTime.Contains(ap.IdLessons) && ap.IdCustomers == appointment.ID_Customers && ap.IdAppointmentsForClasses != appointment.ID_AppointmentsForClasses);

                if (emptyFields || maxPracAlready || custAlready || custBusyInTime)
                    return Conflict("Invalid data format");

                //Role checking
                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                AppointmentsForClass? editedAppointment = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID.HasValue)
                    {
                        if (requestedLesson.IdCoaches != coachID.Value)
                            return Conflict("This lesson belongs to another coach");

                        editedAppointment = requestedAppointment;
                        editedAppointment.DateRecording = appointment.Date_recording;
                        editedAppointment.IdCustomers = appointment.ID_Customers;
                        editedAppointment.IdLessons = appointment.ID_Lessons;
                        editedAppointment.StatusRecording = appointment.Status_recording;
                    }
                }
                if (editedAppointment == null)
                {
                    editedAppointment = requestedAppointment;
                    editedAppointment.DateRecording = appointment.Date_recording;
                    editedAppointment.IdCustomers = appointment.ID_Customers;
                    editedAppointment.IdLessons = appointment.ID_Lessons;
                    editedAppointment.StatusRecording = appointment.Status_recording;
                }
                _dbContext.AppointmentsForClasses.Update(editedAppointment);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на удаление посещения по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор посещения</param>
        /// <returns>Http-ответ, обозначающий результат удаления</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            try
            {
                if (!(await _dbContext.AppointmentsForClasses.AnyAsync(ap => ap.IdAppointmentsForClasses == id)))
                    return NotFound();


                var token = await HttpContext.GetTokenAsync("access_token");
                var userRole = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "Role");
                var userID = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userRole == null || userID == null)
                    return Conflict("Incorrect format of token");
                AppointmentsForClass? deletedAppointment = null;
                if (userRole.Value == "Trainer")
                {
                    var coachID = (await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == Convert.ToInt32(userID.Value)))?.IdCoaches;
                    if (coachID.HasValue)
                    {
                        deletedAppointment = await _dbContext.AppointmentsForClasses.FirstOrDefaultAsync(ap => ap.IdAppointmentsForClasses == id);

                        if((await _dbContext.Lessons.FirstOrDefaultAsync(l => l.IdLessons == deletedAppointment!.IdLessons))?.IdCoaches != coachID)
                            return Conflict("This lesson belongs to another coach");
                    }
                }
                if (deletedAppointment == null)
                {
                    deletedAppointment = await _dbContext.AppointmentsForClasses.FirstOrDefaultAsync(ap => ap.IdAppointmentsForClasses == id);
                }

                _dbContext.AppointmentsForClasses.Remove(deletedAppointment!);
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
