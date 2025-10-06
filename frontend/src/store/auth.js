import { defineStore } from "pinia";
import api from "../api/axios";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: localStorage.getItem("token") || "",
    user: JSON.parse(localStorage.getItem("user")) || null,
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

        // Раскодируем JWT
        const payload = JSON.parse(atob(this.token.split(".")[1]));
        console.log("JWT payload:", payload);

        // Берём username, id и role из payload, переименовываем role в короткое поле
        const userRole =
          payload.role ||
          payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
          "Без роли";

        const userId =
          payload.nameid ||
          payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
          null;

        this.user = {
          username: payload.sub || payload.unique_name || "Пользователь",
          id: userId,
          role: userRole,
        };

        // Сохраняем в localStorage
        localStorage.setItem("user", JSON.stringify(this.user));
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
      localStorage.removeItem("user");
    },

    // Инициализация при старте приложения
    init() {
      const savedToken = localStorage.getItem("token");
      const savedUser = localStorage.getItem("user");
      if (savedToken) this.token = savedToken;
      if (savedUser) this.user = JSON.parse(savedUser);
    },
  },
});
