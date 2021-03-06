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
using System.Security.Claims;
using Microsoft.CodeAnalysis;
using Raunstrup.Contakt.Service.Interface;
using System.IO;

namespace Raunstrup.UI.Controllers
{
    [Authorize(Roles = "SuperUser,User")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IPDFService _PDFService;
        private readonly IContactService _contactService;

        public ProjectController(IProjectService projectService, IPDFService pdfService, IContactService contactService)
        {
            _projectService = projectService;
            _PDFService = pdfService;
            _contactService = contactService;
        }

        // GET: Project
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                var projectDtos = await _projectService.GetProjectAsync(userId, userRole).ConfigureAwait(false);
                return View(ProjectMapper.Map(projectDtos));
            }
            catch (Exception)
            {
                ErrorViewModel model = new ErrorViewModel { RequestId = "Projekterne blev ikke fundet!" };
                return View("Error", model);
            }

        }

        // GET: Project/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var projectViewModel = await _projectService.GetProjectAsync(id.Value).ConfigureAwait(false);

                if (projectViewModel == null)
                {
                    return NotFound();
                }

                return View(ProjectDetailsMapper.Map(projectViewModel));
            }
            catch (Exception)
            {
                ErrorViewModel model = new ErrorViewModel { RequestId = "Projektet blev ikke fundet!" };
                return View("Error", model);
            }

        }

        [Authorize(Roles = "SuperUser")]
        // GET: Project/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "SuperUser")]
        // POST: Project/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,Price,Description,Active,IsFixedPrice,IsAccepted,IsDone,Rowversion,ESTdriving")] ProjectViewModel projectViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _projectService.AddAsync(ProjectMapper.Map(projectViewModel)).ConfigureAwait(false);

                    return RedirectToAction(nameof(Index));

                }
                return View(projectViewModel);
            }
            catch (Exception)
            {
                ErrorViewModel model = new ErrorViewModel { RequestId = "Projektet blev ikke oprettet!" };
                return View("Error", model);
            }
        }

        [Authorize(Roles = "SuperUser")]
        // GET: Project/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var projectViewModel = await _projectService.GetProjectAsync(id).ConfigureAwait(false);
                if (projectViewModel == null)
                {
                    return NotFound();
                }
                return View(ProjectMapper.Map(projectViewModel));
            }
            catch (Exception)
            {
                ErrorViewModel model = new ErrorViewModel { RequestId = "Projektet blev ikke fundet!" };
                return View("Error", model);
            }
        }

        [Authorize(Roles = "SuperUser")]
        // POST: Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Price,Description,Active,IsFixedPrice,IsAccepted,IsDone,ESTdriving,Rowversion")] ProjectViewModel projectViewModel)
        {
            if (id != projectViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _projectService.UpdateAsync(id, ProjectMapper.Map(projectViewModel)).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException dbu)
                {
                    var dbProject = ProjectMapper.Map((ProjectDto)dbu.Data["dbvalue"]);

                    if (projectViewModel.StartDate != dbProject.StartDate)
                    {
                        ModelState.AddModelError("StartDate", "Start Dagen er opdateret af en anden person");
                    }

                    if (projectViewModel.EndDate != dbProject.EndDate)
                    {
                        ModelState.AddModelError("EndDate", "Slut Dagen er opdateret af en anden person");
                    }

                    if (projectViewModel.Price != dbProject.Price)
                    {
                        ModelState.AddModelError("Price", "Prisen er er opdateret af en anden person");
                    }

                    if (projectViewModel.ESTdriving != dbProject.ESTdriving)
                    {
                        ModelState.AddModelError("ESTdriving", "Den Estimeret Kørsel er opdateret af en anden person");
                    }

                    if (projectViewModel.Description != dbProject.Description)
                    {
                        ModelState.AddModelError("Description", "Beskrivelsen er opdateret af en anden person");
                    }

                    if (projectViewModel.IsFixedPrice != dbProject.IsFixedPrice)
                    {
                        ModelState.AddModelError("IsFixedPrice", "Fast pris er opdateret af en anden person");
                    }

                    if (projectViewModel.IsAccepted != dbProject.IsAccepted)
                    {
                        ModelState.AddModelError("IsAccepted", "Accepteret er blevet opdateret af en anden person");
                    }

                    if (projectViewModel.IsDone != dbProject.IsDone)
                    {
                        ModelState.AddModelError("IsDone", "Projectets Status er opdateret af en anden person");
                    }

                    ModelState.AddModelError(string.Empty, "Denne kunde er blevet opdateret af en anden bruger, tryk gem for at overskrive");
                    projectViewModel.Rowversion = dbProject.Rowversion;
                    ModelState.Remove("Rowversion");
                    return View("Edit", projectViewModel);
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Noget gik galt");
                    return View("Edit", projectViewModel);
                }
            }
            return View(projectViewModel);
        }

        [Authorize(Roles = "SuperUser")]
        // GET: Project/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var projectViewModel = await _projectService.GetProjectAsync(id.Value).ConfigureAwait(false);
                if (projectViewModel == null)
                {
                    return NotFound();
                }

                return View(ProjectMapper.Map(projectViewModel));
            }
            catch (Exception)
            {
                ErrorViewModel model = new ErrorViewModel { RequestId = "Projektet blev ikke fundet!" };
                return View("Error", model);
            }
        }

        [Authorize(Roles = "SuperUser")]
        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _projectService.RemoveAsync(id).ConfigureAwait(false);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ErrorViewModel model = new ErrorViewModel { RequestId = "Projektet blev ikke slettet!" };
                return View("Error", model);
            }
        }

        //private bool ProjectViewModelExists(int id)
        //{
        //    return _context.Projects.Any(e => e.Id == id);
        //}
        public async Task<IActionResult> CreatePDF(int id)
        {
            try
            {
                var projectViewModel = await _projectService.GetProjectAsync(id).ConfigureAwait(false);
                _PDFService.CreatePDF(ProjectDetailsMapper.MapToDetailsDto(projectViewModel));
                return RedirectToAction("Index");
            }
            catch
            {
                ErrorViewModel model = new ErrorViewModel { RequestId = "PDF'en kunne ikke laves!" };
                return View("Error", model);
            }
        }

        public async Task<IActionResult> SendPDF(int id)
        {
            try
            {
                var projectViewModel = await _projectService.GetProjectAsync(id);
                string pDFOffer = _PDFService.CreatePDF(ProjectDetailsMapper.MapToDetailsDto(projectViewModel));
                _contactService.SendOffer(pDFOffer, projectViewModel.CustomerDto.Email);
                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception)
            {
                ErrorViewModel model = new ErrorViewModel { RequestId = "Tilbuddet blev ikke sendt!" };
                return View("Error", model);
            }
        }
    }
}




