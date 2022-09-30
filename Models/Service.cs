﻿using System;
using System.Collections.Generic;

namespace Homework1.Models
{
    public partial class Service
    {
        public Service()
        {
            Requests = new HashSet<Request>();
            ServiceActivities = new HashSet<ServiceActivity>();
        }

        public long ServiceId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public string? Activities { get; set; }


        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<ServiceActivity> ServiceActivities { get; set; }
    }
}
