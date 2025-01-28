using SomwApp.data.FitnessApiDataSources;
using SomwApp.domain.commands;
using SomwApp.domain.models;
using SomwApp.domain.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SomwApp.presentation.viewModels
{
    public class PaymentsViewModel : INotifyPropertyChanged
    {
        private RelayCommand<PaymentModel>? _paymentEditCommand = null;
        public RelayCommand<PaymentModel> PaymentEditCommand => _paymentEditCommand ??= new RelayCommand<PaymentModel>(EditPayment, CanEditPayment);

        private RelayCommand<PaymentModel>? _paymentAddCommand = null;
        public RelayCommand<PaymentModel> PaymentAddCommand => _paymentAddCommand ??= new RelayCommand<PaymentModel>(AddPayment, CanAddPayment);

        private RelayCommand<PaymentModel>? _paymentDeleteCommand = null;
        public RelayCommand<PaymentModel> PaymentDeleteCommand => _paymentDeleteCommand ??= new RelayCommand<PaymentModel>(DeletePayment, model => model != null && Payments.FirstOrDefault(p => p.ID_Payment == model.ID_Payment) != null);


        public ObservableCollection<PaymentModel> Payments { get; set; } = new ObservableCollection<PaymentModel>();
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();
        public ObservableCollection<Subscription> Subscriptions { get; set; } = new ObservableCollection<Subscription>();

        private ICustomersDataSource custDataSource = CustomersFitDataSource.GetInstance();
        private ISubscriptionDataSource subsDataSource = SubscriptionsFitDataSource.GetInstance();
        private IPaymentsDataSource paysDataSource = PaymentsFitDataSource.GetInstance();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private PaymentModel _selectedPayment { get; set; } = new PaymentModel();
        public PaymentModel SelectedPayment
        {
            get => _selectedPayment;
            set
            {
                if (_selectedPayment != value && value != null)
                {
                    _selectedPayment = value;
                    onPropertyChanged();
                }
            }
        }

        private Customer? _customerFilter { get; set; }
        public Customer? CustomerFilter
        {
            get => _customerFilter;
            set
            {
                if (_customerFilter != value)
                {
                    _customerFilter = value;
                    onPropertyChanged();
                    _subscribtionFilter = null;
                    onPropertyChanged(nameof(SubscriptionFilter));
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    ApplyFilters();
                }
            }
        }
        private Subscription? _subscribtionFilter { get; set; }
        public Subscription? SubscriptionFilter
        {
            get => _subscribtionFilter;
            set
            {
                if (_subscribtionFilter != value)
                {
                    _subscribtionFilter = value;
                    onPropertyChanged();
                    _customerFilter = null;
                    onPropertyChanged(nameof(CustomerFilter));
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    ApplyFilters();
                }
            }
        }

        private string? _searchString { get; set; }
        public string SearchString
        {
            get => _searchString ?? String.Empty;
            set
            {
                _searchString = value;
                onPropertyChanged();
            }
        }

        public PaymentsViewModel()
        {

            object lockDownloads = new object();
            BindingOperations.EnableCollectionSynchronization(Subscriptions, lockDownloads);
            BindingOperations.EnableCollectionSynchronization(Customers, lockDownloads);
            BindingOperations.EnableCollectionSynchronization(Payments, lockDownloads);
            UpdateLists();
        }
        ~PaymentsViewModel()
        {
            _cts.Cancel();
        }


        /// <summary>
        /// Отправка запроса на изменение оплаты
        /// </summary>
        /// <param name="model">Измененная запись оплаты</param>
        private void EditPayment(PaymentModel model)
        {
            var editedPayment = new Payment()
            {
                ID_Payment = model.ID_Payment,
                Amount = model.Subscription.Cost,
                PaymentDate = model.PaymentDate,
                ID_Customers = model.Customer.ID_Customers,
                ID_Subscription = model.Subscription.ID_Subscription,
                ValidityEndDate = model.PaymentDate.AddDays(model.Subscription.ValidityDaysNumber),
            };
            paysDataSource.EditPayment(editedPayment);
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            ApplyFilters();
        }

        /// <summary>
        /// Отправка запроса на изменение оплаты
        /// </summary>
        /// <param name="model">Добавляемая запись оплаты</param>
        private void AddPayment(PaymentModel model)
        {
            paysDataSource.AddPayment((Payment)model);
            _cts.Cancel();
            ApplyFilters();
        }

        /// <summary>
        /// Отправка запроса на удаление оплаты
        /// </summary>
        /// <param name="model">Удаляемая запись оплаты</param>
        private void DeletePayment(PaymentModel model)
        {
            paysDataSource.DeletePayment(model.ID_Payment);
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            ApplyFilters();
        }

        /// <summary>
        /// Проверяет, может ли запись оплаты быть добавлена
        /// </summary>
        /// <param name="model">Запись оплата</param>
        /// <returns>True - щапись оплаты может быть добавлена, false - не может</returns>
        private bool CanAddPayment(PaymentModel model)
        {
            try
            {
                if(model == null || model.Subscription == null || model.Customer == null)
                    return false;
                if (Payments.Any(p => p.ValidityEndDate.CompareTo(DateOnly.FromDateTime(DateTime.Now)) >= 0))
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Проверяет, может ли запись оплаты быть изменена
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CanEditPayment(PaymentModel model)
        {
            try
            {
                if (model == null || model.Subscription == null || model.Customer == null)
                    return false;
                if (Payments.Any(p => p.ID_Payment != model.ID_Payment && p.ValidityEndDate.CompareTo(DateOnly.FromDateTime(DateTime.Now)) >= 0))
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Загрудка данных оплат, клиентовЮ абонементов
        /// </summary>
        private void UpdateLists()
        {
            Task.Run(() =>
            {
                Payments.Clear();
                Customers.Clear();
                Subscriptions.Clear();
                foreach (var cust in custDataSource.GetCustomers() ?? new List<Customer>())
                    Customers.Add(cust);
                foreach (var subscription in subsDataSource.GetSubscriptions() ?? new List<Subscription>())
                    Subscriptions.Add(subscription);

                foreach (var model in (from payment in paysDataSource.GetPayments()
                                       select new PaymentModel()
                                       {
                                           ID_Payment = payment.ID_Payment,
                                           Amount = payment.Amount,
                                           ValidityEndDate = payment.ValidityEndDate,
                                           PaymentDate = payment.PaymentDate,
                                           Customer = Customers.FirstOrDefault(c => c.ID_Customers == payment.ID_Customers),
                                           Subscription = Subscriptions.FirstOrDefault(s => s.ID_Subscription == payment.ID_Subscription)
                                       }))
                    Payments.Add(model);
            }, _cts.Token);
        }

        /// <summary>
        /// Применение фильтров
        /// </summary>
        private void ApplyFilters()
        {
            Task.Run(() =>
            {

                Payments.Clear();
                if (CustomerFilter != null)
                    foreach (var model in from payment in paysDataSource.GetPaymentsByCustomer(CustomerFilter!.ID_Customers)
                                          select new PaymentModel()
                                          {
                                              ID_Payment = payment.ID_Payment,
                                              Amount = payment.Amount,
                                              ValidityEndDate = payment.ValidityEndDate,
                                              PaymentDate = payment.PaymentDate,
                                              Customer = Customers.FirstOrDefault(c => c.ID_Customers == payment.ID_Customers),
                                              Subscription = Subscriptions.FirstOrDefault(s => s.ID_Subscription == payment.ID_Subscription)
                                          })
                    {
                        Payments.Add(model);
                    }
                else if (SubscriptionFilter != null)
                    foreach (var model in from payment in paysDataSource.GetPaymentsBySubscription(SubscriptionFilter!.ID_Subscription)
                                          select new PaymentModel()
                                          {
                                              ID_Payment = payment.ID_Payment,
                                              Amount = payment.Amount,
                                              ValidityEndDate = payment.ValidityEndDate,
                                              PaymentDate = payment.PaymentDate,
                                              Customer = Customers.FirstOrDefault(c => c.ID_Customers == payment.ID_Customers),
                                              Subscription = Subscriptions.FirstOrDefault(s => s.ID_Subscription == payment.ID_Subscription)
                                          })
                    {
                        Payments.Add(model);
                    }
                else
                    foreach (var model in from payment in paysDataSource.GetPayments()
                                          select new PaymentModel()
                                          {
                                              ID_Payment = payment.ID_Payment,
                                              Amount = payment.Amount,
                                              ValidityEndDate = payment.ValidityEndDate,
                                              PaymentDate = payment.PaymentDate,
                                              Customer = Customers.FirstOrDefault(c => c.ID_Customers == payment.ID_Customers),
                                              Subscription = Subscriptions.FirstOrDefault(s => s.ID_Subscription == payment.ID_Subscription)
                                          })
                    {
                        Payments.Add(model);
                    }
            }, _cts.Token);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
