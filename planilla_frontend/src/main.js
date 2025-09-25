import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css'
import { createRouter, createWebHistory } from 'vue-router';
import LoginForm from './components/LoginForm.vue';

// Templates
import HomePage from './components/HomePage.vue';
import RegisterPage from './components/RegisterPage.vue';
import CreateCompany from './components/CreateCompany.vue';
import { isAuthed } from "./session";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {path: "/", name: "Login", component: LoginForm},
        {path: "/HomePage", name: "Home Page", component: HomePage, meta: { requiresAuth: true }},
        {path:"/CrearEmpresa", name: "Crear Empresa", component: CreateCompany},
    ]
});

router.beforeEach((to, from, next) => {
  const authed = isAuthed();

  if (to.meta.requiresAuth && !authed) {
    // Si no est� logueado, a login
    return next({ name: "Login" });
  }

  if (to.name === "Login" && authed) {
    // Si ya est� logueado y va al login, reenv�a al Home
    return next({ name: "Home Page" });
  }

  next();
});

const app = createApp(App);
app.use(router);
app.use(createPinia());
app.mount('#app');
