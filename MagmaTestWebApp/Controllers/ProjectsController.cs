using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MagmaTestWebApp.Models;
using MagmaTestWebApp.Services;
using MagmaTestWebApp.Enums;

namespace MagmaTestWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly DataService _dataService;

        private const string HandlePrefix = "TH";

        public ProjectsController(DataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Возвращает все проекты.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<ProjectViewModel>> GetAllProjects()
        {
            return Ok(_dataService.Projects);
        }

        /// <summary>
        /// Возвращает проект по указанному Handle.
        /// </summary>
        /// <param name="handle">Уникальный идентификатор проекта (начинается с 'TH').</param>
        [HttpGet("{handle}")]
        public ActionResult<ProjectViewModel> GetProjectByHandle(string handle)
        {
            if (!IsValidHandle(handle))
            {
                return BadRequest($"Некорректный формат Handle. Handle должен начинаться с '{HandlePrefix}'.");
            }

            var project = _dataService.Projects.FirstOrDefault(p => p.Handle == handle);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        /// <summary>
        /// Удаляет проект по указанному Handle.
        /// </summary>
        /// <param name="handle">Уникальный идентификатор проекта (начинается с 'TH').</param>
        [HttpDelete("{handle}")]
        public IActionResult DeleteProject(string handle)
        {
            if (!IsValidHandle(handle))
            {
                return BadRequest($"Некорректный формат Handle. Handle должен начинаться с '{HandlePrefix}'.");
            }

            var project = _dataService.Projects.FirstOrDefault(p => p.Handle == handle);

            if (project == null)
            {
                return NotFound();
            }

            _dataService.Projects.Remove(project);

            return NoContent();
        }

        /// <summary>
        /// Возвращает проекты, отфильтрованные по диапазону дат ModifyTime.
        /// </summary>
        /// <param name="startDate">Начальная дата (необязательно).</param>
        /// <param name="endDate">Конечная дата (необязательно).</param>
        [HttpGet("filterByDate")]
        public ActionResult<IEnumerable<ProjectViewModel>> FilterProjectsByDate([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var filteredProjects = _dataService.Projects.AsEnumerable();

            if (startDate.HasValue)
            {
                filteredProjects = filteredProjects.Where(p => p.ModifyTime.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                filteredProjects = filteredProjects.Where(p => p.ModifyTime.Date <= endDate.Value.Date);
            }

            return Ok(filteredProjects.ToList());
        }

        /// <summary>
        /// Возвращает проекты, содержащие указанную подстроку в описании.
        /// </summary>
        /// <param name="description">Подстрока для поиска в описании.</param>
        [HttpGet("filterByDescription")]
        public ActionResult<IEnumerable<ProjectViewModel>> FilterProjectsByDescription([FromQuery] string? description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return Ok(_dataService.Projects);
            }

            var filteredProjects = _dataService.Projects
                .Where(p => p.Description != null && p.Description.Contains(description, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(filteredProjects);
        }

        // 6. Эндпоинт с параметром roleName для фильтрации по роли
        // TODO: Вынести роли в enum (Enum.TryParse)
        [HttpGet("filterByRole")]
        public ActionResult<IEnumerable<ProjectViewModel>> FilterProjectsByRole([FromQuery] string? roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Параметр roleName обязателен.");
            }

            if (!Enum.TryParse<UserRoleType>(roleName, true, out var parsedRole))
            {
                return BadRequest($"Некорректное значение для roleName: {roleName}. Допустимые значения: member, editor, admin, organizationadmin.");
            }

            var filteredProjects = _dataService.Projects.Where(p =>
            {
                if (p.UserRole == null) return false;

                return parsedRole switch
                {
                    UserRoleType.Member => p.UserRole.Member?.HasRole == true,
                    UserRoleType.Editor => p.UserRole.Editor?.HasRole == true,
                    UserRoleType.Admin => p.UserRole.Admin?.HasRole == true,
                    UserRoleType.OrganizationAdmin => p.UserRole.OrganizationAdmin?.HasRole == true,
                    _ => false
                };
            }).ToList();

            return Ok(filteredProjects);
        }


        /// <summary>
        /// Возвращает список заказчиков с их Handle.
        /// </summary>
        [HttpGet("customers")]
        public ActionResult<IEnumerable<object>> GetProjectCustomers()
        {
            var customers = _dataService.Projects.Select(p => new
            {
                p.Handle,
                p.Customer
            }).ToList();

            return Ok(customers);
        }

        // 8. Эндпоинт, отдающий все архивные проекты
        [HttpGet("archived")]
        public ActionResult<IEnumerable<ProjectViewModel>> GetArchivedProjects()
        {
            var archivedProjects = _dataService.Projects.Where(p => p.Status?.Description == "Архивный").ToList();
            return Ok(archivedProjects);
        }

        /// <summary>
        /// Возвращает проекты со статусом "Архивный".
        /// </summary>
        [HttpPut("{handle}/maximumSize")]
        public IActionResult UpdateProjectMaximumSize(string handle, [FromBody] long maximumSize)
        {
            if (!IsValidHandle(handle))
            {
                return BadRequest($"Некорректный формат Handle. Handle должен начинаться с '{HandlePrefix}'.");
            }

            var project = _dataService.Projects.FirstOrDefault(p => p.Handle == handle);

            if (project == null)
            {
                return NotFound();
            }

            if (maximumSize < 0)
            {
                return BadRequest("MaximumSize не может быть отрицательным.");
            }

            project.MaximumSize = maximumSize;

            return Ok(project);
        }

        private static bool IsValidHandle(string handle)
        {
            return !string.IsNullOrEmpty(handle) && handle.StartsWith(HandlePrefix);
        }
    }
}