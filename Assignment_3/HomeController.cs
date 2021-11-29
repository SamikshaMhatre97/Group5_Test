using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVC_Start.Models;
using Newtonsoft.Json;

namespace MVC_Start.Controllers
{
    public class HomeController : Controller
    {
        HttpClient httpClient;

        static string BASE_URL = "https://api.usa.gov/crime/fbi/sapi/";
        static string API_KEY = "cS8ov8HfpdKkcJS8YgOg92Kqr866oarciE1bbr20"; //Add your API key here inside ""

        public IActionResult Index(int id)
        {
        
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //   string CRIME_DATA_API_PATH = BASE_URL + "/api/summarized/estimates/states?limit=2";
            
            //taking the URL for California from year 2010 and 2016
                string CRIME_DATA_API_PATH = BASE_URL + "/api/summarized/estimates/states/CA/2010/2016?limit=2";
                string crimeData = "";
            

           // Parks parks = new Parks();

            httpClient.BaseAddress = new Uri(CRIME_DATA_API_PATH);

            try
            {
                HttpResponseMessage response = httpClient.GetAsync(CRIME_DATA_API_PATH)
                                                        .GetAwaiter().GetResult();
                //HttpResponseMessage response = httpClient.GetAsync(BASE_URL)
                //                                        .GetAwaiter().GetResult();


                if (response.IsSuccessStatusCode)
                {
                    crimeData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                //if (!crimeData.Equals(""))
                //{
                //    // JsonConvert is part of the NewtonSoft.Json Nuget package
                //    parks = JsonConvert.DeserializeObject<Parks>(crimeData);
                //}

                //dbContext.Parks.Add(parks);
                //await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }

            //  return View(parks);

            return View();
        }

        public IActionResult IndexWithLayout(string id, float abc )
        {
            return View();
        }

        public IActionResult Contact()
        {
            GuestContact contact = new GuestContact();

            contact.Name = "Samiksha";
            contact.Email = "mhatre1@usf.edu";
            contact.Phone = "813-3456788";


            /* alternate syntax to initialize object 
            GuestContact contact2 = new GuestContact
            {
              Name = "Manish Agrawal",
              Email = "magrawal@usf.edu",
              Phone = "813-974-6716"
            };
            */

            //ViewData["Message"] = "Your contact page.";

            return View(contact);
        }

        [HttpPost]
        public IActionResult Contact(GuestContact contact)
        {
            return View(contact);
        }

        /// <summary>
        /// Replicate the chart example in the JavaScript presentation
        /// 
        /// Typically LINQ and SQL return data as collections.
        /// Hence we start the example by creating collections representing the x-axis labels and the y-axis values
        /// However, chart.js expects data as a string, not as a collection.
        ///   Hence we join the elements in the collections into strings in the view model
        /// </summary>
        /// <returns>View that will display the chart</returns>
        public ViewResult DemoChart()
        {
            string[] ChartLabels = new string[] { "Africa", "Asia", "Europe", "Latin America", "North America" };
            int[] ChartData = new int[] { 2478, 5267, 734, 784, 433 };

            ChartModel Model = new ChartModel
            {
                ChartType = "bar",
                Labels = String.Join(",", ChartLabels.Select(d => "'" + d + "'")),
                Data = String.Join(",", ChartData.Select(d => d)),
                Title = "Predicted world population (millions) in 2050"
            };

            return View(Model);
        }
    }
}

//====================================================

//The root object, will basically soemthing that is akela, doesn't have a reference, but is present on the page/jason
