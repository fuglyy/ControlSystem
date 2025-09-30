import { defineStore } from "pinia";
import api from "../api/axios";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: localStorage.getItem("token") || null,
  }),
  actions: {
    async login(username, password) {
      const res = await api.post("/Auth/login", { username, password });
      this.token = res.data.token;
      localStorage.setItem("token", this.token);
    },
    logout() {
      this.token = null;
      localStorage.removeItem("token");
    },
  },
});
