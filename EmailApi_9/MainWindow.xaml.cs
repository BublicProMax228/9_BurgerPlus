using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;

namespace ValidationEmail_8
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string Email = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetDataButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                
                using (HttpClient client = new HttpClient())
                {
                    // Ссылка на API
                    string url = "http://localhost:4444/TransferSimulator/email";

                    string jsonAnswer = await client.GetStringAsync(url);

                    jsonAnswer = jsonAnswer.Replace("{", "");
                    jsonAnswer = jsonAnswer.Replace("}", "");
                    jsonAnswer = jsonAnswer.Replace("\"", "");
                    jsonAnswer = jsonAnswer.Replace("value :", "");

                    Email = jsonAnswer.Trim();

                    txtBoxEmailText.Text = Email;

                    // Очищаем старый результат
                    txtBoxResultText.Text = "";
                }
            }
            // Если произошла ошибка: например API выключен или сервер недоступен
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод вызывается при нажатии кнопки "Отправить результат теста"
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(Email))
            {
                txtBoxResultText.Text = "Сначала получите данные";

                return;
            }


            // Проверка на запрещенные символы

            string forbiddenSymbols = ";,";
            string Probel = " ";
            

            if (Email.Intersect(Probel).Count() > 0)
            {
                txtBoxResultText.Text = "Не корректная электронная почта";
                return;
            }

            if (Email.Intersect(forbiddenSymbols).Count() > 0)
            {
                txtBoxResultText.Text = "Не корректная электронная почта";
                return;
            }
            

            // Если все проверки пройдены
            txtBoxResultText.Text = "Почта корректна";
        }
    }
}

