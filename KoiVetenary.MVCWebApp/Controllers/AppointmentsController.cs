
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KoiVetenary.Data.Models;
using KoiVetenary.Business;
using KoiVetenary.Common;
using Newtonsoft.Json;

namespace KoiVetenary.MVCWebApp.Controllers
{
    public class AppointmentsController : Controller
    {

        public AppointmentsController()
        {
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var apiEndpoint = Const.API_Endpoint + "Appointments";
                    var response = await httpClient.GetAsync(apiEndpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<KoiVetenaryResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<List<Appointment>>(result.Data.ToString());
                            return View(data);
                        }
                    }
                    else
                    {
                        // Log the status code and response message for debugging
                        Console.WriteLine($"API call failed. Status code: {response.StatusCode}, Message: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            return View(new List<Appointment>());

        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_Endpoint + "Appointments/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var service = JsonConvert.DeserializeObject<KoiVetenaryResult>(apiResponse);

                        if (service != null && service.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<Appointment>(service.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }
            return View(new Appointment());
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AppointmentStatus"] = new List<SelectListItem>
            {
                new SelectListItem { Value = AppointmentStatus.Pending, Text = "Pending" },
                new SelectListItem { Value = AppointmentStatus.Confirmed, Text = "Confirmed" },
                new SelectListItem { Value = AppointmentStatus.InProgress, Text = "In Progress" },
                new SelectListItem { Value = AppointmentStatus.Completed, Text = "Completed" },
                new SelectListItem { Value = AppointmentStatus.Canceled, Text = "Canceled" }
            };
            ViewData["OwnerId"] = new SelectList(await GetOwners(), "OwnerId", "OwnerId");
            return View();
        }
        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,OwnerId,AppointmentDate,AppointmentTime,ContactEmail,ContactPhone,Status,SpecialRequests,Notes,TotalEstimatedDuration,TotalCost")] Appointment appointment)

        {
            bool saveStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.PostAsJsonAsync(Const.API_Endpoint + "Appointments/", appointment))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<KoiVetenaryResult>(content);
                            if (result != null && result.Status == Const.SUCCESS_CREATE_CODE)
                            {
                                saveStatus = true;
                            }
                            else
                            {
                                saveStatus = false;
                            }
                        }
                    }
                }
            }
            if (saveStatus)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["AppointmentStatus"] = new List<SelectListItem>
                {
                    new SelectListItem { Value = AppointmentStatus.Pending, Text = "Pending" },
                    new SelectListItem { Value = AppointmentStatus.Confirmed, Text = "Confirmed" },
                    new SelectListItem { Value = AppointmentStatus.InProgress, Text = "In Progress" },
                    new SelectListItem { Value = AppointmentStatus.Completed, Text = "Completed" },
                    new SelectListItem { Value = AppointmentStatus.Canceled, Text = "Canceled" }
                };
                ViewData["OwnerId"] = new SelectList(await GetOwners(), "OwnerId", "OwnerId");
                return View(appointment);
            }
        }

        //GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var appointment = new Appointment();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_Endpoint + "Appointments/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<KoiVetenaryResult>(content);

                        if (result != null && result.Data != null)
                        {
                            appointment = JsonConvert.DeserializeObject<Appointment>(result.Data.ToString());
                        }
                    }
                }
            }
            ViewData["AppointmentStatus"] = new List<SelectListItem>
            {
                new SelectListItem { Value = AppointmentStatus.Pending.ToString(), Text = "Pending", Selected = (appointment.Status == AppointmentStatus.Pending.ToString()) },
                new SelectListItem { Value = AppointmentStatus.Confirmed.ToString(), Text = "Confirmed", Selected = (appointment.Status == AppointmentStatus.Confirmed.ToString()) },
                new SelectListItem { Value = AppointmentStatus.InProgress.ToString(), Text = "In Progress", Selected = (appointment.Status == AppointmentStatus.InProgress.ToString()) },
                new SelectListItem { Value = AppointmentStatus.Completed.ToString(), Text = "Completed", Selected = (appointment.Status == AppointmentStatus.Completed.ToString()) },
                new SelectListItem { Value = AppointmentStatus.Canceled.ToString(), Text = "Canceled", Selected = (appointment.Status == AppointmentStatus.Canceled.ToString()) }
            };
            ViewData["OwnerId"] = new SelectList(await GetOwners(), "OwnerId", "OwnerId", appointment.OwnerId);
            return View(appointment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,OwnerId,AppointmentDate,AppointmentTime,ContactEmail,ContactPhone,Status,SpecialRequests,Notes,TotalEstimatedDuration,TotalCost")] Appointment appointment)

        {
            bool saveStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.PutAsJsonAsync(Const.API_Endpoint + "Appointments/", appointment))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<KoiVetenaryResult>(content);
                            if (result != null && result.Status == Const.SUCCESS_CREATE_CODE)
                            {
                                saveStatus = true;
                            }
                            else
                            {
                                saveStatus = false;
                            }
                        }
                    }
                }
            }
            if (saveStatus)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["AppointmentStatus"] = new List<SelectListItem>
                {
                    new SelectListItem { Value = AppointmentStatus.Pending, Text = "Pending" },
                    new SelectListItem { Value = AppointmentStatus.Confirmed, Text = "Confirmed" },
                    new SelectListItem { Value = AppointmentStatus.InProgress, Text = "In Progress" },
                    new SelectListItem { Value = AppointmentStatus.Completed, Text = "Completed" },
                    new SelectListItem { Value = AppointmentStatus.Canceled, Text = "Canceled" }
                };
                ViewData["OwnerId"] = new SelectList(await GetOwners(), "OwnerId", "OwnerId");
                return View(appointment);
            }
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var appointment = new Appointment();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_Endpoint + "Appointments/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<KoiVetenaryResult>(content);

                        if (result != null && result.Data != null)
                        {
                            appointment = JsonConvert.DeserializeObject<Appointment>(result.Data.ToString());
                            return View(appointment);
                        }
                    }
                }
            }

            return View(new Appointment());
        }


        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleteStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.DeleteAsync(Const.API_Endpoint + "Appointments/" + id))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<KoiVetenaryResult>(content);
                            if (result != null && result.Status == Const.SUCCESS_CREATE_CODE)
                            {
                                deleteStatus = true;
                            }
                            else
                            {
                                deleteStatus = false;
                            }
                        }
                    }
                }
            }
            if (deleteStatus)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Delete));
            }
        }

        //private bool AppointmentExists(int id)
        //{
        //  return (_context.Appointments?.Any(e => e.AppointmentId == id)).GetValueOrDefault();
        //}

        private static async Task<List<Owner>> GetOwners()
        {
            var listOwners = new List<Owner>();
            //
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_Endpoint + "Owners"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var owners = JsonConvert.DeserializeObject<KoiVetenaryResult>(apiResponse);

                        if (owners != null && owners.Data != null)
                        {
                            listOwners = JsonConvert.DeserializeObject<List<Owner>>(owners.Data.ToString());
                        }
                    }
                }
            }
            return listOwners;
        }

    }
}
