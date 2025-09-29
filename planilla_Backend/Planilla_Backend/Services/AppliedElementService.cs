﻿using Planilla_Backend.Models;
using Planilla_Backend.Repositories;
using System.Text.RegularExpressions;

namespace Planilla_Backend.Services
{
    public class AppliedElementService
    {
        private readonly AppliedElementRepository appliedElementRepository;

        public AppliedElementService()
        {
            this.appliedElementRepository = new AppliedElementRepository();
        }

        public List<AppliedElement> getAppliedElements(int employeeId)
        {
            return this.appliedElementRepository.getAppliedElements(employeeId);
        }
    }
}
