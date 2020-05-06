﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Raunstrup.DataAccess.Model
{
    public class WorkingHours
    {
        public int Id { get; set; }

        public int Amount { get; set; }

        public int EmployeeId { get; set; }

        public decimal HourlyPrice { get; set; }

        public int ProjectId { get; set; }
       
        public Employee Employee { get; set; }

        public byte[] Rowversion { get; set; }
    }
}
