<template>
  <div>
    <h1 class="text-xl mb-4">Проекты</h1>
    <div class="flex gap-2 mb-4">
      <input v-model="name" placeholder="Название проекта" class="border p-2" />
      <button @click="createProject" class="bg-green-500 text-white px-3 py-1 rounded">Создать</button>
    </div>
    <ul class="space-y-2">
      <li v-for="p in projects" :key="p.id" class="border p-2 rounded">
        {{ p.name }} ({{ p.stages.length }} этапов)
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import api from "../api/axios";

const projects = ref([]);
const name = ref("");

async function loadProjects() {
  const res = await api.get("/Projects");
  projects.value = res.data;
}

async function createProject() {
  await api.post("/Projects", { name: name.value, stages: [] });
  name.value = "";
  loadProjects();
}

onMounted(loadProjects);
</script>
