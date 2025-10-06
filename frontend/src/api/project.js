import api from "./axios";

export const getProjects = async () => {
  const res = await api.get("/projects");
  return res.data;
};

export const createProject = async (project) => {
  const res = await api.post("/projects", project);
  return res.data;
};

export const updateProject = async (id, project) => {
  await api.put(`/projects/${id}`, project);
};

