import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "../store/auth";

import LoginView from "../views/LoginView.vue";
import DashBoardView from "../views/DashBoardView.vue";
import ProjectsView from "../views/ProjectsView.vue";
import DefectsView from "../views/DefectsView.vue";
import ReportsView from "../views/ReportsView.vue";

const routes = [
  { path: "/login", component: LoginView },
  { path: "/", component: DashBoardView, meta: { requiresAuth: true } },
  { path: "/projects", component: ProjectsView, meta: { requiresAuth: true } },
  { path: "/defects", component: DefectsView, meta: { requiresAuth: true } },
  { path: "/reports", component: ReportsView, meta: { requiresAuth: true } },
  {
    path: "/defects/:id",
    name: "DefectDetail",
    component: () => import("../views/DefectDetail.vue"),
    props: true,
  }

];

const router = createRouter({
  history: createWebHistory(),
  routes,
});


/*router.beforeEach((to) => {
  const auth = useAuthStore();
  if (to.meta.requiresAuth && !auth.token) {
    return "/login";
  }
});*/

export default router;
