import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import { createRouter, createWebHistory } from "vue-router";
import HomePage from './components/HomePage.vue';
import CreateCompany from './components/CreateCompany.vue';
import CreatePayrollElement from './components/CreatePayrollElement.vue';

const router = createRouter ({
    history: createWebHistory(),
    routes: [
        {path:"/", name: "Home Page", component: HomePage},
        {path:"/CrearEmpresa", name: "Crear Empresa", component: CreateCompany},
        {path:"/CrearElementoPlanilla", name: "Crear elemento de planilla", component: CreatePayrollElement},
    ],
});

createApp(App).use(router).mount('#app');
