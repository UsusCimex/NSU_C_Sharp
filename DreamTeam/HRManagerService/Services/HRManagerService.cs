using Shared.Models;
using Shared.Messages;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;
using HRManagerService.Utilities;

namespace HRManagerService.Services
{
    public class HRManagerService
    {
        private readonly ILogger<HRManagerService> _logger;
        private readonly Dictionary<int, List<Wishlist>> _wishlistsPerHackathon;
        private readonly object _lockObject = new object();
        private readonly List<Employee> _teamLeads;
        private readonly List<Employee> _juniors;

        public HRManagerService(ILogger<HRManagerService> logger)
        {
            _logger = logger;
            _wishlistsPerHackathon = new Dictionary<int, List<Wishlist>>();

            // Чтение участников из файлов
            _teamLeads = ParticipantReader.ReadParticipants("TeamLeads.csv", EmployeeRole.TeamLead);
            _juniors = ParticipantReader.ReadParticipants("Juniors.csv", EmployeeRole.Junior);
        }

        public void ProcessWishlist(Wishlist wishlist)
        {
            lock (_lockObject)
            {
                if (!_wishlistsPerHackathon.ContainsKey(wishlist.HackathonId))
                {
                    _wishlistsPerHackathon[wishlist.HackathonId] = new List<Wishlist>();
                }
                _wishlistsPerHackathon[wishlist.HackathonId].Add(wishlist);

                // Проверяем, собраны ли все вишлисты
                if (AllWishlistsCollected(wishlist.HackathonId))
                {
                    _logger.LogInformation($"HRManager: Все вишлисты для хакатона {wishlist.HackathonId} собраны. Формируем команды.");
                    BuildTeamsAndSendToHRDirector(wishlist.HackathonId);
                }
            }
        }

        private bool AllWishlistsCollected(int hackathonId)
        {
            int totalParticipants = _teamLeads.Count + _juniors.Count;
            return _wishlistsPerHackathon[hackathonId].Count >= totalParticipants;
        }

        private void BuildTeamsAndSendToHRDirector(int hackathonId)
        {
            var wishlists = _wishlistsPerHackathon[hackathonId];

            // Здесь используйте вашу стратегию формирования команд
            var teams = TeamBuilder.BuildTeams(hackathonId, _teamLeads, _juniors, wishlists);

            var hrManagerData = new HRManagerData
            {
                Teams = teams,
                Wishlists = wishlists
            };

            // Отправка данных HRDirector через HTTP
            SendDataToHRDirector(hrManagerData).Wait();

            _wishlistsPerHackathon.Remove(hackathonId);
        }

        private async Task SendDataToHRDirector(HRManagerData data)
        {
            try
            {
                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("http://hrdirector:8000/api/teams", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Данные успешно отправлены HRDirector");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Ошибка при отправке данных HRDirector: {response.StatusCode}, {responseContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Ошибка подключения к HRDirector: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError("Превышено время ожидания ответа от HRDirector");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Неизвестная ошибка: {ex.Message}");
            }
        }
    }
}
