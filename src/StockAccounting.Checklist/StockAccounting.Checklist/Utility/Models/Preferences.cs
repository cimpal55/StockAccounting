using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using StockAccounting.Checklist.Models.Data;
using Xamarin.Essentials;

namespace StockAccounting.Checklist.Utility.Models
{
    static class Preferences
    {
        public static ObservableCollection<ExternalDataModel> ScannedStockDataList
        {
            get
            {
                return Deserialize<ObservableCollection<ExternalDataModel>>(Xamarin.Essentials.Preferences.Get(nameof(ScannedStockDataList), "empty"));
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(ScannedStockDataList), serializedList);
            }
        }

        public static EmployeeDataModel FirstSelectedEmployee
        {
            get
            {
                return Deserialize<EmployeeDataModel>(Xamarin.Essentials.Preferences.Get(nameof(FirstSelectedEmployee), ""));
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
                return Deserialize<EmployeeDataModel>(Xamarin.Essentials.Preferences.Get(nameof(SecondSelectedEmployee), ""));
            }
            set
            {
                var serializedList = Serialize(value);
                Xamarin.Essentials.Preferences.Set(nameof(SecondSelectedEmployee), serializedList);
            }
        }

        static T Deserialize<T>(string serializedObject) => JsonConvert.DeserializeObject<T>(serializedObject);

        static string Serialize<T>(T objectToSerialize) => JsonConvert.SerializeObject(objectToSerialize);
    }
}
