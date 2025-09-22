using AppRpgEtec.Models;
using Newtonsoft.Json;

namespace AppRpgEtec.Services.Enderecos
{
    public class EnderecoService
    {
        private readonly HttpClient _httpClient;

        public EnderecoService()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "CeplocatorMaui/1.0 (matheus.duarte35@etec.sp.gov.br)");
        }

        public async Task<List<Endereco>> BuscarCep(string name)
        {
            string url = $"https://nominatim.openstreetmap.org/search?format=json&q={name}";
            var response = await _httpClient.GetStringAsync(url);

            return JsonConvert.DeserializeObject<List<Endereco>>(response);
        }
    }
}