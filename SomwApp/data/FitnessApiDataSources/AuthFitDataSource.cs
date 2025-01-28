using Newtonsoft.Json;
using SomwApp.domain;
using SomwApp.domain.models;
using SomwApp.domain.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace SomwApp.data.FitnessApiDataSources
{
    public class AuthFitDataSource : IAuthService
    {
        private readonly string startURL = $"https://localhost:7129/auth";
        private static AuthFitDataSource? _Instance = null;
        public static AuthFitDataSource GetInstance() => _Instance ?? (_Instance = new AuthFitDataSource());

        private AuthFitDataSource() { }
        public bool Authorize(string token)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("Authorization", token);

                var result = client.GetAsync(startURL + "/authorize").Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось войти в учётную запись. \n{result.StatusCode}: {result.Content}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось войти в учётную запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public AuthResponse? LogIn(AuthRequest request)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                var client = new HttpClient(handler);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = client.PostAsync(startURL + "/signIn", content).Result;
                if (!result.IsSuccessStatusCode)
                    MessageBox.Show($"Не удалось войти в учётную запись. \n{result.StatusCode}: {result.Content}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return JsonConvert.DeserializeObject<AuthResponse>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось войти в учётную запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
