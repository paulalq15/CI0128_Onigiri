namespace Planilla_Backend.Models
{
    public class PayrollElementModel
    {
        public string ElementId { get; set; }

        public string ElementName { get; set; }

        public string PaidBy { get; set; }

        public string CalculationType { get; set; }

        public string CalculationValue { get; set; }

        public bool ElementStatus { get; set; }
    }
}
