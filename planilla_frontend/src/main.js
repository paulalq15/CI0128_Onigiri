import { createApp } from 'vue'
import App from './App.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import { createRouter, createWebHistory } from "vue-router";
import HomePage from './components/HomePage.vue';

const router = createRouter ({
    history: createWebHistory(),
    routes: [
        {path:"/", name: "Home Page", component: HomePage},
    ],
});

createApp(App).use(router).mount('#app');
