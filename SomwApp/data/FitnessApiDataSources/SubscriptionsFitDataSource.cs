using Newtonsoft.Json;
using SomwApp.domain;
using SomwApp.domain.models;
using SomwApp.domain.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SomwApp.data.FitnessApiDataSources
{
    public class SubscriptionsFitDataSource : ISubscriptionDataSource
    {
        private readonly string startURL = $"https://localhost:7129/subscriptions";
        private static SubscriptionsFitDataSource? _Instance = null;
        public static SubscriptionsFitDataSource GetInstance() => _Instance ?? (_Instance = new SubscriptionsFitDataSource());
        private SubscriptionsFitDataSource() { }

        public void AddSubscription(Subscription subscription)
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
                var token = Application.Current.Resources[ResourceKeys.TOKEN_RESOURCE_KEY] as string;
                if (token == null)
                    return;
                client.DefaultRequestHeaders.Add("Authorization", token);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(subscription), Encoding.UTF8, "application/json");
                var result = client.PostAsync(startURL, content).Result;
                if (!result.IsSuccessStatusCode)
                    MessageBox.Show($"Не удалось добавить запись. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else MessageBox.Show($"Запись успешно добавлена.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось добавить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteSubscription(int subscriptionID)
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
                var token = Application.Current.Resources[ResourceKeys.TOKEN_RESOURCE_KEY] as string;
                if (token == null)
                    return;
                client.DefaultRequestHeaders.Add("Authorization", token);
                var result = client.DeleteAsync(startURL + @$"/{subscriptionID}").Result;
                if (!result.IsSuccessStatusCode)
                    MessageBox.Show($"Не удалось удалить запись. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else MessageBox.Show($"Запись успешно удалена", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void EditSubscription(Subscription subscription)
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
                var token = Application.Current.Resources[ResourceKeys.TOKEN_RESOURCE_KEY] as string;
                if (token == null)
                    return;
                client.DefaultRequestHeaders.Add("Authorization", token);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(subscription), Encoding.UTF8, "application/json");
                var result = client.PutAsync(startURL, content).Result;
                if (!result.IsSuccessStatusCode)
                    MessageBox.Show($"Не удалось обновить запись. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else MessageBox.Show($"Запись успешно обновлена.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось обновить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Subscription? GetSubscription(int id)
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
                var token = Application.Current.Resources[ResourceKeys.TOKEN_RESOURCE_KEY] as string;
                if (token == null)
                    return null;
                client.DefaultRequestHeaders.Add("Authorization", token);
                var result = client.GetAsync(startURL + @$"/{id}").Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить запись. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<Subscription>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<Subscription>? GetSubscriptions()
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
                var token = Application.Current.Resources[ResourceKeys.TOKEN_RESOURCE_KEY] as string;
                if (token == null)
                    return null;
                client.DefaultRequestHeaders.Add("Authorization", token);
                var result = client.GetAsync(startURL).Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить записи. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<List<Subscription>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить записи. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
