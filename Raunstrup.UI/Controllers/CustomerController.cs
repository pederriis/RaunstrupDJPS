﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Raunstrup.UI.Data;
using Raunstrup.UI.Models;
using Raunstrup.UI.Services;
using Raunstrup.Contract.Services;
using Raunstrup.Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Raunstrup.DataAccess;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace Raunstrup.UI.Controllers
{
    [Authorize(Roles = "Admin,SuperUser")]
    public class CustomerController : Controller
    {
        private readonly ViewModelContext _context;
        private readonly ICustomerService _customerService;
        private readonly IProjectService _projectService;

        public CustomerController(ViewModelContext context, ICustomerService customerService, IProjectService projectService)
        {
            _context = context;
            _customerService = customerService;
            _projectService = projectService;
        }

        // GET: Customer
      

        public async Task<IActionResult> Index(string searchString)
        {

            IEnumerable<CustomerDto> customerDtos = await _customerService.GetChosenCustomers(searchString);

            return View(CustomerMapper.Map(customerDtos));
          
        }


        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerViewModel = await _customerService.GetCustomerAsync(id.Value).ConfigureAwait(false);
            

            if (customerViewModel == null)
            {
                return NotFound();
            }

            CustomerDetailsViewModel customerDetailsViewModel = CustomerDetailsMapper.Map(customerViewModel);

            
            IEnumerable<ProjectDto> Projects = await _projectService.GetProjectsByCustomerId(id.Value);

          
            customerDetailsViewModel.Projects = ProjectMapper.Map(Projects);

            return View(customerDetailsViewModel);
        }

        // GET: Customer/Create
        public async Task<IActionResult> Create()
        {
            
            CECustomerViewModel cECustomerViewModel = new CECustomerViewModel();

           var customerDiscountTypeDtos = await _customerService.GetAllCustomerDiscountType().ConfigureAwait(false);

            IEnumerable<CustomerDiscountTypeViewModel> customerDiscountTypeViewModels= CustomerMapper.Map(customerDiscountTypeDtos);

            cECustomerViewModel.CustomerDiscountTypeViewModels = customerDiscountTypeViewModels.ToList();

            return View(cECustomerViewModel);
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,Address,Email,Active,Rowversion, SelectedCustomerDiscountViewModel")] CECustomerViewModel cEcustomerViewModel)
        {
            CustomerViewModel customerViewModel = CustomerMapper.Map(cEcustomerViewModel);

        
           
            if (ModelState.IsValid)

            {

                try
                {
                    await _customerService.AddAsync(CustomerMapper.Map(customerViewModel)).ConfigureAwait(false);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception)
                {

                    var dbcustomer = CustomerMapper.Map((customerViewModel));
                    var customerDiscountTypeDtos = await _customerService.GetAllCustomerDiscountType().ConfigureAwait(false);

                    IEnumerable<CustomerDiscountTypeViewModel> customerDiscountTypeViewModels = CustomerMapper.Map(customerDiscountTypeDtos);


                    cEcustomerViewModel.CustomerDiscountTypeViewModels = customerDiscountTypeViewModels.ToList();

                    ModelState.AddModelError(string.Empty, "Email eller Telefonnummer er ikke unikt");
                    return View(cEcustomerViewModel);
                }
            }
            return View(customerViewModel);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var customerViewModel = await _customerService.GetCustomerAsync(id).ConfigureAwait(false);
            customerViewModel.Id = id;

            CECustomerViewModel cECustomerViewModel = CustomerMapper.MaptoCE(customerViewModel);
            var customerDiscountTypeDtos = await _customerService.GetAllCustomerDiscountType().ConfigureAwait(false);
            IEnumerable<CustomerDiscountTypeViewModel> customerDiscountTypeViewModels = CustomerMapper.Map(customerDiscountTypeDtos);

            cECustomerViewModel.CustomerDiscountTypeViewModels = customerDiscountTypeViewModels.ToList();

            if (customerViewModel == null)
            {
                return NotFound();
            }
            return View(cECustomerViewModel);

        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Address,Email,Active,Rowversion,SelectedCustomerDiscountViewModel")] CECustomerViewModel cEcustomerViewModel)
        {
           

            if (id != cEcustomerViewModel.Id)
            {
                return NotFound();
            }
            CustomerViewModel customerViewModel = CustomerMapper.Map(cEcustomerViewModel);
            
            if (ModelState.IsValid)
            {
                try
                {

                    await _customerService.UpdateAsync(id, CustomerMapper.Map(customerViewModel)).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException dbu)
                {
                   
                    
                        var dbcustomer = CustomerMapper.Map((CustomerDto)dbu.Data["dbvalue"]);
                        if(cEcustomerViewModel.Phone != dbcustomer.Phone) 
                        {
                            ModelState.AddModelError("Phone", "telefonnummeret er opdateret af en anden person");
                        }
                    if (cEcustomerViewModel.Name != dbcustomer.Name)
                    {
                        ModelState.AddModelError("Name", "Navnet er opdateret af en anden person");
                    }
                    if (cEcustomerViewModel.Address != dbcustomer.Address)
                    {
                        ModelState.AddModelError("Address", "Addressen er opdateret af en anden person");
                    }
                    if (cEcustomerViewModel.Email != dbcustomer.Email)
                    {
                        ModelState.AddModelError("Email", "E-mailen er opdateret af en anden person");
                    }
                    if (cEcustomerViewModel.CustomerDiscountType != dbcustomer.CustomerDiscountType)
                    {
                        ModelState.AddModelError("SelectedCustomerDiscountViewModel", "Kundetypen er opdateret af en anden person");
                    }

                    var customerDiscountTypeDtos = await _customerService.GetAllCustomerDiscountType().ConfigureAwait(false);
                    IEnumerable<CustomerDiscountTypeViewModel> customerDiscountTypeViewModels = CustomerMapper.Map(customerDiscountTypeDtos);

                    cEcustomerViewModel.CustomerDiscountTypeViewModels = customerDiscountTypeViewModels.ToList();

                    cEcustomerViewModel.CustomerDiscountType = dbcustomer.CustomerDiscountType;
                    ModelState.AddModelError(string.Empty, "Denne kunde er blevet opdateret af en anden bruger, tryk gem for at overskrive");
                    cEcustomerViewModel.Rowversion = dbcustomer.Rowversion;
                    ModelState.Remove("Rowversion");
                    return View("Edit",cEcustomerViewModel);
                }
            }
            return View(cEcustomerViewModel);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerAsync(id.Value).ConfigureAwait(false);
              //  .FirstOrDefaultAsync(m => m.Id == id);
            if (customer== null)
            {
                return NotFound();
            }

            return View(CustomerMapper.Map(customer));
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _customerService.RemoveAsync(id).ConfigureAwait(false);

            return RedirectToAction(nameof(Index));

            //var customerViewModel = await _context.customers.FindAsync(id);

            //_context.customers.Remove(customerViewModel);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool CustomerViewModelExists(int id)
        {
            return _context.customers.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AddProjectCustomer(int id, string searchString)
        {
            IEnumerable<CustomerDto> customerDtos = await _customerService.GetChosenCustomers(searchString);

            return View(CustomerMapper.Map(customerDtos));

         

        }
        public async Task<ActionResult> AddProjectCustomerToProject(int id, int projectid)
        {
            if (ModelState.IsValid)
            {
                 await _customerService.AddAsync(id, projectid).ConfigureAwait(false);

            }

          
            return RedirectToAction("Details", "Project", new { id = projectid });

       
            
        }
     
    }
}
