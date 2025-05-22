using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using MagmaTestWebApp.Models;
using MagmaTestWebApp.Services;

namespace MagmaTestWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private const string _protectedUserId = "1003"; // Админ. Его, так уж и быть, не трогаем
        private readonly DataService _dataService;
        // private readonly object _usersLock = new(); // П - Потокобезопасность
        // В entity framework есть db async, изобретать велосипед не вижу смысла, не вижу смысла использовать lock и ConcurrentBag

        public UsersController(DataService dataService)
        {
            _dataService = dataService;
        }

        // 1. Эндпоинт запроса всех пользователей
        [HttpGet]
        public ActionResult<IEnumerable<UserViewModel>> GetAllUsers()
        {
            return Ok(_dataService.Users);
        }

        // 2. Эндпоинт запроса пользователя по ID
        [HttpGet("{id}")]
        public ActionResult<UserViewModel> GetUserById([FromRoute] string id)
        {
            var user = _dataService.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // 3. Эндпоинт создания нового пользователя
        [HttpPost]
        public ActionResult<UserViewModel> CreateUser([FromBody] UserViewModel newUser)
        {
            if (string.IsNullOrEmpty(newUser.Id))
            {
                return BadRequest("ID пользователя обязателен.");
            }

            if (_dataService.Users.Any(u => u.Id == newUser.Id))
            {
                return BadRequest($"Пользователь с ID {newUser.Id} уже существует.");
            }

            // Глубокая копия
            var userToAdd = new UserViewModel
            {
                Key = newUser.Key,
                Description = newUser.Description,
                FirstName = newUser.FirstName,
                MiddleName = newUser.MiddleName,
                LastName = newUser.LastName,
                Mail = newUser.Mail,
                Phone = newUser.Phone,
                IsDisabled = newUser.IsDisabled,
                Id = newUser.Id,
                Profession = newUser.Profession
            };

            _dataService.Users.Add(userToAdd);
            /*lock (_usersLock)
            {
                _dataService.Users.Add(userToAdd);
            }*/

            return CreatedAtAction(nameof(GetUserById), new { id = userToAdd.Id }, userToAdd);
        }

        // 4. Эндпоинт обновления пользователя по ID
        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromRoute] string id, [FromBody] UserViewModel updatedUser)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(updatedUser.Id) || id != updatedUser.Id)
            {
                return BadRequest("ID пользователя в маршруте и теле запроса должны совпадать и быть указаны.");
            }

            if (id == _protectedUserId)
            {
                return BadRequest("Данного пользователя нельзя изменять.");
            }

            var existingUser = _dataService.Users.FirstOrDefault(u => u.Id == id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Обновляем свойства существующего пользователя
            existingUser.Key = updatedUser.Key;
            existingUser.Description = updatedUser.Description;
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.MiddleName = updatedUser.MiddleName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Mail = updatedUser.Mail;
            existingUser.Phone = updatedUser.Phone;
            existingUser.IsDisabled = updatedUser.IsDisabled;
            existingUser.Profession = updatedUser.Profession;

            return Ok(existingUser); // Возвращаем обновленный объект
            // Либо return Ok(new { Message = "Пользователь обновлен", User = existingUser });
        }

        // 5. Эндпоинт удаления пользователя по ID
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID пользователя обязателен.");
            }

            if (id == _protectedUserId)
            {
                return BadRequest("Данного пользователя нельзя удалять.");
            }

            var userToRemove = _dataService.Users.FirstOrDefault(u => u.Id == id);

            if (userToRemove == null)
            {
                return NotFound();
            }

            _dataService.Users.Remove(userToRemove);

            return NoContent();
        }
    }
} 