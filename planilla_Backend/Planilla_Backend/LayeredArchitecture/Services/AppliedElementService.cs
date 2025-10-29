using Planilla_Backend.Models;
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

        public async void addAppliedElement(AppliedElement newAppliedElement) {
            await this.appliedElementRepository.addAppliedElement(newAppliedElement);
        }

        public void deactivateAppliedElement(AppliedElement appliedElement)
        {
            this.appliedElementRepository.deactivateAppliedElement(appliedElement);
        }
    }
}
