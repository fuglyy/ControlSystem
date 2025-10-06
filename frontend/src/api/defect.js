import api from "./axios";

export const getDefects = async () => {
  const res = await api.get("/defects");
  return res.data;
};

export const createDefect = async (defect) => {
  const res = await api.post("/defects", defect);
  return res.data;
};

export const updateDefect = async (id, defect) => {
  await api.put(`/defects/${id}`, defect);
};

export const deleteDefect = async (id) => {
  await api.delete(`/defects/${id}`);
};
