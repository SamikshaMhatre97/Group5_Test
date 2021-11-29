using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_Crime_Start.DataAccess;
using MVC_Crime_Start.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVC_Crime_Start.Controllers
{
    public class HomeController : Controller
    {
        HttpClient httpClient;

        CrimeRoot crimeRootObj = new CrimeRoot();

        //Defining the base URL
        static string BASE_URL = "https://api.usa.gov/crime/fbi/sapi/";
        static string API_KEY = "cS8ov8HfpdKkcJS8YgOg92Kqr866oarciE1bbr20"; //Add your API key here inside ""

        public ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        private readonly ILogger<HomeController> _logger;

        //Fucntion to fetch API and Store the response
        public IActionResult Index()
        {

            string[] stateList = new string[] { "CA", "FL", "NC", "TX", "OH", "MA", "IL", "NY", "GA", "AL", "MI" };

            for (int s = 0; s < stateList.Length; s++)
            {
                //Adding the query parameters to the base URL
                string CRIME_DATA_API_PATH = BASE_URL + "/api/summarized/estimates/states/" + stateList[s] + "/2010/2016?limit=2";
                string crimeData = "";

                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.BaseAddress = new Uri(CRIME_DATA_API_PATH);

                try
                {
                    HttpResponseMessage response = httpClient.GetAsync(CRIME_DATA_API_PATH)
                                                            .GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        crimeData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }

                    //Commented this and Ensure Created function from the Startup.cs
                    //InsertApiToDb(crimeData);

                }


                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return View();

        }

        //Inserting Fetched Data into DB
        public async Task<ViewResult> InsertApiToDb(string crimeData)
        {
            if (!crimeData.Equals(""))
            {
                crimeRootObj = JsonConvert.DeserializeObject<CrimeRoot>(crimeData);

                for (int i = 0; i < crimeRootObj.results.Length; i++)
                {
                    dbContext.CrimeResults.Add(crimeRootObj.results[i]);
                }
                await dbContext.SaveChangesAsync();
            }
            return View();

        }

        //Create Record
        public IActionResult CreateRecord(string state_abbr, int year,
                                              int population, int robbery, int arson, int homicide)
        {
            if (state_abbr != null)
            {
                CrimeResult cr = new CrimeResult(state_abbr, year, population, robbery, arson, homicide)
                {
                    state_abbr = state_abbr,
                    year = year,
                    population = population,
                    robbery = robbery,
                    arson = arson,
                    homicide = homicide
                };
                dbContext.CrimeResults.Add(cr);
                dbContext.SaveChanges();
            }

            return View();
        }



        //READ
        public IActionResult SearchRecord(string search)
        {
            string[] ChartColors = new string[] { "#C70039", "#55CAA2", "#283980" };

            if (search != null)
            {
                var res = dbContext.CrimeResults.Where(x => x.year.ToString() == search.Trim())
                    .Select(x => new CrimeResult(x.state_abbr, x.year, x.population,
                                              x.arson, x.robbery, x.homicide)).ToList();
                int homicide = 0;
                int robbery = 0;
                int arson = 0;
                List<string> crimes = new List<string>();
                crimes.Add("homicide");
                crimes.Add("robbery");
                crimes.Add("arson");
                foreach (var x in res)
                {
                    homicide += x.homicide;
                    robbery += x.robbery;
                    arson += x.arson;
                }
                List<int> counts = new List<int>();
                counts.Add(homicide);
                counts.Add(robbery);
                counts.Add(arson);

                ViewBag.counts = String.Join(",", counts.Select(d => d));
                ViewBag.crimes = String.Join(",", crimes.Select(d => "'" + d + "'"));
                ViewBag.colors = String.Join(",", ChartColors.Select(d => "'" + d + "'"));
                return View(res);

            }
            return View();
        }

        //UPDATE
        public IActionResult UpdateRecords(string state_abbr, int year, int population)
        {
            if (state_abbr != null)
            {
                var res1 = dbContext.CrimeResults.Where(x => x.year == year && x.state_abbr == state_abbr.Trim()).FirstOrDefault();

                if (res1 != null)
                {
                    res1.population = population;
                    dbContext.SaveChanges();
                }
                return View(res1);
            }

            return View();
        }

        //DELETE
        public IActionResult DeleteRecord(string state_abbr, int year)
        {
            if (state_abbr != null)
            {

            }
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult CrimeChart(List<CrimeResult> crimeList)
        {

            return View();
        }

    }
}