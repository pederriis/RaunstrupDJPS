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

namespace Raunstrup.UI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ViewModelContext _context;
        private readonly ICustomerService _customerService;

        public CustomerController(ViewModelContext context, ICustomerService customerService)
        {
            _context = context;
            _customerService = customerService;
        }

        // GET: Customer
      

        public async Task<IActionResult> Index(string searchString)
        {

            var customerDtos = await _customerService.GetCustomerAsync().ConfigureAwait(false);


            IEnumerable<CustomerDto> filterdCustomersDtos = _customerService.GetFilterdCustomers(customerDtos, searchString);


            return View(CustomerMapper.Map(filterdCustomersDtos));

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

            return View(CustomerMapper.Map(customerViewModel));
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,Address,Email,Active,Rowversion")] CustomerViewModel customerViewModel)
        {

            if (ModelState.IsValid)
            {
                await _customerService.AddAsync(CustomerMapper.Map(customerViewModel)).ConfigureAwait(false);

                return RedirectToAction(nameof(Index));
                //_context.Add(employeeViewModel);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return View(customerViewModel);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var customerViewModel = await _customerService.GetCustomerAsync(id).ConfigureAwait(false);
            if (customerViewModel == null)
            {
                return NotFound();
            }
            return View(CustomerMapper.Map(customerViewModel));
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Address,Email,Active,Rowversion")] CustomerViewModel customerViewModel)
        {
            if (id != customerViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _customerService.UpdateAsync(id, CustomerMapper.Map(customerViewModel)).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerViewModelExists(customerViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(customerViewModel);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerViewModel = await _context.customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerViewModel == null)
            {
                return NotFound();
            }

            return View(customerViewModel);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerViewModel = await _context.customers.FindAsync(id);
            _context.customers.Remove(customerViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerViewModelExists(int id)
        {
            return _context.customers.Any(e => e.Id == id);
        }
        public async Task<IActionResult> AddProjectCustomer(int id, string searchString)
        {
            var customerDtos = await _customerService.GetCustomerAsync().ConfigureAwait(false);

            IEnumerable<CustomerDto> filterdCustomersDtos = _customerService.GetFilterdCustomers(customerDtos, searchString);

            return View(CustomerMapper.Map(filterdCustomersDtos));
        }
        public async Task<IActionResult> AddProjectCustomerToProject(int id, int projectid)
        {
            if (ModelState.IsValid)
            {
                 await _customerService.AddAsync(id, projectid).ConfigureAwait(false);

                //_context.Add(employeeViewModel);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

            }
           
            return RedirectToAction("AddProjectCustomer", new { id = projectid });
            
        }
    }
}
