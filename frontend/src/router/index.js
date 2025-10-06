import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "../store/auth";

import LoginView from "../views/LoginView.vue";
import Register from "../views/Register.vue";
import DashBoardView from "../views/DashBoardView.vue";
import ProjectsView from "../views/ProjectsView.vue";
import DefectsView from "../views/DefectsView.vue";
import ReportsView from "../views/ReportsView.vue";

const routes = [
  { path: "/login", component: LoginView },
  { path: "/register", component: Register },
  { path: "/", component: DashBoardView, meta: { requiresAuth: true } },
  { path: "/projects", component: ProjectsView, meta: { requiresAuth: true } },
  { path: "/defects", component: DefectsView, meta: { requiresAuth: true } },
  { path: "/reports", component: ReportsView, meta: { requiresAuth: true } },
  {
    path: "/defects/:id",
    name: "DefectDetail",
    component: () => import("../views/DefectDetail.vue"),
    props: true,
    meta: { requiresAuth: true }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Глобальный guard
router.beforeEach((to) => {
  const auth = useAuthStore();

  // Если страница требует авторизацию
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return "/login";
  }

  // Если страница требует конкретную роль
  if (to.meta.roles && auth.user && !to.meta.roles.includes(auth.user.role)) {
    return "/"; // редирект на дашборд
  }
});

export default router;
