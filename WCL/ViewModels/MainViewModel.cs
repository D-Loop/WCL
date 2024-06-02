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

            CommandLogIn = new Command(async () => await OnLogIn());
            CommandLogOut = new Command(async () => await OnLogOut());
            CommandRegistrationUser = new Command(async () => await OnRegistrationUser());

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

        private string? _errorStrig { get; set; }
        public string ErrorStrig
        {
            get => _errorStrig ?? string.Empty;
            set
            {
                _errorStrig = value;
                OnPropertyChanged("ErrorStrig");
            }
        }
        #endregion

       

        #region Command

        /// <summary> Авторизация пользователя по введенным даным</summary>
        public ICommand CommandLogIn { get; set; }
        private async Task OnLogIn()
        {
            try
            {
                User.IsNullError = true;
                User.ValidateForLogIn();
                //продожаем только если нет ошибок
                if (User.IsNullError)
                {
                    using (var httpClient = new HttpClient())
                    {
                        //пытаемся войти в систему
                        var response = await httpClient.GetAsync($"https://petstore.swagger.io/v2/user/login?username={User.username}&password={User.password}");

                        // Проверка успешности ответа
                        response.EnsureSuccessStatusCode();

                        if (response.StatusCode.HasFlag(System.Net.HttpStatusCode.OK))
                        {    //пытаемся получить данные пользователя
                            response = await httpClient.GetAsync($"https://petstore.swagger.io/v2/user/{User.username}");
                        }
                        if (response.StatusCode.HasFlag(System.Net.HttpStatusCode.NotFound))
                        {
                            //если пользовател не еашло все сбрасываем
                            User = new UserViewModel(); 
                        }
                        else
                        {
                            //получаем тело ответа в формате сериализованного обьекта
                            var json = await response.Content.ReadAsStringAsync();
                            //присваем десериализовванный обьект пользователя 
                            User = JsonSerializer.Deserialize<UserViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new UserViewModel();
                            //если есть id значит получили пользователя и считаем вход успешеым
                            if (User.id != 0) User.IsLogIn = true;
                        }

                    }
                }
            }
            catch 
            {
                ErrorStrig = "Ошибка входа в систему";
            }
            finally 
            {
                User.IsValedEnable = true;
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

        /// <summary> Регистрация пользователя по введенным даным</summary>
        public ICommand CommandRegistrationUser { get; set; }
        private async Task OnRegistrationUser()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("https://petstore.swagger.io/v2/user");
                    response.EnsureSuccessStatusCode();

                }
            }
            catch
            {
                ErrorStrig = "Ошибка выхода из системы";
            }
        }

        #endregion
    }
}
