import axios from 'axios';

const api = axios.create({
  baseURL:
    'https://planillabackend20251121185053-dyhtgeh6hxdtg9hw.canadacentral-01.azurewebsites.net/',
  headers: { 'Content-Type': 'application/json' },
});

export default api;
