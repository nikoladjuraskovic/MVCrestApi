using Microsoft.Ajax.Utilities;
using MVCrestAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Text.Json;
using System.Web.UI.WebControls;
using System.Xml;
using RestSharp;
using Newtonsoft.Json.Bson;
using System.Text.Json.Serialization;

namespace MVCrestAPI.Controllers
{
    public class EmployeeController : Controller
    {

        EmployeeContext db = new EmployeeContext();

        // GET: Employee
        public ActionResult Index()
        {

            DbSet<Employee> EmployeesData = null;
            List<Employee> Employees = null;

            string apiData = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(apiData);

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = httpClient.GetAsync(apiData).Result;

            string data;

            if(responseMessage.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("Succes: " + responseMessage.StatusCode);

                data = responseMessage.Content.ReadAsStringAsync().Result;

                System.Diagnostics.Debug.WriteLine("JSON data: " + data);




                //System.Diagnostics.Debug.WriteLine("Data: " + data);

                //EmployeesData = JsonConvert.DeserializeObject<DbSet<Employee>>(data);

                //EmployeesData = JsonSerializer.Deserialize<Employee>(data);

                //TextReader textReader = new StringReader(data);


                string pathName = Server.MapPath("~/EmployeesData/Employees.json");


                //JsonTextReader reader = new JsonTextReader(new StringReader(data));

                //JsonReader reader = new JsonTextReader(new StringReader(data));


                /*
                JArray jArray = JArray.Parse(data);
                
                foreach (JObject item in jArray)
                {
                    System.Diagnostics.Debug.Write(item.ToString());
                }
                */

                /*

                  var employeesData = new List<Dictionary<string, object>>();
                  var zaposleni = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(reader);

                  */



                string id = "";
                string EmployeeName = "";
                string StartTime = "";
                string EndTime = "";
                string EntryNotes = "";
                int? DeletedOn;
                int counter = 0;


                /* 
                 XmlDocument doc = new XmlDocument();


                 //throws exception: XmlNodeConverter can only convert JSON that begins with an object. Path '', line 1, position 195001.
                 // JSon format is not valid maybe, visual studio 2022 issues warning in .json file
                 doc = JsonConvert.DeserializeXmlNode(data);



                 System.Diagnostics.Debug.WriteLine(doc.ToString());


                 */


                /*
                                var jsonSettings = new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    MissingMemberHandling = MissingMemberHandling.Ignore
                                };




                                //throws Exception: The JSON value could not be converted to System.Collections.Generic.IList`1[MVCrestAPI.Models.Employee].
                                // doesn't work: https://www.tutorialsteacher.com/articles/convert-json-string-to-object-in-csharp
                                var emplList = System.Text.Json.JsonSerializer.Deserialize<IList<Employee>>(data).Where(cd => cd.DeletedOn != null);


                                foreach (var employee in emplList)
                                {
                                    System.Diagnostics.Debug.WriteLine("Department Id is: {0}", employee.Id);
                                    System.Diagnostics.Debug.WriteLine("Department Name is: {0}", employee.EmployeeName);
                                }
                */


                /*we are writing json data from the api to a .json file*/

                string apiDataPath = Server.MapPath("~/EmployeesData/ApiData.json");
                string validJsonPath = Server.MapPath("~/EmployeesData/validJson.json");


                StreamWriter writer = new StreamWriter(apiDataPath);

                writer.Write(data);

                writer.Close();


                StreamReader citac = new StreamReader(apiDataPath);
                JsonReader reader = null;
                string line = "";
                while (citac.Read() != -1)
                {
                    line = citac.ReadLine();
                    reader = new JsonTextReader(new StringReader(line));
                    System.Diagnostics.Debug.WriteLine($"New line: {line}");
                }


                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                      
                };
               
                
                //Throws Exception, invalid property name "\"
                var emplList = System.Text.Json.JsonSerializer.Deserialize<IList<Employee>>(line, options);
                System.Diagnostics.Debug.WriteLine(emplList);

                while (reader.Read())
                    {
                    System.Diagnostics.Debug.WriteLine("Read Json token");
                    //TODO: treats data like a string instead of multiple json objects. Why??
                    // should read json properties one by one but it doesn't
                    System.Diagnostics.Debug.WriteLine("Json Token: " + reader.Value);
                    System.Diagnostics.Debug.WriteLine("Json Token Type: " + reader.TokenType);

                   /*
                    switch(reader.TokenType)
                    {
                        
                        case JsonToken.PropertyName:

                    */
                            if (reader.Value.ToString() == "Id")
                            {
                        System.Diagnostics.Debug.WriteLine("Json Token ID: " + reader.Value);
                                id = reader.ReadAsString();                       
                            } else if(reader.Value.ToString() == "EmployeeName")
                            {
                                EmployeeName = reader.ReadAsString();
                            } else if (reader.Value.ToString() == "StarTimeUtc")
                            {
                                StartTime = reader.ReadAsString();
                            } else if ((reader.Value.ToString() == "EndTimeUtc"))
                            {
                                EndTime = reader.ReadAsString();
                            } else if ((reader.Value.ToString() == "EntryNotes"))
                            {
                                EntryNotes = reader.ReadAsString();
                            } else if ((reader.Value.ToString() == "DeletedOn"))
                            {
                                DeletedOn = reader.ReadAsInt32();

                                Employee employee = new Employee();
                                employee.Id = id;
                                employee.EmployeeName = EmployeeName;
                                employee.StarTimeUtc = StartTime;
                                employee.EndTimeUtc = EndTime;
                                employee.EntryNotes = EntryNotes;
                                employee.DeletedOn = DeletedOn;

                                employee.DaysWorked = TotalTimeWorkedDays(StartTime, EndTime);

                                db.Employees.Add(employee);

                                System.Diagnostics.Debug.WriteLine("Added new Employee");
                            }

                            
                    //}
                }

                
            }

            db.SaveChanges();

            /*Chart image*/
            Chart chart = new Chart(200, 200);

            chart.AddTitle("Employee work time by day");

            foreach(var entity in db.Employees)
            {
                chart.AddSeries(entity.EmployeeName, "Pie");
                
            }

            chart.ToWebImage("png");

            return View(db.Employees.ToList());
        }


        //diference in days
        static int TotalTimeWorkedDays(string startTime, string endTime)
        {
            DateTime startDate = DateTime.Parse(startTime);
            DateTime endDate = DateTime.Parse(endTime);

            var razlika = endDate - startDate;

            int daniRazlika = razlika.Days;

            return daniRazlika;
        }
    }
}