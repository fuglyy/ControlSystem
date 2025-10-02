<template>
  <div class="login-container">
    <div class="animated-bg">
      <div class="gradient-orb orb-1"></div>
      <div class="gradient-orb orb-2"></div>
      <div class="gradient-orb orb-3"></div>
    </div>
    
    <div class="login-card">
      <div class="login-header">
        <div class="logo-wrapper">
          <div class="logo-icon">
            <span>📊</span>
          </div>
          <div class="logo-glow"></div>
        </div>
        <h1>Project Manager</h1>
        <p class="login-subtitle">Добро пожаловать обратно! 👋</p>
      </div>

      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="username">
            <span class="label-icon">👤</span>
            <span>Имя пользователя</span>
          </label>
          <div class="input-wrapper">
            <input 
              id="username"
              v-model="username" 
              type="text"
              placeholder="Введите имя пользователя" 
              required
              class="modern-input"
            />
            <div class="input-border"></div>
          </div>
        </div>

        <div class="form-group">
          <label for="password">
            <span class="label-icon">🔒</span>
            <span>Пароль</span>
          </label>
          <div class="input-wrapper">
            <input 
              id="password"
              v-model="password" 
              type="password" 
              placeholder="Введите пароль" 
              required
              class="modern-input"
            />
            <div class="input-border"></div>
          </div>
        </div>

        <button type="submit" class="gradient-button" :disabled="isLoading">
          <span v-if="!isLoading" class="button-content">
            <span>Войти</span>
            <span class="button-arrow">→</span>
          </span>
          <span v-else class="loading-content">
            <span class="loading-spinner"></span>
            <span>Вход...</span>
          </span>
          <div class="button-glow"></div>
        </button>
      </form>

      <div v-if="error" class="error-banner">
        <span class="error-icon">⚠️</span>
        <span>{{ error }}</span>
      </div>

      <div class="login-footer">
        <p>Нет аккаунта? <a href="#" class="link">Создать</a></p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useAuthStore } from "../store/auth";
import { useRouter } from "vue-router";

const username = ref("");
const password = ref("");
const error = ref("");
const isLoading = ref(false);
const auth = useAuthStore();
const router = useRouter();

// Function to handle login logic
async function handleLogin() {
  if (isLoading.value) return;
  
  isLoading.value = true;
  error.value = "";

  try {
    await auth.login(username.value, password.value);
    router.push("/");
  } catch (err) {
    error.value = err.message || "Ошибка входа! Проверьте учетные данные.";
  } finally {
    isLoading.value = false;
  }
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  position: relative;
  overflow: hidden;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.animated-bg {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  overflow: hidden;
}

.gradient-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.6;
  animation: float 20s ease-in-out infinite;
}

.orb-1 {
  width: 500px;
  height: 500px;
  background: radial-gradient(circle, #ff6b6b, transparent);
  top: -10%;
  left: -10%;
  animation-delay: 0s;
}

.orb-2 {
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, #4ecdc4, transparent);
  bottom: -10%;
  right: -10%;
  animation-delay: 7s;
}

.orb-3 {
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, #f7b731, transparent);
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  animation-delay: 14s;
}

@keyframes float {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  33% {
    transform: translate(30px, -30px) scale(1.1);
  }
  66% {
    transform: translate(-20px, 20px) scale(0.9);
  }
}

.login-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 24px;
  padding: 3rem;
  width: 100%;
  max-width: 450px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  position: relative;
  z-index: 2;
  border: 1px solid rgba(255, 255, 255, 0.3);
  animation: slideUp 0.6s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.login-header {
  text-align: center;
  margin-bottom: 2.5rem;
}

.logo-wrapper {
  position: relative;
  display: inline-block;
  margin-bottom: 1.5rem;
}

.logo-icon {
  width: 80px;
  height: 80px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2.5rem;
  position: relative;
  z-index: 1;
  animation: bounce 2s ease-in-out infinite;
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

.logo-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100px;
  height: 100px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 50%;
  filter: blur(30px);
  opacity: 0.5;
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { transform: translate(-50%, -50%) scale(1); opacity: 0.5; }
  50% { transform: translate(-50%, -50%) scale(1.2); opacity: 0.8; }
}

.login-card h1 {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-size: 2rem;
  font-weight: 800;
  margin: 0 0 0.5rem;
  letter-spacing: -0.5px;
}

.login-subtitle {
  color: #718096;
  font-size: 1.1rem;
  margin: 0;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 1.75rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.form-group label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #2d3748;
  font-weight: 600;
  font-size: 0.95rem;
}

.label-icon {
  font-size: 1.25rem;
}

.input-wrapper {
  position: relative;
}

.modern-input {
  width: 100%;
  padding: 1rem 1.25rem;
  background: #f7fafc;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  color: #2d3748;
  font-size: 1rem;
  outline: none;
  transition: all 0.3s ease;
}

.modern-input:focus {
  background: white;
  border-color: transparent;
  transform: translateY(-2px);
}

.input-border {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  border-radius: 12px;
  padding: 2px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
  mask-composite: exclude;
  opacity: 0;
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.modern-input:focus + .input-border {
  opacity: 1;
}

.modern-input::placeholder {
  color: #a0aec0;
}

.gradient-button {
  position: relative;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 1.25rem 2rem;
  border-radius: 12px;
  font-size: 1.1rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 0.5rem;
  overflow: hidden;
  box-shadow: 0 10px 30px rgba(102, 126, 234, 0.4);
}

.gradient-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 15px 40px rgba(102, 126, 234, 0.5);
}

.gradient-button:active:not(:disabled) {
  transform: translateY(0);
}

.gradient-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.button-content,
.loading-content {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  position: relative;
  z-index: 1;
}

.button-arrow {
  font-size: 1.5rem;
  font-weight: bold;
  transition: transform 0.3s ease;
}

.gradient-button:hover .button-arrow {
  transform: translateX(5px);
}

.button-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 0;
  height: 0;
  background: rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  transition: width 0.6s ease, height 0.6s ease;
}

.gradient-button:hover .button-glow {
  width: 300px;
  height: 300px;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top: 2px solid white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-banner {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem 1.25rem;
  border-radius: 12px;
  font-size: 0.95rem;
  font-weight: 500;
  margin-top: 1rem;
  background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(255, 107, 107, 0.3);
  animation: shake 0.5s ease;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-10px); }
  75% { transform: translateX(10px); }
}

.error-icon {
  font-size: 1.5rem;
}

.login-footer {
  margin-top: 2rem;
  text-align: center;
  color: #718096;
  font-size: 0.95rem;
}

.link {
  color: #667eea;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.2s;
}

.link:hover {
  color: #764ba2;
  text-decoration: underline;
}

@media (max-width: 480px) {
  .login-card {
    padding: 2rem 1.5rem;
  }
  
  .logo-icon {
    width: 60px;
    height: 60px;
    font-size: 2rem;
  }
  
  .login-card h1 {
    font-size: 1.75rem;
  }
}
</style>
