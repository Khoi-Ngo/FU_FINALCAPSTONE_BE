using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AISEA.ApiService.DAL.Infrastructure
{
    public class HolidayService : IHolidayService
    {
        private readonly BookingSettings _bookingSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HolidayService> _logger;

        public HolidayService(BookingSettings bookingSettings, ILogger<HolidayService> logger, HttpClient httpClient)
        {
            _bookingSettings = bookingSettings;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<List<HolidayResponse>> CheckHolidayAsync(DateOnly date)
        {
            var apiKey = _bookingSettings.abstractapi_HolidayApiApiKey;
            var countryCode = _bookingSettings.CountryCode_holiday;
            var baseUrl = _bookingSettings.Abstractapi_HolidayApiBaseUrl;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(baseUrl))
            {
                _logger.LogWarning("Holiday API settings are not fully configured.");
                return new List<HolidayResponse>();
            }

            try
            {
                var url = $"{baseUrl}?api_key={apiKey}&country={countryCode}&year={date.Year}&month={date.Month}&day={date.Day}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var holidayList = JsonSerializer.Deserialize<List<HolidayResponse>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return holidayList ?? new List<HolidayResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking holiday for date {Date}. Returning empty list.", date);
                return new List<HolidayResponse>();
            }
        }
    }
}
