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
    public class SubscriptionsController : ControllerBase
    {
        private GymContext _dbContext;

        public SubscriptionsController(GymContext dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// Обрабатывает запрос на получение всех абонементов
        /// </summary>
        /// <returns>Список всех абнементов</returns>
        [HttpGet]
        public async Task<ActionResult<List<SubscriptionModel>>> GetSubscriptions()
        {
            try
            {
                var subscriptions = from subscription in await _dbContext.Subscriptions.ToListAsync()
                                    select new SubscriptionModel()
                                    {
                                        ID_Subscription = subscription.IdSubscription,
                                        Cost = decimal.ToDouble(subscription.Cost),
                                        Conditions = subscription.Conditions,
                                        NumberofVisits = subscription.NumberofVisits,
                                        TypeofSubscription = subscription.TypeofSubscriptuion,
                                        ValidityDaysNumber = subscription.ValidityPeriod,
                                    };
                return subscriptions.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение абонемента с ID, соответствующим переданному
        /// </summary>
        /// <param name="id">Идентификатор абонемента</param>
        /// <returns>Абонемент с ID, соответствующим переданному</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionModel>> GetSubscription(int id)
        {
            try
            {
                var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.IdSubscription == id);
                if (subscription == null)
                    return NotFound();
                return new SubscriptionModel()
                {
                    ID_Subscription = subscription.IdSubscription,
                    Cost = decimal.ToDouble(subscription.Cost),
                    Conditions = subscription.Conditions,
                    NumberofVisits = subscription.NumberofVisits,
                    TypeofSubscription = subscription.TypeofSubscriptuion,
                    ValidityDaysNumber = subscription.ValidityPeriod,
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на добавление абонемента
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns>Http-ответ, обозначающий результат добавления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<ActionResult> AddSubscription(SubscriptionModel subscription)
        {
            try
            {
                var exists = await _dbContext.Subscriptions.AnyAsync(s => s.TypeofSubscriptuion == subscription.TypeofSubscription);
                if (exists)
                    return Conflict("Подписка с таким названием уже существует");
                if (string.IsNullOrEmpty(subscription.TypeofSubscription)
                    || string.IsNullOrEmpty(subscription.Conditions)
                    || subscription.NumberofVisits <= 0
                    || subscription.ValidityDaysNumber <= 0
                    || subscription.Cost <= 0)
                    return Conflict("Некорректный формат данных");

                var newSubscription = new Subscription()
                {
                    Conditions = subscription.Conditions,
                    Cost = Convert.ToDecimal(subscription.Cost),
                    NumberofVisits = subscription.NumberofVisits,
                    ValidityPeriod = subscription.ValidityDaysNumber,
                    TypeofSubscriptuion = subscription.TypeofSubscription,
                };

                _dbContext.Subscriptions.Add(newSubscription);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на изменение абонемента
        /// </summary>
        /// <param name="subscription">Измененная запись абонемента</param>
        /// <returns>Http-ответ, обозначающий результат изменения</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPut]
        public async Task<ActionResult> EditSubscription(SubscriptionModel subscription)
        {
            try
            {
                var exists = await _dbContext.Subscriptions.AnyAsync(s => s.IdSubscription == subscription.ID_Subscription);
                if (!exists)
                    return NotFound("Абонемент с таким id не найден");
                if (string.IsNullOrEmpty(subscription.TypeofSubscription)
                    || string.IsNullOrEmpty(subscription.Conditions)
                    || subscription.NumberofVisits <= 0
                    || subscription.ValidityDaysNumber <= 0
                    || subscription.Cost <= 0)
                    return Conflict("Некорректный формат данных");
                var editedSubscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.IdSubscription == subscription.ID_Subscription);

                    editedSubscription!.Conditions = subscription.Conditions;
                    editedSubscription.Cost = Convert.ToDecimal(subscription.Cost);
                    editedSubscription.NumberofVisits = subscription.NumberofVisits;
                    editedSubscription.ValidityPeriod = subscription.ValidityDaysNumber;
                    editedSubscription.TypeofSubscriptuion = subscription.TypeofSubscription;

                _dbContext.Subscriptions.Add(editedSubscription);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на удаление абонемента
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Http-ответ, обозначающий результат удаления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubscription(int id)
        {
            try
            {
                var existed = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.IdSubscription == id);
                if (existed == null)
                    return NotFound("Подписка с таким id не найдена");

                _dbContext.Subscriptions.Remove(existed);
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
