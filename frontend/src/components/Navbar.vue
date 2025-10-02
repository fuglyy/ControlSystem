<template>
  <nav class="navbar">
    <div class="navbar-container">
      <div class="logo-container">
        <router-link to="/" class="logo">
          <div class="logo-icon-wrapper">
            <span class="logo-icon">📊</span>
          </div>
          <span class="logo-text">Control System</span>
        </router-link>
      </div>
      
      <div class="nav-desktop">
        <ul class="nav-links">
          <li 
            v-for="(item, index) in navItems" 
            :key="index"
            :class="{ active: isActive(item.route) }"
            @click="goTo(item.route)"
          >
            <span class="nav-icon">{{ item.icon }}</span>
            <span>{{ item.name }}</span>
          </li>
        </ul>
      </div>
      
      <div class="nav-actions">
        <div class="user-menu">
          <div class="user-info">
            <div class="user-avatar">{{ getUserInitials() }}</div>
            <div class="user-details">
              <span class="user-name">{{ getUserName() }}</span>
              <span class="user-role">Администратор</span>
            </div>
          </div>
          <button @click="logout" class="logout-button">
            <span>Выйти</span>
            <span class="logout-icon">→</span>
          </button>
        </div>
        
        <button class="mobile-menu-button" @click="toggleMobileMenu">
          <span>☰</span>
        </button>
      </div>
    </div>
    
    <div class="mobile-menu" :class="{ 'menu-open': mobileMenuOpen }">
      <ul class="mobile-nav-links">
        <li 
          v-for="(item, index) in navItems" 
          :key="index"
          :class="{ active: isActive(item.route) }"
          @click="goToMobile(item.route)"
        >
          <span class="nav-icon">{{ item.icon }}</span>
          <span>{{ item.name }}</span>
        </li>
        <li class="mobile-logout" @click="logout">
          <span class="nav-icon">→</span>
          <span>Выйти</span>
        </li>
      </ul>
    </div>
  </nav>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../store/auth';

const router = useRouter();
const route = useRoute();
const auth = useAuthStore();
const mobileMenuOpen = ref(false);

const navItems = [
  { name: 'Главная', route: '/', icon: '' },
  { name: 'Проекты', route: '/projects', icon: '' },
  { name: 'Дефекты', route: '/defects', icon: '' },
  { name: 'Отчёты', route: '/reports', icon: '' }
];

const goTo = (path) => {
  router.push(path);
};

const goToMobile = (path) => {
  router.push(path);
  mobileMenuOpen.value = false;
};

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value;
};

const isActive = (path) => {
  return route.path === path;
};

const getUserInitials = () => {
  const user = auth.user;
  if (!user || !user.username) return '?';
  return user.username.substring(0, 2).toUpperCase();
};

const getUserName = () => {
  const user = auth.user;
  return user?.username || 'Пользователь';
};

const logout = () => {
  if (confirm('Вы уверены, что хотите выйти?')) {
    auth.logout();
    router.push('/login');
  }
};
</script>

<style scoped>
.navbar {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  position: sticky;
  top: 0;
  z-index: 100;
}

.navbar-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  transition: transform 0.2s;
}

.logo:hover {
  transform: scale(1.02);
}

.logo-icon-wrapper {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.logo-icon {
  font-size: 1.5rem;
}

.logo-text {
  font-size: 1.25rem;
  font-weight: 700;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.nav-links {
  display: flex;
  list-style: none;
  gap: 0.5rem;
}

.nav-links li {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  padding: 0.75rem 1.25rem;
  border-radius: 12px;
  transition: all 0.2s ease;
  font-weight: 500;
  color: #4a5568;
}

.nav-links li:hover {
  background: linear-gradient(135deg, rgba(102, 126, 234, 0.1) 0%, rgba(118, 75, 162, 100%, 0.1) 100%);
  color: #667eea;
}

.nav-links li.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.nav-icon {
  font-size: 1.25rem;
}

.nav-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-menu {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 1rem;
  background: #f7fafc;
  border-radius: 12px;
}

.user-avatar {
  width: 36px;
  height: 36px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.9rem;
  font-weight: 600;
  color: white;
}

.user-details {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-weight: 600;
  font-size: 0.9rem;
  color: #2d3748;
}

.user-role {
  font-size: 0.75rem;
  color: #718096;
}

.logout-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: linear-gradient(135deg, #fc8181 0%, #f56565 100%);
  color: white;
  border: none;
  padding: 0.75rem 1.25rem;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 4px 12px rgba(252, 129, 129, 0.3);
}

.logout-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(252, 129, 129, 0.4);
}

.logout-icon {
  font-size: 1rem;
  font-weight: bold;
}

.mobile-menu-button {
  display: none;
  background: #f7fafc;
  border: none;
  color: #4a5568;
  font-size: 1.5rem;
  cursor: pointer;
  width: 40px;
  height: 40px;
  border-radius: 10px;
  transition: all 0.2s;
}

.mobile-menu-button:hover {
  background: #edf2f7;
}

.mobile-menu {
  display: none;
  background: white;
  padding: 1rem;
  max-height: 0;
  overflow: hidden;
  transition: max-height 0.3s ease;
  border-top: 1px solid #e2e8f0;
}

.mobile-menu.menu-open {
  max-height: 500px;
}

.mobile-nav-links {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.mobile-nav-links li {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s ease;
  font-weight: 500;
  color: #4a5568;
}

.mobile-nav-links li:hover,
.mobile-nav-links li.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.mobile-logout {
  background: linear-gradient(135deg, #fc8181 0%, #f56565 100%);
  color: white;
  margin-top: 0.5rem;
}

.mobile-logout:hover {
  opacity: 0.9;
}

@media (max-width: 768px) {
  .navbar-container {
    padding: 1rem;
  }
  
  .nav-desktop {
    display: none;
  }
  
  .user-menu {
    display: none;
  }
  
  .mobile-menu-button {
    display: flex;
    align-items: center;
    justify-content: center;
  }
  
  .mobile-menu {
    display: block;
  }
}
</style>
