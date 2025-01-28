using FitnessAPI.domain.models;
using FitnessAPI.FitnessDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SomwApp.domain.models;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitnessAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReviewFilesController : ControllerBase
    {
        private readonly string staticFilesPath;
        private readonly GymContext _dbContext;

        public ReviewFilesController(IWebHostEnvironment env, GymContext dbContext)
        {
            staticFilesPath = $@"{env.ContentRootPath}\static";
            _dbContext = dbContext;
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех отчётов
        /// </summary>
        /// <returns>Http-ответ, содержащий список всех отчётов</returns>
        [HttpGet]
        public ActionResult<List<ReviewFile>> GetFiles()
        {
            try
            {
                var directoryInfo = new DirectoryInfo(staticFilesPath + @"\attendance");
                var files = (from file in directoryInfo.GetFiles()
                             select new ReviewFile()
                             {
                                 ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/attendance/{file.Name}",
                                 ReviewDate = file.CreationTime,
                                 ReviewName = file.Name.Replace(file.Extension, ""),
                                 ReviewType = ReviewType.Attendance,
                             }).ToList();
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\branch_efficiency").GetFiles())
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/branch_efficiency/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Branch_Efficiency,
                    });
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\income").GetFiles())
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/income/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Income,
                    });
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\trainer_busines").GetFiles())
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/trainer_busines/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Trainer_Business,
                    });
                return files;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение отчётов, отсортированных по типу отчёта
        /// </summary>
        /// <param name="type">Тип отчёта, по которому необходимо отсортировать записи</param>
        /// <returns>Http-ответ, содержащий список отчётов, отсортированных по типу отчёта</returns>
        [HttpGet("bytype")]
        public ActionResult<List<ReviewFile>> GetFilesByType([FromQuery(Name ="type")]ReviewType type)
        {
            try
            {
                string directoryName = string.Empty;
                switch (type)
                {
                    case ReviewType.Trainer_Business:
                        directoryName = "trainer_busines";
                        break;
                    case ReviewType.Income:
                        directoryName = "income";
                        break;
                    case ReviewType.Attendance:
                        directoryName = "attendance";
                        break;
                    case ReviewType.Branch_Efficiency:
                        directoryName = "branch_efficiency";
                        break;
                }
                if (string.IsNullOrEmpty(directoryName))
                    return Conflict("Requested type not found");
                var files = (from file in new DirectoryInfo(staticFilesPath + @$"\{directoryName}").GetFiles()
                             select new ReviewFile()
                             {
                                 ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/{directoryName}/{file.Name}",
                                 ReviewDate = file.CreationTime,
                                 ReviewName = file.Name.Replace(file.Extension, ""),
                                 ReviewType = ReviewType.Attendance,
                             }).ToList();
                return files;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Обрабатывает запрос на получение отчётов, отсортированных по дате создания
        /// </summary>
        /// <param name="date">Дата, по которой необходимо отсортировать записи</param>
        /// <returns>Http-ответ, содержащий список отчётов, отсортированных по дате создания</returns>
        [HttpGet("bydate")]
        public ActionResult<List<ReviewFile>> GetFilesByDate([FromQuery(Name ="date")]DateTime date)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(staticFilesPath + @"\attendance");
                var files = (from file in directoryInfo.GetFiles()
                             where file.CreationTime.CompareTo(date) == 0
                             select new ReviewFile()
                             {
                                 ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/attendance/{file.Name}",
                                 ReviewDate = file.CreationTime,
                                 ReviewName = file.Name.Replace(file.Extension, ""),
                                 ReviewType = ReviewType.Attendance,
                             }).ToList();
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\branch_efficiency").GetFiles())
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/branch_efficiency/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Branch_Efficiency,
                    });
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\income").GetFiles())
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/income/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Income,
                    });
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\trainer_busines").GetFiles())
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/trainer_busines/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Trainer_Business,
                    });
                return files;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Обрабатывает запрос на получение отчётов, отсортированных по дате создания
        /// </summary>
        /// <param name="request">Период, в котрый был создан отчёт</param>
        /// <returns></returns>
        [HttpPost("inperiod")]
        public ActionResult<List<ReviewFile>> GetFilesInPeriod([FromBody]DatePeriodRequest request)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(staticFilesPath + @"\attendance");
                var files = (from file in directoryInfo.GetFiles()
                             select new ReviewFile()
                             {
                                 ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/attendance/{file.Name}",
                                 ReviewDate = file.CreationTime,
                                 ReviewName = file.Name.Replace(file.Extension, ""),
                                 ReviewType = ReviewType.Attendance,
                             }).ToList();
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\branch_efficiency").GetFiles()
                    .Where(f => f.CreationTime.CompareTo(request.DateFrom) >= 0 && f.CreationTime.CompareTo(request.DateTo) <= 0))
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/branch_efficiency/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Branch_Efficiency,
                    });
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\income").GetFiles()
                    .Where(f => f.CreationTime.CompareTo(request.DateFrom) >= 0 && f.CreationTime.CompareTo(request.DateTo) <= 0))
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/income/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Income,
                    });
                foreach (var file in new DirectoryInfo(staticFilesPath + @"\trainer_busines").GetFiles()
                    .Where(f => f.CreationTime.CompareTo(request.DateFrom) >= 0 && f.CreationTime.CompareTo(request.DateTo) <= 0))
                    files.Add(new ReviewFile()
                    {
                        ReviewAddress = $@"{Request.Scheme}://{Request.Host.Value}/static/trainer_busines/{file.Name}",
                        ReviewDate = file.CreationTime,
                        ReviewName = file.Name.Replace(file.Extension, ""),
                        ReviewType = ReviewType.Trainer_Business,
                    });
                return files;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на формирование отчёта
        /// </summary>
        /// <param name="request">Запрос, содержащий информациб для формирования отчёта (тип, даты учёта данных)</param>
        /// <returns>Http-ответ, обозначающий успешность добавления </returns>
        [HttpPost]
        public async Task<ActionResult> AddFile([FromBody]ReviewFileRequest request)
        {
            try
            {
                bool fileFormed = false;
                switch (request.ReviewType)
                {
                    case ReviewType.Trainer_Business:
                        fileFormed = await FormTrainerBusinessReview(request.DateFrom, request.DateTo, @$"{staticFilesPath}\trainer_busines\{request.ReviewName}.pdf");
                        break;
                    case ReviewType.Attendance:
                        fileFormed = await FormAttendanceReview(request.DateFrom, request.DateTo, @$"{staticFilesPath}\attendance\{request.ReviewName}.pdf");
                        break;
                    case ReviewType.Income:
                        fileFormed = await FormIncomeReview(request.DateFrom, request.DateTo, @$"{staticFilesPath}\income\{request.ReviewName}.pdf");
                        break;
                    case ReviewType.Branch_Efficiency:
                        fileFormed = await FormBranchEfficiencyReview(request.DateFrom, request.DateTo, @$"{staticFilesPath}\branch_efficiency\{request.ReviewName}.pdf");
                        break;
                }

                if (!fileFormed)
                    return Conflict("Could not form review");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Обрабатывает запрос на удаление отчёта
        /// </summary>
        /// <param name="fileName">Название отчёта, которыё нужно удалить</param>
        /// <returns>Http-ответ, обозначающий успешность удаления</returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteFile([FromQuery(Name = "fileName")]string fileName)
        {
            try
            {
                bool fileDeleted = false;
                if (System.IO.File.Exists(staticFilesPath + $@"\attendance\{fileName}.pdf"))
                {
                    System.IO.File.Delete(staticFilesPath + $@"\attendance\{fileName}.pdf");
                    fileDeleted = true;
                }
                else if (System.IO.File.Exists(staticFilesPath + $@"\branch_efficiency\{fileName}.pdf"))
                {
                    System.IO.File.Delete(staticFilesPath + $@"\branch_efficiency\{fileName}.pdf");
                    fileDeleted = true;
                }
                else if (System.IO.File.Exists(staticFilesPath + $@"\income\{fileName}.pdf"))
                {
                    System.IO.File.Delete(staticFilesPath + $@"\income\{fileName}.pdf");
                    fileDeleted = true;
                }
                else if (System.IO.File.Exists(staticFilesPath + $@"\trainer_busines\{fileName}.pdf"))
                {
                    System.IO.File.Delete(staticFilesPath + $@"\trainer_busines\{fileName}.pdf");
                    fileDeleted = true;
                }
                if (!fileDeleted)
                    return NotFound("File review not found");
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Формирует файл отчёта о посещаемости
        /// </summary>
        /// <param name="dateFrom"> дата начала учёта данных о посещаемости</param>
        /// <param name="dateTo">дата окончания учёта данных о посещаемости</param>
        /// <param name="fileFullPath">Полный путь файла, который нужно создать</param>
        /// <returns>Создан ли файл или нет</returns>
        private async Task<bool> FormAttendanceReview(DateTime dateFrom, DateTime dateTo, string fileFullPath)
        {
            try
            {
                var lessons = (await _dbContext.Lessons.ToListAsync()).Where(l => l.Date.CompareTo(DateOnly.FromDateTime(dateFrom)) >= 0 && l.Date.CompareTo(DateOnly.FromDateTime(dateTo)) <= 0);

                var builder = new StringBuilder();
                builder.AppendLine("<h1>Отчёт о посещаемости</h1>");
                builder.AppendLine($"<h2>Учёт данных с {dateFrom.ToString("dd:MM:yyyy")} по {dateTo.ToString("dd:MM:yyyy")}</h2>");
                builder.AppendLine("<h5></h5>");
                builder.AppendLine("<h5></h5>");
                foreach (var lesson in lessons)
                {
                    builder.AppendLine("<h5></h5>");

                    var coach = _dbContext.Coaches.FirstOrDefault(c => c.IdCoaches == lesson.IdCoaches);
                    var appointments = (await _dbContext.AppointmentsForClasses.ToListAsync()).Where(ap => ap.IdLessons == lesson.IdLessons).ToList();
                    double attendancePercentage = Convert.ToDouble(appointments.Count(ap => ap.StatusRecording.Equals("here", StringComparison.OrdinalIgnoreCase))) / Convert.ToDouble(lesson.NumberOfPracticants) * 100;
                    builder.AppendLine($"<h3>Занияте N {lesson.IdLessons} - \"{lesson.Title}\"</h3>");
                    builder.AppendLine($"<h4>Дата начала: {lesson.Date.ToDateTime(lesson.Time).ToString("dd:MM:yyyy HH:MM")}        " +
                        $"Дата окончания: {lesson.Date.ToDateTime(lesson.Time.AddHours(decimal.ToDouble(lesson.DurationClasses))).ToString("dd:MM:yyyy HH:MM")}</h5>");
                    builder.AppendLine($"<h5>Тренер: {(coach != null ? $"{coach.Surname} {coach.Name[0]}.{(string.IsNullOrEmpty(coach.MiddleName) ? string.Empty : $" {coach.MiddleName?[0]}.")}" : "Не установлен")}</h5>");
                    builder.AppendLine($"<h5>Количество участников: {lesson.NumberOfPracticants}        Количество посещений: {appointments.Count(ap => ap.StatusRecording.Equals("here", StringComparison.OrdinalIgnoreCase))}</h5>");
                    builder.AppendLine($"<h5>Процент посещаемости: {attendancePercentage}%</h5>");
                    builder.AppendLine($"<h5>Общий учёт посещений</h5>");
                    foreach (var appointment in appointments)
                    {
                        var client = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomers == appointment.IdCustomers);
                        builder.AppendLine($"<p>Запись учёта N {appointment.IdAppointmentsForClasses}</p>");
                        builder.AppendLine($"<p>Дата учёта: {appointment.DateRecording.ToDateTime(new TimeOnly(0)).ToString("dd:MM:yyyy")} статус посещения: {appointment.StatusRecording}</p>");
                        builder.AppendLine($"<p>Клиент: {(client != null ? $"{client.Surname} {client.Name[0]}.{(string.IsNullOrEmpty(client.MiddleName) ? string.Empty : $" {client.MiddleName[0]}.")}" : "Не установлен")}</p>");
                    }
                }

                if (System.IO.File.Exists(fileFullPath))
                    System.IO.File.Delete(fileFullPath);
                var renderer = new ChromePdfRenderer();
                PdfDocument pdf = renderer.RenderHtmlAsPdf(builder.ToString());
                pdf.SaveAs(fileFullPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Формирует файл отчёта о доходности
        /// </summary>
        /// <param name="dateFrom"> дата начала учёта данных</param>
        /// <param name="dateTo">дата окончания учёта данных</param>
        /// <param name="fileFullPath">Полный путь файла, который нужно создать</param>
        /// <returns>Создан ли файл или нет</returns>
        private async Task<bool> FormIncomeReview(DateTime dateFrom, DateTime dateTo, string fileFullPath)
        {
            try
            {
                var payments = (await _dbContext.Payments.ToListAsync()).Where(p => p.PaymentDate.CompareTo(DateOnly.FromDateTime(dateFrom)) >= 0 && p.PaymentDate.CompareTo(DateOnly.FromDateTime(dateTo)) <= 0);
                var builder = new StringBuilder();
                builder.AppendLine("<h1>Отчёт о доходах</h1>");
                builder.AppendLine($"<h2>Учёт данных с {dateFrom.ToString("dd:MM:yyyy")} по {dateTo.ToString("dd:MM:yyyy")}</h3>");
                builder.AppendLine("<h5></h5>");
                builder.AppendLine("<h5></h5>");
                foreach (var payment in payments)
                {
                    builder.AppendLine($"<h5></h5>");
                    var client = await _dbContext.Customers.FirstOrDefaultAsync(p => p.IdCustomers == payment.IdCustomers);
                    var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(p => p.IdSubscription == payment.IdSubscription);

                    builder.AppendLine($"<h3>Оплата N {payment.IdPayment}\tДата: {payment.PaymentDate.ToDateTime(new TimeOnly(0)).ToString("dd:MM:yyyy")}</h3>");
                    builder.AppendLine($"<h5>Клиент: {(client != null ? $"{client.Surname} {client.Name[0]}.{(string.IsNullOrEmpty(client.MiddleName) ? string.Empty : $" {client.MiddleName?[0]}.")}" : "Не установлен")}</h5>");
                    builder.AppendLine($"<h5>Подписка: {(subscription != null ? $"{subscription.TypeofSubscriptuion}" : "Не установлена")}</h5>");
                    builder.AppendLine($"<h5>Сумма: {payment.Amount}</h5>");
                }

                builder.Append($"<p></p>");
                builder.Append($"<p></p>");
                builder.Append($"<p>Общие доходы за период: {payments.Sum(p => p.Amount)}</p>");
                builder.Append($"<p>Средний чек: {payments.Average(p => p.Amount)}</p>");


                if (System.IO.File.Exists(fileFullPath))
                    System.IO.File.Delete(fileFullPath);
                var renderer = new ChromePdfRenderer();
                PdfDocument pdf = renderer.RenderHtmlAsPdf(builder.ToString());
                pdf.SaveAs(fileFullPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Формирует файл отчёта о загруженности тренеров
        /// </summary>
        /// <param name="dateFrom"> дата начала учёта данных</param>
        /// <param name="dateTo">дата окончания учёта данных</param>
        /// <param name="fileFullPath">Полный путь файла, который нужно создать</param>
        /// <returns>Создан ли файл или нет</returns>
        private async Task<bool> FormTrainerBusinessReview(DateTime dateFrom, DateTime dateTo, string fileFullPath)
        {
            try
            {
                var coaches = await _dbContext.Coaches.ToListAsync();
                var builder = new StringBuilder();
                double possibleHours = 5 * 10;
                builder.AppendLine("<h1>Отчёт о загруженности тренеров</h1>");
                builder.AppendLine($"<h2>Учёт данных с {dateFrom.ToString("dd:MM:yyyy")} по {dateTo.ToString("dd:MM:yyyy")}</h2>");
                builder.AppendLine("<h5></h5>");
                builder.AppendLine("<h5></h5>");

                foreach (var coach in coaches)
                {
                    builder.AppendLine("<h5></h5>");
                    var lessons = (await _dbContext.Lessons.ToListAsync()).Where(l => l.IdCoaches == coach.IdCoaches && l.Date.CompareTo(DateOnly.FromDateTime(dateFrom)) >= 0 && l.Date.CompareTo(DateOnly.FromDateTime(dateTo)) <= 0);
                    builder.AppendLine($"<h3>Тренер: {(coach != null ? $"{coach.Surname} {coach.Name[0]}.{(string.IsNullOrEmpty(coach.MiddleName) ? string.Empty : $" {coach.MiddleName[0]}.")}" : "Не установлен")}</h3>");
                    builder.AppendLine($"<h5>Общее количество занятий за период: {lessons.Count()}      Общее время занятий: {lessons.Sum(l => l.DurationClasses)}</h5>");
                    builder.AppendLine($"<h5>Загруженность тренера: {Convert.ToInt32((decimal.ToDouble(lessons.Sum(l => l.DurationClasses)) / possibleHours * 100))}%</h5>");
                    builder.AppendLine();
                }
                if (System.IO.File.Exists(fileFullPath))
                    System.IO.File.Delete(fileFullPath);
                var renderer = new ChromePdfRenderer();
                PdfDocument pdf = renderer.RenderHtmlAsPdf(builder.ToString());
                pdf.SaveAs(fileFullPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Формирует файл отчёта об эффективности филиалов
        /// </summary>
        /// <param name="dateFrom"> дата начала учёта данных</param>
        /// <param name="dateTo">дата окончания учёта данных</param>
        /// <param name="fileFullPath">Полный путь файла, который нужно создать</param>
        /// <returns>Создан ли файл или нет</returns>
        private async Task<bool> FormBranchEfficiencyReview(DateTime dateFrom, DateTime dateTo, string fileFullPath)
        {
            try
            {
                var branches = await _dbContext.Branches.ToListAsync();
                var builder = new StringBuilder();
                double TotalLessonsCount = await _dbContext.Lessons.CountAsync(l => l.Date.CompareTo(DateOnly.FromDateTime(dateFrom)) >= 0 && l.Date.CompareTo(DateOnly.FromDateTime(dateTo)) <= 0);
                builder.AppendLine("<h1>Отчёт о рентабельности</h1>");
                builder.AppendLine($"<h2>Учёт данных с {dateFrom.ToString("dd:MM:yyyy")} по {dateTo.ToString("dd:MM:yyyy")}</h2>");
                builder.AppendLine("<h5></h5>");
                builder.AppendLine("<h5></h5>");

                foreach (var branch in branches)
                {
                    builder.AppendLine("<h5></h5>");
                    var lessons = (await _dbContext.Lessons.ToListAsync()).Where(l => l.IdBranches == branch.IdBracnches && l.Date.CompareTo(DateOnly.FromDateTime(dateFrom)) >= 0 && l.Date.CompareTo(DateOnly.FromDateTime(dateTo)) <= 0);
                    builder.AppendLine($"<h3>Филиал N {branch.IdBracnches}</h3>");
                    builder.AppendLine($"<h5>Название - {(branch != null ? $"{branch.NameBranches}" : "Не установлено")}     Адрес - {(branch != null ? $"{branch.AddressBranches}" : "Не установлен")}</h5>");
                    builder.AppendLine($"<h5>Общее количество занятий за период: {lessons.Count()}      Общее время занятий: {lessons.Sum(l => l.DurationClasses)}</h5>");
                    builder.AppendLine($"<h5>Рентабельность филиала: {(TotalLessonsCount > 0 ? Convert.ToDouble(lessons.Count()) / TotalLessonsCount : 0)}</h5>");
                }
                if (System.IO.File.Exists(fileFullPath))
                    System.IO.File.Delete(fileFullPath);
                var renderer = new ChromePdfRenderer();
                PdfDocument pdf = renderer.RenderHtmlAsPdf(builder.ToString());
                pdf.SaveAs(fileFullPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
