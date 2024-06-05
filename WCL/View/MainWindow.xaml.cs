using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WCL.ViewModels;

namespace WCL.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        /// Обработчик события MouseLeftButtonDown для перетаскивания окна 
        private void Window_Move(object sender, MouseButtonEventArgs e)
        {
            //Пока кнопка зажата перетаскиваем окно
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        //обработка события нажатия на клавиатуру для вызова регистрации или логин
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(this.DataContext is MainViewModel viewModel)
            {
                if ((e.Key == Key.Enter))
                {
                    //если окно регистрации не выведено на экран
                    if(viewModel.VisibilityWindowRegistration.HasFlag(Visibility.Collapsed))
                    {
                        viewModel.CommandLogIn.Execute(null);
                    }
                    //иначе вызываем команду регистрации
                    else
                    {
                        viewModel.CommandRegistrationUser.Execute(null);
                    }
                }
            }
        }
    }
}