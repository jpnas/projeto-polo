﻿using projeto_polo.Core;
using projeto_polo.Model;
using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Text;
using System.Windows.Controls;
using Microsoft.Win32;

namespace projeto_polo.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private HttpClient _httpClient = new();
        private TableFilter _tableFilter = new();
        public ObservableCollection<Item> Items { get; set; }
        private string jsonResponse;
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                    if (!value)
                    {
                        if (Items.Count() == 0)
                        {
                            EmptyStateVisibility = Visibility.Visible;
                        } else
                        {
                            EmptyStateVisibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }

        private Visibility _emptyStateVisibility = Visibility.Hidden;
        public Visibility EmptyStateVisibility
        {
            get { return _emptyStateVisibility; }
            set
            {
                _emptyStateVisibility = value;
                OnPropertyChanged(nameof(EmptyStateVisibility));
            }
        }

        public class ResponseWrapper
        {
            public List<Item>? Value { get; set; }
        }
        public TableFilter TableFilter
        {
            get => _tableFilter;
            set
            {
                if (_tableFilter != value)
                {
                    _tableFilter = value;
                    OnPropertyChanged(nameof(TableFilter));
                }
            }
        }
        public ICommand ExportCommand { get; private set; }

        public RelayCommand RefreshCommand => new RelayCommand(execute =>  FetchDataAsync());

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            IsLoading = true;
            Items = new ObservableCollection<Item>();
            TableFilter = new TableFilter
            {
                ShowIPCA = false,
                ShowIGPM = false,
                ShowSelic = false,
                FromDate = DateTime.Now.AddMonths(-12),
                ToDate = DateTime.Now
            };
            ExportCommand = new RelayCommand(ExportToCSV);
            _ = FetchDataAsync();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExportToCSV(object parameter)
        {
            if (parameter is DataGrid dataGrid)
            {
                SaveFileDialog dlg = new()
                {
                    Filter = "Arquivos CSV (*.csv)|*.csv",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    FileName = "expectativaMercadoMensal"
                };
             
                if (dlg.ShowDialog() == true)
                {
                    string filePath = dlg.FileName;

                    var csvData = new StringBuilder();
   
                    csvData.Append("Indicador,");
                    csvData.Append("Data,");
                    csvData.Append("DataReferencia,");
                    csvData.Append("Media,");
                    csvData.Append("Mediana,");
                    csvData.Append("DesvioPadrao,");
                    csvData.Append("Minimo,");
                    csvData.Append("Maximo,");
                    csvData.Append("Indicador,");
                    csvData.Append("numeroRespondentes,");
                    csvData.Append("baseCalculo,");
                    csvData.AppendLine();

                    foreach (var item in Items)
                    {
                        csvData.Append($"{item.Indicador},");
                        csvData.Append($"{item.Data},");
                        csvData.Append($"{item.DataReferencia},");
                        csvData.Append($"{item.Media},");
                        csvData.Append($"{item.Mediana},");
                        csvData.Append($"{item.DesvioPadrao},");
                        csvData.Append($"{item.Minimo},");
                        csvData.Append($"{item.Maximo},");
                        csvData.Append($"{item.numeroRespondentes},");
                        csvData.Append($"{item.baseCalculo}");
                        csvData.AppendLine();
                    }

                    System.IO.File.WriteAllText(filePath, csvData.ToString());
                }

                using DatabaseContext dbContext = new();
                string guid = Guid.NewGuid().ToString();
                dbContext.Exports.Add(new Export() { Id = guid, CreatedOn = DateTime.Now });
                foreach (var item in Items)
                {
                    dbContext.Items.Add(new ExportItem()
                    {
                        ItemId = Guid.NewGuid().ToString(),
                        ExportId = guid,
                        Indicador = item.Indicador,
                        Data = item.Data,
                        DataReferencia = item.DataReferencia,
                        Media = item.Media,
                        Mediana = item.Mediana,
                        DesvioPadrao = item.DesvioPadrao,
                        Minimo = item.Minimo,
                        Maximo = item.Maximo,
                        numeroRespondentes = item.numeroRespondentes,
                        baseCalculo = item.baseCalculo
                    });
                }
                dbContext.SaveChanges();
            }
        }

        private async Task FetchDataAsync()
        {
            try
            {
                Items.Clear();
                IsLoading = true;
                bool hasIndicatorFilter = false;

                string baseUrl = "https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais";

                string queryParams = $"?$format=json";

                if (TableFilter != null)
                {
        
                    queryParams += $"&$filter=(Data ge '{TableFilter.FromDate:yyyy-MM-dd}' and Data le '{TableFilter.ToDate:yyyy-MM-dd}')";

                    if (TableFilter.ShowIPCA)
                    {
                        queryParams += " and (Indicador eq 'IPCA'";
                        hasIndicatorFilter = true;
                    }
                    if (TableFilter.ShowIGPM)
                    {
                        if (hasIndicatorFilter)
                        {
                            queryParams += " or Indicador eq 'IGP-M'";
                        }
                        else
                        {
                            queryParams += " and (Indicador eq 'IGP-M'";
                            hasIndicatorFilter = true;
                        }
                    }
                    if (TableFilter.ShowSelic)
                    {
                        if (hasIndicatorFilter)
                        {
                            queryParams += " or Indicador eq 'Selic'";
                        }
                        else
                        {
                            queryParams += " and (Indicador eq 'Selic'";
                            hasIndicatorFilter = true;
                        }
                    }

                    if (hasIndicatorFilter)
                    {
                        queryParams += ")";
                    }
                    else
                    {
                        queryParams += " and (Indicador eq 'IPCA' or Indicador eq 'IGP-M' or Indicador eq 'Selic')";
                    }
                }
                string url = baseUrl + queryParams;
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        jsonResponse = await response.Content.ReadAsStringAsync();
                        var responseObj = JsonConvert.DeserializeObject<ResponseWrapper>(jsonResponse);
                        var items = responseObj.Value;
                        foreach (var item in items)
                        {
                            Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar dados: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

    }
}
