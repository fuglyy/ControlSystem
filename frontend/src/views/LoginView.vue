<template>
  <div class="flex flex-col items-center mt-20">
    <h1 class="text-2xl mb-4">Вход</h1>
    <input v-model="username" placeholder="Username" class="border p-2 mb-2" />
    <input v-model="password" type="password" placeholder="Password" class="border p-2 mb-2" />
    <button @click="handleLogin" class="bg-blue-500 text-white px-4 py-2 rounded">Войти</button>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useAuthStore } from "../store/auth";
import { useRouter } from "vue-router";

const username = ref("");
const password = ref("");
const auth = useAuthStore();
const router = useRouter();

async function handleLogin() {
  try {
    await auth.login(username.value, password.value);
    router.push("/");
  } catch {
    alert("Ошибка входа!");
  }
}
</script>
