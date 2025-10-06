
import { defineStore } from "pinia";
import api from "../api/axios";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: localStorage.getItem("token") || "",
    user: null,
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
  },
  actions: {
    async login(username, password) {
      try {
        const res = await api.post("/auth/login", { username, password });
        this.token = res.data.token;
        localStorage.setItem("token", this.token);

        // Раскодируем JWT, чтобы получить имя пользователя и роль
        const payload = JSON.parse(atob(this.token.split(".")[1]));
        this.user = { username: payload.sub, id: payload.nameid, role: payload.role };
      } catch (err) {
        throw new Error("Неверный логин или пароль");
      }
    },

    async register(username, password, role = "Engineer") {
      try {
        const res = await api.post("/auth/register", { username, password, role });
        return res.data;
      } catch (err) {
        throw new Error(err.response?.data || "Ошибка регистрации");
      }
    },

    logout() {
      this.token = "";
      this.user = null;
      localStorage.removeItem("token");
    }
  }
});
