using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using MagmaTestWebApp.Models;
using MagmaTestWebApp.Services;

namespace MagmaTestWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TreeController : ControllerBase
    {
        private readonly DataService _dataService;

        public TreeController(DataService dataService)
        {
            _dataService = dataService;
        }

        // 1. Эндпоинт запроса полного дерева
        [HttpGet]
        public ActionResult<IEnumerable<TreeItemViewModel>> GetFullTree()
        {
            return Ok(_dataService.Tree);
        }

        // 2. Эндпоинт запроса всех дочерних элементов (включая нижележащие) для заданного узла
        [HttpGet("{handle}/descendants")]
        public ActionResult<IEnumerable<TreeItemViewModel>> GetDescendants(string handle)
        {
            if (string.IsNullOrEmpty(handle) || !handle.StartsWith("TH")) // TODO: Мб TH вынести в константу??
            {
                return BadRequest("Некорректный формат Handle. Handle должен начинаться с 'TH'.");
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

        // 3. Эндпоинт запроса дочерних элементов первого уровня для заданного узла
        [HttpGet("{handle}/children")]
        public ActionResult<IEnumerable<TreeItemViewModel>> GetFirstLevelChildren(string handle)
        {
            if (string.IsNullOrEmpty(handle) || !handle.StartsWith("TH"))
            {
                return BadRequest("Некорректный формат Handle. Handle должен начинаться с 'TH'.");
            }

            var node = FindNodeByHandle(_dataService.Tree, handle);
            // var node2 = _dataService.Tree.Where(t => t.Handle == handle).FirstOrDefault(); - TOOD: ../

            if (node == null)
            {
                return NotFound();
            }

            return Ok(node.Children ?? new List<TreeItemViewModel>());
        }

        // 4. Эндпоинт для поиска папки по названию
        [HttpGet("search")]
        public ActionResult<IEnumerable<TreeItemViewModel>> SearchFoldersByName([FromQuery] string description, [FromQuery] string searchMode)
        {
            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(searchMode))
            {
                return BadRequest("Параметры 'description' и 'searchMode' обязательны.");
            }

            var searchModeLower = searchMode.ToLower();
            if (searchModeLower != "substring" && searchModeLower != "exact")
            {
                return BadRequest("Некорректное значение для searchMode. Допустимые значения: substring, exact.");
            }

            var allNodes = new List<TreeItemViewModel>();
            CollectAllNodes(_dataService.Tree, allNodes);

            var filteredNodes = allNodes.Where(node =>
            {
                if (node.Description == null) return false;

                if (searchModeLower == "exact")
                {
                    return node.Description.Equals(description, StringComparison.OrdinalIgnoreCase);
                }
                else // substring
                {
                    return node.Description.Contains(description, StringComparison.OrdinalIgnoreCase);
                }
            }).ToList();

            return Ok(filteredNodes);
        }

        // 5. Эндпоинт, выдающий плоский список всех папок
        [HttpGet("flat")]
        public ActionResult<IEnumerable<TreeItemViewModel>> GetFlatTree()
        {
            var allNodes = new List<TreeItemViewModel>();
            CollectAllNodes(_dataService.Tree, allNodes);
            return Ok(allNodes);
        }

        // Вспомогательный метод для поиска узла
        private TreeItemViewModel? FindNodeByHandle(List<TreeItemViewModel> nodes, string handle)
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
        private void CollectDescendants(TreeItemViewModel node, List<TreeItemViewModel> descendants)
        {
            if (node.Children == null) return;

            foreach (var child in node.Children)
            {
                descendants.Add(child);
                CollectDescendants(child, descendants);
            }
        }

        // Вспомогательный метод для разворачивания дерева в список
        private void CollectAllNodes(List<TreeItemViewModel> nodes, List<TreeItemViewModel> flatList)
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
    }
}