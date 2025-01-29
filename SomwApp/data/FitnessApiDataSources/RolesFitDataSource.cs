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
    public class RolesFitDataSource : IRolesDataSource
    {
        private readonly string startURL = $"https://localhost:7129/roles";
        private static RolesFitDataSource? _Instance = null;
        public static RolesFitDataSource GetInstance() => _Instance ?? (_Instance = new RolesFitDataSource());

        private RolesFitDataSource() { }


        public Role? GetRoleByID(int roleID)
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
                var result = client.GetAsync(startURL + @$"/{roleID}").Result;
                if (!result.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Не удалось получить запись. \n{result.StatusCode}: {result.Content.ReadAsStringAsync().Result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                return JsonConvert.DeserializeObject<Role>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить запись. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<Role>? GetRoles()
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
                return JsonConvert.DeserializeObject<List<Role>>(result.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить записи. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
