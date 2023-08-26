
using Location = Microsoft.Maui.Devices.Sensors.Location;
using System.Text.RegularExpressions;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
namespace My_Map_MAUI.ViewModels
{
    public partial class MapViewModels : DataOfMapViewModels
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        [ObservableProperty]
        private Location currentLocation;

        [ObservableProperty]
        private string searchContent;

        public MapViewModels(IConnectivity connectivity, Serviecs.SQLiteDatabase database) : base(connectivity, database)
        {
            Title = "Map";
            IsBusy = false;
            IsFirstTimes = true;
            currentLocation = null;
            searchContent = string.Empty;
            IsShowUser = false;
        }

       
        public async Task GetCachedLocation()
        {
            try
            {
                IsBusy = true;
                Location location = await Geolocation.Default.GetLastKnownLocationAsync();

                if (location != null)
                    CurrentLocation = location;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await Shell.Current.DisplayAlert("!!!ERROR!!!", fnsEx.Message, "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await Shell.Current.DisplayAlert("!!!ERROR!!!", fneEx.Message, "Ok");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await Shell.Current.DisplayAlert("!!!ERROR!!!", pEx.Message, "Ok");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }

        }

        public async Task GetCurrentLocation()
        {
            try
            {
                IsBusy = true;
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(20));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    CurrentLocation = location;
                }
                else
                {
                    await GetCachedLocation();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await Shell.Current.DisplayAlert("!!!ERROR!!!", fnsEx.Message, "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await Shell.Current.DisplayAlert("!!!ERROR!!!", fneEx.Message, "Ok");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await Shell.Current.DisplayAlert("!!!ERROR!!!", pEx.Message, "Ok");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
                _isCheckingLocation = false;
            }
        }

        public void CancelRequest()
        {
            IsBusy = true;
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();

            IsBusy = false;
        }

        [RelayCommand]
        public async Task SearchAsync(Microsoft.Maui.Controls.Maps.Map map)
        {
            if (SearchContent == string.Empty)
            {
                return;
            }
            
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                using (HttpClient  client = new HttpClient()) 
                {
                    await LoadPinsAsync();
                    using var httpRequestMessage = new HttpRequestMessage();
                    httpRequestMessage.Method = HttpMethod.Get;
                    httpRequestMessage.RequestUri = new Uri(@$"https://www.google.com/maps/search/{string.Join("+", SearchContent.Split(" ", StringSplitOptions.RemoveEmptyEntries))}/");
                    httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0");
                    httpRequestMessage.Headers.Add("Accept", "text/html,application/xhtml+xml+json");
                    var result = await client.SendAsync(httpRequestMessage);
                    if(result.IsSuccessStatusCode)
                    {
                        //format number
                        var nfi = new CultureInfo("en-US", false).NumberFormat;
                        nfi.NumberGroupSeparator = "";
                        nfi.NumberDecimalDigits = 0;

                        string regex = @"(?<=staticmap\?center=).*(?=(&amp;client=google-maps-frontend&amp;signature=).*(itemprop=""image"">))";
                        string content = await result.Content.ReadAsStringAsync();
                        var list_content = Regex.Match(content, regex).ToString().Split("&amp;",StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);

                        var temp = list_content[0].Split("%2C", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        Location center = new Location(float.Parse(temp[0], NumberStyles.Float, nfi), float.Parse(temp[1], NumberStyles.Float, nfi));
                        var zoom = list_content[1].Split("=");
                        double latlongDegrees =float.Parse(zoom[1]);
                        if (list_content[3].StartsWith("markers="))
                        {
                            list_content[3] = list_content[3].Remove(0,8);
                            foreach (var item in list_content[3].Split("%7C", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                            {
                                temp = item.Split("%2C", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                Location location = new Location(double.Parse(temp[0], NumberStyles.Float, nfi), double.Parse(temp[1], NumberStyles.Float, nfi));
                                map.Pins.Add(new Pin()
                                {
                                    Address = "New pin",
                                    Label = "Search",
                                    Location = location,
                                    Type = PinType.SearchResult,
                                });
                            }
                        }
                        else
                        {
                            regex = @"(?<=\/@).*(?=\/data\\\\)";
                            temp = Regex.Match(content, regex).ToString().Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                            if(temp.Length > 0) 
                            {
                                Location location = new Location(double.Parse(temp[0], NumberStyles.Float, nfi), double.Parse(temp[1], NumberStyles.Float, nfi));
                                map.Pins.Add(new Pin()
                                {
                                    Address = "New pin",
                                    Label = "Search",
                                    Location = location
                                });
                            }
                            else
                            {
                                await Shell.Current.DisplayAlert("Notification", "Location not found", "Ok");

                            }
                        }
                        map.MoveToRegion(new MapSpan(center, latlongDegrees, latlongDegrees));

                    }
                }
            
            }
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }

        }

    }

}
