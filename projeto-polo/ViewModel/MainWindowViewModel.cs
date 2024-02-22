using projeto_polo.Core;
using projeto_polo.Model;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Input;
using System.Text;
using System.Windows.Controls;
using Microsoft.Win32;
using static System.Net.WebRequestMethods;

namespace projeto_polo.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private HttpClient _httpClient = new HttpClient();
        private TableFilter _tableFilter = new TableFilter();
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
                }
            }
        }
        public class ResponseWrapper
        {
            public List<Item> Value { get; set; }
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

        public RelayCommand FilterCommand => new RelayCommand(execute => FilterTable());
        public RelayCommand RefreshCommand => new RelayCommand(execute => RefreshTable());

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            Items = new ObservableCollection<Item>();
            TableFilter = new TableFilter
            {
                ShowIPCA = true,
                ShowIGPM = true,
                ShowSelic = true,
                FromDate = DateTime.Now.AddMonths(-1),
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
                SaveFileDialog dlg = new SaveFileDialog
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





            }
        }

        public async void RefreshTable()
        {
            try
            {
                FetchDataAsync();
                TableFilter = new TableFilter
                {
                    ShowIPCA = true,
                    ShowIGPM = true,
                    ShowSelic = true,
                    FromDate = DateTime.Now.AddMonths(-1),
                    ToDate = DateTime.Now
                };
                MessageBox.Show("Dados atualizados.");
            }
            catch
            {
                MessageBox.Show("Erro ao atualizar dados.");
            }
        }

        private void FilterTable()
        {
            var filteredItems = JsonConvert.DeserializeObject<ResponseWrapper>(jsonResponse).Value.Where(item =>
         (TableFilter.ShowIPCA && item.Indicador.StartsWith("IPCA")) ||
         (TableFilter.ShowIGPM && item.Indicador.StartsWith("IGP-M")) ||
         (TableFilter.ShowSelic && item.Indicador.StartsWith("Selic"))
     ).Where(item =>
         item.Data >= TableFilter.FromDate && item.Data <= TableFilter.ToDate
     ).ToList();

            Items.Clear();
            foreach (var item in filteredItems)
            {
                Items.Add(item);
            }
        }

        private async Task FetchDataAsync()
        {
            try
            {
                IsLoading = true;
                string url = "https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?%24format=json&%24top=1000";
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        jsonResponse = await response.Content.ReadAsStringAsync();
                        var responseObj = JsonConvert.DeserializeObject<ResponseWrapper>(jsonResponse);
                        var items = responseObj.Value;
                        Items.Clear();
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
