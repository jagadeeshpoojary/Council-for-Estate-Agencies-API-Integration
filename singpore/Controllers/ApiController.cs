using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace singpore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        [HttpPost]
        public async Task<IActionResult> MakeApiRequestAsync(string regno)
        {
            
            try
            {
               
                
                string apiUrl = "https://www.cea.gov.sg/aceas/api/internet/profile/v2/public-register/filter";
                string jsonPayload = $"{{\"page\":1,\"pageSize\":100,\"sortAscFlag\":true,\"sort\":\"name\",\"registrationNumber\":\"{regno}\",\"profileType\":2}}";
                var client = _httpClientFactory.CreateClient();
                Console.WriteLine(client);
                
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");          
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                   
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        using (var jsonDocument = await JsonDocument.ParseAsync(stream))
                        {
                            var jsonElement = jsonDocument.RootElement;
                            var dataArray = jsonElement.GetProperty("data");

                          
                            var namesList = new List<string>();

                          
                            foreach (var dataElement in dataArray.EnumerateArray())
                            {
                                
                                var name = dataElement.GetProperty("registrationNumber").GetString();
                               namesList.Add(name);
                            }

                           
                            return Ok(namesList);
                        }

                    }
                }
                else
                {
                    return BadRequest($"Error: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"HttpRequestException: {ex.Message}");
            }
        }
    }
}

