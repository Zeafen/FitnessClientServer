using Azure.Core;
using FitnessAPI.FitnessDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipes_API.Domain.Services;
using SomwApp.domain.models;

namespace FitnessAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserAccountsController : ControllerBase
    {
        private GymContext _dbContext;
        private IHashingService _hashingService;
        public UserAccountsController( GymContext ctx, IHashingService hashingService )
        {
            _dbContext = ctx;
            _hashingService = hashingService;
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех учётных записей
        /// </summary>
        /// <returns>Спискок учётных записей</returns>
        [HttpGet]
        public async Task<ActionResult<List<UserAccounts>>> GetAccounts()
        {
            try
            {
                var accounts = from account in (await _dbContext.UserAccounts.ToListAsync())
                               select new UserAccounts()
                               {
                                   ID_UserAccounts = account.IdUserAccounts,
                                   ID_Roles = account.IdRoles,
                                   Login = account.Login,
                                   Password = "",
                               };
                return accounts.ToList();
                
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение всех учётных записей, отсортированных по роли
        /// </summary>
        /// <param name="roleID">Роль для сортировки учётных записей</param>
        /// <returns>Http-ответ, содержащий список учётных записей, отсортированных по роли</returns>
        [HttpGet("byrole/{roleID}")]
        public async Task<ActionResult<List<UserAccounts>>> GetAccountsByRole(int roleID)
        {
            try
            {
                var accounts = await _dbContext.UserAccounts.Where(ac => ac.IdRoles == roleID).ToListAsync();
                var result = from account in accounts
                               select new UserAccounts()
                               {
                                   ID_UserAccounts = account.IdUserAccounts,
                                   ID_Roles = account.IdRoles,
                                   Login = account.Login,
                                   Password = "",
                               };
                return result.ToList();
                
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на получение учётной записи по ID
        /// </summary>
        /// <param name="id">Идентификатор учётной записи</param>
        /// <returns>Учётную запись с ID, соответствующим переданному</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAccounts>> GetAccountByID(int id)
        {
            try
            {
                var account = await _dbContext.UserAccounts.FirstOrDefaultAsync(ac => ac.IdUserAccounts == id);
                if (account == null)
                    return NotFound("Запись с таким id не найдена");
                var result = new UserAccounts()
                {
                    ID_Roles = account.IdRoles,
                    Login = account.Login,
                    ID_UserAccounts = account.IdUserAccounts,
                    Password = ""
                };

                return result;
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на добавление учётной записи
        /// </summary>
        /// <param name="account">Добавляемая запись</param>
        /// <returns>Http-ответ, обозначающий результат добавления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="AdminPolicy")]
        [HttpPost]
        public async Task<ActionResult> AddAccount([FromBody]UserAccounts account)
        {
            try
            {
                var accounts = await _dbContext.UserAccounts.ToListAsync();
                var requestedRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.IdRoles == account.ID_Roles);
                if (requestedRole == null)
                    return NotFound("Роль не найдена");
                else if (accounts.Count(ac => ac.IdRoles == requestedRole.IdRoles) == 10)
                    return Conflict("Макимальное количество тренеров в системе");
                else if (await _dbContext.UserAccounts.AnyAsync(ac => ac.Login == account.Login))
                    return Conflict("Пользователь с таким логином уже существует");
                else
                {
                    bool areFieldsEmpty = string.IsNullOrEmpty(account.Login) || string.IsNullOrEmpty(account.Password);
                    bool isPasswShort = account.Password.Length < 8;
                    bool passwContainsSpecials = account.Password.Any(" !\"\'@#$%^(){}*-_=+<>.,\\/;:\'\"|?~`".Contains);
                    bool passwContainsLetters = account.Password.Any("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm".Contains);
                    bool passwContainsNums = account.Password.Any("0123456789".Contains);
                    if (areFieldsEmpty || isPasswShort || !passwContainsLetters || !passwContainsNums || !passwContainsSpecials)
                        return Conflict();

                    var saltedHash = _hashingService.GenerateHash(account.Password);

                    _dbContext.UserAccounts.Add(new UserAccount()
                    {
                        IdRoles = account.ID_Roles,
                        Login = account.Login,
                        Password = saltedHash.hash,
                        Salt = saltedHash.salt,
                    });
                    _dbContext.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на удаление учётной записи
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns>Http-ответ, обозначающий результат удаления</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            try
            {
                var accountToDelete = await _dbContext.UserAccounts.FirstOrDefaultAsync(ac => ac.IdUserAccounts == id);
                if (accountToDelete == null)
                    return NotFound("Запись с таким id не найдена");
                else
                {
                    var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.IdRoles == accountToDelete.IdRoles);
                    if (role == null)
                        return Conflict("Информация о роли не найдена");

                    var trainer = await _dbContext.Coaches.FirstOrDefaultAsync(c => c.IdUserAccounts == accountToDelete.IdUserAccounts);
                    if(trainer != null)
                    {
                        trainer.IdUserAccounts = null;
                        _dbContext.Coaches.Update(trainer);
                        _dbContext.SaveChanges();
                    }

                    _dbContext.UserAccounts.Remove(accountToDelete);
                    _dbContext.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает запрос на изменение учётной записи
        /// </summary>
        /// <param name="account">Изменяемая запись</param>
        /// <returns>Http-ответ, обозначающий результат изменения</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="AdminPolicy")]
        [HttpPut]
        public async Task<ActionResult> EditAccount(UserAccounts account)
        {
            try
            {
                var accounts = await _dbContext.UserAccounts.ToListAsync();

                //Check if record exists
                var recordExists = await _dbContext.UserAccounts.AnyAsync(ac => ac.IdUserAccounts == account.ID_UserAccounts);
                var loginExists = await _dbContext.UserAccounts.AnyAsync(ac => ac.IdUserAccounts != account.ID_UserAccounts && account.Login == ac.Login);
                if (!recordExists)
                    return NotFound("Запись с таким id не найдена");
                if (loginExists)
                    return Conflict("Логин уде существует");


                //Check if edited role does not break the rules
                var requestedRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.IdRoles == account.ID_Roles);
                if (requestedRole == null)
                    return NotFound("Информация о роли не найдена");
                else if (accounts.Count(ac => ac.IdRoles == requestedRole.IdRoles) == 10)
                    return Conflict("Максимально допустимое количество тренеров в системе");

                //check if edited login and password have correct format
                bool areFieldsEmpty = string.IsNullOrEmpty(account.Login) || string.IsNullOrEmpty(account.Password);
                bool isPasswShort = account.Password.Length < 8;
                bool passwContainsSpecials = account.Password.Any(" !\"\'@#$%^(){}*-_=+<>.,\\/;:\'\"|?~`".Contains);
                bool passwContainsLetters = account.Password.Any("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm".Contains);
                bool passwContainsNums = account.Password.Any("0123456789".Contains);
                if (areFieldsEmpty || isPasswShort || !passwContainsLetters || !passwContainsNums || !passwContainsSpecials)
                    return Conflict();

                //editing database
                var saltedHash = _hashingService.GenerateHash(account.Password);
                var editedAccount = await _dbContext.UserAccounts.FirstOrDefaultAsync(ac => ac.IdUserAccounts == account.ID_UserAccounts);

                editedAccount!.IdRoles = account.ID_Roles;
                editedAccount.Password = saltedHash.hash;
                editedAccount.Salt = saltedHash.salt;
                editedAccount.Login = account.Login;

                _dbContext.UserAccounts.Update(editedAccount);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Непредвиденная ошибка сервера");
            }
        }
    }
}
