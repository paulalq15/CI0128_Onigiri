import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'bootstrap-icons/font/bootstrap-icons.css';
import { createApp } from 'vue';
import { createPinia } from 'pinia';
import { createRouter, createWebHistory } from 'vue-router';
import { isAuthed } from './session';
import { useSession } from './utils/useSession'
import App from './App.vue';
import LoginForm from './components/LoginForm.vue';
import WebPageLayout from './layouts/WebPageLayout.vue';
import AuthLayout from './layouts/AuthLayout.vue';
import HomePage from './components/HomePage.vue';
import RegisterPage from './components/RegisterPage.vue';
import CreateCompany from './components/CreateCompany.vue';
import CreatePayrollElement from './components/CreatePayrollElement.vue';
import ActivationAccountPage from './components/ActivationAccountpage.vue';
import ResendActivationAccountPage from './components/ResendActivationAccountPage.vue';

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
        { path: 'ResendActivationAccount', name: 'ResendActivation', component: ResendActivationAccountPage}
      ]
    },

    //private
    {
      path: '/app',
      component: WebPageLayout,
      meta: { requiresAuth: true },
      children: [
        { path: 'Home', name: 'Home Page', component: HomePage },
        { path: 'CrearEmpresa', name: 'Crear Empresa', component: CreateCompany },
        { path: 'CrearBeneficiosDeducciones', name: 'Crear Beneficios y Deducciones', component: CreatePayrollElement }
      ]
    },

    // redirects
    { path: '/', redirect: '/auth/Login' },
    { path: '/:pathMatch(.*)*', redirect: '/auth/Login' }
  ],
});

router.beforeEach((to, from, next) => {
  const authed = isAuthed();
  const needsAuth = to.matched.some(r => r.meta?.requiresAuth)

  if (needsAuth && !authed) return next({ name: 'Login' })
  if (to.name === 'Login' && authed) return next({ name: 'Home Page' })
  next()
});

const app = createApp(App);
const session = useSession()

app.config.globalProperties.$session = session
app.use(router);
app.use(createPinia());
app.mount('#app');