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
    public class AppointmentsFitDataSource : IAppointmentsForClassesDataSource
    {
        private readonly string startURL = $"https://localhost:7129/appointments";
        private static AppointmentsFitDataSource? _Instance = null;
        public static AppointmentsFitDataSource GetInstance() => _Instance ?? (_Instance = new AppointmentsFitDataSource());
        private AppointmentsFitDataSource() { }
        public void AddAppointment(AppointmentsForClasses appointment)
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
                HttpContent content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, "application/json");
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

        public void DeleteAppointment(int appointmentID)
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
                var result = client.DeleteAsync(startURL + @$"/{appointmentID}").Result;
                if (!result.IsSuccessStatusCode)
                    MessageBox.Show($"Не удалось удалить запись. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else MessageBox.Show($"Запись успешно удалена", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void EditAppointment(AppointmentsForClasses appointment)
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
                HttpContent content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, "application/json");
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

        public AppointmentsForClasses? GetAppointment(int id)
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
                return JsonConvert.DeserializeObject<AppointmentsForClasses>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<AppointmentsForClasses>? GetAppointments()
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
                return JsonConvert.DeserializeObject<List<AppointmentsForClasses>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить записи. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<AppointmentsForClasses>? GetAppointmentsByCoach(int coachID)
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
                var result = client.GetAsync(startURL + @$"/bycoach/{coachID}").Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить записи. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<List<AppointmentsForClasses>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить записи. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<AppointmentsForClasses>? GetAppointmentsByCustomer(int CustomerID)
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
                var result = client.GetAsync(startURL + @$"/bycustomer/{CustomerID}").Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить записи. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<List<AppointmentsForClasses>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить записи. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<AppointmentsForClasses>? GetAppointmentsByDate(DateOnly date)
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
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить записи. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<List<AppointmentsForClasses>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить записи. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<AppointmentsForClasses>? GetAppointmentsByLessons(int LessonID)
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
                var result = client.GetAsync(startURL + @$"/bylesson/{LessonID}").Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить записи. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<List<AppointmentsForClasses>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить записи. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<AppointmentsForClasses>? GetAppointmentsInPeriod(DateOnly dateFrom, DateOnly dateTo)
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

                var content = new StringContent(JsonConvert.SerializeObject(new DatePeriodRequest() { DateFrom = dateFrom, DateTo = dateTo }), Encoding.UTF8, "application/json");

                var result = client.PostAsync(startURL + @$"/inperiod", content).Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить записи. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<List<AppointmentsForClasses>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить записи. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
