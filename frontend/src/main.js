import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './style.css'

const app = createApp(App)

// создаём pinia
const pinia = createPinia()

// подключаем pinia и router
app.use(pinia)
app.use(router)

app.mount('#app')