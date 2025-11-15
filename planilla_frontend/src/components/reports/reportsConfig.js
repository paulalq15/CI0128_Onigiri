// Componentes
import EmployeeReportDetailPayroll from './EmployeeReportDetailPayroll.vue';
import EmployeeReportHistoryPayroll from './EmployeeReportHistoryPayroll.vue';
import EmployerReportDetailEmployeePayroll from './EmployerReportDetailEmployeePayroll.vue';
import EmployerReportDetailPayroll from './EmployerReportDetailPayroll.vue';
import EmployerReportHistoryPayroll from './EmployerReportHistoryPayroll.vue';

export function getReportsByUserType(typeUser) {
  const employeeReports = [
    {
      id: 'detail-payroll',
      description: 'Detalle de pago de planilla',
      component: EmployeeReportDetailPayroll,
    },
    {
      id: 'history-payroll',
      description: 'Histórico de pago de planilla',
      component: EmployeeReportHistoryPayroll,
    },
  ];

  const employerReports = [
    {
      id: 'detail-payroll',
      description: 'Detalle de pago de planilla',
      component: EmployerReportDetailPayroll,
    },
    {
      id: 'history-payroll',
      description: 'Histórico de pago de planilla',
      component: EmployerReportHistoryPayroll,
    },
    {
      id: 'employee-payroll',
      description: 'Pago de planilla de empleados',
      component: EmployerReportDetailEmployeePayroll,
    },
  ];

  return typeUser === 'Empleador' ? employerReports : employeeReports;
}
