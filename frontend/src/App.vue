<template>
  <div id="app" class="app-container">
    <NavBar  />
    
    <main :class="['main-content', { 'no-nav': !isAuthenticated }]">
      <router-view />
    </main>
    
    <footer  class="app-footer">
      <div class="footer-content">
        <div class="footer-left">
          <span class="footer-logo">Project Manager</span>
          <span class="footer-divider">|</span>
          <span class="footer-text">Управление проектами</span>
        </div>
        <div class="footer-right">
          &copy; {{ currentYear }} Все права защищены
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useAuthStore } from './store/auth';
import NavBar from './components/NavBar.vue';

const auth = useAuthStore();
const isAuthenticated = computed(() => auth.isAuthenticated);
const currentYear = new Date().getFullYear();
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  background-attachment: fixed;
  color: #1a202c;
  line-height: 1.6;
}

.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.main-content {
  flex: 1;
  padding: 2rem;
  max-width: 1400px;
  width: 100%;
  margin: 0 auto;
}

.main-content.no-nav {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
}

.app-footer {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-top: 1px solid rgba(0, 0, 0, 0.1);
  padding: 1.5rem 0;
  margin-top: auto;
}

.footer-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.footer-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.footer-logo {
  font-weight: 700;
  color: #667eea;
}

.footer-divider {
  color: #cbd5e0;
}

.footer-text {
  color: #718096;
  font-size: 0.9rem;
}

.footer-right {
  color: #718096;
  font-size: 0.9rem;
}

@media (max-width: 768px) {
  .main-content {
    padding: 1rem;
  }
  
  .footer-content {
    flex-direction: column;
    gap: 0.5rem;
    text-align: center;
  }
  
  .footer-left {
    flex-direction: column;
    gap: 0.25rem;
  }
  
  .footer-divider {
    display: none;
  }
}
</style>
