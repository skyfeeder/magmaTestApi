using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MagmaTestWebApp.Models;

namespace MagmaTestWebApp.Services
{
    public class DataService
    {
        public List<ProjectViewModel> Projects { get; private set; }
        public List<TreeItemViewModel> Tree { get; private set; }
        public List<UserViewModel> Users { get; private set; }

        private readonly string _dataFolderPath;

        public DataService(IHostEnvironment env)
        {
            _dataFolderPath = Path.Combine(env.ContentRootPath, "Data");
            // _dataFolderPath = Directory.GetCurrentDirectory();
            // _dataFolderPath = AppDomain.CurrentDomain.BaseDirectory; TODO: fixit
            Projects = new List<ProjectViewModel>();
            Tree = new List<TreeItemViewModel>();
            Users = new List<UserViewModel>();
        }

        public async Task LoadDataAsync()
        {
            await LoadProjectsAsync();
            await LoadTreeAsync();
            await LoadUsersAsync();
        }

        private async Task LoadProjectsAsync()
        {
            var filePath = Path.Combine(_dataFolderPath, "projects.json");
            using FileStream openStream = File.OpenRead(filePath);
            Projects = await JsonSerializer.DeserializeAsync<List<ProjectViewModel>>(openStream) ?? new List<ProjectViewModel>();
        }
        /* Можно и обобщить
         * private async Task<List<T>> LoadJsonAsync<T>(string fileName)
         * И использовать
         * Projects = await LoadJsonAsync<ProjectViewModel>("projects.json");
         */

        private async Task LoadTreeAsync()
        {
            var filePath = Path.Combine(_dataFolderPath, "tree.json");
            using FileStream openStream = File.OpenRead(filePath);
            Tree = await JsonSerializer.DeserializeAsync<List<TreeItemViewModel>>(openStream) ?? new List<TreeItemViewModel>();
        }

        private async Task LoadUsersAsync()
        {
            var filePath = Path.Combine(_dataFolderPath, "users.json");
            using FileStream openStream = File.OpenRead(filePath);
            Users = await JsonSerializer.DeserializeAsync<List<UserViewModel>>(openStream) ?? new List<UserViewModel>();
        }
    }
}