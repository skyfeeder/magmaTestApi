using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using MagmaTestWebApp.Models;
using MagmaTestWebApp.Services;
using MagmaTestWebApp.Enums;

namespace MagmaTestWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TreeController : ControllerBase
    {
        private readonly DataService _dataService;

        private const string HandlePrefix = "TH";

        public TreeController(DataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Возвращает полное дерево.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<TreeItemViewModel>> GetFullTree()
        {
            return Ok(_dataService.Tree);
        }

        /// <summary>
        /// Возвращает всех потомков, включая вложенные, для указанного узла по handle.
        /// </summary>
        /// <param name="handle">Идентификатор узла</param>
        [HttpGet("{handle}/descendants")]
        public ActionResult<IEnumerable<TreeItemViewModel>> GetDescendants(string handle)
        {
            if (!IsValidHandle(handle))
            {
                return BadRequest($"Некорректный формат Handle. Handle должен начинаться с '{HandlePrefix}'.");
            }

            var node = FindNodeByHandle(_dataService.Tree, handle);

            if (node == null)
            {
                return NotFound();
            }

            var descendants = new List<TreeItemViewModel>();
            CollectDescendants(node, descendants);
            return Ok(descendants);
        }

        /// <summary>
        /// Возвращает дочерние элементы 1 уровня для указанного узла по handle.
        /// </summary>
        /// <param name="handle">Идентификатор узла</param>
        [HttpGet("{handle}/children")]
        public ActionResult<IEnumerable<TreeItemViewModel>> GetFirstLevelChildren(string handle)
        {
            if (!IsValidHandle(handle))
            {
                return BadRequest($"Некорректный формат Handle. Handle должен начинаться с '{HandlePrefix}'.");
            }

            var node = FindNodeByHandle(_dataService.Tree, handle);

            if (node == null)
            {
                return NotFound();
            }

            return Ok(node.Children ?? new List<TreeItemViewModel>());
        }

        /// <summary>
        /// Выполняет поиск узлов по описанию с заданным режимом сравнения.
        /// </summary>
        /// <param name="description">Описание для поиска</param>
        /// <param name="searchMode">Режим поиска: substring (подстрока) или exact (точное совпадение) </param>
        [HttpGet("search")]
        public ActionResult<IEnumerable<TreeItemViewModel>> SearchFoldersByName([FromQuery] string description, [FromQuery] string searchMode)
        {
            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(searchMode))
            {
                return BadRequest("Параметры 'description' и 'searchMode' обязательны.");
            }

            if (!Enum.TryParse<SearchMode>(searchMode, true, out var parsedMode))
            {
                // Не выводим допустимые значения через Enum.GetNames, чтобы при добавлении новых значений явно обновлять switch
                return BadRequest("Некорректное значение для searchMode. Допустимые значения: substring, exact.");
            }

            var allNodes = new List<TreeItemViewModel>();
            CollectAllNodes(_dataService.Tree, allNodes);

            var filteredNodes = allNodes.Where(node =>
            {
                if (node.Description == null) return false;

                return parsedMode switch
                {
                    SearchMode.Exact => node.Description.Equals(description, StringComparison.OrdinalIgnoreCase),
                    SearchMode.Substring => node.Description.Contains(description, StringComparison.OrdinalIgnoreCase),
                    _ => false // Отработка default случая лишней не будет
                };
            }).ToList();

            return Ok(filteredNodes);
        }

        /// <summary>
        /// Возвращает дерево в виде плоского списка всех узлов.
        /// </summary>
        [HttpGet("flat")]
        public ActionResult<IEnumerable<TreeItemViewModel>> GetFlatTree()
        {
            var allNodes = new List<TreeItemViewModel>();
            CollectAllNodes(_dataService.Tree, allNodes);
            return Ok(allNodes);
        }

        // Вспомогательный метод для поиска узла
        private static TreeItemViewModel? FindNodeByHandle(List<TreeItemViewModel> nodes, string handle)
        {
            foreach (var node in nodes)
            {
                if (node.Handle == handle)
                {
                    return node;
                }
                if (node.Children != null)
                {
                    var found = FindNodeByHandle(node.Children, handle);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }

        // Вспомогательный метод для сбора всех потомков (descendants)
        private static void CollectDescendants(TreeItemViewModel node, List<TreeItemViewModel> descendants)
        {
            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                descendants.Add(child);
                CollectDescendants(child, descendants);
            }
        }

        // Вспомогательный метод для разворачивания дерева в список
        private static void CollectAllNodes(List<TreeItemViewModel> nodes, List<TreeItemViewModel> flatList)
        {
            foreach (var node in nodes)
            {
                flatList.Add(node);
                if (node.Children != null)
                {
                    CollectAllNodes(node.Children, flatList);
                }
            }
        }

        private static bool IsValidHandle(string handle)
        {
            return !string.IsNullOrEmpty(handle) && handle.StartsWith(HandlePrefix);
        }
    }
}