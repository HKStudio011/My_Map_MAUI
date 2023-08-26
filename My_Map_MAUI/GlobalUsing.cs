global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using CommunityToolkit.Maui;
global using CommunityToolkit.Maui.Markup;
global using CommunityToolkit.Mvvm.Input;
global using My_Map_MAUI.Views;
global using Microsoft.Maui.Maps;
global using Microsoft.Maui.Controls.Maps;
global using My_Map_MAUI.ViewModels;
global using My_Map_MAUI.Serviecs;
global using SQLite;
global using My_Map_MAUI.Models;
global using CommunityToolkit.Mvvm.ComponentModel;
global using System.ComponentModel;
global using System.Runtime.CompilerServices;
global using System.Collections.ObjectModel;
global using CommunityToolkit.Maui.Alerts;
global using CommunityToolkit.Maui.Core;
global using System.ComponentModel.DataAnnotations;
global using System.Globalization;

namespace My_Map_MAUI
{
    public static class GlobalUsing
    {
        public static ObservableCollection<MapItem> MapItemCollection { get; set; }
        public static ObservableCollection<Detail> DetailCollection { get; set; }
        public static ObservableCollection<Models.Image> ImageCollection { get; set; }
        public static ObservableCollection<ITableSQLite> ITableSQLiteCollection { get; set; }
        public static ObservableCollection<ItemType> ItemTypeCollection { get; set; }
        public static ObservableCollection<Models.Location> LocationCollection { get; set; }
        public static ObservableCollection<User> UserCollection { get; set; }

        public static User UserCurrent { get; set; }
        static GlobalUsing()
        {

        }

    }

    public enum MapType
    {
        Street,
        Satellite,
        Hybrid,
    }
    public enum DistanceType
    {
        Miles,
        Meters,
        Kilometers,
    }
    public enum MapItemType
    {
        Pin,
        Polygon,
        Polyline,

    }
    [Flags]
    public enum UserState
    {
        NotActivated = 0,
        Activated = 1,
        UseCloud = 2,
    }
    [Flags]
    public enum MapItiemState
    {
        UnShow = 1,
        Show = 2,
        Star = 4,
    }
    public static class Config
    {
        public static event PropertyChangedEventHandler PropertyChanged;

        private static DistanceType isDistanceType;
        private static bool isShowingUser;
        private static bool isScrollEnabled;
        private static bool isZoomEnabled;
        private static bool isTrafficEnabled;

        public static readonly List<string> mapType;
        public static SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache |
            // encrypted and inaccessible while the device is locked.
            SQLite.SQLiteOpenFlags.ProtectionComplete;
        public static readonly string DatabasePath;

        public static DistanceType IsDistanceType
        {
            get => isDistanceType;
            set
            {
                if (isDistanceType != value)
                {
                    isDistanceType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public static bool IsShowingUser
        {
            get => isShowingUser;
            set
            {
                if (isShowingUser != value)
                {
                    isShowingUser = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public static bool IsScrollEnabled
        {
            get => isScrollEnabled;
            set
            {
                if (isScrollEnabled != value)
                {
                    isScrollEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public static bool IsZoomEnabled
        {
            get => isZoomEnabled;
            set
            {
                if (isZoomEnabled != value)
                {
                    isZoomEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public static bool IsTrafficEnabled
        {
            get => isTrafficEnabled;
            set
            {
                if (isTrafficEnabled != value)
                {
                    isTrafficEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        static Config()
        {
            IsDistanceType = DistanceType.Meters;
            IsShowingUser = true;
            IsScrollEnabled = true;
            IsZoomEnabled = true;
            IsTrafficEnabled = true;
            mapType = new List<string>() { "Street", "Satellite", "Hybrid" };
            DatabasePath = Path.Combine(FileSystem.Current.AppDataDirectory, "Database.db");
        }

        private static void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


}
