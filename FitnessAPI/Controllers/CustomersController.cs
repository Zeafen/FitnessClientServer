using FitnessAPI.Data.Helpers;
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
    public class CustomersController : ControllerBase
    {
        private readonly GymContext _dbContext;
        public CustomersController(GymContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Обрабатывает запрос на получение клиентов
        /// </summary>
        /// <returns>Http-ответ, содержащий список клиентов</returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerModel>>> GetCustomers()
        {
            try
            {
                var customers = from customer in (await _dbContext.Customers.ToListAsync())
                                select new CustomerModel()
                                {
                                    ID_Customers = customer.IdCustomers,
                                    BirthDate = customer.BirthDate,
                                    MiddleName = customer.MiddleName,
                                    Name = customer.Name,
                                    PhoneNumber = customer.PhoneNumber,
                                    Surname = customer.Surname
                                };
                return customers.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на плоучение клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Http-ответ, содержащий клиента с ID равному переданному</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerModel>> GetCustomer(int id)
        {
            try
            {
                var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomers == id);
                if(customer == null)
                    return NotFound();
                return new CustomerModel()
                {
                    ID_Customers = customer.IdCustomers,
                    BirthDate = customer.BirthDate,
                    MiddleName = customer.MiddleName,
                    Name = customer.Name,
                    PhoneNumber = customer.PhoneNumber,
                    Surname = customer.Surname
                };
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на добавление клиента
        /// </summary>
        /// <param name="customer">Добалвяемый клиент</param>
        /// <returns>Http-ответ, обозначающий результат добавления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<ActionResult> AddCustomer(CustomerModel customer)
        {
            try
            {
                if (customer == null
                    || string.IsNullOrEmpty(customer.Surname) || string.IsNullOrEmpty(customer.Name)
                    || !AgeCheckHelper.CheckAge(customer.BirthDate, DateOnly.FromDateTime(DateTime.Now), 18)
                    || string.IsNullOrEmpty(customer.PhoneNumber))
                    return Conflict();

                var newCustomer = new Customer()
                {
                    BirthDate = customer.BirthDate,
                    MiddleName = customer.MiddleName,
                    Name = customer.Name,
                    PhoneNumber = customer.PhoneNumber,
                    Surname = customer.Surname,
                };
                await _dbContext.Customers.AddAsync(newCustomer);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на изменение клиента
        /// </summary>
        /// <param name="customer">Измененный клиент</param>
        /// <returns>Http-ответ, обозначающий результат изменения</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpPut]
        public async Task<ActionResult> EditCustomer(CustomerModel customer)
        {
            try
            {
                if (customer == null
                    || string.IsNullOrEmpty(customer.Surname) || string.IsNullOrEmpty(customer.Name)
                    || !AgeCheckHelper.CheckAge(customer.BirthDate, DateOnly.FromDateTime(DateTime.Now), 18)
                    || string.IsNullOrEmpty(customer.PhoneNumber)
                    || !_dbContext.Customers.Any(c => c.IdCustomers == customer.ID_Customers))
                    return Conflict("Неверный формат данных. Проверьте корректность введенных данных");
                var editedCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomers == customer.ID_Customers);

                editedCustomer!.BirthDate = customer.BirthDate;
                editedCustomer.MiddleName = customer.MiddleName;
                editedCustomer.Name = customer.Name;
                editedCustomer.PhoneNumber = customer.PhoneNumber;
                editedCustomer.Surname = customer.Surname;

                _dbContext.Customers.Update(editedCustomer);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на удаление клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Http-ответ, обозначающий результат удаления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            try
            {
                var custToDelete = await _dbContext.Customers.FirstOrDefaultAsync(c => c.IdCustomers == id);
                if (custToDelete == null)
                    return NotFound();
                _dbContext.Customers.Remove(custToDelete);
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
