<template>
  <div id="app">
    <h1>Система управления проектами</h1>
    <nav>
      <router-link to="/">Главная</router-link> |
      <router-link to="/projects">Проекты</router-link> |
      <router-link to="/defects">Дефекты</router-link> |
      <router-link to="/reports">Отчёты</router-link>
    </nav>

    <!-- вот тут важно -->
    <router-view />
  </div>
</template>

<script setup>
</script>
