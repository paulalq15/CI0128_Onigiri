import { createApp } from 'vue';
import App from "./App.vue";
import { createRouter, createWebHistory } from "vue-router";
import CountriesList from './components/CountriesList.vue';
import CountryForm from './components/CountryForm.vue';

const router = createRouter ({
  history: createWebHistory(),
  routes: [
    {path:"/", name: "home", component: CountriesList},
    {path:"/country", name: "countryForm", component: CountryForm},
  ]
})

createApp(App).use(router).mount('#app')
