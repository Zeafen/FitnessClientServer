using Azure.Core;
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
    public class PaymentsController : ControllerBase
    {
        private readonly GymContext _dbContext;

        public PaymentsController(GymContext dbContext) {
            _dbContext = dbContext;
        }


        /// <summary>
        ///  Обрабатывает запрос на получение всех оплат
        /// </summary>
        /// <returns>Http-ответ, содержащий список всех оплат</returns>
        [HttpGet]
        public async Task<ActionResult<List<PaymentModel>>> GetPayments()
        {
            try
            {
                var payments = from payment in await _dbContext.Payments.ToListAsync()
                               select new PaymentModel()
                               {
                                   ID_Payment = payment.IdPayment,
                                   Amount = decimal.ToDouble(payment.Amount),
                                   ID_Customers = payment.IdCustomers,
                                   ID_Subscription = payment.IdSubscription,
                                   PaymentDate = payment.PaymentDate,
                                   ValidityEndDate = payment.ValidityEndDate
                               };
                return payments.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех оплат, отсортирвованных по клиенту
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Http-ответ, содержащий список всех оплат, отсортирвованных по клиенту</returns>
        [HttpGet("bycustomer/{id}")]
        public async Task<ActionResult<List<PaymentModel>>> GetPaymentsByCustomer(int id)
        {
            try
            {
                var payments = from payment in await _dbContext.Payments.ToListAsync()
                               where payment.IdCustomers == id
                               select new PaymentModel()
                               {
                                   ID_Payment = payment.IdPayment,
                                   Amount = decimal.ToDouble(payment.Amount),
                                   ID_Customers = payment.IdCustomers,
                                   ID_Subscription = payment.IdSubscription,
                                   PaymentDate = payment.PaymentDate,
                                   ValidityEndDate = payment.ValidityEndDate
                               };
                return payments.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех оплат, отсортирвованных по абонементу
        /// </summary>
        /// <param name="id">Идентификатор абонемента</param>
        /// <returns>Http-ответ, содержащий список всех оплат, отсортирвованных по абонементу</returns>
        [HttpGet("bysubscription/{id}")]
        public async Task<ActionResult<List<PaymentModel>>> GetPaymentsBySubscription(int id)
        {
            try
            {
                var payments = from payment in await _dbContext.Payments.ToListAsync()
                               where payment.IdSubscription == id
                               select new PaymentModel()
                               {
                                   ID_Payment = payment.IdPayment,
                                   Amount = decimal.ToDouble(payment.Amount),
                                   ID_Customers = payment.IdCustomers,
                                   ID_Subscription = payment.IdSubscription,
                                   PaymentDate = payment.PaymentDate,
                                   ValidityEndDate = payment.ValidityEndDate
                               };
                return payments.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение оплаты по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор оплаты</param>
        /// <returns>Http-ответ, содержащий запись оплаты с ID, соответствующим переданному</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentModel>> GetPaymentsByID(int id)
        {
            try
            {
                var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.IdPayment == id);
                if(payment == null)
                    return NotFound();
                return new PaymentModel()
                {
                    ID_Payment = payment.IdPayment,
                    Amount = decimal.ToDouble(payment.Amount),
                    ID_Customers = payment.IdCustomers,
                    ID_Subscription = payment.IdSubscription,
                    PaymentDate = payment.PaymentDate,
                    ValidityEndDate = payment.ValidityEndDate
                };
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }
        /// <summary>
        /// Обрабатывает запрос на добавление записи
        /// </summary>
        /// <param name="payment">Добавляемая оплата</param>
        /// <returns>Http-ответ, обозначающий успешность добавления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<ActionResult<PaymentModel>> AddPayment(PaymentModel payment)
        {
            try
            {
                if (payment == null)
                    return Conflict();
                var hasAlready = await _dbContext.Payments.AnyAsync(p => p.IdCustomers == payment.ID_Customers && p.ValidityEndDate.CompareTo(DateOnly.FromDateTime(DateTime.Now)) >= 0);
                var customerExists = await _dbContext.Customers.AnyAsync(c => c.IdCustomers == payment.ID_Customers);
                var subscriptionExists = await _dbContext.Subscriptions.AnyAsync(s => s.IdSubscription == payment.ID_Subscription);
                if (hasAlready || !customerExists || !subscriptionExists)
                    return Conflict();
                var requestedSubs = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.IdSubscription == payment.ID_Subscription);
                var newPayment = new Payment()
                {
                    Amount = requestedSubs!.Cost,
                    IdCustomers = payment.ID_Customers,
                    IdSubscription = payment.ID_Subscription,
                    PaymentDate = payment.PaymentDate,
                    ValidityEndDate = payment.PaymentDate.AddDays(requestedSubs.ValidityPeriod)
                };
                _dbContext.Payments.Add(newPayment);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на изменение записи
        /// </summary>
        /// <param name="payment">Измененнная оплата</param>
        /// <returns>Http-ответ, обозначающий успешность изменения</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPut]
        public async Task<ActionResult<PaymentModel>> EditPayment(PaymentModel payment)
        {
            try
            {
                if (payment == null)
                    return Conflict();
                var exists = await _dbContext.Payments.AnyAsync(p => p.IdPayment == payment.ID_Payment);
                var hasAlready = await _dbContext.Payments.AnyAsync(p => p.IdCustomers == payment.ID_Customers && p.ValidityEndDate.CompareTo(DateOnly.FromDateTime(DateTime.Now)) >= 0 && p.IdPayment != payment.ID_Payment);
                var customerExists = await _dbContext.Customers.AnyAsync(c => c.IdCustomers == payment.ID_Customers);
                var subscriptionExists = await _dbContext.Subscriptions.AnyAsync(s => s.IdSubscription == payment.ID_Subscription);
                if (!exists || exists && (hasAlready || !customerExists || !subscriptionExists))
                    return Conflict();
                var requestedSubs = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.IdSubscription == payment.ID_Subscription);

                var editedPayment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.IdPayment == payment.ID_Payment);
                editedPayment!.Amount = requestedSubs!.Cost;
                editedPayment.IdCustomers = payment.ID_Customers;
                editedPayment.IdSubscription = payment.ID_Subscription;
                editedPayment.PaymentDate = payment.PaymentDate;
                editedPayment.ValidityEndDate = payment.PaymentDate.AddDays(requestedSubs.ValidityPeriod);

                _dbContext.Payments.Update(editedPayment);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на удаление записи по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор удаляемой записи</param>
        /// <returns>Http-ответ, обозначающий  успешность удаления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<PaymentModel>> DeletePayment(int id)
        {
            try
            {
                var deletedPayment = _dbContext.Payments.FirstOrDefault(p => p.IdPayment == id);
                if (deletedPayment == null)
                    return NotFound();

                _dbContext.Payments.Remove(deletedPayment);
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
