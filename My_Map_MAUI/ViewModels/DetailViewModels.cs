

using System.Threading;

namespace My_Map_MAUI.ViewModels
{
    [QueryProperty(nameof(IsCreateNew),"IsCreateNew")]
    [QueryProperty(nameof(MapLocation),"Location")]
    [QueryProperty(nameof(MapItemType),"MapItemType")]
    [QueryProperty( nameof(PinItem),"PinItem")]
    public partial class DetailViewModels : DataOfMapViewModels
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotCreateNew))]
        private bool isCreateNew;
        [ObservableProperty]
        private Microsoft.Maui.Devices.Sensors.Location mapLocation;
        [ObservableProperty]
        private MapItemType mapItemType;
        [ObservableProperty]
        private PinItems pinItem;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "{0} can not be blank")]
        private string name;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "{0} is 1 to 100 characters long")]
        [Required(ErrorMessage = "{0} can not be blank")]
        private string shortDescription;
        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string nameError;

        [ObservableProperty]
        private string shortDescriptionError;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotNameReady))]
        private bool isNameReady;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotShortDescriptionReady))]
        private bool isShortDescriptionReady;

        [ObservableProperty]
        private PinType pinTypes;
        [ObservableProperty]
        private List<PinType> listPinType; 
        public bool IsNotShortDescriptionReady => !IsShortDescriptionReady;
        public bool IsNotNameReady  => !IsNameReady;
        public bool IsNotCreateNew => !IsCreateNew;
        public DetailViewModels(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            Title = "Detail";
            IsBusy = false;
            IsNameReady = false;
            IsShortDescriptionReady = false;
            
            PinTypes = PinType.Place;
            listPinType = Enum.GetValues(typeof(PinType)).Cast<PinType>().ToList();
        }

        public void CheckName()
        {
            ValidateProperty(Name, nameof(Name));
            if (HasErrors)
            {
                IsNameReady = false;
                NameError = string.Join(Environment.NewLine, GetErrors().Where(p => p.MemberNames.All(q => q == nameof(Name))).Select(e => e.ErrorMessage));
                if (NameError == "")
                {
                    IsNameReady = true;
                }
            }
            else
            {
                NameError = "";
                IsNameReady = true;
            }
        }

        public void CheckShortDescription()
        {
            ValidateProperty(ShortDescription, nameof(ShortDescription));
            if (HasErrors)
            {
                IsShortDescriptionReady = false;
                ShortDescriptionError = string.Join(Environment.NewLine, GetErrors().Where(p => p.MemberNames.All(q => q == nameof(ShortDescription))).Select(e => e.ErrorMessage));
                if (ShortDescriptionError == "")
                {
                    IsShortDescriptionReady = true;
                }
            }
            else
            {
                ShortDescriptionError = "";
                IsShortDescriptionReady = true;
            }
        }

        [RelayCommand]
        public async Task Save()
        {
            if (IsBusy) { return; }
            try
            {
                IsBusy = true;
                if (MapItemType == MapItemType.Pin)
                {
                    if (IsCreateNew)
                    {
                        var mapItem = new MapItem();
                        var detail = new Detail();
                        var location = new Models.Location();
                        var itemTyoe = new ItemType();
                        var image = new Models.Image();
                        mapItem.ShortDescription = ShortDescription;
                        mapItem.Email = GlobalUsing.UserCurrent.Email;
                        mapItem.Name = Name;
                        mapItem.State = MapItiemState.Show;
                        //load id map item
                        // detail , itemTyoe id == map id
                        await database.SaveItemAsync(mapItem);
                        mapItem = await database.GetLatestMapItemAsync();
                        image.ItemId = mapItem.Id;
                        detail.Id = mapItem.Id;
                        detail.Description = Description;
                        location.ItemId = mapItem.Id;
                        location.Latitude = MapLocation.Latitude;
                        location.Longitude = MapLocation.Longitude;
                        itemTyoe.Id = mapItem.Id;
                        itemTyoe.MapItemType = MapItemType.Pin;
                        itemTyoe.PinType = PinTypes;


                        var array = new ArrayList()
                        {
                            location ,
                            image ,
                        };

                        foreach (ITableSQLite item in array)
                        {
                            await database.SaveItemAsync(item);
                        }
                        array = new ArrayList()
                        {
                            detail ,
                            itemTyoe ,
                        };
                        
                        foreach (ITableSQLite item in array)
                        {
                            await database.InsertItemAsync(item);
                        }

                        
                    }
                    else
                    {
                        ArrayList data = await LoadData(MapItemType.Pin, PinItem.Id);
                        if (data == null) return;
                        List<MapItem> mapItems = (List<MapItem>)data[0];
                        List<Detail> details = (List<Detail>)data[1];
                        List<ItemType> itemTypes = (List<ItemType>)data[2];
                        List<Models.Image> images = (List<Models.Image>)data[3];
                        List<Models.Location> Location = (List<Models.Location>)data[4];

                        mapItems[0].ShortDescription = ShortDescription;
                        mapItems[0].Email = GlobalUsing.UserCurrent.Email;
                        mapItems[0].Name = Name;
                        mapItems[0].State = MapItiemState.Show;

                        details[0].Description = Description;
                        Location[0].Latitude = PinItem.Location.Latitude;
                        Location[0].Longitude = PinItem.Location.Longitude;
                        itemTypes[0].MapItemType = MapItemType.Pin;
                        itemTypes[0].PinType = PinTypes;

                        var array = new ArrayList()
                        {
                            mapItems[0] ,
                            details[0] ,
                            Location[0],
                            itemTypes[0],
                            images[0],
                        };
                        foreach (ITableSQLite item in array)
                        {
                            await database.SaveItemAsync(item);
                        }
                    }
                    


                }
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                string text = "Save success";
                ToastDuration duration = ToastDuration.Short;
                double fontSize = 15;

                var toast = Toast.Make(text, duration, fontSize);

                await toast.Show(cancellationTokenSource.Token);
                await Shell.Current.GoToAsync("//Home", true);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally { IsBusy = false; }

        }
        [RelayCommand]
        public async Task Delete()
        {
            if (IsBusy) { return; }
            try
            {
                IsBusy = true;
                if (MapItemType == MapItemType.Pin)
                {

                        ArrayList data = await LoadData(MapItemType.Pin, PinItem.Id);
                        if (data == null) return;
                        List<MapItem> mapItems = (List<MapItem>)data[0];
                        List<Detail> details = (List<Detail>)data[1];
                        List<ItemType> itemTypes = (List<ItemType>)data[2];
                        List<Models.Image> images = (List<Models.Image>)data[3];
                        List<Models.Location> Location = (List<Models.Location>)data[4];

                        var array = new ArrayList()
                        {
                            mapItems[0] ,
                            details[0] ,
                            Location[0],
                            itemTypes[0],
                            images[0],
                        };
                        foreach (ITableSQLite item in array)
                        {
                            await database.DeleteItemAsync(item);
                        }
                    

                }
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                string text = "Save success";
                ToastDuration duration = ToastDuration.Short;
                double fontSize = 15;

                var toast = Toast.Make(text, duration, fontSize);

                await toast.Show(cancellationTokenSource.Token);
                await Shell.Current.GoToAsync("//Home", true);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally { IsBusy = false; }
        }
        public async void Load()
        {
            if (MapItemType == MapItemType.Pin)
            {
                if(PinItem != null)
                {
                    Name = PinItem.Label;
                    ShortDescription = PinItem.Address;
                    var temp = await LoadData(MapItemType.Pin,PinItem.Id);
                    Description = ((List<Detail>)temp[1])[0].Description;
                    PinTypes = PinItem.PinType;
                }
            }
        }
        [RelayCommand]
        public async Task Init()
        {
            if (IsBusy) { return; }
            try
            {
                IsBusy = true;
                if(IsCreateNew)
                {
                    return;
                }
                else
                {
                    Load();
                }
            }
            catch (Exception ex) 
            {
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally { IsBusy = false; }
        }

    }
}
