import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import { createRouter, createWebHistory } from "vue-router";
import WebPageLayout from './layouts/WebPageLayout.vue';
import HomePage from './components/HomePage.vue';
import CreateCompany from './components/CreateCompany.vue';
import CreatePayrollElement from './components/CreatePayrollElement.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: WebPageLayout,
      children: [
        { path: 'Home', name: 'Página principal', component: HomePage },
        { path: 'CrearEmpresa', name: 'Crear empresa', component: CreateCompany },
        { path: 'CrearBeneficiosDeducciones', name: 'Crear beneficios y deducciones', component: CreateCompany },
      ],
    },
  ],
});

createApp(App).use(router).mount('#app');
