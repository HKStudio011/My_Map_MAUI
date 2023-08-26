using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Location = Microsoft.Maui.Devices.Sensors.Location;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;

namespace My_Map_MAUI.ViewModels
{
    public partial class DataOfMapViewModels : BaseViewModel
    {
        public ObservableCollection<Items> ItemsCollection { get; } = new();

        public DataOfMapViewModels(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            Title = "";
            IsBusy = false;
            IsFirstTimes = true;
        }


        [RelayCommand]
        public async Task GoToDetailCreatePinAsync(Location location)
        {
            if (IsBusy) return;
            IsBusy = true;

            await Shell.Current.GoToAsync("//MapPage/DetailPage", true, new Dictionary<string, object>()
            {
                {"IsCreateNew",true },
                {"Location",location },
                {"MapItemType", MapItemType.Pin },

            });

            IsBusy = false;
        }
        [RelayCommand]
        public async Task GoToDetailViewPinAsync(PinItems pinItems)
        {
            if (IsBusy) return;
            IsBusy = true;

            await Shell.Current.GoToAsync("//LocationPage/DetailPage", true, new Dictionary<string, object>()
            {
                {"IsCreateNew",false },
                {"MapItemType", MapItemType.Pin },
                { "PinItem",pinItems }
            });

            IsBusy = false;
        }

        public async Task<ArrayList> LoadData(MapItemType type, int? itemId = null)
        {
            string query;

            if (itemId == null)
            {
                query = $"select MapItem.* from MapItem,ItemType where MapItem.Id = ItemType.Id and ItemType.MapItemType = {(int)type} and MapItem.Email = '{GlobalUsing.UserCurrent.Email}'";
            }
            else
            {
                query = $"select MapItem.* from MapItem,ItemType where MapItem.Id = {itemId} and MapItem.Id = ItemType.Id and ItemType.MapItemType = {(int)type} and MapItem.Email = '{GlobalUsing.UserCurrent.Email}'";
            }

            var mapItems = await database.Database
                .QueryAsync<MapItem>(query);
            if (mapItems.Count == 0) { return null; }
            var details = (await database.Database.Table<Detail>().ToListAsync()).Join(mapItems, d => d.Id, m => m.Id,
                (d, b) =>
                {
                    return d;
                }).ToList();
            var itemTypes = (await database.Database.Table<ItemType>().ToListAsync()).Join(mapItems, d => d.Id, m => m.Id,
                (d, b) =>
                {
                    return d;
                }).Cast<ItemType>().ToList();
            var images = (await database.Database.Table<My_Map_MAUI.Models.Image>().ToListAsync()).Join(mapItems, d => d.ItemId, m => m.Id,
                (d, b) =>
                {
                    return d;
                }).ToList();
            var Location = (await database.Database.Table<My_Map_MAUI.Models.Location>().ToListAsync()).Join(mapItems, d => d.ItemId, m => m.Id,
                (d, b) =>
                {
                    return d;
                }).ToList();

            return new ArrayList() {
                mapItems,
                details,
                itemTypes,
                images,
                Location};
        }


        public async Task LoadPinsAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                ArrayList data = await LoadData(MapItemType.Pin);
                if (data == null) return;
                ItemsCollection.Clear();
                List<MapItem> mapItems = (List<MapItem>)data[0];
                List<Detail> details = (List<Detail>)data[1];
                List<ItemType> itemTypes = (List<ItemType>)data[2];
                List<Models.Image> images = (List<Models.Image>)data[3];
                List<Models.Location> Location = (List<Models.Location>)data[4];
                for (int i = 0; i < mapItems.Count; i++)
                {
                    PinItems pinItems = new PinItems();
                    pinItems.Id = mapItems[i].Id;
                    pinItems.Label = mapItems[i].Name;
                    pinItems.Address = mapItems[i].ShortDescription;
                    pinItems.Location = new Location(Location[i].Latitude, Location[i].Longitude);
                    pinItems.PinType = itemTypes[i].PinType;
                    ItemsCollection.Add(pinItems);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }


        public async Task ImportAsyc()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                string path = "";
                var result = await FilePicker.Default.PickAsync(PickOptions.Default);

                if (result != null)
                {

                    if (result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        path = result.FullPath;
                    }
                }

                if (path == "")
                {
                    await Shell.Current.DisplayAlert("!!!ERROR!!!", "Path does not exist", "Ok");
                    return;
                }

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                string text;
                ToastDuration duration = ToastDuration.Short;
                double fontSize = 15;

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new System.IO.FileInfo(path)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên

                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        try
                        {
                            //format number
                            var nfi = new CultureInfo("en-US", false).NumberFormat;
                            nfi.NumberGroupSeparator = "";
                            nfi.NumberDecimalDigits = 0;

                            var mapItem = new MapItem();
                            var detail = new Detail();
                            var location = new Models.Location();
                            var itemTyoe = new ItemType();
                            var image = new Models.Image();
                            mapItem.ShortDescription = worksheet.Cells[row, 2].Value.ToString();
                            mapItem.Email = GlobalUsing.UserCurrent.Email;
                            mapItem.Name = worksheet.Cells[row, 1].Value.ToString(); ;
                            mapItem.State = MapItiemState.Show;
                            //load id map item
                            // detail , itemTyoe id == map id
                            await database.SaveItemAsync(mapItem);
                            mapItem = await database.GetLatestMapItemAsync();
                            image.ItemId = mapItem.Id;
                            detail.Id = mapItem.Id;
                            detail.Description = worksheet.Cells[row, 4].Value.ToString();
                            location.ItemId = mapItem.Id;
                            var local = worksheet.Cells[row, 3].Value.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                            location.Latitude = double.Parse(local[0], NumberStyles.Float, nfi);
                            location.Longitude = double.Parse(local[1], NumberStyles.Float, nfi);
                            itemTyoe.Id = mapItem.Id;
                            itemTyoe.MapItemType = MapItemType.Pin;
                            itemTyoe.PinType = PinType.SavedPin;

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
                        catch (Exception ex)
                        {
                            text = ex.Message;
                            var toast1 = Toast.Make(text, duration, fontSize);
                            await toast1.Show(cancellationTokenSource.Token);
                        }
                    }

                }

                text = "Import success";
                var toast = Toast.Make(text, duration, fontSize);
                await toast.Show(cancellationTokenSource.Token);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
