using System.Collections.ObjectModel;
using System.Xml.Xsl;
using System.Xml;
using Microsoft.Maui.Storage;
using System.IO;


namespace sportsectionfcsc
{
        public partial class MainPage : ContentPage
        
        {     

            private IXmlFilterStrategy _currentStrategy;
            private XmlFilterContext _context;
            private string _inputFilePath;
            private string _outputFilePath;
            public MainPage()
                {
                    InitializeComponent();

                // Встановлюємо стратегію за замовчуванням (LINQ to XML)
                _currentStrategy = new LinqToXmlStrategy();
                _context = new XmlFilterContext(_currentStrategy);
                _outputFilePath = Path.Combine("C:\\Users\\elpee\\Documents", "output.xml");

                }


        private async void OnSearchClicked(object sender, EventArgs e)
        {
            try
            {
                string name = this.FindByName<Entry>("NameEntry")?.Text?.Trim() ?? "";
                string ageFrom = this.FindByName<Entry>("AgeFromEntry")?.Text?.Trim() ?? "";
                string ageTo = this.FindByName<Entry>("AgeToEntry")?.Text?.Trim() ?? "";
                string gender = GetSelectedGender();

                string timeFrom = this.FindByName<Entry>("TimeFromEntry")?.Text?.Trim() ?? "";
                string timeTo = this.FindByName<Entry>("TimeToEntry")?.Text?.Trim() ?? "";
                List<string> selectedDays = GetSelectedDays();

                if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(ageFrom) && string.IsNullOrEmpty(ageTo) && string.IsNullOrEmpty(gender) &&
                    string.IsNullOrEmpty(timeFrom) && string.IsNullOrEmpty(timeTo) && selectedDays.Count == 0)
                {
                    await DisplayAlert("Помилка", "Будь ласка, введіть значення для пошуку.", "ОК");
                    return;
                }
                var filters = new Filters();
                if (!string.IsNullOrEmpty(name)) filters.Name = name;
                if (int.TryParse(ageFrom, out int from)) filters.AgeFrom = from;
                if (int.TryParse(ageTo, out int to)) filters.AgeTo = to;
                if (!string.IsNullOrEmpty(gender)) filters.Gender = gender;
                if (!string.IsNullOrEmpty(timeFrom)) filters.TimeFrom = timeFrom;
                if (!string.IsNullOrEmpty(timeTo)) filters.TimeTo = timeTo;
                filters.Days = selectedDays;

                _context.FilterXml(filters, _inputFilePath, _outputFilePath);
                await DisplayAlert("Успіх", "Документ XML успішно відфільтровано.", "ОК");
            }


            catch (Exception ex)
            {
                await DisplayAlert("Помилка", "Помилка при обробці файлу: " + ex.Message, "ОК");
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
            {
                // Очищення полів в блоці Members
                var nameEntry = this.FindByName<Entry>("NameEntry");
                var ageFromEntry = this.FindByName<Entry>("AgeFromEntry");
                var ageToEntry = this.FindByName<Entry>("AgeToEntry");
                var genderMaleRadio = this.FindByName<RadioButton>("MaleRadioButton");
                var genderFemaleRadio = this.FindByName<RadioButton>("FemaleRadioButton");

                if (nameEntry != null) nameEntry.Text = string.Empty;
                if (ageFromEntry != null) ageFromEntry.Text = string.Empty;
                if (ageToEntry != null) ageToEntry.Text = string.Empty;
                if (genderMaleRadio != null) genderMaleRadio.IsChecked = false;
                if (genderFemaleRadio != null) genderFemaleRadio.IsChecked = false;

                // Очищення полів в блоці Schedule
                var timeFromEntry = this.FindByName<Entry>("TimeFromEntry");
                var timeToEntry = this.FindByName<Entry>("TimeToEntry");

                if (timeFromEntry != null) timeFromEntry.Text = string.Empty;
                if (timeToEntry != null) timeToEntry.Text = string.Empty;

                // Очищення вибраних днів тижня
                foreach (var dayCheckBox in new[] { "MondayCheckBox", "TuesdayCheckBox", "WednesdayCheckBox", "ThursdayCheckBox", "FridayCheckBox", "SaturdayCheckBox", "SundayCheckBox" })
                {
                    var checkBox = this.FindByName<CheckBox>(dayCheckBox);
                    if (checkBox != null) checkBox.IsChecked = false;
                }

                // Очищення результатів пошуку
            }

        private void OnStrategySelected(object sender, CheckedChangedEventArgs e)
        {
            if (!(sender is RadioButton radioButton) || !radioButton.IsChecked) return;

            switch (radioButton.Content.ToString())
            {
                case "DOM":
                    _currentStrategy = new DomStrategy();
                    break;
                case "SAX":
                    _currentStrategy = new SaxStrategy();
                    break;
                case "LINQ to XML":
                    _currentStrategy = new LinqToXmlStrategy();
                    break;
                default:
                    _currentStrategy = new LinqToXmlStrategy();
                    break;
            }

            // Оновлюємо стратегію в контексті
            _context.SetStrategy(_currentStrategy);
        }

        private async void OnTransformToHtmlClicked(object sender, EventArgs e)
        {
            try
            {
                // Використовуємо відфільтрований XML-файл
                string xmlPath = _outputFilePath;

                // Шлях до XSL-файлу
                string xslPath = Path.Combine(@"C:\Users\elpee\source\repos\sportsectionfcsc1\sportsectionfcsc\Resources\Raw", "transform.xsl");

                // Шлях до вихідного HTML-файлу
                string outputHtmlPath = Path.Combine(@"C:\Users\elpee\Documents", "output.html");

                if (!File.Exists(xmlPath))
                {
                    await DisplayAlert("Помилка", "Вихідний XML-файл не знайдено. Спочатку виконайте фільтрацію.", "ОК");
                    return;
                }

                if (!File.Exists(xslPath))
                {
                    await DisplayAlert("Помилка", "Файл XSL не знайдено. Переконайтеся, що файл доступний.", "ОК");
                    return;
                }

                await TransformToHtml(xmlPath, xslPath, outputHtmlPath);

                await DisplayAlert("Успіх", "Документ XML успішно перетворено на HTML.", "ОК");

                // Відкрити HTML-файл у браузері
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(outputHtmlPath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", "Сталася помилка при трансформації: " + ex.Message, "ОК");
            }
        }

        public async Task TransformToHtml(string xmlPath, string xslPath, string outputHtmlPath)
        {
            try
            {
                var xslTransform = new XslCompiledTransform();
                xslTransform.Load(xslPath);

                xslTransform.Transform(xmlPath, outputHtmlPath);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", "Сталася помилка під час трансформації: " + ex.Message, "ОК");
            }
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            ConfirmExit();
        }

        private async void ConfirmExit()
        {
            bool answer = await DisplayAlert("Підтвердження", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
            if (answer)
            {
                if (Application.Current != null)
                {
                    Application.Current.Quit(); // Закриває додаток, якщо користувач підтвердив
                }
            }
        }

        private async Task LoadAttributePickerAsync()
        {
            try
            {
                var attributeNames = new List<string> { "Name", "Age", "Gender", "Time", "Day" };
                // Прив'язка атрибутів до UI компонента, якщо потрібно
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", "Не вдалося завантажити атрибути: " + ex.Message, "ОК");
            }
        }

        private string GetSelectedGender()
            {
                var genderMaleRadio = this.FindByName<RadioButton>("MaleRadioButton");
                var genderFemaleRadio = this.FindByName<RadioButton>("FemaleRadioButton");

                if (genderMaleRadio != null && genderMaleRadio.IsChecked) return "Male";
                if (genderFemaleRadio != null && genderFemaleRadio.IsChecked) return "Female";

                return string.Empty;
            }

            private List<string> GetSelectedDays()
            {
                List<string> selectedDays = new List<string>();
                foreach (var dayCheckBoxName in new[] { "MondayCheckBox", "TuesdayCheckBox", "WednesdayCheckBox", "ThursdayCheckBox", "FridayCheckBox", "SaturdayCheckBox", "SundayCheckBox" })
                {
                    var checkBox = this.FindByName<CheckBox>(dayCheckBoxName);
                    if (checkBox != null && checkBox.IsChecked)
                    {
                        selectedDays.Add(dayCheckBoxName.Replace("CheckBox", ""));
                    }
                }

                return selectedDays;
            }

        private async void OnSelectFileClicked(object sender, EventArgs e)
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/xml" } },
                    { DevicePlatform.iOS, new[] { "public.xml" } },
                    { DevicePlatform.WinUI, new[] { ".xml" } }
                });

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select an XML file",
                    FileTypes = customFileType
                });

                if (result != null)
                {
                    _inputFilePath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", "Не вдалося завантажити файл: " + ex.Message, "ОК");
            }
        }
    }
}


