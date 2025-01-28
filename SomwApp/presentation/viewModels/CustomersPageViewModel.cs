using SomwApp.data.FitnessApiDataSources;
using SomwApp.domain.commands;
using SomwApp.domain.models;
using SomwApp.domain.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SomwApp.presentation.viewModels
{
    public class CustomersPageViewModel : INotifyPropertyChanged
    {
        private ICustomersDataSource _custsDataSource = CustomersFitDataSource.GetInstance();
        private IPaymentsDataSource _paymentsDataSource = PaymentsFitDataSource.GetInstance();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private RelayCommand<CustomerModel>? _customerEditCommand = null;
        public RelayCommand<CustomerModel> CustomerEditCommand => _customerEditCommand ??= new RelayCommand<CustomerModel>(EditCustomer, model => model != null
        && Customers.Any(c => c.ID_Customer == model.ID_Customer)
        && !string.IsNullOrEmpty(model.PhoneNumber)
        && !string.IsNullOrEmpty(model.Surname)
        && !string.IsNullOrEmpty(model.Name));

        private RelayCommand<CustomerModel>? _customerAddCommand = null;
        public RelayCommand<CustomerModel> CustomerAddCommand => _customerAddCommand ??= new RelayCommand<CustomerModel>(AddCustomer, model => model != null
        && !string.IsNullOrEmpty(model.PhoneNumber)
        && !string.IsNullOrEmpty(model.Surname)
        && !string.IsNullOrEmpty(model.Name));

        private RelayCommand<CustomerModel>? _customerDeleteCommand = null;
        public RelayCommand<CustomerModel> CustomerDeleteCommand => _customerDeleteCommand ??= new RelayCommand<CustomerModel>(DeleteCustomer, model => model != null
        && Customers.Any(p => p.ID_Customer == model.ID_Customer));

        private RelayCommand? _clearFiltersCommand = null;
        public RelayCommand ClearFiltersCommand => _clearFiltersCommand ??= new RelayCommand(obj => { _statusFilter = null; }, obj => true);

        public CustomersPageViewModel()
        {

            object lockDownloads = new object();
            BindingOperations.EnableCollectionSynchronization(Customers, lockDownloads);
            UpdateData();
            _selectedCustomer = new CustomerModel();
        }
        ~CustomersPageViewModel()
        {
            _cts.Cancel();
        }

        public ObservableCollection<CustomerModel> Customers { get; set; } = new ObservableCollection<CustomerModel>();
        public List<SubscriptionStatus> Statuses { get; private set; } = new List<SubscriptionStatus>()
        {
            SubscriptionStatus.NotObtain,
            SubscriptionStatus.Expired,
            SubscriptionStatus.Active,
        };

        private CustomerModel _selectedCustomer { get; set; } = new CustomerModel();
        public CustomerModel SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if(_selectedCustomer != value)
                {
                    _selectedCustomer = value;
                    onPropertyChanged();
                }
            }
        }

        private SubscriptionStatus? _statusFilter { get; set; } = null;
        public SubscriptionStatus? StatusFilter
        {
            get => _statusFilter;
            set
            {
                if(_statusFilter != value)
                {
                    _statusFilter = value;
                    onPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        public void UpdateData()
        {
            Task.Run(() =>
            {
                Customers.Clear();
                foreach (var customer in _custsDataSource.GetCustomers()??new List<Customer>())
                {
                    SubscriptionStatus status = SubscriptionStatus.NotObtain;
                    if (_paymentsDataSource.GetPaymentsByCustomer(customer.ID_Customers)?.Any()??false)
                        status = SubscriptionStatus.NotObtain;
                    else if (_paymentsDataSource.GetPaymentsByCustomer(customer.ID_Customers)?.Any(pay => DateOnly.FromDateTime(DateTime.Now).CompareTo(pay.ValidityEndDate) <= 0)??false)
                        status = SubscriptionStatus.Active;
                    else status = SubscriptionStatus.Expired;
                    Customers.Add(new CustomerModel()
                    {
                        ID_Customer = customer.ID_Customers,
                        Name = customer.Name,
                        MiddleName = customer.MiddleName,
                        Surname = customer.Surname,
                        PhoneNumber = customer.PhoneNumber,
                        SubscriptionStatus = status
                    });
                }
            }, _cts.Token);
        }
        public void ApplyFilters()
        {
            Task.Run(() =>
            {

                if (StatusFilter != null)
                {
                    Customers.Clear();

                    switch (StatusFilter)
                    {
                        case SubscriptionStatus.NotObtain:
                            foreach (var customer in _custsDataSource.GetCustomers()??new List<Customer>())
                                if (!_paymentsDataSource.GetPaymentsByCustomer(customer.ID_Customers)?.Any()??false)
                                    Customers.Add(new CustomerModel()
                                    {
                                        ID_Customer = customer.ID_Customers,
                                        Name = customer.Name,
                                        MiddleName = customer.MiddleName,
                                        Surname = customer.Surname,
                                        PhoneNumber = customer.PhoneNumber,
                                        SubscriptionStatus = SubscriptionStatus.NotObtain,
                                    });
                            break;
                        case SubscriptionStatus.Active:
                            foreach (var customer in _custsDataSource.GetCustomers()??new List<Customer>())
                                if (!_paymentsDataSource.GetPaymentsByCustomer(customer.ID_Customers)?.Any(p => DateOnly.FromDateTime(DateTime.Now).CompareTo(p.ValidityEndDate) >= 0) ?? false)
                                    Customers.Add(new CustomerModel()
                                    {
                                        ID_Customer = customer.ID_Customers,
                                        Name = customer.Name,
                                        MiddleName = customer.MiddleName,
                                        Surname = customer.Surname,
                                        PhoneNumber = customer.PhoneNumber,
                                        SubscriptionStatus = SubscriptionStatus.Active,
                                    });
                            break;
                        case SubscriptionStatus.Expired:
                            foreach (var customer in _custsDataSource.GetCustomers() ?? new List<Customer>())
                                if (!_paymentsDataSource.GetPaymentsByCustomer(customer.ID_Customers)?.Any(p => DateOnly.FromDateTime(DateTime.Now).CompareTo(p.ValidityEndDate) < 0) ?? false)
                                    Customers.Add(new CustomerModel()
                                    {
                                        ID_Customer = customer.ID_Customers,
                                        Name = customer.Name,
                                        MiddleName = customer.MiddleName,
                                        Surname = customer.Surname,
                                        PhoneNumber = customer.PhoneNumber,
                                        SubscriptionStatus = SubscriptionStatus.Active,
                                    });
                            break;
                    }
                }
                else UpdateData();
            },_cts.Token);
        }
        private void DeleteCustomer(CustomerModel model)
        {
            _custsDataSource.DeleteCustomer(model.ID_Customer);
            ApplyFilters();
        }
        private void EditCustomer(CustomerModel model)
        {
            _custsDataSource.EditCustomer((Customer)model);
            ApplyFilters();
        }
        private void AddCustomer(CustomerModel model)
        {
            _custsDataSource.AddCustomer((Customer)model);
            ApplyFilters();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
