namespace SVGService;

public sealed class SVGInfo
{
    private readonly HttpClient _client;

    public SVGInfo(
        HttpClient client)
    {
        _client = client;
    }

    public async Task<string?> GetSVGData(string fullName)
    {    
        HttpResponseMessage response = await _client.GetAsync($"get-initials?name={fullName}");

        return await response.Content.ReadAsStringAsync();
    }
}
