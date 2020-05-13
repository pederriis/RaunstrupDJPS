﻿using Raunstrup.Contract;
using Raunstrup.Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Raunstrup.UI.Models
{
    public static class CustomerMapper
    {

        public static CustomerViewModel Map(CustomerDto dto)
        {
            if (dto == null)
                return null;
            return new CustomerViewModel
            { Id=dto.Id, Name=dto.Name, Phone=dto.Phone, Address=dto.Address, Email=dto.Email, Active=dto.Active, Rowversion=dto.Rowversion, CustomerDiscountType=CustomerDiscountTypeMapper.Map( dto.CustomerDiscountType)};
        }

        public static IEnumerable<CustomerViewModel> Map(IEnumerable<CustomerDto> dtos)
        {
            return dtos.Select(x => Map(x)).AsEnumerable();
        }

        public static CustomerDto Map(CustomerViewModel view)
        {
            if (view == null)
                return null;
            return new CustomerDto
            { Id = view.Id, Name = view.Name, Phone = view.Phone, Address = view.Address, Email = view.Email, Active = view.Active, Rowversion = view.Rowversion, CustomerDiscountType=CustomerDiscountTypeMapper.Map( view.CustomerDiscountType) };
        }

        public static CustomerDiscountTypeViewModel Map(CustomerDiscountTypeDto dto)
        {
            if (dto == null)
                return null;
            return new CustomerDiscountTypeViewModel
            { Id = dto.Id, Name = dto.Name };
        }


        public static IEnumerable<CustomerDiscountTypeViewModel> Map(IEnumerable<CustomerDiscountTypeDto> dtos)
        {
            return dtos.Select(x => Map(x)).AsEnumerable();
        }

        //public static IEnumerable< CECustomerViewModel> Map(IEnumerable< CustomerDiscountTypeDto> dto)
        //{
        //    return dto.Select(x => Map(x)).AsEnumerable();
        //}

    }
}
