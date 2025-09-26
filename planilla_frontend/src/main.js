import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css'
import { createRouter, createWebHistory } from 'vue-router';
import LoginForm from './components/LoginForm.vue';
import WebPageLayout from './layouts/WebPageLayout.vue';
import HomePage from './components/HomePage.vue';
import RegisterPage from './components/RegisterPage.vue';
import CreateCompany from './components/CreateCompany.vue';
import { isAuthed } from "./session";

const router = createRouter({
    history: createWebHistory(),
    routes: [
          //public
      {
        path: '/auth',
        component: AuthLayout,
        children: [
          { path: 'Login', name: 'Login', component: LoginForm },
          { path: 'Register', name: 'RegisterAccount', component: RegisterPage }
        ]
      },

      //private
      {
        path: '/app',
        component: WebPageLayout,
        meta: { requiresAuth: true },
        children: [
          { path: 'Home', name: 'Home Page', component: HomePage },
          { path: 'CrearEmpresa', name: 'Crear Empresa', component: CreateCompany }
        ]
      },

      // redirects
      { path: '/', redirect: '/auth/Login' },
      { path: '/:pathMatch(.*)*', redirect: '/auth/Login' }
    ]
});

router.beforeEach((to, from, next) => {
  const authed = isAuthed();

  if (to.meta.requiresAuth && !authed) {
    // Si no esta logueado, a login
    return next({ name: "Login" });
  }

  if (to.name === "Login" && authed) {
    // Si ya esta logueado y va al login, reenvia al Home
    return next({ name: "Home Page" });
  }

  next();
});

const app = createApp(App);
app.use(router);
app.use(createPinia());
app.mount('#app');
