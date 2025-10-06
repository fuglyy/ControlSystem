import api from "./axios";

export const getUsers = async () => {
  const res = await api.get("/users");
  return res.data;
};

export const updateDUser = async (id, user) => {
  await api.put(`/users/${id}`, user);
};


