﻿using Core.Models;
using DAL.Persistence.Core;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace DAL.Persistence.Repositories;

public class AnonymousChildRepository : IAnonymousChildRepository<AnonymousChildModel>
{
    private readonly IHttpClientFactory _clientFactory;

    public AnonymousChildRepository(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<AnonymousChildModel> AddAnonymousChild(AnonymousChildModel anonymousChild)
    {
        var client = _clientFactory.CreateClient("meta");

        try
        {
            var content = JsonConvert.SerializeObject(anonymousChild);

            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);

            var s = await client.PostAsync(DbNodePath.AnonymousChild(), byteContent);

            if (s.IsSuccessStatusCode)
            {
                return anonymousChild;
            }
            else
            {
                throw new Exception(s.ReasonPhrase);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<AnonymousChildModel>> GetAnonymousChildren()
    {
        var client = _clientFactory.CreateClient("meta");

        try
        {
            var s = await client.GetFromJsonAsync<Dictionary<string, AnonymousChildModel>>(DbNodePath.AnonymousChild());

            return s.Values.ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
