<template>
  <nav class="bg-gray-800 text-white p-4 flex justify-between">
    <div class="space-x-4">
      <RouterLink to="/">🏠 Dashboard</RouterLink>
      <RouterLink to="/projects">📂 Projects</RouterLink>
      <RouterLink to="/defects">🐞 Defects</RouterLink>
      <RouterLink to="/reports">📊 Reports</RouterLink>
    </div>
    <button @click="logout" class="bg-red-500 px-3 py-1 rounded">Logout</button>
  </nav>
</template>

<script setup>
import { useAuthStore } from "../store/auth";
import { useRouter } from "vue-router";

const auth = useAuthStore();
const router = useRouter();

function logout() {
  auth.logout();
  router.push("/login");
}
</script>
