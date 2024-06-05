using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WCL.Helpers;
using WCL.Models;

namespace WCL.ViewModels
{
    class MainViewModel: INotifyPropertyChanged
    {
        #region OnPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            User = new UserViewModel();
            ErrorStrig = string.Empty;
            VisibilityWindowRegistration = Visibility.Collapsed;

            CommandCloseWindow = new Command(OnCloseWindow);
            CommandMinimizeWindow = new Command(OnMinimizeWindow);
            CommandMaximizeRestoreWindow = new Command(OnMaximizeRestoreWindow);

            CommandLogIn = new Command(async () => await OnLogIn());
            CommandLogOut = new Command(async () => await OnLogOut());
            CommandRegistrationUser = new Command(async () => await OnRegistrationUser());
            CommandClearErrorString = new Command(OnClearErrorString);
            CommandChangeVisibilityWindowRegistration= new Command(OnChangeVisibilityWindowRegistration);

        }
        #endregion

        #region Properies
        private UserViewModel? _user { get; set; }
        public  UserViewModel User
        {
            get => _user ?? new UserViewModel();
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }

        private UserViewModel? _newUser { get; set; }
        public UserViewModel NewUser
        {
            get => _newUser ?? new UserViewModel();
            set
            {
                _newUser = value;
                OnPropertyChanged("NewUser");
            }
        }

        public bool IsHasError => !string.IsNullOrEmpty(_errorStrig);
        public bool IsHasErrorReg => !string.IsNullOrEmpty(_errorStrigReg);

        private string? _errorStrig { get; set; }
        public string ErrorStrig
        {
            get => _errorStrig ?? string.Empty;
            set
            {
                _errorStrig = value;
                OnPropertyChanged("IsHasError");
                OnPropertyChanged("ErrorStrig");
            }
        }
        private string? _errorStrigReg { get; set; }
        public string ErrorStringReg
        {
            get => _errorStrigReg ?? string.Empty;
            set
            {
                _errorStrigReg = value;
                OnPropertyChanged("IsHasErrorReg");
                OnPropertyChanged("ErrorStringReg");
            }
        }
        private Visibility _visibilityWindowRegistration { get; set; }
        public Visibility VisibilityWindowRegistration
        {
            get => _visibilityWindowRegistration ;
            set
            {
                _visibilityWindowRegistration = value;
                OnPropertyChanged("VisibilityWindowRegistration");
            }
        }
        #endregion

        #region Command

        /// <summary> Свернуть окно</summary>
        public ICommand CommandMinimizeWindow { get; set; }
        private void OnMinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        
        /// <summary> Свернуть окно</summary>
        public ICommand CommandCloseWindow { get; set; }
        private void OnCloseWindow()
        {
            Application.Current.MainWindow.Close();
        }

        /// <summary> Развернуть или воcстановить окно</summary>
        public ICommand CommandMaximizeRestoreWindow { get; set; }
        private void OnMaximizeRestoreWindow()
        {
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow.WindowState == WindowState.Maximized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
        }

        /// <summary> Изменить видимость окна регистрации</summary>
        public ICommand CommandChangeVisibilityWindowRegistration { get; set; }
        private void OnChangeVisibilityWindowRegistration()
        {
            NewUser = new UserViewModel();
            CommandClearErrorString.Execute(null);
            //инвертируем видимость окна
            VisibilityWindowRegistration = VisibilityWindowRegistration.HasFlag(Visibility.Collapsed)
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }


        /// <summary> Авторизация пользователя по введенным даным</summary>
        public ICommand CommandLogIn { get; set; }
        private async Task OnLogIn()
        {
            try
            {
                User.ValidateForLogIn();
                //продожаем только если нет ошибок
                if (User.IsNullError)
                {
                    using (var httpClient = new HttpClient())
                    {
                        //Попытка входа в систему
                        var response = await httpClient.GetAsync($"https://petstore.swagger.io/v2/user/login?username={User.username}&password={User.password}");

                        //Проверка успешности ответа
                        response.EnsureSuccessStatusCode();

                        if (response.StatusCode.HasFlag(System.Net.HttpStatusCode.OK))
                        {    //попытка получить данные пользователя
                            response = await httpClient.GetAsync($"https://petstore.swagger.io/v2/user/{User.username}");

                            if (response.StatusCode.HasFlag(System.Net.HttpStatusCode.NotFound))
                            {
                                //если пользовател не наашло все сбрасываем
                                User = new UserViewModel();
                                ErrorStrig = "Ошибка входа в систему";

                            }
                            else
                            {
                                //получаем тело ответа в формате сериализованного обьекта
                                var json = await response.Content.ReadAsStringAsync();
                                //присваем десериализовванный обьект пользователя 
                                User = JsonSerializer.Deserialize<UserViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new UserViewModel();
                                //если есть id значит получили пользователя и считаем вход успешеым
                                if (User.id != 0) 
                                    User.IsLogIn = true;
                                CommandClearErrorString.Execute(null);

                            }
                        }
                        else
                        {
                            ErrorStrig = "Ошибка входа в систему, проверьте логин и пароль";
                        }
                    }
                }
                else
                {
                    ErrorStrig = "Введите логин и пароль";
                }
            }
            catch 
            {
                ErrorStrig = "Ошибка входа в систему";
            }
            finally 
            {
                User.ValidateForLogIn();
            }
        }


        /// <summary> Авторизация выхода пользователя из системы</summary>
        public ICommand CommandLogOut { get; set; }
        private async Task OnLogOut()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("https://petstore.swagger.io/v2/user/logout");
                    response.EnsureSuccessStatusCode();

                    if (response.StatusCode.HasFlag(System.Net.HttpStatusCode.OK))
                    {
                        User.IsLogIn = false;
                        User = new UserViewModel();
                    }
                }
            }
            catch
            {
                ErrorStrig = "Ошибка выхода из системы";
            }
        }

        /// <summary> очистка строки ошибки</summary>
        public ICommand CommandClearErrorString { get; set; }
        private void OnClearErrorString()
        {
            try
            {
                ErrorStrig = string.Empty;
                ErrorStringReg= string.Empty;
            }
            catch
            {
                ErrorStrig = "Ошибка выхода из системы";
            }
        }

        /// <summary> Регистрация пользователя по введенным даным</summary>
        public ICommand CommandRegistrationUser { get; set; }
        private async Task OnRegistrationUser()
        {
            try
            {
                NewUser.ValidateForReg();

                if (!NewUser.IsNullError) 
                {
                    ErrorStringReg = "Все поля обязательны";
                    return;
                }

                if(NewUser.password!= NewUser.RepeatedPassword) 
                {
                    ErrorStringReg = "Пароли не совпадают";
                    return;
                }

                using (var httpClient = new HttpClient())
                {
                    var data = JsonSerializer.Serialize(NewUser);
                    //запрос на регистрацию пользователя
                    var response = await httpClient.PostAsync("https://petstore.swagger.io/v2/user", new StringContent(data, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();

                    if (response.StatusCode.HasFlag(System.Net.HttpStatusCode.OK))
                    {
                        //Попытка входа в систему
                        response = await httpClient.GetAsync($"https://petstore.swagger.io/v2/user/login?username={NewUser.username}&password={NewUser.password}");

                        //Проверка успешности ответа
                        response.EnsureSuccessStatusCode();

                        if (response.StatusCode.HasFlag(System.Net.HttpStatusCode.OK))
                        {
                            //попытка получить данные пользователя
                            response = await httpClient.GetAsync($"https://petstore.swagger.io/v2/user/{NewUser.username}");
                            if (response.StatusCode.HasFlag(System.Net.HttpStatusCode.OK))
                            {
                                //получаем тело ответа в формате сериализованного обьекта
                                var json = await response.Content.ReadAsStringAsync();
                                //присваем десериализовванный обьект пользователя 
                                User = JsonSerializer.Deserialize<UserViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new UserViewModel();
                                //если есть id значит получили пользователя и считаем вход успешеым
                                if (User.id != 0)
                                    User.IsLogIn = true;
                                CommandClearErrorString.Execute(null);
                                VisibilityWindowRegistration = Visibility.Collapsed;
                            }
                            else
                            {
                                ErrorStringReg = "Нет данных пользователя";
                            }
                        }
                        else
                        {
                            ErrorStringReg = "Ошибка входа нового пользователя";
                        }
                    }
                }
            }
            catch
            {
                ErrorStrig = "Ошибка регистрации";
            }
        }

        #endregion
    }
}
