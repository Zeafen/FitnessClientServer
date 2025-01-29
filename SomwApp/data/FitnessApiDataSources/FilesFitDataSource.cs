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
using System.Web;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SomwApp.data.FitnessApiDataSources
{
    public class FilesFitDataSource : IFilesService
    {
        private readonly string startURL = "https://localhost:7129/reviewfiles";
        private static FilesFitDataSource? _Instance = null;
        public static FilesFitDataSource GetInstance() => _Instance ?? (_Instance = new FilesFitDataSource());
        private FilesFitDataSource() { }


        public void CreateFile(ReviewFileRequest fileRequest)
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
                HttpContent content = new StringContent(JsonConvert.SerializeObject(fileRequest), Encoding.UTF8, "application/json");
                var result = client.PostAsync(startURL, content).Result;
                if (!result.IsSuccessStatusCode)
                    MessageBox.Show($"Не удалось добавить отчёт. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else MessageBox.Show($"Отчёт успешно добавлена.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось добавить отчёт. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteFile(string fileName)
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
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["fileName"] = fileName;
                var result = client.DeleteAsync(startURL + @$"/{query.ToString()}").Result;
                if (!result.IsSuccessStatusCode)
                    MessageBox.Show($"Не удалось удалить запись. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else MessageBox.Show($"Запись успешно удалена", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<ReviewFile>? GetFiles()
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
                    MessageBox.Show($"Не удалось получить отчёты. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<List<ReviewFile>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить отчёты. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<ReviewFile>? GetFilesByDate(DateTime date)
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
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["date"] = JsonConvert.SerializeObject(date);
                var result = client.GetAsync(startURL + @$"/bydate/{query.ToString()}").Result;
                if (!result.IsSuccessStatusCode){
                    MessageBox.Show($"Не удалось получить отчёты. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return JsonConvert.DeserializeObject<List<ReviewFile>>(result.Content.ReadAsStringAsync().Result);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null ;
            }
        }

        public List<ReviewFile>? GetFilesByType(ReviewType type)
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
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["type"] = JsonConvert.SerializeObject(type);
                var result = client.GetAsync(startURL + @$"/bydate/{query.ToString()}").Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить отчёты. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return JsonConvert.DeserializeObject<List<ReviewFile>>(result.Content.ReadAsStringAsync().Result);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<ReviewFile>? GetFilesInPeriod(DateTime dateFrom, DateTime dateTo)
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
                var content = new StringContent(JsonConvert.SerializeObject(new DatePeriodRequest() { DateFrom = DateOnly.FromDateTime(dateFrom), DateTo = DateOnly.FromDateTime(dateTo) }), Encoding.UTF8, "application/json");

                var result = client.PostAsync(startURL + @$"/inperiod", content).Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить отчёты. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return JsonConvert.DeserializeObject<List<ReviewFile>>(result.Content.ReadAsStringAsync().Result);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
