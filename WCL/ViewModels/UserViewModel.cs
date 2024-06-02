using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WCL.Models;

namespace WCL.ViewModels
{
    internal class UserViewModel : User, INotifyPropertyChanged, IDataErrorInfo
    {
        #region OnPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        #region Properties
        private bool _isLogIn { get; set; }
        /// <summary>Произошел ли вход в систему</summary>
        public bool IsLogIn
        {
            get => _isLogIn;
            set
            {
                _isLogIn = value;
                OnPropertyChanged("IsLogIn");
            }
        }

        /// <summary>Текстовое представление индефикатора</summary>
        public string idVM => id == 0 ? "- - -" : id.ToString();
        /// <summary>Уникальный индефикатор пользователя</summary>
        public new int id
        {
            get => base.id;
            set
            {
                base.id = value;
                OnPropertyChanged("id");
            }
        }
        /// <summary>Логин</summary>
        public new string username
        {
            get => base.username ?? string.Empty;
            set
            {
                base.username = value;
                OnPropertyChanged("username");
            }
        }
        /// <summary>Имя</summary>
        public new string firstName
        {
            get => base.firstName ?? string.Empty;
            set
            {
                base.firstName = value;
                OnPropertyChanged("firstName");
            }
        }
        /// <summary>Пароль</summary>
        public new string password
        {
            get => base.password ?? string.Empty;
            set
            {
                base.password = value;
                OnPropertyChanged("password");
            }
        }
        /// <summary>Фамилия</summary>
        public new string lastName
        {
            get => base.lastName ?? string.Empty;
            set
            {
                base.lastName = value;
                OnPropertyChanged("lastName");
            }
        }

        /// <summary>Электронная почта</summary>
        public new string email
        {
            get => base.email ?? string.Empty;
            set
            {
                base.email = value;
                OnPropertyChanged("email");
            }
        }

        /// <summary>Номер телефона</summary>
        public new string phone
        {
            get => base.phone ?? string.Empty;
            set
            {
                base.phone = value;
                OnPropertyChanged("phone");
            }
        }
        #endregion

        #region Validate
        /// <summary> Валидация для входа в систему </summary>
        public void ValidateForLogIn()
        {
            OnPropertyChanged(nameof(username));
            OnPropertyChanged(nameof(password));
        }

        /// <summary> Валидация для регистрации</summary>
        public void ValidateForReg()
        {
            OnPropertyChanged(nameof(username));
            OnPropertyChanged(nameof(firstName));
            OnPropertyChanged(nameof(lastName));
            OnPropertyChanged(nameof(phone));
            OnPropertyChanged(nameof(password));
        }

        public bool IsValedEnable { get; set; }
        public string this[string columnName]
        {
            get
            {
                //если не хотим валидировать данные то сразу выходим
                if(!IsValedEnable) return string.Empty;

                Error = ValidateFields(columnName);
                return Error;
            }
        }
        public string? Error { get; set; }
        #endregion


    }
}
