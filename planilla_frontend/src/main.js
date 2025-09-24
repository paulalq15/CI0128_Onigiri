import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css'
import { createRouter, createWebHistory } from 'vue-router';
import LoginForm from './components/LoginForm.vue';
import HomePage from './components/HomePage.vue';
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
    // Si no está logueado, a login
    return next({ name: "Login" });
  }

  if (to.name === "Login" && authed) {
    // Si ya está logueado y va al login, reenvía al Home
    return next({ name: "Home Page" });
  }

  next();
});

createApp(App).use(router).mount("#app");
