import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'

// Router
import { createRouter, createWebHistory } from "vue-router";

// Templates
import HomePage from './components/HomePage.vue';
import RegisterPage from './components/RegisterPage.vue';

const router = createRouter ({
    history: createWebHistory(),
    routes: [
        {path:"/", name: "Home Page", component: HomePage},
        {path:"/registerAccount", name: "RegisterAccount", component: RegisterPage},
    ],
});

createApp(App).use(router).mount('#app');
