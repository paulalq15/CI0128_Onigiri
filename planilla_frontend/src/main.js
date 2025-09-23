import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router';
import LoginForm from './components/LoginForm.vue';

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {path: "/", name: "Home", component: LoginForm},
    ]

});

createApp(App).mount('#app')
