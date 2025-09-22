using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Enderecos;
using AppRpgEtec.Services.Usuarios;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;
using System.Windows.Input; 
using Microsoft.Maui.Controls; 

namespace AppRpgEtec.ViewModels.Usuarios
{
    class LocalizacaoViewModel : BaseViewModel
    {
        private UsuarioService uService;
        private EnderecoService enderecoService; 
        private string cep;

        public LocalizacaoViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);
            enderecoService = new EnderecoService(); 
        }

        private Map meuMapa;
        public Map MeuMapa
        {
            get => meuMapa;
            set
            {
                if (value != null)
                {
                    meuMapa = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Cep
        {
            get => cep;
            set
            {
                if (cep != value)
                {
                    cep = value;
                    OnPropertyChanged();
                }
            }
        }

        public async void BuscarEnderecoEAdicionarPin()
        {
            if (string.IsNullOrEmpty(Cep))
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Informe um CEP válido.", "OK");
                return;
            }

            try
            {
                var enderecos = await enderecoService.BuscarCep(Cep);

                if (enderecos == null || enderecos.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Endereço não encontrado.", "OK");
                    return;
                }

                var endereco = enderecos.First();

                double latitude = Convert.ToDouble(endereco.lat);
                double longitude = Convert.ToDouble(endereco.lon);

                Location location = new Location(latitude, longitude);
                Pin pinEndereco = new Pin()
                {
                    Type = PinType.Place,
                    Label = endereco.name,
                    Address = endereco.display_name,
                    Location = location
                };

                Map map = new Map();
                MapSpan mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5));
                map.Pins.Add(pinEndereco);
                map.MoveToRegion(mapSpan);

                MeuMapa = map; 
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private ICommand _buscarEnderecoCommand;
        public ICommand BuscarEnderecoCommand
        {
            get
            {
                return _buscarEnderecoCommand ??= new Command(() => BuscarEnderecoEAdicionarPin());
            }
        }

        public async void InicializarMapa()
        {
            try
            {
                Location location = new Location(-23.5200241d, -46.596498d);
                Pin pinEtec = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Etec Horácio",
                    Address = "Rua alcantara, 113, vila Guilherme",
                    Location = location
                };

                Map map = new Map();
                MapSpan mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5));
                map.Pins.Add(pinEtec);
                map.MoveToRegion(mapSpan);

                MeuMapa = map;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", e.Message, "OK");
            }
        }

        public async void ExibirUsuarioNoMapa()
        {
            try
            {
                ObservableCollection<Usuario> ocUsuarios = await uService.GetUsuariosAsync();
                List<Usuario> listaUsuarios = new List<Usuario>(ocUsuarios);
                Map map = new Map();

                foreach (Usuario u in listaUsuarios)
                {
                    if (u.Latitude != null && u.Longitude != null)
                    {
                        double latitude = (double)u.Latitude;
                        double longitude = (double)u.Longitude;
                        Location location = new Location(latitude, longitude);

                        Pin pinAtual = new Pin()
                        {
                            Type = PinType.Place,
                            Label = u.Username,
                            Address = $"E-mail: {u.Email}",
                            Location = location
                        };
                        map.Pins.Add(pinAtual);
                    }
                }
                MeuMapa = map;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}
