using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



namespace MVC_Crime_Start.Models
{
    public class CrimeRoot
    {
        public int Id { get; set; }
        public CrimeResult[] results { get; set; }
        public Pagination pagination { get; set; }
        public Agency[] agency { get; set; }

    }



    public class Pagination
    {
        public int Id { get; set; }
        public int count { get; set; }
        public int page { get; set; }
        public int pages { get; set; }
        public int per_page { get; set; }
    }

    public class CrimeResult
    {
        public CrimeResult(string state_abbr, int year, int population, int arson, int homicide, int robbery)
        {
            //for now only adding a few fields and limiting the ones that would be required for quering
            this.state_abbr = state_abbr;
            this.population = population;
            this.year = year;
            this.arson = arson;
            this.homicide = homicide;
            this.robbery = robbery;
        }

        public CrimeResult(string state_abbr, int population, int year)
        {
            this.state_abbr = state_abbr;
            this.population = population;
            this.year = year;
        }

        public CrimeResult(string state_abbr, int year)
        {
            this.state_abbr = state_abbr;
            this.year = year;
        }

        public CrimeResult()
        {

        }

        public int Id { get; set; }
        public int state_id { get; set; }
        public string state_abbr { get; set; }
        public int year { get; set; }
        public int population { get; set; }
        public int violent_crime { get; set; }
        public int homicide { get; set; }
        public int rape_legacy { get; set; }
        public int? rape_revised { get; set; }
        public int robbery { get; set; }
        public int aggravated_assault { get; set; }
        public int property_crime { get; set; }
        public int burglary { get; set; }
        public int larceny { get; set; }
        public int motor_vehicle_theft { get; set; }
        public int arson { get; set; }
        public List<CrimeRoot> CrimeRoots { get; set; }
    }

    public class Agency
    {
        public int Id { get; set; }
        public string ori { get; set; }
        public int data_year { get; set; }
        public string offense { get; set; }
        public string state_abbr { get; set; }
        public int cleared { get; set; }
        public int actual { get; set; }
        public List<CrimeRoot> CrimeRoots { get; set; }

    }

}