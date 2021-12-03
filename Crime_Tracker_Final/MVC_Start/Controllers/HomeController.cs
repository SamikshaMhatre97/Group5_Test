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
using LINQtoCSV;
using System.IO;

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


            return View();
        }

        //Inserting Fetched Data into DB
        public IActionResult InsertApiToDb()
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
                    if (!crimeData.Equals(""))
                    {
                        crimeRootObj = JsonConvert.DeserializeObject<CrimeRoot>(crimeData);

                        for (int i = 0; i < crimeRootObj.results.Length; i++)
                        {
                            dbContext.CrimeResults.Add(crimeRootObj.results[i]);
                        }
                        dbContext.SaveChangesAsync();
                    }

                }


                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return View("Index");


        }





        //Create Record
        public IActionResult CreateRecord(string state_abbr, int year,
                                              int population, int arson, int homicide, int robbery, string search)
        {
            try
            {
                string[] ChartColors = new string[] { "#C70039", "#55CAA2", "#283980" };

                if (state_abbr != null)
                {
                    CrimeResult cr = new CrimeResult(state_abbr, year, population, arson, homicide, robbery)
                    {
                        state_abbr = state_abbr,
                        year = year,
                        population = population,
                        arson = arson,
                        homicide = homicide,
                        robbery = robbery

                    };
                    dbContext.CrimeResults.Add(cr);
                    dbContext.SaveChanges();
                    ViewBag.Message = String.Format("Crime Record Added for the state " + state_abbr + " and year " + year);
                }
                if (search != null)
                {
                    SearchRecord(search);
                }
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message.
                Console.WriteLine(e.Message);
            }
            return View();
        }



        //READ
        public IActionResult SearchRecord(string search)
        {
            try
            {
                string[] ChartColors = new string[] { "#C70039", "#55CAA2", "#283980" };

                if (search != null)
                {
                    var res = dbContext.CrimeResults.Where(x => x.year.ToString() == search.Trim())
                        .Select(x => new CrimeResult(x.state_abbr, x.year, x.population,
                                                  x.arson, x.homicide, x.robbery)).ToList();
                    int homicide = 0;
                    int robbery = 0;
                    int arson = 0;
                    List<string> crimes = new List<string>();
                    crimes.Add("arson");
                    crimes.Add("homicide");
                    crimes.Add("robbery");
                    foreach (var x in res)
                    {
                        arson += x.arson;
                        homicide += x.homicide;
                        robbery += x.robbery;
                    }
                    List<int> counts = new List<int>();
                    counts.Add(arson);
                    counts.Add(homicide);
                    counts.Add(robbery);

                    ViewBag.counts = String.Join(",", counts.Select(d => d));
                    ViewBag.crimes = String.Join(",", crimes.Select(d => "'" + d + "'"));
                    ViewBag.colors = String.Join(",", ChartColors.Select(d => "'" + d + "'"));
                    return View(res);

                }
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message.
                Console.WriteLine(e.Message);
            }
            return View();
        }

        //UPDATE
        public IActionResult UpdateRecords(string state_abbr, int year, int population)
        {
            try
            {
                if (state_abbr != null)
                {
                    var res1 = dbContext.CrimeResults.Where(x => x.year == year && x.state_abbr == state_abbr.Trim()).FirstOrDefault();

                    if (res1 != null)
                    {
                        res1.population = population;
                        dbContext.SaveChanges();
                        ViewBag.Message = String.Format("Population Updated for the state " + state_abbr + " and year " + year);

                    }
                    return View(res1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View();
        }

        //DELETE
        public IActionResult DeleteRecords(string state_abbr, int year)
        {
            try
            {
                if (state_abbr != null)
                {

                    CrimeResult cr3 = new CrimeResult();
                    cr3 = dbContext.CrimeResults.Where(x => x.year == year && x.state_abbr == state_abbr.Trim()).FirstOrDefault();
                    if (cr3 != null)
                    {
                        dbContext.CrimeResults.Remove(cr3);
                        dbContext.SaveChanges();
                        ViewBag.Message = String.Format("Crime Record Deleted for the state " + state_abbr + " and year " + year);

                    }

                    return View(cr3);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        //upload agency data
        public async Task<ViewResult> UploadCsv()
        {
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext csvContext = new CsvContext();
            StreamReader streamReader = new StreamReader("agency.csv");
            IEnumerable<Agency> list = csvContext.Read<Agency>(streamReader, csvFileDescription);
            dbContext.Agency.AddRange(list);
            await dbContext.SaveChangesAsync();
            // return Redirect("GetAllEmployeeData");
            return View("Index");

        }


    }
}