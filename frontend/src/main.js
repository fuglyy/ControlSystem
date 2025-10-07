import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './style.css'
// Пример настройки интерцептора (должен быть в начале приложения)
import axios from 'axios';
import { useAuthStore } from './store/auth'; // Импортируем ваш магазин

axios.interceptors.request.use(config => {
  const authStore = useAuthStore();
  const token = authStore.token; // Убедитесь, что токен здесь хранится

  if (token) {
    // 💡 ЭТО КЛЮЧЕВОЙ МОМЕНТ
    config.headers.Authorization = `Bearer ${token}`; 
  }
  return config;
}, error => {
  return Promise.reject(error);
});


const app = createApp(App)

// создаём pinia
const pinia = createPinia()

// подключаем pinia и router
app.use(pinia)
app.use(router)

app.mount('#app')