﻿using Raunstrup.Contract;
using Raunstrup.Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Raunstrup.UI.Models
{
    public static class ProjectDetailsMapper
    {

        public static ProjectDetailsViewModel Map(ProjectDto dto)
        {

            return new ProjectDetailsViewModel
            {
                Id = dto.Id,
                Active = dto.Active,
                Description = dto.Description,
                EndDate = dto.EndDate,
                IsAccepted = dto.IsAccepted,
                IsDone = dto.IsDone,
                IsFixedPrice = dto.IsFixedPrice,
                Price = dto.Price,
                StartDate = dto.StartDate,
                Rowversion = dto.Rowversion,
                CustomerId = dto.CustomerId,
                Customer = CustomerMapper.Map(dto.CustomerDto),

                WorkingHours = WorkingHoursMapper.Map(dto.WorkingHoursDtos).ToList(),
                ProjectDrivings = ProjectDrivingMapper.Map(dto.ProjectDrivingDtos).ToList(),
                ProjectEmployees = ProjectEmployeeMapper.Map(dto.ProjectEmployeeDtos).ToList(),

                //used items summed up
                UsedItems = ProjectUsedItemMapper.Map(dto.UsedItemsDtos).GroupBy(PUI => PUI.ItemId)
                    .Select(PUI => new ProjectUsedItemViewModel
                    {
                        Amount = PUI.Sum(c => c.Amount),
                        Item = PUI.First().Item,
                        Price = PUI.First().Price,
                        ProjectId = PUI.First().ProjectId
                    }).ToList()

            };
        }

        public static IEnumerable<ProjectDetailsViewModel> Map(IEnumerable<ProjectDto> model)
        {
            return model.Select(x => Map(x)).AsEnumerable();
        }

        public static ProjectDto Map(ProjectDetailsViewModel project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Active = project.Active,
                Description = project.Description,
                EndDate = project.EndDate,
                IsAccepted = project.IsAccepted,
                IsDone = project.IsDone,
                IsFixedPrice = project.IsFixedPrice,
                Price = project.Price,
                StartDate = project.StartDate,
                Rowversion = project.Rowversion,
                CustomerId = project.CustomerId,
                //UsedItemsDtos = ProjectMapper.Map(project.UsedItems).ToList()
                ProjectEmployeeDtos = ProjectEmployeeMapper.Map(project.ProjectEmployees).ToList(),
                CustomerDto = CustomerMapper.Map(project.Customer)
            };
        }
    }
}
