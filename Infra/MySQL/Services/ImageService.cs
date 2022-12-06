using Domain.Aggregates;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.MySQL.Services
{
    public class ImageDbService : IImageDbService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ImageDbService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Create(Image image)
        {
            using var dataContext = serviceScopeFactory.CreateScope()
                .ServiceProvider.GetRequiredService<DataContext>();

            dataContext.Image.Add(image);
            await dataContext.SaveChangesAsync();
        }
    }
}
