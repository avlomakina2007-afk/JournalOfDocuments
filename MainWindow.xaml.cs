using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;


namespace JournalOfDocuments
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Document> _documents = new ObservableCollection<Document>();
        private Document _selectedDocument;
        private readonly string _dataFile = "documents.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            dgDocuments.ItemsSource = _documents;
            dpDate.SelectedDate = DateTime.Today; // устанавливаем сегодняшнюю дату
        }

        // Загрузка из JSON
        private void LoadData()
        {
            if (File.Exists(_dataFile))
            {
                string json = File.ReadAllText(_dataFile);
                var list = JsonConvert.DeserializeObject<ObservableCollection<Document>>(json);
                if (list != null)
                    _documents = list;
            }
            // Тестовые записи, если файл пустой или его нет
            if (_documents.Count == 0)
            {
                _documents.Add(new Document
                {
                    Id = 1,
                    IncomingNumber = "01",
                    DateReceived = DateTime.Today.AddDays(-5),
                    Sender = "ООО Нефтехим",
                    Subject = "Заявка на материалы",
                    Executor = "Иванов И.И."
                });
                _documents.Add(new Document
                {
                    Id = 2,
                    IncomingNumber = "02",
                    DateReceived = DateTime.Today.AddDays(-2),
                    Sender = "АО Энерго",
                    Subject = "Акт сверки",
                    Executor = "Петрова А.С."
                });
            }
        }

        // Сохранение в JSON
        private void SaveData()
        {
            string json = JsonConvert.SerializeObject(_documents, Formatting.Indented);
            File.WriteAllText(_dataFile, json);
        }

        // Генерация нового ID
        private int GetNextId()
        {
            if (_documents.Count == 0) return 1;
            return _documents.Max(d => d.Id) + 1;
        }

        // Очистка формы
        private void ClearForm()
        {
            txtNumber.Text = "";
            dpDate.SelectedDate = DateTime.Today;
            txtSender.Text = "";
            txtSubject.Text = "";
            txtExecutor.Text = "";
            _selectedDocument = null;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnAdd.IsEnabled = true;
        }

        // Выбор строки в таблице
        private void dgDocuments_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgDocuments.SelectedItem is Document doc)
            {
                _selectedDocument = doc;
                txtNumber.Text = doc.IncomingNumber;
                dpDate.SelectedDate = doc.DateReceived;
                txtSender.Text = doc.Sender;
                txtSubject.Text = doc.Subject;
                txtExecutor.Text = doc.Executor;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnAdd.IsEnabled = false;
            }
            else
            {
                // Если выделение снято и нет выбранного документа для редактирования
                if (_selectedDocument == null)
                    ClearForm();
            }
        }

        // Добавление
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text) || string.IsNullOrWhiteSpace(txtSender.Text))
            {
                MessageBox.Show("Заполните номер и отправителя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newDoc = new Document
            {
                Id = GetNextId(),
                IncomingNumber = txtNumber.Text.Trim(),
                DateReceived = dpDate.SelectedDate ?? DateTime.Today,
                Sender = txtSender.Text.Trim(),
                Subject = txtSubject.Text.Trim(),
                Executor = txtExecutor.Text.Trim()
            };
            _documents.Add(newDoc);
            SaveData();
            ClearForm();
            dgDocuments.SelectedItem = null;
            MessageBox.Show("Документ добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Редактирование
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDocument == null)
            {
                MessageBox.Show("Выберите документ из списка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedDocument.IncomingNumber = txtNumber.Text.Trim();
            _selectedDocument.DateReceived = dpDate.SelectedDate ?? DateTime.Today;
            _selectedDocument.Sender = txtSender.Text.Trim();
            _selectedDocument.Subject = txtSubject.Text.Trim();
            _selectedDocument.Executor = txtExecutor.Text.Trim();

            // Обновляем DataGrid
            dgDocuments.Items.Refresh();
            SaveData();
            ClearForm();
            dgDocuments.SelectedItem = null;
            MessageBox.Show("Документ изменён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Удаление
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDocument == null)
            {
                MessageBox.Show("Выберите документ для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить документ №{_selectedDocument.IncomingNumber}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _documents.Remove(_selectedDocument);
                SaveData();
                ClearForm();
                dgDocuments.SelectedItem = null;
                MessageBox.Show("Документ удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Очистка формы
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            dgDocuments.SelectedItem = null;
        }
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilter.Text))
            {
                dgDocuments.ItemsSource = _documents;
            }
            else
            {
                var filtered = new ObservableCollection<Document>(
                    _documents.Where(d => d.Sender.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                );
                dgDocuments.ItemsSource = filtered;
            }
        }
    }
}