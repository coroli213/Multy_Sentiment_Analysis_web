using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using testtest.Models;
using Microsoft.ML;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace testtest
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
          

            Configuration = configuration;

            Program.LoadModelFromFile();

            //  Console.WriteLine($"=============== Загрузка данных  ===============");
           //  Program.trainingDataView = Program.mlContext.Data.LoadFromTextFile<SentimentData>(Program._RUMuTrainDataPath, hasHeader: true);
            /// //  Console.WriteLine($"=============== Данные загружены ===============");         
           //    var pipeline = Program.ProcessData_();
           //  Program.BuildAndTrainModel_(Program.trainingDataView, pipeline);
            ///
            // Console.WriteLine();
            // Console.WriteLine(UseModelWithSingleItem("я с января хотела записаться, я думала ты еще ходишь"));
            // Console.WriteLine();

            //  UseLoadedModelWithBatchItems();

            // Program.SaveModelAsFile();
            

        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ValuesContext>(opt =>
                 opt.UseInMemoryDatabase("SentysList"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseFileServer();
        }
    }
}
