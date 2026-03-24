import axios from "axios";
import type { Category } from "../model/Category";

const API_URL = "https://localhost:7250/api/category";

const categoryService = {
  getAll: () => axios.get<Category[]>(API_URL),
  getById: (id: string) => axios.get<Category>(`${API_URL}/${id}`),
  create: (category: Category) => axios.post<Category>(API_URL, category),
  update: (id: string, category: Category) => axios.put<Category>(`${API_URL}/${id}`, category),
  remove: (id: string) => axios.delete(`${API_URL}/${id}`),
};

export default categoryService;
