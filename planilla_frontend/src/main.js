import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'bootstrap-icons/font/bootstrap-icons.css';
import { createApp } from 'vue';
import { createPinia } from 'pinia';
import { createRouter, createWebHistory } from 'vue-router';
import { isAuthed, getUser } from './session';
import { useSession } from './utils/useSession'
import App from './App.vue';
import VueApexCharts from 'vue3-apexcharts';
import AuthLayout from './layouts/AuthLayout.vue';
import RegisterPage from './components/RegisterPage.vue';
import LoginForm from './components/LoginForm.vue';
import WebPageLayout from './layouts/WebPageLayout.vue';
import HomePage from './components/HomePage.vue';
import ViewCompanies from './components/ViewCompanies.vue';
import CreateCompany from './components/CreateCompany.vue';
import ModifyCompany from './components/ModifyCompany.vue';
import ViewEmployees from './components/ViewEmployees.vue';
import CreateEmployee from './components/CreateEmployee.vue';
import ModifyEmployees from './components/ModifyEmployees.vue';
import CreateTimesheets from './components/CreateTimesheets.vue';
import CreatePayroll from './components/CreatePayroll.vue';
import ViewPayrollElement from './components/ViewPayrollElement.vue';
import ModifyPayrollElement from './components/ModifyPayrollElement.vue';
import CreatePayrollElement from './components/CreatePayrollElement.vue';
import AssignDeduction from './components/AssignDeduction.vue';
import SelectBenefit from './components/SelectBenefit.vue';
import ViewReports from './components/ViewReports.vue';
import ActivationAccountPage from './components/ActivationAccountpage.vue';
import ResendActivationAccountPage from './components/ResendActivationAccountPage.vue';
import ForbiddenPage from './components/ForbiddenPage.vue';
import EmployeeActivation from './components/EmployeeActivation.vue';
import ModifyProfile from './components/ModifyProfile.vue';

const employerOnly = { requiresAuth: true, roles: ['Empleador'] }
const employeeOnly = { requiresAuth: true, roles: ['Empleado'] }
const employerOrAdmin = { requiresAuth: true, roles: ['Empleador','Administrador'] }
const employerOrApprover = { requiresAuth: true, roles: ['Empleador','Aprobador'] }
const employerApproverEmployee = { requiresAuth: true, roles: ['Empleador','Aprobador','Empleado'] }

const router = createRouter({
  history: createWebHistory(),
  routes: [
    //public
    {
      path: '/auth',
      component: AuthLayout,
      children: [
        { path: 'Login', name: 'Login', component: LoginForm },
        { path: 'Register', name: 'RegisterAccount', component: RegisterPage },
        { path: 'ActivateAccount', name: 'ActivateAccount', component: ActivationAccountPage },
        { path: 'ResendActivationAccount', name: 'ResendActivation', component: ResendActivationAccountPage},
        { path: 'EmployeeActivation', name: 'EmployeeActivation', component: EmployeeActivation }
      ]
    },

    //private
    {
      path: '/app',
      component: WebPageLayout,
      meta: { requiresAuth: true },
      children: [
        { path: 'Home', name: 'Home Page', component: HomePage },
        { path: 'Empresas/VerEmpresas', name: 'Ver Empresas', component: ViewCompanies, meta: employerOrAdmin },
        { path: 'Empresas/CrearEmpresa', name: 'Crear Empresa', component: CreateCompany, meta: employerOnly },
        { path: 'Empresas/ModificarEmpresa', name: 'Modificar Empresa', component: ModifyCompany, meta: employerOnly },
        { path: 'Empleados/VerEmpleados', name: 'Ver Empleados', component: ViewEmployees, meta: employerOrApprover},
        { path: 'Empleados/CrearEmpleado', name: 'Crear Empleado', component: CreateEmployee, meta: employerOnly },
        { path: 'Empleados/ModificarEmpleado', name: 'ModifyEmployees', component: ModifyEmployees, meta: employerApproverEmployee},
        { path: 'Empleados/ModificarPerfil', name: 'Modificar Datos', component: ModifyProfile, meta: employerApproverEmployee},
        { path: 'Timesheets/CrearTimesheets', name: 'Crear Timesheets', component: CreateTimesheets, meta: employeeOnly },
        { path: 'Planilla/CrearPlanilla', name: 'Crear Planilla', component: CreatePayroll, meta: employerOrAdmin },
        { path: 'Planilla/VerBeneficiosDeducciones', name: 'Ver Beneficios y Deducciones', component: ViewPayrollElement},
        { path: 'Planilla/CrearBeneficiosDeducciones', name: 'Crear Beneficios y Deducciones', component: CreatePayrollElement, meta: employerOnly },
        { path: 'Planilla/EditarBeneficioDeduccion/:PEId', name: 'EditarBeneficioODeduccion', component: ModifyPayrollElement, meta: employerOnly },
        { path: 'Planilla/AsignarDeducciones', name: 'Asignar Deducciones', component: AssignDeduction, meta: employerOrApprover },
        { path: 'Planilla/SeleccionarBeneficios', name: 'Seleccionar Beneficios', component: SelectBenefit, meta: employerApproverEmployee },
        { path: 'Reportes/VerReportes', name: 'Ver Reportes', component: ViewReports, meta: employerApproverEmployee },
        { path: 'AccesoRestringido', name: 'AccesoRestringido', component: ForbiddenPage },
      ]
    },

    // redirects
    { path: '/', redirect: '/auth/Login' },
    { path: '/:pathMatch(.*)*', redirect: '/auth/Login' }
  ],
});

router.beforeEach((to, from, next) => {
  const authed = isAuthed();
  const requiresAuth = to.matched.some(r => r.meta?.requiresAuth);
  const allowedRoles = to.matched.flatMap(r => r.meta?.roles ?? []);

  if (requiresAuth && !authed) return next({ name: 'Login' })
  if (to.name === 'Login' && authed) return next({ name: 'Home Page' })

  if (allowedRoles.length > 0) {
    const u = getUser();
    const usrRoles = Array.isArray(u?.typeUser) ? u.typeUser : [u?.typeUser].filter(Boolean);
    const allow = new Set(allowedRoles.map(x => String(x).toLowerCase()));
    const ok = usrRoles.some(r => allow.has(String(r).toLowerCase()));
    if (!ok) return next({ name: 'AccesoRestringido' }); 
  }

  next()
});

const app = createApp(App);
const session = useSession()

app.config.globalProperties.$session = session
app.use(router);
app.use(createPinia());
app.use(VueApexCharts);
app.component('ApexChartComponent', VueApexCharts);
app.mount('#app');