using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using StockAccounting.Checklist.Models.Data;
using Xamarin.Essentials;

namespace StockAccounting.Checklist.Utility.Models
{
    public class Preferences : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static ObservableCollection<ExternalDataModel> ScannedStockDataList
        {
            get
            {
                try
                {
                    return Deserialize<ObservableCollection<ExternalDataModel>>(Xamarin.Essentials.Preferences.Get(nameof(ScannedStockDataList), ""));
                }
                catch
                {
                    return new ObservableCollection<ExternalDataModel>();
                }
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(ScannedStockDataList), serializedList);
            }
        }

        public static ObservableCollection<ToolkitDataModel> ToolkitDataList
        {
            get
            {
                try
                {
                    return Deserialize<ObservableCollection<ToolkitDataModel>>(Xamarin.Essentials.Preferences.Get(nameof(ToolkitDataList), ""));
                }
                catch
                {
                    return new ObservableCollection<ToolkitDataModel>();
                }
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(ToolkitDataList), serializedList);
            }
        }

        public static ObservableCollection<EmployeeDataModel> EmployeeDataList
        {
            get
            {
                try
                {
                    return Deserialize<ObservableCollection<EmployeeDataModel>>(Xamarin.Essentials.Preferences.Get(nameof(EmployeeDataList), ""));
                }
                catch
                {
                    return new ObservableCollection<EmployeeDataModel>();
                }
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(EmployeeDataList), serializedList);
            }
        }

        public static ObservableCollection<ExternalDataModel> ExternalDataList
        {
            get
            {
                try
                {
                    return Deserialize<ObservableCollection<ExternalDataModel>>(Xamarin.Essentials.Preferences.Get(nameof(ExternalDataList), ""));
                }
                catch
                {
                    return new ObservableCollection<ExternalDataModel>();
                }
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(ExternalDataList), serializedList);
            }
        }

        public static ObservableCollection<ToolkitHistoryModel> ToolkitHistoryList
        {
            get
            {
                try
                {
                    return Deserialize<ObservableCollection<ToolkitHistoryModel>>(Xamarin.Essentials.Preferences.Get(nameof(ToolkitHistoryList), ""));
                }
                catch
                {
                    return new ObservableCollection<ToolkitHistoryModel>();
                }
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(ToolkitHistoryList), serializedList);
            }
        }

        public static EmployeeDataModel FirstSelectedEmployee
        {
            get
            {
                try
                {
                    return Deserialize<EmployeeDataModel>(Xamarin.Essentials.Preferences.Get(nameof(FirstSelectedEmployee), ""));

                }
                catch
                {
                    return new EmployeeDataModel();
                }
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(FirstSelectedEmployee), serializedList);
            }
        }

        public static EmployeeDataModel SecondSelectedEmployee
        {
            get
            {
                try
                {
                    return Deserialize<EmployeeDataModel>(Xamarin.Essentials.Preferences.Get(nameof(SecondSelectedEmployee), ""));
                }
                catch
                {
                    return new EmployeeDataModel();
                }
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(SecondSelectedEmployee), serializedList);
            }
        }

        static T Deserialize<T>(string serializedObject) => JsonConvert.DeserializeObject<T>(serializedObject);

        static string Serialize<T>(T objectToSerialize) => JsonConvert.SerializeObject(objectToSerialize);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
